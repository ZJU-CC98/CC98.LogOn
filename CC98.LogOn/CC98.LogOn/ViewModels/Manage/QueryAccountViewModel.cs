using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{

	/// <summary>
	/// 定义查询账号需要提供的数据。
	/// </summary>
	public class QueryAccountViewModel
	{
		/// <summary>
		/// 要查询的账号名称。
		/// </summary>
		[Required]
		[Display(Name = "账号名")]
		public string Name { get; set; }
	}
}
