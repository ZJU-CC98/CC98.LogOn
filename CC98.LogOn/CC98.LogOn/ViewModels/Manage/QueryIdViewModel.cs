using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 定义查询学号功能需要的用户输入。
	/// </summary>
	public class QueryIdViewModel
	{
		/// <summary>
		/// 获取或设置用户要查询的账号或者学号。
		/// </summary>
		[Required]
		public string Id { get; set; }

		/// <summary>
		/// 获取或设置用户输入的账号的类型。
		/// </summary>
		[Required]
		public QueryIdType Type { get; set; }
	}
}
