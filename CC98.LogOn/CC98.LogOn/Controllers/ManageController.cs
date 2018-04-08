using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.ViewModels.Manage;
using CC98.LogOn.ZjuInfoAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供管理相关功能。
	/// </summary>
	public class ManageController : Controller
	{
		public ManageController(CC98IdentityDbContext dbContext, IConfiguration configuration)
		{
			DbContext = dbContext;
			Configuration = configuration;
		}

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		private IConfiguration Configuration { get; }

		/// <summary>
		/// 显示查询学号界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.QueryId)]
		public IActionResult QueryId()
		{
			return View();
		}

		/// <summary>
		/// 执行查询学号操作。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <returns>操作结果。</returns>

		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.QueryId)]
		public async Task<IActionResult> QueryId(QueryIdViewModel model)
		{
			var schoolId = model.Id;

			if (model.Type == QueryIdType.AccountId)
			{
				var userName = model.Id;

				var user = await (from i in DbContext.Users
								  where i.Name == userName
								  select i).FirstOrDefaultAsync();
				if (user == null)
				{
					ModelState.AddModelError("", "未找到使用该名称的 CC98 账号。");
					return View(model);
				}

				if (string.IsNullOrEmpty(user.RegisterZjuInfoId))
				{
					ModelState.AddModelError("", "该 CC98 账号没有绑定浙大通行证账号。");
					return View(model);
				}

				schoolId = user.RegisterZjuInfoId;
			}

			var cc98Ids = await (from i in DbContext.Users
								 where i.RegisterZjuInfoId == schoolId
								 select i).ToArrayAsync();

			using (var service = new ZjuInfoService())
			{
				service.AppKey = Configuration["Authentication:ZjuInfo:ClientId"];
				service.AppSecret = Configuration["Authentication:ZjuInfo:ClientSecret"];

				var userDetail = await service.GetUserInfoAsync(schoolId);

				var resultModel = new QueryIdResultModel
				{
					SchoolId = schoolId,
					Users = cc98Ids,
					ZjuUserInfo = userDetail
				};

				return View("QueryIdResult", resultModel);
			}
		}
	}
}