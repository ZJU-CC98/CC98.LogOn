using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.ZjuInfoAuth;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 表示高级账号管理页面的展示数据。
	/// </summary>
	public class AccountResultModel
	{
		/// <summary>
		/// 获取或设置要显示的 CC98 用户信息。
		/// </summary>
		public CC98User CC98UserInfo { get; set; }

		/// <summary>
		/// 获取或设置要显示的浙大通行证信息。
		/// </summary>
		public ZjuInfoUserInfo ZjuUserInfo { get; set; }
	}
}
