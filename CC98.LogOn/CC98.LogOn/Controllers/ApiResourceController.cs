using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sakura.AspNetCore;

namespace CC98.LogOn.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// 提供对 API 资源的控制。
    /// </summary>
    public class ApiResourceController : Controller
    {
        public ApiResourceController(CC98IdentityDbContext dbContext, IOperationMessageAccessor messageAccessor)
        {
            DbContext = dbContext;
            MessageAccessor = messageAccessor;
        }

        /// <summary>
        /// 获取消息访问器。
        /// </summary>
        private IOperationMessageAccessor MessageAccessor { get; }

        /// <summary>
        /// 获取数据库上下文对象。
        /// </summary>
        private CC98IdentityDbContext DbContext { get; }

        /// <summary>
        /// 显示 API 资源列表。
        /// </summary>
        /// <param name="page">页码。</param>
        /// <returns>操作结果。</returns>
        public async Task<IActionResult> Index(int page = 1)
        {
            var result = from i in DbContext.ApiResources
                         orderby i.Id
                         select i;

            return View(await result.ToPagedListAsync(20, page));
        }

        /// <summary>
        /// 显示创建 API 资源页面。
        /// </summary>
        /// <returns>操作结果。</returns>
        [HttpGet]
        [Authorize(Policies.OperateApps)]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// 执行创建操作。
        /// </summary>
        /// <param name="model">数据模型。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        [Authorize(Policies.OperateApps)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AppApiResource model)
        {
            if (ModelState.IsValid)
            {
                model.Secret = Guid.NewGuid();
                DbContext.ApiResources.Add(model);

                try
                {
                    await DbContext.SaveChangesAsync();
                    MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
                        string.Format(CultureInfo.CurrentUICulture, "你已经成功创建了 API 资源 {0}", model.DisplayName));
                    return RedirectToAction("Index", "ApiResource");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", ex.GetMessage());
                }
            }

            return View(model);
        }

        /// <summary>
        /// 显示编辑界面。
        /// </summary>
        /// <param name="id">资源标识。</param>
        /// <returns>操作结果。</returns>
        [HttpGet]
        [Authorize(Policies.OperateApps)]
        public async Task<IActionResult> Edit(string id)
        {
            var item = await DbContext.ApiResources.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return View(item);
        }

        /// <summary>
        /// 执行编辑操作。
        /// </summary>
        /// <param name="model">数据模型。</param>
        /// <returns>操作结果。</returns>
        [HttpPost]
        [Authorize(Policies.OperateApps)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AppApiResource model)
        {
            if (ModelState.IsValid)
            {
                var item = await DbContext.ApiResources.FindAsync(model.Id);

                if (item == null)
                {
                    return NotFound();
                }

                // 修改原始数据，注意 ID 和机密保持不变
                item.PatchExclude(model, i => new { i.Id, i.Secret });

                try
                {
                    await DbContext.SaveChangesAsync();
                    MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
                        string.Format(CultureInfo.CurrentUICulture, "你已经成功修改了 API 资源 {0}", model.DisplayName));
                    return RedirectToAction("Index", "ApiResource");
                }
                catch (DbUpdateException ex)
                {
                    ModelState.AddModelError("", ex.GetMessage());
                }
            }

            return View(model);
        }
    }
}