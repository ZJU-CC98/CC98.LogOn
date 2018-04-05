using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace CC98.LogOn.Controllers
{
	/// <summary>
	/// 提供管理相关功能。
	/// </summary>
    public class ManageController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}