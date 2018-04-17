using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 定义删除用户需要提供的数据。
	/// </summary>
	public class DeleteUserInputModel
	{
		/// <summary>
		/// 获取或设置要操作的 CC98 账户名称。
		/// </summary>
		[Required]
		public string CC98UserName { get; set; }
	}
}
