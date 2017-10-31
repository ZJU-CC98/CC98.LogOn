namespace CC98.LogOn.ViewModels.Account
{
	/// <summary>
	/// 表示确认界面使用的数据模型。
	/// </summary>
	public class ConsentViewModel
	{
		/// <summary>
		/// 获取或设置一个值，指示是否要记住确认操作的结果。
		/// </summary>
		public bool RememberConsent { get; set; }

		/// <summary>
		/// 获取或设置用户确认的授权领域。
		/// </summary>
		public string[] Scopes { get; set; }

		public bool IsConsent { get; set; }

		/// <summary>
		/// 获取或设置确认的结果。
		/// </summary>
		public ConsentResult Result { get; set; }

        public string ReturnUrl { get; set; }
	}

	/// <summary>
	/// 表示确认操作的结果。
	/// </summary>
	public enum ConsentResult
	{
		/// <summary>
		/// 用户执行了同意操作。
		/// </summary>
		Granted,
		/// <summary>
		/// 用户执行了拒绝操作。
		/// </summary>
		Denied
	}
}
