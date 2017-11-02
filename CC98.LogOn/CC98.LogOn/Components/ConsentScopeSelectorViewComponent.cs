using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Components
{
    /// <summary>
    /// 提供在授权界面选择领域的界面。
    /// </summary>
    public class ConsentScopeSelectorViewComponent : ViewComponent
    {
        public ConsentScopeSelectorViewComponent(IResourceStore resourceStore)
        {
            ResourceStore = resourceStore;
        }

        /// <summary>
        /// 获取资源存储区服务。
        /// </summary>
        private IResourceStore ResourceStore { get; }


        public async Task<IViewComponentResult> InvokeAsync(string inputName, IEnumerable<string> requiredScopes)
        {
            var resources = await ResourceStore.FindEnabledResourcesByScopeAsync(requiredScopes);
            ViewBag.InputName = inputName;
            return View(resources);
        }
    }
}
