using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 执行用户管理操作界面的数据模型。
	/// </summary>
	public class AccountViewModel
	{
		/// <summary>
		/// 获取或设置要管理的用户名。
		/// </summary>
		[Required]
		[Display(Name = "用户名")]
		public string Name { get; set; }
	}
}
