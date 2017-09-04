using System;

namespace CC98.LogOn.Data
{
	/// <summary>
	/// 指定应用允许的授权类型。
	/// </summary>
	[Flags]
	public enum AppGrantTypes
	{
		/// <summary>
		/// 不允许任何授权。
		/// </summary>
		None = 0x0,
		/// <summary>
		/// 应用程序授权码验证。
		/// </summary>
		AuthorizationCode = 0x1,
		/// <summary>
		/// 隐式授权验证。
		/// </summary>
		Implicit = 0x2,
		/// <summary>
		/// 客户端凭据授权验证。
		/// </summary>
		ClientCredentials = 0x4,
		/// <summary>
		/// 混合授权验证。
		/// </summary>
		Hybrid = 0x8,
		/// <summary>
		/// 所有者密码授权验证。
		/// </summary>
		ResourceOwnerPassword = 0x10

	}
}