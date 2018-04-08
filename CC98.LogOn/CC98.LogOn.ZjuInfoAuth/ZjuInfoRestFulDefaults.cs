namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     提供 <see cref="ZjuInfoRestFulAuthenticationOptions" /> 对象的默认值。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoRestFulDefaults
	{
		/// <summary>
		///     获取 <see cref="ZjuInfoRestFulAuthenticationOptions.AuthenticationEndpoint" /> 属性的默认值。该字段为常量。
		/// </summary>
		public const string AuthenticationEndpoint = "http://zuinfo.zju.edu.cn/v2/identify.zf";

		/// <summary>
		///     获取 <see cref="ZjuInfoRestFulAuthenticationOptions.ProfileEndpoint" /> 属性的默认值。该字段为常量。
		/// </summary>
		public const string ProfileEndpoint = "http://zuinfo.zju.edu.cn/v2/session.zf";
	}
}