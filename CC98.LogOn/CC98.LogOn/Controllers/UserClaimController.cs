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
	/// <summary>
	/// 定义用户声明对象。
	/// </summary>
	public class UserClaimController : Controller
	{
		public UserClaimController(CC98IdentityDbContext dbContext, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			MessageAccessor = messageAccessor;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 获取消息操作对象。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 显示所有用户声明。
		/// </summary>
		/// <returns>操作结果。</returns>
		public async Task<IActionResult> Index(int page = 1)
		{
			var items = from i in DbContext.UserClaims
						orderby i.Id ascending
						select i;

			return View(await items.ToPagedListAsync(20, page));
		}

		/// <summary>
		/// 显示创建界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.OperateApps)]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 创建用户声明。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(Policies.OperateApps)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(CC98UserClaim model)
		{
			if (ModelState.IsValid)
			{
				DbContext.UserClaims.Add(model);
				try
				{
					await DbContext.SaveChangesAsync();
					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
						string.Format(CultureInfo.CurrentUICulture, "你已经成功添加了声明 {0}", model.DisplayName));
					return RedirectToAction("Index", "UserClaim");

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
		/// <param name="id">标识。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.OperateApps)]
		public async Task<IActionResult> Edit(string id)
		{
			var item = await DbContext.UserClaims.FindAsync(id);

			if (item == null)
			{
				return NotFound();
			}

			return View(item);
		}

		/// <summary>
		/// 编辑用户声明。
		/// </summary>
		/// <param name="model">编辑后的数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(Policies.OperateApps)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(CC98UserClaim model)
		{
			if (ModelState.IsValid)
			{
				DbContext.UserClaims.Update(model);
				try
				{
					await DbContext.SaveChangesAsync();
					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
						string.Format(CultureInfo.CurrentUICulture, "你已经成功编辑了声明 {0}", model.DisplayName));
					return RedirectToAction("Index", "UserClaim");

				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError("", ex.GetMessage());
				}
			}

			return View(model);
		}

		/// <summary>
		/// 删除用户声明。
		/// </summary>
		/// <param name="id">要删除的声明的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize(Policies.OperateApps)]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id)
		{
			var item = await DbContext.UserClaims.FindAsync(id);

			if (item == null)
			{
				return NotFound();
			}

			DbContext.UserClaims.Remove(item);
			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
					string.Format(CultureInfo.CurrentUICulture, "你已经成功删除了声明 {0}", item.DisplayName));
				return RedirectToAction("Index", "UserClaim");

			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}
	}
}