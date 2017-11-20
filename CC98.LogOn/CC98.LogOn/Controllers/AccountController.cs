using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using CC98.LogOn.ZjuInfoAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sakura.AspNetCore;
using Sakura.AspNetCore.Authentication;
using Sakura.AspNetCore.Localization;

namespace CC98.LogOn.Controllers
{
	/// <inheritdoc />
	/// <summary>
	/// 提供用户账户相关操作。
	/// </summary>
	[Route("[action]")]
	public class AccountController : Controller
	{
		public AccountController(CC98IdentityDbContext identityDbContext, IDynamicStringLocalizer<AccountController> localizer, CC98PasswordHashService cc98PasswordHashService, IOptions<AppSetting> appSetting, IOperationMessageAccessor messageAccessor, CC98DataService cc98DataService, ExternalSignInManager externalSignInManager)
		{
			IdentityDbContext = identityDbContext;
			Localizer = localizer;
			CC98PasswordHashService = cc98PasswordHashService;
			MessageAccessor = messageAccessor;
			CC98DataService = cc98DataService;
			ExternalSignInManager = externalSignInManager;
			AppSetting = appSetting.Value;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext IdentityDbContext { get; }

		/// <summary>
		///获取本地化服务对象。
		/// </summary>
		private IDynamicStringLocalizer<AccountController> Localizer { get; }

		/// <summary>
		/// 获取 CC98 密码散列服务。
		/// </summary>
		private CC98PasswordHashService CC98PasswordHashService { get; }

		/// <summary>
		/// 获取应用程序设置服务。
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		/// 获取消息服务。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 外部登录服务。
		/// </summary>
		public ExternalSignInManager ExternalSignInManager { get; }

		/// <summary>
		/// 获取 CC98 相关数据服务。
		/// </summary>
		private CC98DataService CC98DataService { get; }

		/// <summary>
		/// 显示注册界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		/// <summary>
		/// 执行注册操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			var userName = model.UserName ?? string.Empty;
			string zjuInfoId = null;

			// 用户名字符长度检测
			var charCount = 0;

			// 简化算法：只考虑 ASCII 范围
			foreach (var c in userName)
			{
				if (c < 128)
				{
					charCount++;
				}
				else
				{
					charCount += 2;
				}
			}

			if (charCount > 10)
			{
				ModelState.AddModelError("", "用户名的长度超过限制。只能包含最多 10 个字符（非英文字母数字均视为两个字符）。");
			}

			// 激活检测
			if (User.Identity.IsAuthenticated)
			{
				zjuInfoId = User.GetId();
				var bindCount = await (from i in IdentityDbContext.Users
									   where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
									   select i).CountAsync();

				if (bindCount >= AppSetting.MaxCC98AccountPerZjuInfoId)
				{
					ModelState.AddModelError("", "当前浙大通行证绑定的账户数量已经达到上限，无法激活新账号");
				}
			}
			else if (model.BindToZjuInfoId)
			{
				ModelState.AddModelError("", "未登录到浙大通行证账号，无法激活新账号。");
			}

			var userExists = await (from i in IdentityDbContext.Users
									where string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
									select i).AnyAsync();

			if (userExists)
			{
				ModelState.AddModelError("", "该用户名已经存在，请换个用户名再试一次。");
			}


			if (ModelState.IsValid)
			{

				try
				{
					var newUserId = await IdentityDbContext.CreateAccountAsync(model.UserName, model.Password, model.Gender,
						HttpContext.Connection.RemoteIpAddress.ToString(), zjuInfoId);

					if (newUserId != -1)
					{
						ViewBag.NeedBind = (zjuInfoId == null);
						return View("AfterRegister");
					}
					else
					{
						ModelState.AddModelError("", "无法创建新用户，请稍后再试一次或联系管理员。");
					}

				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.GetMessage());
				}

			}

			return View(model);
		}

		/// <summary>
		/// 显示激活页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Activate()
		{
			return await CC98DataService.CanActivateUsersAsync(User.GetId()) ? View() : View("ActivateFull");
		}

