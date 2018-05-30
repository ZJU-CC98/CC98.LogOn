using CC98.LogOn.Data;
using Sakura.AspNetCore;

namespace CC98.LogOn.ViewModels.Manage
{
	/// <summary>
	/// 定义查询账号的结果。
	/// </summary>
	public class QueryAccountResultModel
	{
		/// <summary>
		/// 被查询的账号。
		/// </summary>
		public string QueryUserName { get; set; }

		/// <summary>
		/// 被查询的账号关联的其他账号信息。
		/// </summary>
		public IPagedList<CC98User> Users { get; set; }
	}
}