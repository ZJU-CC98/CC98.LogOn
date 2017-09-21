using System;
using System.Collections.Generic;
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
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakura.AspNetCore.Localization;
using AuthenticationProperties = Microsoft.AspNetCore.Authentication.AuthenticationProperties;

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
			return View();
		}

		/// <summary>
		/// 执行确认操作。
		/// </summary>
		/// <param name="interactionService"></param>
		/// <param name="clientStore"></param>
		/// <param name="returnUrl"></param>
		/// <param name="model"></param>
		/// <returns></returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Consent([FromServices]IIdentityServerInteractionService interactionService, [FromServices]IClientStore clientStore, string returnUrl, object model)
		{
			if (!interactionService.IsValidReturnUrl(returnUrl))
			{
				return BadRequest();
			}

			var authorizeContext = await interactionService.GetAuthorizationContextAsync(returnUrl);
			var client = await clientStore.FindClientByIdAsync(authorizeContext.ClientId);

			var response = new ConsentResponse
			{
				RememberConsent = true
			};

			await interactionService.GrantConsentAsync(authorizeContext, response);
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
			await HttpContext.SignInAsync(IdentityServerConstants.DefaultCookieAuthenticationScheme, CreatePrincipalFromUserInfo(user), properties);

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

			var authMethods = new[]
			{
				IdentityServerConstants.DefaultCookieAuthenticationScheme
			};

			// 登录相关的额外声明信息
			var extraClaims = new List<Claim>
			{
				new Claim(JwtClaimTypes.Id, userId, ClaimValueTypes.Integer, provider)
			};

			foreach (var role in user.Roles)
			{
				extraClaims.Add(new Claim(JwtClaimTypes.Role, role.Role.Name, ClaimValueTypes.String, provider));
			}

			return IdentityServerPrincipal.Create(userId, user.Name, provider, authMethods, DateTime.Now, extraClaims.ToArray());
		}
	}
}