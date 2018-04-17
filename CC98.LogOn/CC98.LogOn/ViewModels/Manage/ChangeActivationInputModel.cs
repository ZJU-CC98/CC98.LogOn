using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 表示执行更改激活信息操作需要提供的信息。
	/// </summary>
	public class ChangeActivationInputModel
	{
		/// <summary>
		/// 获取或设置一个值，指示账号是否已经被激活。
		/// </summary>
		public bool IsActivated { get; set; }

		/// <summary>
		/// 获取或设置账号要关联到的浙大通行证标识。
		/// </summary>
		public string ZjuInfoId { get; set; }

		/// <summary>
		/// 获取或设置要操作的 CC98 账户名称。
		/// </summary>
		[Required]
		public string CC98UserName { get; set; }
	}
}
