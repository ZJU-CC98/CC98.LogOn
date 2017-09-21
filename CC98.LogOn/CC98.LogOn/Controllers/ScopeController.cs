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
	/// 提供对领域的相关操作。
	/// </summary>
	public class ScopeController : Controller
	{
		public ScopeController(CC98IdentityDbContext dbContext, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			MessageAccessor = messageAccessor;
		}

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 消息访问器对象。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 显示所有领域。
		/// </summary>
		/// <param name="page">页码。</param>
		/// <returns>操作结果。</returns>
		public async Task<IActionResult> Index(int page = 1)
		{
			var items = from i in DbContext.AppScopes
						where !i.IsHidden
						orderby i.Region, i.DisplayName
						select i;

			return View(await items.ToPagedListAsync(20, page));
		}

		/// <summary>
		/// 显示创建领域界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.OperateApps)]
		public IActionResult Create()
		{
			return View();
		}

		[HttpPost]
		[Authorize(Policies.OperateApps)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(AppScope model)
		{
			if (ModelState.IsValid)
			{
				DbContext.AppScopes.Add(model);

				try
				{
					await DbContext.SaveChangesAsync();
					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
						string.Format(CultureInfo.CurrentUICulture, "你已经成功添加了领域 {0}。", model.Id));
					return RedirectToAction("Index", "Scope");
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
		/// <param name="id">领域标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.OperateApps)]
		public async Task<IActionResult> Edit(string id)
		{
			var item = await DbContext.AppScopes.FindAsync(id);
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
		public async Task<IActionResult> Edit(AppScope model)
		{
			if (ModelState.IsValid)
			{
				DbContext.AppScopes.Update(model);

				try
				{
					await DbContext.SaveChangesAsync();
					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
						string.Format(CultureInfo.CurrentUICulture, "你已经成功编辑了领域 {0}。", model.Id));
					return RedirectToAction("Index", "Scope");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.GetMessage());
				}
			}

			return View(model);
		}

		/// <summary>
		/// 执行删除操作。
		/// </summary>
		/// <param name="id">要删除的领域的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(Policies.OperateApps)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			var item = await DbContext.AppScopes.FindAsync(id);
			if (item == null)
			{
				return NotFound();
			}

			DbContext.AppScopes.Remove(item);

			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
					string.Format(CultureInfo.CurrentUICulture, "你已经成功删除了领域 {0}。", id));
				return RedirectToAction("Index", "Scope");
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}
	}
}
