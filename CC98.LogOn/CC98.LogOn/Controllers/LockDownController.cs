using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading;
using System.Threading.Tasks;

using CC98.LogOn.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Sakura.AspNetCore;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 账号锁定管理。
	/// </summary>
	[Authorize(Policies.Admin)]
	public class LockDownController : Controller
	{
		public LockDownController(CC98IdentityDbContext dbContext, IOperationMessageAccessor messageAccessor)
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

		[HttpGet]
		public async Task<IActionResult> Index(int page = 1, CancellationToken cancellationToken = default)
		{
			var data =
				from i in DbContext.ZjuAccountLockDownRecords
				orderby i.Time descending
				select i;

			return View(await data.ToPagedListAsync(20, page, cancellationToken));
		}

		/// <summary>
		/// 创建锁定记录。
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 执行创建操作。
		/// </summary>
		/// <param name="model"></param>
		/// <param name="cancellationToken"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ZjuAccountLockDownRecord model, CancellationToken cancellationToken)
		{
			if (ModelState.IsValid)
			{
				model.Time = DateTimeOffset.Now;
				DbContext.ZjuAccountLockDownRecords.Add(model);

				try
				{
					await DbContext.SaveChangesAsync(cancellationToken);
					MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
						string.Format(CultureInfo.CurrentUICulture, "已将通行证账号 {0} 加入锁定列表。", model.ZjuAccountId));

					return RedirectToAction("Index", "LockDown");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError(string.Empty, ex.GetMessage());
				}
			}

			return View(model);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
		{
			var item =
				await (from i in DbContext.ZjuAccountLockDownRecords
					   where i.ZjuAccountId == id
					   select i).SingleOrDefaultAsync(cancellationToken);

			if (item == null)
			{
				return BadRequest("要删除的账号不存在。");
			}

			try
			{
				DbContext.ZjuAccountLockDownRecords.Remove(item);
				await DbContext.SaveChangesAsync(cancellationToken);

				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功，",
					string.Format(CultureInfo.CurrentUICulture, "已将通行证账号 {0} 从锁定列表中移除。", id));
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}

		}
	}
}