		/// <summary>
		/// 执行激活操作。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(ActivateViewModel model)
		{
			if (ModelState.IsValid)
			{
				var zjuInfoId = User.GetId();

				var userName = model.CC98UserName;
				var passwordHash = CC98PasswordHashService.GetPasswordHash(model.CC98Password);

				var user = await (from i in IdentityDbContext.Users
								  where i.Name == userName && i.PasswordHash == passwordHash
								  select i).FirstOrDefaultAsync();

				if (user == null)
				{
					ModelState.AddModelError("", "用户名或密码不正确。请检查后重新输入。");
				}
				else if (user.IsVerified)
				{
					ModelState.AddModelError("", "这个账户已经被激活。");
				}
				else
				{
					if (!await CC98DataService.CanActivateUsersAsync(zjuInfoId))
					{
						ModelState.AddModelError("", "这个浙大通行证账号绑定的 CC98 账号数量已经达到上限，无法继续绑定。");
					}
					else
					{
						user.IsVerified = true;
						user.RegisterZjuInfoId = zjuInfoId;

						try
						{
							await IdentityDbContext.SaveChangesAsync();
							MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
								string.Format(CultureInfo.CurrentUICulture, "你已经成功激活了账号 {0}", user.Name));
							return RedirectToAction("My", "Account");
						}
						catch (DbUpdateException ex)
						{
							ModelState.AddModelError("", ex.GetMessage());
						}
					}
				}

			}

			return View(model);
		}

		/// <summary>
		/// 显示重置密码界面。
		/// </summary>
		/// <param name="userName">如果给定了该参数，则提供预先选择的用户名。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public IActionResult ResetPassword(string userName = null)
		{
			ViewBag.SelectedUserName = userName;
			return View();
		}

		/// <summary>
		/// 执行重置密码操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
		{
			if (ModelState.IsValid)
			{
				var zjuInfoId = User.GetId();
				var userName = model.UserName;

				var user = await (from i in IdentityDbContext.Users
								  where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
									&& string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
								  select i).FirstOrDefaultAsync();

				if (user == null)
				{
					ModelState.AddModelError("", "给定的用户不存在或位未关联到当前的浙大通行证账户。");
				}
				else
				{
					user.PasswordHash = CC98PasswordHashService.GetPasswordHash(model.NewPassword);

					try
					{
						await IdentityDbContext.SaveChangesAsync();
						MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
							string.Format(CultureInfo.CurrentUICulture, "你已经成功重置了 CC98 账户“{0}”的密码。", user.Name));
						return RedirectToAction("My", "Account");
					}
					catch (DbUpdateException ex)
					{
						ModelState.AddModelError("", ex.GetMessage());
					}
				}
			}

			return View(model);
		}

		/// <summary>
		/// 执行登录操作。
		/// </summary>
		/// <param name="returnUrl">登录完成后要返回的地址。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		[HttpGet]
		public IActionResult LogOn(string returnUrl)
		{
			var authProperties = new AuthenticationProperties
			{
				RedirectUri = Url.Action("LogOnCallback", "Account", new { returnUrl })
			};

			return Challenge(authProperties, ZjuInfoOAuthDefaults.AuthenticationScheme);
		}

		/// <summary>
		/// 执行登录后回调。
		/// </summary>
		/// <param name="returnUrl">登录完成后要返回的地址。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> LogOnCallback(string returnUrl)
		{
			var principal = await ExternalSignInManager.SignInFromExternalCookieAsync();

			if (principal?.Identity == null)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "操作失败", "登录过程中发生错误，请稍后再试一次");
				return RedirectToAction("Index", "Home");
			}

			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction("Index", "Home"); 
			}
		}

		/// <summary>
		/// 执行注销操作。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			await ExternalSignInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// 管理当前浙大通行证下的 CC98 账号。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> My()
		{
			var zjuInfoId = User.GetId();

			var accounts = from i in IdentityDbContext.Users
						   where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
						   select i;

			return View(await accounts.ToArrayAsync());
		}

		/// <summary>
		/// 显示访问被拒绝页面。
		/// </summary>
		/// <param name="returnUrl">尝试访问的地址。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
	}
}