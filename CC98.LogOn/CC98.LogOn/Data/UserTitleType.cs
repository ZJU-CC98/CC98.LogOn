namespace CC98.LogOn.Data
{
	/// <summary>
	/// 定义用户头衔的类型。
	/// </summary>
	public enum UserTitleType
	{
		/// <summary>
		/// 和用户的论坛活跃等级相关的头衔。
		/// </summary>
		Level,
		/// <summary>
		/// 和用户的权限相关的头衔。
		/// </summary>
		Permission,
		/// <summary>
		/// 可自定义的头衔。
		/// </summary>
		Custom,
		/// <summary>
		/// 特殊头衔。
		/// </summary>
		Special
	}
}