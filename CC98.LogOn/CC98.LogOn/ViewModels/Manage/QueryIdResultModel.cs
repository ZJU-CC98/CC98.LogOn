using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.ZjuInfoAuth;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 表示查询学号的结果。
	/// </summary>
	public class QueryIdResultModel
	{
		/// <summary>
		/// 获取或设置学工号。
		/// </summary>
		public string SchoolId { get; set; }

		/// <summary>
		/// 获取或设置学工号关联的用户。
		/// </summary>
		public CC98User[] Users { get; set; }

		/// <summary>
		/// 获取或设置学工号关联的浙大通行证的详细信息。
		/// </summary>
		public ZjuInfoUserInfo ZjuUserInfo { get; set; }
	}
}
