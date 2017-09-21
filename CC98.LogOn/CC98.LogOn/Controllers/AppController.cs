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
	/// �ṩ���ڽ���Ӧ�õĹ��������
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
		/// ��ȡ���ݿ������Ķ���
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// �����֤����
		/// </summary>
		private IAuthorizationService AuthorizationService { get; }

		/// <summary>
		/// ��Ϣ����
		/// </summary>
		private IOperationMessageAccessor MessageAccessor { get; }

		/// <summary>
		/// ��ͼ��ҳ��
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
		/// ��ʾ�ҵ�Ӧ�á�
		/// </summary>
		/// <param name="page">��ҳҳ�롣</param>
		/// <returns>���������</returns>
		[Authorize]
		public async Task<IActionResult> My(int page = 1)
		{
			// ��ǰ�û���
			var userName = User.GetName();

			var myItems = from i in DbContext.Apps
						  where i.OwnerUserName == userName
						  orderby i.CreateTime descending
						  select i;

			return View(await myItems.ToPagedListAsync(20, page));
		}

		/// <summary>
		/// ��ʾ����Ӧ��ҳ�档
		/// </summary>
		/// <returns>���������</returns>
		[Authorize]
		[HttpGet]
		public IActionResult Create()
		{
			return View();
		}

		/// <summary>
		/// ִ�д���Ӧ�ò�����
		/// </summary>
		/// <param name="model">����ģ�͡�</param>
		/// <returns>���������</returns>
		[Authorize]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(App model)
		{
			if (ModelState.IsValid)
			{
				// �ؼ���Ϣ
				model.Secret = Guid.NewGuid();
				model.OwnerUserName = User.Identity.Name;
				model.CreateTime = DateTimeOffset.Now;

				// ������ݿ���Ŀ
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
		/// ��ʾӦ�ñ༭���档
		/// </summary>
		/// <param name="id">Ӧ�ñ�ʶ��</param>
		/// <returns>���������</returns>
		[Authorize]
		[HttpGet]
		public async Task<IActionResult> Edit(Guid id)
		{
			var app = await LoadAppAndCheckPermissionAsync(id);
			return View(app);
		}

		/// <summary>
		/// ִ��Ӧ�ñ༭������
		/// </summary>
		/// <param name="model">����ģ�͡�</param>
		/// <returns>���������</returns>
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
		/// ִ��ɾ��������
		/// </summary>
		/// <param name="id">Ҫɾ���ı�ʶ��</param>
		/// <returns>���������</returns>
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
				MessageAccessor.Messages.Add(OperationMessageLevel.Success, "�����ɹ�",
					string.Format(CultureInfo.CurrentUICulture, "���Ѿ��ɹ�ɾ����Ӧ�� {0}", app.DisplayName));
				return RedirectToAction("My", "App");

			}
			catch (DbUpdateException ex)
			{
				return BadRequest(ex.GetMessage());
			}
		}

		/// <summary>
		/// ����Ӧ�û��ܡ�
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
		/// ���ݸ�����ʶ����Ӧ����Ϣ������鵱ǰ�û��Ƿ���Ȩ���ʸ�Ӧ�õ���Ϣ��
		/// </summary>
		/// <param name="id">Ӧ�õı�ʶ��</param>
		/// <returns>����ҵ�Ӧ�ò��ҵ�ǰ�û���Ȩ���ʸ���Ϣ������Ӧ����Ϣ�����������쳣��</returns>
		private async Task<App> LoadAppAndCheckPermissionAsync(Guid id)
		{
			// ���� APP ����
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
		/// �жϵ�ǰ�û��Ƿ����Ӧ�ù���Ȩ�ޡ�
		/// </summary>
		/// <returns>�����ǰ�û�����Ӧ�ù���Ȩ�ޣ����� <c>true</c>�����򷵻� <c>false</c>��</returns>
		private async Task<bool> CanManageAppsAsync()
		{
			var result = await AuthorizationService.AuthorizeAsync(User, Policies.OperateApps);
			return result.Succeeded;
		}

		/// <summary>
		/// �жϵ�ǰ�û��Ƿ���Ӧ�õ������ߡ�
		/// </summary>
		/// <param name="app">Ҫ�жϵ�Ӧ�á�</param>
		/// <returns>�����ǰ�û���Ӧ�õ������ߣ����� <c>true</c>�����򷵻� <c>false</c>��</returns>
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
		/// �жϵ�ǰ�û��Ƿ���Ӧ�ù����ߣ�����Ӧ�������ߡ�
		/// </summary>
		/// <param name="app">Ҫ�жϵ�Ӧ�á�</param>
		/// <returns>�����ǰ�û������ߣ������Ǹ���Ӧ�õ������ߣ����� <c>true</c>�����򷵻� <c>false</c>��</returns>
		private async Task<bool> CanManageAppsOrIsOwnerAsync(App app)
		{
			return await CanManageAppsAsync() || IsAppOwner(app);
		}

		/// <summary>
		/// �鿴Ӧ�ù�����档
		/// </summary>
		/// <returns>���������</returns>
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