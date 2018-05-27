using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn
{

	/// <summary>
	/// 定义管理功能的模式。
	/// </summary>
    public enum ManagementMode
    {
		/// <summary>
		/// 不具有任何管理功能。
		/// </summary>
		None = 0,
		/// <summary>
		/// 自我管理功能。
		/// </summary>
		Self,
		/// <summary>
		/// 管理员管理功能。
		/// </summary>
		Admin
    }
}
