using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// �ṩ���ڽ���Ӧ�õĹ��������
	/// </summary>
	public class AppController : Controller
	{
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
	}
}