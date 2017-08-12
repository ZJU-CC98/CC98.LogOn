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
	/// �ṩ���ڽ���Ӧ�õĹ��������
	/// </summary>
	public class AppController : Controller
	{
		public AppController(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// ��ȡ���ݿ������Ķ���
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <summary>
		/// ��ͼ��ҳ��
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
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
		public IActionResult Create(App model)
		{
			if (ModelState.IsValid)
			{
				// �����û���
				model.OwnerUserName = User.Identity.Name;
			}

			return View(model);
		}
	}
}