using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using CC98.LogOn.ZjuInfoAuth;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Sakura.AspNetCore;
using Sakura.AspNetCore.Localization;

namespace CC98.LogOn.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// �ṩ�û��˻���ز�����
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// �����������
        /// </summary>
        private IIdentityServerInteractionService InteractionService { get; }

        /// <summary>
        /// �ͻ��˴洢����
        /// </summary>
        private IClientStore ClientStore { get; }

        public AccountController(CC98IdentityDbContext identityDbContext, IDynamicStringLocalizer<AccountController> localizer, CC98PasswordHashService cc98PasswordHashService, IIdentityServerInteractionService interactionService, IClientStore clientStore, IOptions<AppSetting> appSetting, IOperationMessageAccessor messageAccessor, CC98DataService cc98DataService)
        {
            IdentityDbContext = identityDbContext;
            Localizer = localizer;
            CC98PasswordHashService = cc98PasswordHashService;
            InteractionService = interactionService;
            ClientStore = clientStore;
            MessageAccessor = messageAccessor;
            CC98DataService = cc98DataService;
            AppSetting = appSetting.Value;
        }

        /// <summary>
        /// ��ȡ���ݿ������Ķ���
        /// </summary>
        private CC98IdentityDbContext IdentityDbContext { get; }

        /// <summary>
        ///��ȡ���ػ��������
        /// </summary>
        private IDynamicStringLocalizer<AccountController> Localizer { get; }

        /// <summary>
        /// ��ȡ CC98 ����ɢ�з���
        /// </summary>
        private CC98PasswordHashService CC98PasswordHashService { get; }

        /// <summary>
        /// ��ȡӦ�ó������÷���
        /// </summary>
        private AppSetting AppSetting { get; }

        /// <summary>
        /// ��ȡ��Ϣ����
        /// </summary>
        private IOperationMessageAccessor MessageAccessor { get; }

        /// <summary>
        /// ��ȡ CC98 ������ݷ���
        /// </summary>
        private CC98DataService CC98DataService { get; }

        /// <summary>
        /// ��ʾȷ�Ͻ��档
        /// </summary>
        /// <returns>���������</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Consent(string returnUrl)
        {
            var authorizeContext = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            var client = await ClientStore.FindClientByIdAsync(authorizeContext.ClientId);

            ViewBag.Client = client;
            ViewBag.ReturnUrl = returnUrl;
            ViewBag.Scopes = authorizeContext.ScopesRequested;
            return View();
        }

        /// <summary>
        /// ִ��ȷ�ϲ�����
        /// </summary>
        /// <param name="model">����ģ�͡�</param>
        /// <param name="returnUrl">��֤�ɹ��󷵻صĵ�ַ��</param>
        /// <returns>���������</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Consent(ConsentViewModel model, string returnUrl)
        {
            if (!InteractionService.IsValidReturnUrl(returnUrl))
            {
                return BadRequest();
            }

            var authorizeContext = await InteractionService.GetAuthorizationContextAsync(returnUrl);
            var response = new ConsentResponse
            {
                ScopesConsented = model.Scopes,
                RememberConsent = model.RememberConsent
            };

            await InteractionService.GrantConsentAsync(authorizeContext, response);
            return Redirect(returnUrl);

        }

        /// <summary>
        /// ��ʾ��¼���档
        /// </summary>
        /// <param name="returnUrl">��¼�ɹ��󷵻صĵ�ַ��</param>
        /// <returns>���������</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        /// <summary>
        /// ��ʾע����档
        /// </summary>
        /// <returns>���������</returns>
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// ִ��ע�������
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
                ModelState.AddModelError("", "�û����ĳ��ȳ������ơ�ֻ�ܰ������ 10 ���ַ�����Ӣ����ĸ���־���Ϊ�����ַ�����");
            }

            // ������
            if (User.IsAuthenticatedWith(ZjuInfoOAuthDefaults.AuthenticationScheme))
            {
                zjuInfoId = User.GetSubjectId();
                var bindCount = await (from i in IdentityDbContext.Users
                                       where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
                                       select i).CountAsync();

                if (bindCount >= AppSetting.MaxCC98AccountPerZjuInfoId)
                {
                    ModelState.AddModelError("", "��ǰ���ͨ��֤�󶨵��˻������Ѿ��ﵽ���ޣ��޷��������˺�");
                }
            }
            else if (model.BindToZjuInfoId)
            {
                ModelState.AddModelError("", "δ��¼�����ͨ��֤�˺ţ��޷��������˺š�");
            }

            var userExists = await (from i in IdentityDbContext.Users
                                    where string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
                                    select i).AnyAsync();

            if (userExists)
            {
                ModelState.AddModelError("", "���û����Ѿ����ڣ��뻻���û�������һ�Ρ�");
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
                        ModelState.AddModelError("", "�޷��������û������Ժ�����һ�λ���ϵ����Ա��");
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
        /// ��ʾ����ҳ�档
        /// </summary>
        /// <returns>���������</returns>
        [HttpGet]
        [Authorize(Policies.ZjuInfoAccountLogOn)]
        public async Task<IActionResult> Activate()
        {
            if (await CC98DataService.CanActivateUsersAsync(User.GetSubjectId()))
            {
                return View();
            }
            else
            {
                return View("ActivateFull");
            }
        }

        /// <summary>
        /// ִ�м��������
        /// </summary>
        /// <param name="model">��ͼģ�͡�</param>
        /// <returns>���������</returns>
        [HttpPost]
        [Authorize(Policies.ZjuInfoAccountLogOn)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(ActivateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var zjuInfoId = User.GetSubjectId();

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
                    {
                        ModelState.AddModelError("", "������ͨ��֤�˺Ű󶨵� CC98 �˺������Ѿ��ﵽ���ޣ��޷������󶨡�");
                    }
                    else
                    {
                        user.IsVerified = true;
                        user.RegisterZjuInfoId = zjuInfoId;

                        try
                        {
                            await IdentityDbContext.SaveChangesAsync();
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

            }

            return View(model);
        }

        /// <summary>
        /// ��ʾ����������档
        /// </summary>
        /// <param name="userName">��������˸ò��������ṩԤ��ѡ����û�����</param>
        /// <returns>���������</returns>
        [HttpGet]
        [Authorize(Policies.ZjuInfoAccountLogOn)]
        public IActionResult ResetPassword(string userName = null)
        {
            ViewBag.SelectedUserName = userName;
            return View();
        }

        /// <summary>
        /// ִ���������������
        /// </summary>
        /// <param name="model">����ģ�͡�</param>
        /// <returns>���������</returns>
        [HttpPost]
        [Authorize(Policies.ZjuInfoAccountLogOn)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var zjuInfoId = User.GetSubjectId();
                var userName = model.UserName;

                var user = await (from i in IdentityDbContext.Users
                                  where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
                                    && string.Equals(i.Name, userName, StringComparison.OrdinalIgnoreCase)
                                  select i).FirstOrDefaultAsync();

                if (user == null)
                {
                    ModelState.AddModelError("", "�������û������ڻ�λδ��������ǰ�����ͨ��֤�˻���");
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
        /// ִ�е�¼������
        /// </summary>
        /// <param name="model">��¼��Ϣ</param>
        /// <param name="returnUrl">��¼��ķ��ص�ַ��</param>
        /// <returns>���������</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOn(LogOnViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // ��ȡ�û���������ɢ��
            var userName = model.UserName;
            var passwordHash = CC98PasswordHashService.GetPasswordHash(model.Password);

            // ����¼��Ϣ�Ƿ���ȷ
            var user = await (from i in IdentityDbContext.Users
                              .Include(p => p.Roles).ThenInclude(p => p.Role)
                              where i.Name == userName && i.PasswordHash == passwordHash
                              select i).FirstOrDefaultAsync();

            if (user == null)
            {
                ModelState.AddModelError("", Localizer.Text.UserNameOrPasswordErrorMessage);
                return View(model);
            }

            // ִ�е�¼
            var properties = new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.Now,
                ExpiresUtc = model.ValidTime == null ? (DateTimeOffset?)null : DateTimeOffset.Now + model.ValidTime.Value,
                IsPersistent = model.ValidTime != null,
                RedirectUri = returnUrl
            };

            // ִ�е�¼
            await HttpContext.SignInAsync(CreateIdentityServerUserFromUserInfo(user), properties);

            // ���ص�¼ǰҳ��
            var realReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home");
            return Redirect(realReturnUrl);
        }

        /// <summary>
        /// ִ��ע��������
        /// </summary>
        /// <returns>���������</returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            await HttpContext.SignOutAsync(IdentityServerConstants.SignoutScheme);
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Create the <see cref="ClaimsPrincipal"/> object which meets the require for both Cookie and IdentityServer authentication from a given <see cref="CC98User"/> object.
        /// </summary>
        /// <param name="user">The <see cref="CC98User"/> object.</param>
        /// <returns>The created <see cref="ClaimsPrincipal"/> object.</returns>
        private static IdentityServerUser CreateIdentityServerUserFromUserInfo(CC98User user)
        {
            const string provider = "CC98";
            var userId = user.Id.ToString("D");

            var identity = new IdentityServerUser(userId)
            {
                DisplayName = user.Name,
                AuthenticationTime = DateTime.UtcNow
            };

            identity.AuthenticationMethods.Add(IdentityServerConstants.DefaultCookieAuthenticationScheme);
            identity.AdditionalClaims.Add(new Claim(JwtClaimTypes.Id, userId, ClaimValueTypes.Integer, provider));

            foreach (var role in user.Roles)
            {
                identity.AdditionalClaims.Add(new Claim(JwtClaimTypes.Role, role.Role.Name, ClaimValueTypes.String, provider));
            }

            return identity;
        }

        /// <summary>
        /// ����ǰ���ͨ��֤�µ� CC98 �˺š�
        /// </summary>
        /// <returns>���������</returns>
        [HttpGet]
        [Authorize(Policies.ZjuInfoAccountLogOn)]
        public async Task<IActionResult> My()
        {
            var zjuInfoId = User.GetSubjectId();

            var accounts = from i in IdentityDbContext.Users
                           where string.Equals(i.RegisterZjuInfoId, zjuInfoId, StringComparison.OrdinalIgnoreCase)
                           select i;

            return View(await accounts.ToArrayAsync());
        }

        /// <summary>
        /// ��ʾ���ʱ��ܾ�ҳ�档
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