using System.ComponentModel.DataAnnotations;

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
		[Display(Name = "未审核")]
		TestOnly = 0,
		/// <summary>
		/// 通过身份验证的普通应用。
		/// </summary>
		[Display(Name = "已审核")]
		Normal,
		/// <summary>
		/// 受信任的开发者应用。受信任应用允许使用较为危险的授权类型。
		/// </summary>
		[Display(Name = "受信任")]
		Trusted,
		/// <summary>
		/// 应用被吊销并无法使用。
		/// </summary>
		[Display(Name = "已吊销")]
		Revoked
	}
}