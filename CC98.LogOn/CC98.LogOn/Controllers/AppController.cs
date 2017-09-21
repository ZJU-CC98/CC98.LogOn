using CC98.LogOn.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using Sakura.AspNetCore;
using Sakura.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <inheritdoc />
	/// <summary>
	/// 提供对于接入应用的管理操作。
	/// </summary>
	public class AppController : Controller
	{
		public AppController(CC98IdentityDbContext dbContext, IAuthorizationService authorizationService, IOperationMessageAccessor messageAccessor)
		{
			DbContext = dbContext;
			AuthorizationService = authorizationService;
			MessageAccessor = messageAccessor;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// 身份验证服务。
		/// </summary>
		private IAuthorizationService AuthorizationService { get; }

		/// <summary>
		/// 消息服务。
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// 视图主页。
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> Index(int page = 1)
		{
			var items = from i in DbContext.Apps
						orderby i.CreateTime descending
						select i;

			return View(await items.ToPagedListAsync(20, page));
		}

		/// <summary>
		/// 显示我的应用。
		/// </summary>
		/// <param name="page">分页页码。</param>
		/// <returns>操作结果。</returns>
		[Authorize]
		public async Task<IActionResult> My(int page = 1)
		{
			// 当前用户名
			var userName = User.GetName();

			var myItems = from i in DbContext.Apps
						  where i.OwnerUserName == userName
						  orderby i.CreateTime descending
						  select i;

			return View(await myItems.ToPagedListAsync(20, page));
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
		public async Task<IActionResult> Create(App model)
		{
			if (ModelState.IsValid)
			{
				// 关键信息
				model.Secret = Guid.NewGuid();
				model.OwnerUserName = User.Identity.Name;
				model.CreateTime = DateTimeOffset.Now;

				// 添加数据库项目
				DbContext.Apps.Add(model);

				try
				{
					await DbContext.SaveChangesAsync();
					return RedirectToAction("Index", "App");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 显示应用编辑界面。
		/// </summary>
		/// <param name="id">应用标识。</param>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			var app = await LoadAppAndCheckPermissionAsync(id);
			return View(app);
		}

		/// <summary>
		/// 执行应用编辑操作。
		/// </summary>
		/// <param name="model">数据模型。</param>
		/// <returns>操作结果。</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Edit(App model)
		{
			if (ModelState.IsValid)
			{
				var item = await LoadAppAndCheckPermissionAsync(model.Id);
				item.PatchExclude(model, i => new { i.Id, i.Secret, i.CreateTime });

				try
				{
					await DbContext.SaveChangesAsync();
					return RedirectToAction("My", "App");
				}
				catch (DbUpdateException ex)
				{
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}

			return View(model);
		}

		/// <summary>
		/// 执行删除操作。
		/// </summary>
		/// <param name="id">要删除的标识。</param>
		/// <returns>操作结果。</returns>
		[HttpPost]
		[Authorize]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(Guid id)
		{
			var app = await LoadAppAndCheckPermissionAsync(id);

			DbContext.Apps.Remove(app);

			try
			{
				await DbContext.SaveChangesAsync();
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "操作成功",
					string.Format(CultureInfo.CurrentUICulture, "你已经成功删除了应用 {0}", app.DisplayName));
				return RedirectToAction("My", "App");

			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}

		/// <summary>
		/// 创建应用机密。
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[Authorize]
		public async Task<IActionResult> CreateSecret(Guid id)
		{
			var app = await LoadAppAndCheckPermissionAsync(id);
			return Ok();
		}

		[Authorize]
		public async Task<IActionResult> LoadSecret(Guid id)
		{
			var app = await LoadAppAndCheckPermissionAsync(id);
			return View("_SecretPartial");
		}

		/// <summary>
		/// 根据给定标识加载应用信息，并检查当前用户是否有权访问该应用的信息。
		/// </summary>
		/// <param name="id">应用的标识。</param>
		/// <returns>如果找到应用并且当前用户有权访问该信息，返回应用信息，否则将引发异常。</returns>
		private async Task<App> LoadAppAndCheckPermissionAsync(Guid id)
		{
			// 查找 APP 对象
			var app = await DbContext.Apps.FindAsync(id);

			if (app == null)
			{
				throw new ActionResultException(HttpStatusCode.NotFound);
			}

			if (await CanManageAppsOrIsOwnerAsync(app))
			{
				return app;
			}
			else
			{
				throw User.IsAuthenticated()
					? new ActionResultException(HttpStatusCode.Forbidden)
					: new ActionResultException(HttpStatusCode.Unauthorized);
			}
		}

		/// <summary>
		/// 判断当前用户是否具有应用管理权限。
		/// </summary>
		/// <returns>如果当前用户具有应用管理权限，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private async Task<bool> CanManageAppsAsync()
		{
			var result = await AuthorizationService.AuthorizeAsync(User, Policies.OperateApps);
			return result.Succeeded;
		}

		/// <summary>
		/// 判断当前用户是否是应用的所有者。
		/// </summary>
		/// <param name="app">要判断的应用。</param>
		/// <returns>如果当前用户是应用的所有者，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private bool IsAppOwner(App app)
		{
			if (app.OwnerUserName == null)
			{
				return false;
			}

			var userName = User.GetName();
			return string.Equals(userName, app.OwnerUserName, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// 判断当前用户是否是应用管理者，或者应用所有者。
		/// </summary>
		/// <param name="app">要判断的应用。</param>
		/// <returns>如果当前用户管理者，或者是给定应用的所有者，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		private async Task<bool> CanManageAppsOrIsOwnerAsync(App app)
		{
			return await CanManageAppsAsync() || IsAppOwner(app);
		}

		/// <summary>
		/// 查看应用管理界面。
		/// </summary>
		/// <returns>操作结果。</returns>
		[HttpGet]
		[Authorize(Policies.OperateApps)]
		public async Task<IActionResult> Manage(int page = 1)
		{
			var items = from i in DbContext.Apps
						orderby i.CreateTime descending
						select i;

			return View(await items.ToPagedListAsync(20, page));
		}
	}
}