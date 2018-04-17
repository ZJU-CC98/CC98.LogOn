namespace CC98.LogOn
{
	/// <summary>
	///     定义应用程序的设置。
	/// </summary>
	public class AppSetting
	{
		/// <summary>
		///     获取或设置单个浙大通行证账号最多允许关联的 CC98 账号数目。
		/// </summary>
		public int MaxCC98AccountPerZjuInfoId { get; set; }

		/// <summary>
		///     获取或设置相对路径头像的基路径。
		/// </summary>
		public string BaseUriForPortrait { get; set; }

		/// <summary>
		/// 获取或设置权限设置。
		/// </summary>
		public PermissionSettingGroup Permissions { get; set; }

	}

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