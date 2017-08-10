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

        private IIdentityServerInteractionService InteractionService { get; }

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

               await HttpContext.Authentication.SignInAsync()
            }

            //await HttpContext.Authentication.SignInAsync(new IdentityServerOptions().Authentication.AuthenticationScheme);

            return View(model);
        }
    }
}