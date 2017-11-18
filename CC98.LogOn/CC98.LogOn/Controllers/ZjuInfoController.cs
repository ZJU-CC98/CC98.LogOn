using System.Threading.Tasks;
using CC98.LogOn.ZjuInfoAuth;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore;

namespace CC98.LogOn.Controllers
{
    /// <summary>
    /// 提供浙大通行证相关的操作。
    /// </summary>
    public class ZjuInfoController : Controller
    {
        public ZjuInfoController(IOperationMessageAccessor messageAccessor)
        {
            MessageAccessor = messageAccessor;
        }

        /// <summary>
        /// 消息访问服务。
        /// </summary>
        private IOperationMessageAccessor MessageAccessor { get; }

        /// <summary>
        /// 请求使用浙大通行证登录。
        /// </summary>
        /// <param name="returnUrl">登录 URL。</param>
        /// <returns>登录后的返回地址。</returns>
        [AllowAnonymous]
        public IActionResult LogOn(string returnUrl)
        {
            var authProperties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("LogOnCallback", "ZjuInfo", new { returnUrl })
            };

            return Challenge(authProperties, ZjuInfoOAuthDefaults.AuthenticationScheme);
        }


        /// <summary>
        /// 在登录后提供回调功能。
        /// </summary>
        /// <param name="returnUrl">登录后要返回的地址。</param>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        public async Task<IActionResult> LogOnCallback(string returnUrl)
        {
            var authenticatoinResult = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);

            if (!authenticatoinResult.Succeeded)
            {
                MessageAccessor.Messages.Add(OperationMessageLevel.Error, "登录失败",
                    authenticatoinResult.Failure.GetMessage());
                return RedirectToAction("Index", "Home");
            }
            var idenetityUser = authenticatoinResult.Principal.CreateIdentityServerUser(ZjuInfoOAuthDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(idenetityUser, authenticatoinResult.Properties);

            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }

            return Redirect(returnUrl);
        }

    }
}