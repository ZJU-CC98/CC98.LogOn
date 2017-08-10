using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Query.ExpressionTranslators.Internal;
using Sakura.AspNetCore.Localization;

namespace CC98.LogOn.Controllers
{
	/// <inheritdoc />
	/// <summary>
	/// �ṩ�û��˻���ز�����
	/// </summary>
	public class AccountController : Controller
	{
		public AccountController(CC98IdentityDbContext identityDbContext, IDynamicStringLocalizer<AccountController> localizer, CC98PasswordHashService cc98PasswordHashService)
		{
			IdentityDbContext = identityDbContext;
			Localizer = localizer;
			CC98PasswordHashService = cc98PasswordHashService;
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
		/// ��ʾ��¼���档
		/// </summary>
		/// <returns>���������</returns>
		[AllowAnonymous]
		[HttpGet]
		public IActionResult LogOn()
		{
			return View();
		}

		/// <summary>
		/// ִ�е�¼������
		/// </summary>
		/// <param name="model">��¼��Ϣ</param>
		/// <returns>���������</returns>
		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LogOn(LogOnViewModel model, string returnUrl)
		{
			if (ModelState.IsValid)
			{
				var userName = model.UserName;
				var passwordHash = CC98PasswordHashService.GetPasswordHash(model.Password);

				var user = await (from i in IdentityDbContext.Users
								  where i.Name == userName && i.PasswordHash == passwordHash
								  select i).FirstOrDefaultAsync();

				if (user == null)
				{
					ModelState.AddModelError("", Localizer.Text.UserNameOrPasswordErrorMessage);
					return View(model);
				}

				var properties = new AuthenticationProperties
				{
					IssuedUtc = DateTimeOffset.Now,
					ExpiresUtc = model.ValidTime == null ? (DateTimeOffset?)null : DateTimeOffset.Now + model.ValidTime.Value,
					IsPersistent = model.ValidTime != null,
					RedirectUri = returnUrl
				};

				await HttpContext.Authentication.SignInAsync(HttpContext.Authentication.GetIdentityServerAuthenticationScheme(), CreatePrincipalFromUserInfo(user), properties);

				if (Url.IsLocalUrl(returnUrl))
				{
					return Redirect(returnUrl);
				}
				else
				{
					return RedirectToAction("Index", "Home");
				}
			}


			return View(model);
		}

		/// <summary>
		/// Create the <see cref="ClaimsPrincipal"/> object which meets the require for both Cookie and IdentityServer authentication from a given <see cref="CC98User"/> object.
		/// </summary>
		/// <param name="user">The <see cref="CC98User"/> object.</param>
		/// <returns>The created <see cref="ClaimsPrincipal"/> object.</returns>
		private static ClaimsPrincipal CreatePrincipalFromUserInfo(CC98User user)
		{
			var userId = user.Id.ToString("D");
			var provider = "CC98";

			var extraClaims = new[]
			{
				new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.Integer, provider)
			};

			return IdentityServerPrincipal.Create(userId, user.Name, provider, new[] { CookieAuthenticationDefaults.AuthenticationScheme }, extraClaims);
		}
	}
}