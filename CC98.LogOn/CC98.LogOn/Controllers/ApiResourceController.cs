using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sakura.AspNetCore;

namespace CC98.LogOn.Controllers
{
	/// <inheritdoc />
	/// <summary>
	/// 提供对 API 资源的控制。
	/// </summary>
	public class ApiResourceController : Controller
	{
		public ApiResourceController(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

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
	}
}