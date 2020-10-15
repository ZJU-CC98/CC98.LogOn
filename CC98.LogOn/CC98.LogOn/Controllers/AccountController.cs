using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using CC98.LogOn.ZjuInfoAuth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
			// ���Ҫ���ͨ��֤����ǿ�Ƶ�¼��
			if (AppSetting.ForceZjuInfoIdBind && !User.Identity.IsAuthenticated)
			{
				return Challenge();
			}

			return View();
		}

		/// <summary>
		///     ִ��ע�������
		/// </summary>
		/// <param name="model">����ģ�͡�</param>
		/// <param name="cancellationToken">����ȡ�����������ơ�</param>
		/// <returns>���������</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
		{
			// ���Ҫ���ͨ��֤����ǿ�Ƶ�¼��
			if (AppSetting.ForceZjuInfoIdBind)
			{
				// δ��¼
				if (!User.Identity.IsAuthenticated)
				{
					return Challenge();
				}

				// ѡ�񲻰�
				if (!model.BindToZjuInfoId)
				{
					ModelState.AddModelError(string.Empty, "Ŀǰϵͳ����ԱҪ��ע���˺ű�������ͨ��֤��");
				}
			}

			// ����󶨵�ͨ��֤����ͨ��֤���������򷵻ش���
			if (model.BindToZjuInfoId && await IsLockDownAsync(cancellationToken))
			{
				ModelState.AddModelError(string.Empty, "���ͨ��֤�˺��ѱ��������޷�ע���˻���");
			}

			var userName = model.UserName ?? string.Empty;
			string zjuInfoId = null;

			// HACK: ��̨����û����Ƿ�Ϸ�
			if (!Regex.IsMatch(userName, @"^\w+$", RegexOptions.Compiled | RegexOptions.Singleline))
			{
				ModelState.AddModelError("", "�û����в��ܰ��������š��հ׺��������������ַ���");
			}

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
									   where i.RegisterZjuInfoId == zjuInfoId
									   select i).CountAsync();

				if (bindCount >= AppSetting.MaxCC98AccountPerZjuInfoId)
					ModelState.AddModelError("", "��ǰ���ͨ��֤�󶨵��˻������Ѿ��ﵽ���ޣ��޷��������˺š�");
			}
			else if (model.BindToZjuInfoId)
			{
				ModelState.AddModelError("", "δ��¼�����ͨ��֤�˺ţ��޷��������˺š�");
			}

			var userExists = await (from i in IdentityDbContext.Users
									where i.Name == userName
									select i).AnyAsync();

			if (userExists)
				ModelState.AddModelError("", "���û����Ѿ����ڣ��뻻���û�������һ�Ρ�");


			if (ModelState.IsValid)
				try
				{
					var newUserId = await IdentityDbContext.CreateAccountAsync(model.UserName,
						CC98PasswordHashService.GetPasswordHash(model.Password), model.Gender,
						HttpContext.Connection.RemoteIpAddress.ToString(), model.BindToZjuInfoId ? zjuInfoId : null, cancellationToken);

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
		/// <param name="cancellationToken">����ȡ�����������ơ�</param>
		/// <returns>���������</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Activate(ActivateViewModel model, CancellationToken cancellationToken)
		{
			// ������⡣
			if (await IsLockDownAsync(cancellationToken))
			{
				ModelState.AddModelError(string.Empty, "���ͨ��֤�˺��Ѿ����������޷��������˺š�");
			}

			if (ModelState.IsValid)
			{
				var zjuInfoId = User.GetId();

				var userName = model.CC98UserName;
				var passwordHash = CC98PasswordHashService.GetPasswordHash(model.CC98Password);

				var user = await (from i in IdentityDbContext.Users
								  where i.Name == userName && i.PasswordHash == passwordHash
								  select i).FirstOrDefaultAsync(cancellationToken);

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
					if (!await CC98DataService.CanActivateUsersAsync(zjuInfoId, cancellationToken))
						ModelState.AddModelError("", "������ͨ��֤�˺Ű󶨵� CC98 �˺������Ѿ��ﵽ���ޣ��޷������󶨡�");
					else
						try
						{
							await IdentityDbContext.BindUserAsync(user.Id, zjuInfoId, user.Name, user.PasswordHash,
								HttpContext.Connection.RemoteIpAddress.ToString(), cancellationToken);

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
		/// <param name="cancellationToken">����ȡ�����������ơ�</param>
		/// <returns>���������</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken cancellationToken)
		{
			if (ModelState.IsValid)
			{
				var zjuInfoId = User.GetId();
				var userName = model.UserName;

				var user = await (from i in IdentityDbContext.Users
								  where i.RegisterZjuInfoId == zjuInfoId && i.Name == userName
								  select i).FirstOrDefaultAsync(cancellationToken);

				if (user == null)
				{
					ModelState.AddModelError("", "�������û������ڻ�δ��������ǰ�����ͨ��֤�˻���");
				}
				else
				{
					user.PasswordHash = CC98PasswordHashService.GetPasswordHash(model.NewPassword);

					try
					{
						await IdentityDbContext.SaveChangesAsync(cancellationToken);
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
		/// <param name="cancellationToken">����ȡ�����������ơ�</param>
		/// <returns>���������</returns>
		[AllowAnonymous]
		[HttpGet]
		public async Task<IActionResult> LogOnCallback(string returnUrl, CancellationToken cancellationToken)
		{
			var principal = await ExternalSignInManager.GetExternalPrincipalAsync();

			if (principal?.Identity == null)
			{
				MessageAccessor.Messages.Add(OperationMessageLevel.Error, "����ʧ��", "��¼���ͨ��֤�з���������ȷ���������ͨ��֤��¼��������������ȷ���û������롣����㿴���������û���������Ľ��棬�볢�Է������ͨ��֤��ҳ�ֶ���¼���ͨ��֤��Ȼ������һ�Ρ�");
				return RedirectToAction("Index", "Home");
			}

			// ��ȡ��ǰͨ��֤���
			var userId = principal.GetId();

			// ��ȡȨ������
			var roles = await GetSpecialRolesForIdAsync(userId, cancellationToken);

			// ��¼
			await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme,
				CloneWithClaims(principal, IdentityConstants.ApplicationScheme,
					roles.Select(i => new Claim(ClaimTypes.Role, i, ClaimValueTypes.String))));

			if (Url.IsLocalUrl(returnUrl))
				return Redirect(returnUrl);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// ��ȡ�����û���ʶ�����������ɫ��
		/// </summary>
		/// <param name="userId">�û���ʶ����</param>
		/// <param name="cancellationToken"></param>
		/// <returns>�������������ɫ�ļ��ϡ�</returns>
		private async Task<IEnumerable<string>> GetSpecialRolesForIdAsync(string userId, CancellationToken cancellationToken)
		{
			// û���κ�Ȩ������
			if (AppSetting.Permissions == null)
			{
				return Enumerable.Empty<string>();
			}

			var userTitles = (await IdentityDbContext.GetZjuInfoRelatedUserTitlesAsync(userId, cancellationToken))
				.Select(i => i.Name).Distinct()
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
		/// ��ȡһ��ֵ��ָʾ��ǰ�û��˺��Ƿ��Ѿ���������
		/// </summary>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		private async Task<bool> IsLockDownAsync(CancellationToken cancellationToken)
		{
			// ��ǰ�û���ʶ
			var userId = User.GetId();

			// �û�δ��¼
			if (string.IsNullOrEmpty(userId))
			{
				return false;
			}

			var lockDownRecord = await IdentityDbContext.ZjuAccountLockDownRecords.FindAsync(new object[] { userId }, cancellationToken);
			return lockDownRecord != null;
		}

		/// <summary>
		/// �жϸ������û��Ƿ���и�����Ȩ�ޡ�
		/// </summary>
		/// <param name="userId">�û��ı�ʶ��</param>
		/// <param name="userTitles">�û�������ͷ���顣</param>
		/// <param name="permissionSetting">���嵥��Ȩ�޵����á�</param>
		/// <returns>����û����и�Ȩ�ޣ����� <c>true</c>�����򷵻� <c>false</c>��</returns>
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
		public async Task<IActionResult> My(int page = 1, CancellationToken cancellationToken = default)
		{
			var zjuInfoId = User.GetId();

			var accounts = from i in IdentityDbContext.Users
						   where i.RegisterZjuInfoId == zjuInfoId
						   select i;

			return View(await accounts.ToPagedListAsync(20, page, cancellationToken));
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