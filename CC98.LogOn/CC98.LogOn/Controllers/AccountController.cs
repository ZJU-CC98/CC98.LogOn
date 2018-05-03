using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using CC98.LogOn.ZjuInfoAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
	///     提供用户账户相关操作。
	/// </summary>
	[RequireHttps]
	[Route("[action]")]
	public class AccountController : Controller
	{
		public AccountController(CC98IdentityDbContext identityDbContext,
			IDynamicStringLocalizer<AccountController> localizer, CC98PasswordHashService cc98PasswordHashService,
			IOptions<AppSetting> appSetting, IOperationMessageAccessor messageAccessor, CC98DataService cc98DataService,
			ExternalSignInManager externalSignInManager)
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
		///     获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext IdentityDbContext { get; }

		/// <summary>
		///     获取本地化服务对象。
		/// </summary>
		private IDynamicStringLocalizer<AccountController> Localizer { get; }

		/// <summary>
		///     获取 CC98 密码散列服务。
		/// </summary>
		private CC98PasswordHashService CC98PasswordHashService { get; }

		/// <summary>
		///     获取应用程序设置服务。
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		///     获取消息服务。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		///     外部登录服务。
		/// </summary>
		public ExternalSignInManager ExternalSignInManager { get; }

		/// <summary>
		///     获取 CC98 相关数据服务。
		/// </summary>
		private CC98DataService CC98DataService { get; }

		/// <summary>
		///     显示注册界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		/// <summary>
		///     执行注册操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			var userName = model.UserName ?? string.Empty;
			string zjuInfoId = null;

			// HACK: 后台检测用户名是否合法
			if (!Regex.IsMatch(userName, @"^\w+$", RegexOptions.Compiled | RegexOptions.Singleline))
			{
				ModelState.AddModelError("", "用户名中不能包含标点符号、空白和其它不非文字类字符。");
			}

			// 用户名字符长度检测
			var charCount = 0;

			// 简化算法：只考虑 ASCII 范围
			foreach (var c in userName)
				if (c < 128)
					charCount++;
				else
					charCount += 2;

			if (charCount > 10)
				ModelState.AddModelError("", "用户名的长度超过限制。只能包含最多 10 个字符（非英文字母数字均视为两个字符）。");

			// 激活检测
			if (User.Identity.IsAuthenticated)
			{
				zjuInfoId = User.GetId();
				var bindCount = await (from i in IdentityDbContext.Users
									   where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
									   select i).CountAsync();

				if (bindCount >= AppSetting.MaxCC98AccountPerZjuInfoId)
					ModelState.AddModelError("", "当前浙大通行证绑定的账户数量已经达到上限，无法激活新账号");
			}
			else if (model.BindToZjuInfoId)
			{
				ModelState.AddModelError("", "未登录到浙大通行证账号，无法激活新账号。");
			}

			var userExists = await (from i in IdentityDbContext.Users
									where string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
									select i).AnyAsync();

			if (userExists)
				ModelState.AddModelError("", "该用户名已经存在，请换个用户名再试一次。");


			if (ModelState.IsValid)
				try
				{
					var newUserId = await IdentityDbContext.CreateAccountAsync(model.UserName,
						CC98PasswordHashService.GetPasswordHash(model.Password), model.Gender,
						HttpContext.Connection.RemoteIpAddress.ToString(), zjuInfoId);

					if (newUserId != -1)
					{
						ViewBag.NeedBind = zjuInfoId == null;
						return View("AfterRegister");
					}
					ModelState.AddModelError("", "无法创建新用户，请稍后再试一次或联系管理员。");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.GetMessage());
				}

			return View(model);
		}

		/// <summary>
		///     显示激活页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Activate()
		{
			return await CC98DataService.CanActivateUsersAsync(User.GetId()) ? View() : View("ActivateFull");
		}

		/// <summary>
		///     执行激活操作。
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
						ModelState.AddModelError("", "这个浙大通行证账号绑定的 CC98 账号数量已经达到上限，无法继续绑定。");
					else
						try
						{
							await IdentityDbContext.BindUserAsync(user.Id, zjuInfoId, user.Name, user.PasswordHash,
								HttpContext.Connection.RemoteIpAddress.ToString());

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

			return View(model);
		}

		/// <summary>
		///     显示重置密码界面。
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
		///     执行重置密码操作。
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
					ModelState.AddModelError("", "给定的用户不存在或未关联到当前的浙大通行证账户。");
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
		///     执行登录操作。
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
		///     执行登录后回调。
		/// </summary>
		/// <param name="returnUrl">登录完成后要返回的地址。</param>
		/// <returns>操作结果。</returns>
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> LogOnCallback(string returnUrl)
		{
			var principal = await ExternalSignInManager.GetExternalPrincipalAsync();

			if (principal?.Identity == null)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "操作失败", "登录过程中发生错误，请稍后再试一次");
				return RedirectToAction("Index", "Home");
			}

			// 获取当前通行证编号
			var userId = principal.GetId();

			// 提取权限设置
			var roles = await GetSpecialRolesForIdAsync(userId);

			// 登录
			await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, CloneWithClaims(principal, IdentityConstants.ApplicationScheme, roles.Select(i => new Claim(ClaimTypes.Role, i, ClaimValueTypes.String))));

			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// 获取给定用户标识的所有特殊角色。
		/// </summary>
		/// <param name="userId">用户标识对象。</param>
		/// <returns>包含所有特殊角色的集合。</returns>
		private async Task<IEnumerable<string>> GetSpecialRolesForIdAsync(string userId)
		{
			var userTitles = (await IdentityDbContext.GetZjuInfoRelatedUserTitlesAsync(userId)).Select(i => i.Name).Distinct()
				.ToArray();

			var result = new List<string>();

			if (HasPermission(userId, userTitles, AppSetting.Permissions.Admin))
			{
				result.Add(Policies.Roles.Adiminstrators);
			}

			if (HasPermission(userId, userTitles, AppSetting.Permissions.QueryId))
			{
				result.Add(Policies.Roles.QueryIdOperators);
			}

			if (HasPermission(userId, userTitles, AppSetting.Permissions.QueryAccount))
			{
				result.Add(Policies.Roles.QueryAccountOperators);
			}

			return result.ToArray();
		}

		/// <summary>
		/// 判断给定的用户是否具有给定的权限。
		/// </summary>
		/// <param name="userId">用户的标识。</param>
		/// <param name="userTitles">用户所属的头衔组。</param>
		/// <param name="permissionSetting">定义单个权限的设置。</param>
		/// <returns>如果用户具有该权限，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private bool HasPermission(string userId, string[] userTitles, PermissionSetting permissionSetting)
		{
			if (permissionSetting == null)
			{
				return false;
			}

			if (permissionSetting.Ids.NotNullAndContains(userId))
			{
				return true;
			}

			if (permissionSetting.Groups.IsIntersectedWith(userTitles))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// 复制主体对象并添加必要的新声明。
		/// </summary>
		/// <param name="principal">要复制的主体对象。</param>
		/// <param name="authenticatoinType">新标识的验证类型。</param>
		/// <param name="claims">要添加到主体对象的新声明列表。</param>
		/// <returns>新的主体对象。</returns>
		private static ClaimsPrincipal CloneWithClaims(ClaimsPrincipal principal, string authenticatoinType, IEnumerable<Claim> claims)
		{
			var newIdentities = principal.Identities.Select(i =>
				new ClaimsIdentity(i.Claims.Concat(claims), authenticatoinType, i.NameClaimType, i.RoleClaimType));
			return new ClaimsPrincipal(newIdentities);
		}

		/// <summary>
		///     执行注销操作。
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
		///     管理当前浙大通行证下的 CC98 账号。
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
		///     显示访问被拒绝页面。
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