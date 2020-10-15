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

		/// <summary>
		/// 获取或设置一个值，指示注册账号时是否强制要求浙大通行证信息绑定。
		/// </summary>
		public bool ForceZjuInfoIdBind { get; set; }

		/// <summary>
		/// 获取或设置用户许可协议的地址。
		/// </summary>
		public string TermsAddress { get; set; }

		/// <summary>
		/// 电子邮件支持地址。
		/// </summary>
		public string SupportEmailLink { get; set; }

		/// <summary>
		/// QQ 群支持地址。
		/// </summary>
		public string SupportQQGroupLink { get; set; }

	}
}