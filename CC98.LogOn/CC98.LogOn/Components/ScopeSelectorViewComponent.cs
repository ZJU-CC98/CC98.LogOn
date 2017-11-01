using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CC98.LogOn.Components
{
    /// <inheritdoc />
    /// <summary>
    /// 显示领域的选择器。
    /// </summary>
    public class ScopeSelectorViewComponent : ViewComponent
    {
        public ScopeSelectorViewComponent(CC98IdentityDbContext dbContext)
        {
            DbContext = dbContext;
        }

        /// <summary>
        /// 数据库上下文对象。
        /// </summary>
        private CC98IdentityDbContext DbContext { get; }

        /// <summary>
        /// 执行视图组件。
        /// </summary>
        /// <param name="inputName">用于输入字段的名称。</param>
        /// <param name="selectedItems">已经选中的项目。</param>
        /// <returns>操作结果。</returns>
        public async Task<IViewComponentResult> InvokeAsync(string inputName, IEnumerable<string> selectedItems)
        {
            var items = from i in DbContext.AppScopes
                        orderby i.Type, i.ApiId, i.Id
                        select i;

            var apiResources = from i in DbContext.ApiResources
                               orderby i.Id
                               select i;

            ViewBag.Scopes = await items.ToArrayAsync();
            ViewBag.ApiResources = await apiResources.ToArrayAsync();

            ViewBag.InputName = inputName;
            ViewBag.SelectedItems = selectedItems ?? Enumerable.Empty<string>();
            return View();
        }
    }
}
