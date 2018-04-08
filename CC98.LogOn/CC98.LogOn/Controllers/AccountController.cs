using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
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
	///     �ṩ�û��˻���ز�����
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
		///     ��ȡ���ݿ������Ķ���
		/// </summary>
		private CC98IdentityDbContext IdentityDbContext { get; }

		/// <summary>
		///     ��ȡ���ػ��������
		/// </summary>
		private IDynamicStringLocalizer<AccountController> Localizer { get; }

		/// <summary>
		///     ��ȡ CC98 ����ɢ�з���
		/// </summary>
		private CC98PasswordHashService CC98PasswordHashService { get; }

		/// <summary>
		///     ��ȡӦ�ó������÷���
		/// </summary>
		private AppSetting AppSetting { get; }

		/// <summary>
		///     ��ȡ��Ϣ����
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		///     �ⲿ��¼����
		/// </summary>
		public ExternalSignInManager ExternalSignInManager { get; }

		/// <summary>
		///     ��ȡ CC98 ������ݷ���
		/// </summary>
		private CC98DataService CC98DataService { get; }

		/// <summary>
		///     ��ʾע����档
		/// </summary>
		/// <returns>���������</returns>
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		/// <summary>
		///     ִ��ע�������
		/// </summary>
		/// <param name="model">����ģ�͡�</param>
		/// <returns>���������</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model)
		{
			var userName = model.UserName ?? string.Empty;
			string zjuInfoId = null;

			// �û����ַ����ȼ��
			var charCount = 0;

			// ���㷨��ֻ���� ASCII ��Χ
			foreach (var c in userName)
				if (c < 128)
					charCount++;
				else
					charCount += 2;

			if (charCount > 10)
				ModelState.AddModelError("", "�û����ĳ��ȳ������ơ�ֻ�ܰ������ 10 ���ַ�����Ӣ����ĸ���־���Ϊ�����ַ�����");

			// ������
			if (User.Identity.IsAuthenticated)
			{
				zjuInfoId = User.GetId();
				var bindCount = await (from i in IdentityDbContext.Users
									   where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
									   select i).CountAsync();

				if (bindCount >= AppSetting.MaxCC98AccountPerZjuInfoId)
					ModelState.AddModelError("", "��ǰ���ͨ��֤�󶨵��˻������Ѿ��ﵽ���ޣ��޷��������˺�");
			}
			else if (model.BindToZjuInfoId)
			{
				ModelState.AddModelError("", "δ��¼�����ͨ��֤�˺ţ��޷��������˺š�");
			}

			var userExists = await (from i in IdentityDbContext.Users
									where string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
									select i).AnyAsync();

			if (userExists)
				ModelState.AddModelError("", "���û����Ѿ����ڣ��뻻���û�������һ�Ρ�");


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
					ModelState.AddModelError("", "�޷��������û������Ժ�����һ�λ���ϵ����Ա��");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.GetMessage());
				}

			return View(model);
		}

		/// <summary>
		///     ��ʾ����ҳ�档
		/// </summary>
		/// <returns>���������</returns>
		[HttpGet]
		[Authorize]
		public async Task<IActionResult> Activate()
		{
			return await CC98DataService.CanActivateUsersAsync(User.GetId()) ? View() : View("ActivateFull");
		}

		/// <summary>
		///     ִ�м��������
		/// </summary>
		/// <param name="model">��ͼģ�͡�</param>
		/// <returns>���������</returns>
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
					ModelState.AddModelError("", "�û��������벻��ȷ��������������롣");
				}
				else if (user.IsVerified)
				{
					ModelState.AddModelError("", "����˻��Ѿ������");
				}
				else
				{
					if (!await CC98DataService.CanActivateUsersAsync(zjuInfoId))
						ModelState.AddModelError("", "������ͨ��֤�˺Ű󶨵� CC98 �˺������Ѿ��ﵽ���ޣ��޷������󶨡�");
					else
						try
						{
							await IdentityDbContext.BindUserAsync(user.Id, zjuInfoId, user.Name, user.PasswordHash,
								HttpContext.Connection.RemoteIpAddress.ToString());

							MessageAccessor.Messages.Add(OperationMessageLevel.Success, "�����ɹ�",
								string.Format(CultureInfo.CurrentUICulture, "���Ѿ��ɹ��������˺� {0}", user.Name));
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
		///     ��ʾ����������档
		/// </summary>
		/// <param name="userName">��������˸ò��������ṩԤ��ѡ����û�����</param>
		/// <returns>���������</returns>
		[HttpGet]
		[Authorize]
		public IActionResult ResetPassword(string userName = null)
		{
			ViewBag.SelectedUserName = userName;
			return View();
		}

		/// <summary>
		///     ִ���������������
		/// </summary>
		/// <param name="model">����ģ�͡�</param>
		/// <returns>���������</returns>
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
					ModelState.AddModelError("", "�������û������ڻ�δ��������ǰ�����ͨ��֤�˻���");
				}
				else
				{
					user.PasswordHash = CC98PasswordHashService.GetPasswordHash(model.NewPassword);

					try
					{
						await IdentityDbContext.SaveChangesAsync();
						MessageAccessor.Messages.Add(OperationMessageLevel.Success, "�����ɹ�",
							string.Format(CultureInfo.CurrentUICulture, "���Ѿ��ɹ������� CC98 �˻���{0}�������롣", user.Name));
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
		///     ִ�е�¼������
		/// </summary>
		/// <param name="returnUrl">��¼��ɺ�Ҫ���صĵ�ַ��</param>
		/// <returns>���������</returns>
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
		///     ִ�е�¼��ص���
		/// </summary>
		/// <param name="returnUrl">��¼��ɺ�Ҫ���صĵ�ַ��</param>
		/// <returns>���������</returns>
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> LogOnCallback(string returnUrl)
		{
			var principal = await ExternalSignInManager.GetExternalPrincipalAsync();

			if (principal?.Identity == null)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "����ʧ��", "��¼�����з����������Ժ�����һ��");
				return RedirectToAction("Index", "Home");
			}

			// ��ȡ��ǰͨ��֤���
			var userId = principal.GetId();

			// ����Ȩ���������������
			var newClaims = new List<Claim>();

			if (AppSetting.QueryAccounts.Contains(userId))
			{
				newClaims.Add(new Claim(ClaimTypes.Role, Policies.Roles.QueryIdOperators));
			}

			// ��¼
			await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, CloneWithClaims(principal, IdentityConstants.ApplicationScheme, newClaims));

			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// �������������ӱ�Ҫ����������
		/// </summary>
		/// <param name="principal">Ҫ���Ƶ��������</param>
		/// <param name="authenticatoinType">�±�ʶ����֤���͡�</param>
		/// <param name="claims">Ҫ��ӵ����������������б�</param>
		/// <returns>�µ��������</returns>
		private static ClaimsPrincipal CloneWithClaims(ClaimsPrincipal principal, string authenticatoinType, IEnumerable<Claim> claims)
		{
			var newIdentities = principal.Identities.Select(i =>
				new ClaimsIdentity(i.Claims.Concat(claims), authenticatoinType, i.NameClaimType, i.RoleClaimType));
			return new ClaimsPrincipal(newIdentities);
		}

		/// <summary>
		///     ִ��ע��������
		/// </summary>
		/// <returns>���������</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOff()
		{
			await ExternalSignInManager.SignOutAsync();
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		///     ����ǰ���ͨ��֤�µ� CC98 �˺š�
		/// </summary>
		/// <returns>���������</returns>
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
		///     ��ʾ���ʱ��ܾ�ҳ�档
		/// </summary>
		/// <param name="returnUrl">���Է��ʵĵ�ַ��</param>
		/// <returns>���������</returns>
		[HttpGet]
		[AllowAnonymous]
		public IActionResult AccessDenied(string returnUrl)
		{
			ViewBag.ReturnUrl = returnUrl;
			return View();
		}
	}
}