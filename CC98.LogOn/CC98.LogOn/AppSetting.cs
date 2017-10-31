using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CC98.LogOn
{
	/// <summary>
	/// 定义应用程序的设置。
	/// </summary>
    public class AppSetting
    {
		/// <summary>
		/// 获取或设置单个浙大通行证账号最多允许关联的 CC98 账号数目。
		/// </summary>
		public int MaxCC98AccountPerZjuInfoId { get; set; }

		/// <summary>
		/// 获取或设置相对路径头像的基路径。
		/// </summary>
		public string BaseUriForPortrait { get; set; }
    }
}
