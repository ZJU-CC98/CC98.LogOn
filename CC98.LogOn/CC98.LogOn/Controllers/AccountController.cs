using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public AccountController(CC98IdentityDbContext identityDbContext, IDynamicStringLocalizer<AccountController> localizer, CC98PasswordHashService cc98PasswordHashService, IIdentityServerInteractionService interactionService, IClientStore clientStore)
        {
            IdentityDbContext = identityDbContext;
            Localizer = localizer;
            CC98PasswordHashService = cc98PasswordHashService;
            InteractionService = interactionService;
            ClientStore = clientStore;
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
            await HttpContext.SignInAsync(CreatePrincipalFromUserInfo(user), properties);

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
        private static ClaimsPrincipal CreatePrincipalFromUserInfo(CC98User user)
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

            return identity.CreatePrincipal();
        }

        /// <summary>
        /// 管理当前浙大通行证下的 CC98 账号。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> My()
        {
            var zjuInfoId = User.GetSubjectId();

            var accounts = from i in IdentityDbContext.Users
                           where i.RegisterZjuInfoId == zjuInfoId
                           select i;

            return View(await accounts.ToArrayAsync());
        }
    }
}