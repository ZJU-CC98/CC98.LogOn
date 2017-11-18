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
    /// 提供用户账户相关操作。
    /// </summary>
    public class AccountController : Controller
    {
        /// <summary>
        /// 交互服务对象。
        /// </summary>
        private IIdentityServerInteractionService InteractionService { get; }

        /// <summary>
        /// 客户端存储服务。
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
        /// 获取 CC98 相关数据服务。
        /// </summary>
        private CC98DataService CC98DataService { get; }

        /// <summary>
        /// 显示确认界面。
        /// </summary>
        /// <returns>操作结果。</returns>
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
        /// 执行确认操作。
        /// </summary>
        /// <param name="model">数据模型。</param>
        /// <param name="returnUrl">验证成功后返回的地址。</param>
        /// <returns>操作结果。</returns>
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
        /// 显示登录界面。
        /// </summary>
        /// <param name="returnUrl">登录成功后返回的地址。</param>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOn(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

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
            if (User.IsAuthenticatedWith(ZjuInfoOAuthDefaults.AuthenticationScheme))
            {
                zjuInfoId = User.GetSubjectId();
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
        /// 执行激活操作。
        /// </summary>
        /// <param name="model">视图模型。</param>
        /// <returns>操作结果。</returns>
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
        [Authorize(Policies.ZjuInfoAccountLogOn)]
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
        /// <param name="model">登录信息</param>
        /// <param name="returnUrl">登录后的返回地址。</param>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOn(LogOnViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 提取用户名和密码散列
            var userName = model.UserName;
            var passwordHash = CC98PasswordHashService.GetPasswordHash(model.Password);

            // 检测登录信息是否正确
            var user = await (from i in IdentityDbContext.Users
                              .Include(p => p.Roles).ThenInclude(p => p.Role)
                              where i.Name == userName && i.PasswordHash == passwordHash
                              select i).FirstOrDefaultAsync();

            if (user == null)
            {
                ModelState.AddModelError("", Localizer.Text.UserNameOrPasswordErrorMessage);
                return View(model);
            }

            // 执行登录
            var properties = new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.Now,
                ExpiresUtc = model.ValidTime == null ? (DateTimeOffset?)null : DateTimeOffset.Now + model.ValidTime.Value,
                IsPersistent = model.ValidTime != null,
                RedirectUri = returnUrl
            };

            // 执行登录
            await HttpContext.SignInAsync(CreateIdentityServerUserFromUserInfo(user), properties);

            // 返回登录前页面
            var realReturnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : Url.Action("Index", "Home");
            return Redirect(realReturnUrl);
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
        /// 管理当前浙大通行证下的 CC98 账号。
        /// </summary>
        /// <returns>操作结果。</returns>
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