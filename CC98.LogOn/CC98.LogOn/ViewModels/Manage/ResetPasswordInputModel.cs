using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 表示激活操作要提供的数据模型。
	/// </summary>
	public class ResetPasswordInputModel
	{
		/// <summary>
		/// 获取或设置要操作的 CC98 账户名称。
		/// </summary>
		[Required]
		public string CC98UserName { get; set; }

		/// <summary>
		/// 要提供的新密码。
		/// </summary>
		public string NewPassword { get; set; }
	}
}
