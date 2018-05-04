using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.ViewModels.Manage;
using CC98.LogOn.ZjuInfoAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Sakura.AspNetCore;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供管理相关功能。
	/// </summary>
	public class ManageController : Controller
	{
		public ManageController(CC98IdentityDbContext dbContext, IConfiguration configuration, IOperationMessageAccessor messageAccessor, CC98PasswordHashService cc98PasswordHashService)
		{
			DbContext = dbContext;
			Configuration = configuration;
			MessageAccessor = messageAccessor;
			CC98PasswordHashService = cc98PasswordHashService;
		}

		/// <summary>
		/// 数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 配置信息。
		/// </summary>
		private IConfiguration Configuration { get; }

		/// <summary>
		/// 操作消息访问器。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 提供 CC98 的密码散列服务。
		/// </summary>
		private CC98PasswordHashService CC98PasswordHashService { get; }

		/// <summary>
		/// 查询账号。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.QueryAccount)]
		public async Task<IActionResult> QueryAccount(QueryAccountViewModel model)
		{
			// 显示查询界面
			if (!ModelState.IsValid)
			{
				ModelState.Clear();
				return View();
			}

			var userName = model.Name;

			// 找不到学号
			var cc98IdInfo = await (from i in DbContext.Users
									where i.Name == userName
									select i).FirstOrDefaultAsync();

			if (cc98IdInfo == null)
			{
				ModelState.AddModelError("", "未找到使用该名称的 CC98 账号。");
				return View(model);
			}

			var zjuId = cc98IdInfo.RegisterZjuInfoId;

			if (string.IsNullOrEmpty(zjuId))
			{
				ModelState.AddModelError("", "该 CC98 账号没有绑定浙大通行证。");
				return View(model);
			}

			var items = await (from i in DbContext.Users
							   where i.RegisterZjuInfoId == zjuId
							   select i).ToArrayAsync();

			var result = new QueryAccountResultModel
			{
				QueryUserName = userName,
				Users = items
			};

			return View("QueryAccountResult", result);
		}

		/// <summary>
		/// 执行查询学号操作。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.QueryId)]
		public async Task<IActionResult> QueryId(QueryIdViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.Clear();
				return View(model);
			}

			var schoolId = model.Id;

			if (model.Type == QueryIdType.AccountId)
			{
				var userName = model.Id;

				var user = await (from i in DbContext.Users
								  where i.Name == userName
								  select i).FirstOrDefaultAsync();
				if (user == null)
				{
					ModelState.AddModelError("", "未找到具有给定名称的 CC98 账号。");
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

			var resultModel = new QueryIdResultModel
			{
				SchoolId = schoolId,
				Users = cc98Ids,
			};

			try
			{
				resultModel.ZjuUserInfo = await GetUserInfoAsync(schoolId);
			}
			catch (Exception)
			{
				// ignored
			}

			return View("QueryIdResult", resultModel);

		}

		/// <summary>
		/// 显示管理账号界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.Admin)]
		public async Task<IActionResult> Account(AccountViewModel model)
		{
			if (!ModelState.IsValid)
			{
				ModelState.Clear();
				return View(model);
			}

			var userName = model.Name;

			var cc98IdInfo = await (from i in DbContext.Users
									where i.Name == userName
									select i).FirstOrDefaultAsync();

			if (cc98IdInfo == null)
			{
				ModelState.AddModelError("", "找不到具有给定名称的 CC98 账号。");
				return View(model);
			}

			// 获取通行证信息
			var zjuInfo = string.IsNullOrEmpty(cc98IdInfo.RegisterZjuInfoId)
				? null
				: await GetUserInfoAsync(cc98IdInfo.RegisterZjuInfoId);

			var result = new AccountResultModel
			{
				CC98UserInfo = cc98IdInfo,
				ZjuUserInfo = zjuInfo
			};

			return View("AccountInfo", result);
		}

		/// <summary>
		/// 执行修改激活信息操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Admin)]
		public async Task<IActionResult> ChangeActivationInfo(ChangeActivationInputModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userName = model.CC98UserName;
			var cc98User = await (from i in DbContext.Users
								  where i.Name == userName
								  select i).FirstOrDefaultAsync();

			if (cc98User == null)
			{
				return BadRequest("要操作的 CC98 账户不存在。");
			}

			cc98User.IsVerified = model.IsActivated;
			cc98User.RegisterZjuInfoId = model.IsActivated ? model.ZjuInfoId : null;

			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功", "你已经成功修改了这个账户的激活和绑定信息");
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}

		/// <summary>
		/// 实现密码重置功能。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Admin)]
		public async Task<IActionResult> ResetPassword(ResetPasswordInputModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userName = model.CC98UserName;
			var cc98User = await (from i in DbContext.Users
								  where i.Name == userName
								  select i).FirstOrDefaultAsync();

			if (cc98User == null)
			{
				return BadRequest("要操作的 CC98 账户不存在。");
			}

			if (string.IsNullOrEmpty(cc98User.RegisterZjuInfoId) && string.IsNullOrEmpty(model.NewPassword))
			{
				return BadRequest("这个账户没有绑定浙大通行证，因此你必须提供一个新密码。");
			}

			// 需要使用的新密码
			var newPassword = string.IsNullOrEmpty(model.NewPassword) ? cc98User.RegisterZjuInfoId : model.NewPassword;
			// 设置新密码
			cc98User.PasswordHash = CC98PasswordHashService.GetPasswordHash(newPassword);

			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功", "你已经成功修改了这个账户的密码");
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}

		/// <summary>
		/// 提供删除账户功能。
		/// </summary>
		/// <param name="model">视图模型。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policies.Admin)]
		public async Task<IActionResult> DeleteUser(DeleteUserInputModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var userName = model.CC98UserName;
			var cc98User = await (from i in DbContext.Users
								  where i.Name == userName
								  select i).FirstOrDefaultAsync();

			if (cc98User == null)
			{
				return BadRequest("要操作的 CC98 账户不存在。");
			}

			DbContext.Users.Remove(cc98User);

			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功", string.Format(CultureInfo.CurrentUICulture, "你已经成功删除了账户 {0}", model.CC98UserName));
				return Ok();
			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}

		/// <summary>
		/// 提取浙大通行证信息的辅助方法。
		/// </summary>
		/// <param name="schoolId">浙大通行证编号。</param>
		/// <returns>表示异步操作的对象。操作结果包含浙大通行证信息。</returns>
		private async Task<ZjuInfoUserInfo> GetUserInfoAsync(string schoolId)
		{
			using (var service = new ZjuInfoService())
			{
				service.AppKey = Configuration["Authentication:ZjuInfo:ClientId"];
				service.AppSecret = Configuration["Authentication:ZjuInfo:ClientSecret"];
				return await service.GetUserInfoAsync(schoolId);
			}
		}
	}
}