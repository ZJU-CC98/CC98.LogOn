using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	///     主要功能控制器。
	/// </summary>
	public class HomeController : Controller
	{
		/// <summary>
		///     显示主页。
		/// </summary>
		/// <returns>操作结果。</returns>
		public IActionResult Index()
		{
			return View();
		}
	}
}