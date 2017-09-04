namespace CC98.LogOn.Data
{
	/// <summary>
	/// 定义应用的类型。
	/// </summary>
	public enum AppState
	{
		/// <summary>
		/// 测试应用。测试应用具有所有访问权限但对于授权时间有限制。
		/// </summary>
		TestOnly = 0,
		/// <summary>
		/// 通过身份验证的普通应用。
		/// </summary>
		Normal,
		/// <summary>
		/// 受信任的开发者应用。受信任应用允许使用较为危险的授权类型。
		/// </summary>
		Trusted,
		/// <summary>
		/// 应用被吊销并无法使用。
		/// </summary>
		Revoked
	}
}