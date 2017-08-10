using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供对于接入应用的管理操作。
	/// </summary>
	public class AppController : Controller
	{
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
	}
}