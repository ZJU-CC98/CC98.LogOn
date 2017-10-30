namespace CC98.LogOn
{
	/// <summary>
	/// 定义应用程序使用的身份验证策略。该类型为静态类型。
	/// </summary>
    public static class Policies
	{
		/// <summary>
		/// 定义完全管理应用系统的策略。
		/// </summary>
		public const string AdminApps = nameof(AdminApps);

		/// <summary>
		/// 定义操作应用系统的策略。
		/// </summary>
		public const string OperateApps = nameof(OperateApps);

		/// <summary>
		/// 定义应用程序中的角色。该类型为静态类型。
		/// </summary>
	    public static class Roles
		{
			/// <summary>
			/// 表示系统管理员角色。系统管理员可以进行无限制的修改。该字段为常量。
			/// </summary>
			public const string Administrators = "Administrators";

			/// <summary>
			/// 表示操作员角色。账户操作员可以修改其他账户的权限信息，但不能将自己提升为管理员。该字段为常量。
			/// </summary>
			public const string AccountOperators = "Account Operators";

			/// <summary>
			/// 表示应用管理员角色。应用管理员可以对应用系统执行所有操作。该字段为常量。
			/// </summary>
			public const string AppAdministrators = "App Administrators";

			/// <summary>
			/// 表示应用操作员角色。应用操作员可以管理应用系统的大部分功能，但不能修改其他人的权限。该字段为常量。
			/// </summary>
			public const string AppOperators = "App Operators";
		}
    }
}
