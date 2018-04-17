namespace CC98.LogOn
{
	/// <summary>
	/// 表示对单个权限组的设置。
	/// </summary>
	public class PermissionSetting
	{
		/// <summary>
		/// 具有该权限的用户组的列表。
		/// </summary>
		public string[] Groups { get; set; }
		/// <summary>
		/// 具有该权限的用户标识的列表。
		/// </summary>
		public string[] Ids { get; set; }
	}
}