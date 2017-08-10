using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.Services;
using CC98.LogOn.ViewModels.Account;
using IdentityServer4.Configuration;
using IdentityServer4.Services;
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
    /// 提供用户账户相关操作。
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
        /// 获取数据库上下文对象。
        /// </summary>
        private CC98IdentityDbContext IdentityDbContext { get; }

        private IIdentityServerInteractionService InteractionService { get; }

        /// <summary>
        ///获取本地化服务对象。
        /// </summary>
        private IDynamicStringLocalizer<AccountController> Localizer { get; }

        /// <summary>
        /// 获取 CC98 密码散列服务。
        /// </summary>
        private CC98PasswordHashService CC98PasswordHashService { get; }

        /// <summary>
        /// 显示登录界面。
        /// </summary>
        /// <returns>操作结果。</returns>
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogOn()
        {
            return View();
        }

        /// <summary>
        /// 执行登录操作。
        /// </summary>
        /// <param name="model">登录信息</param>
        /// <returns>操作结果。</returns>
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

               await HttpContext.Authentication.SignInAsync()
            }

            //await HttpContext.Authentication.SignInAsync(new IdentityServerOptions().Authentication.AuthenticationScheme);

            return View(model);
        }
    }
}