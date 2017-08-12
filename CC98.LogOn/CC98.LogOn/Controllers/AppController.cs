using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <inheritdoc />
	/// <summary>
	/// 提供对于接入应用的管理操作。
	/// </summary>
	public class AppController : Controller
	{
		public AppController(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 视图主页。
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// 显示创建应用页面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// 执行创建应用操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult Create(App model)
		{
			if (ModelState.IsValid)
			{
				// 覆盖用户名
				model.OwnerUserName = User.Identity.Name;
			}

			return View(model);
		}
	}
}