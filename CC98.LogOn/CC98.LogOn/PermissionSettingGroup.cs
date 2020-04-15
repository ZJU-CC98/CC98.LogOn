namespace CC98.LogOn
{
	/// <summary>
	/// 定义权限的集合。
	/// </summary>
	public class PermissionSettingGroup
	{
		/// <summary>
		/// 查询学号的权限。
		/// </summary>
		public PermissionSetting QueryId { get; set; }
		/// <summary>
		/// 查询账户的权限。
		/// </summary>
		public PermissionSetting QueryAccount { get; set; }
		/// <summary>
		/// 管理权限。
		/// </summary>
		public PermissionSetting Admin { get; set; }
	}
}