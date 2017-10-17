using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.ZjuInfoAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

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


		public IActionResult LogOnCallback(string returnUrl)
		{
			if (!Url.IsLocalUrl(returnUrl))
			{
				returnUrl = Url.Action("Index", "Home");
			}

			return Redirect(returnUrl);
		}
	}
}