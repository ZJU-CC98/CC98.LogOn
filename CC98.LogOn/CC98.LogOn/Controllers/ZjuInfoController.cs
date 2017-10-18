﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.ZjuInfoAuth;
using IdentityModel;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore.Authentication;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供浙大通行证相关的操作。
	/// </summary>
	public class ZjuInfoController : Controller
	{
		/// <summary>
		/// 请求使用浙大通行证登录。
		/// </summary>
		/// <param name="returnUrl">登录 URL。</param>
		/// <returns>登录后的返回地址。</returns>
		public IActionResult LogOn(string returnUrl)
		{
			var authProperties = new AuthenticationProperties
			{
				RedirectUri = Url.Action("LogOnCallback", "ZjuInfo", new { returnUrl })
			};

			return Challenge(authProperties, ZjuInfoOAuthDefaults.AuthenticationScheme);
		}


		public async Task<IActionResult> LogOnCallback(string returnUrl)
		{
			var authenticatoinResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
			var principal = authenticatoinResult.Principal;

			if (principal != null)
			{
				await HttpContext.SignInAsync(principal.CreateIdentityServerPrincipal());
			}

			if (!Url.IsLocalUrl(returnUrl))
			{
				returnUrl = Url.Action("Index", "Home");
			}

			return Redirect(returnUrl);
		}
	}
}