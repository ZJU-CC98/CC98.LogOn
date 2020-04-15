using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using CC98.LogOn.ZjuInfoAuth;
using Sakura.AspNetCore;

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
		public IPagedList<CC98User> Users { get; set; }

		/// <summary>
		/// 获取或设置学工号关联的浙大通行证的详细信息。
		/// </summary>
		public ZjuInfoUserInfo ZjuUserInfo { get; set; }

		/// <summary>
		/// 获取或设置一个值，指示该学号是否被锁定。
		/// </summary>
		public bool IsLocked { get; set; }
	}
}
