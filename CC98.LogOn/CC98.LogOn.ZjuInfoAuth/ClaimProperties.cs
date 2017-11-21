using System.Security.Claims;
using JetBrains.Annotations;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     提供声明的其他辅助属性相关操作。该类型为静态类型。
	/// </summary>
	[PublicAPI]
	public static class ClaimProperties
	{
		/// <summary>
		///     获取声明中具有给定键的属性值。
		/// </summary>
		/// <param name="claim">要获取属性值的声明对象。</param>
		/// <param name="key">要获取的属性的键。</param>
		/// <returns>具有给定键的声明的属性值。如果找不到该属性，则返回 <c>null</c>。</returns>
		public static string GetProperty(this Claim claim, string key)
		{
			return claim.Properties.TryGetValue(key, out var result) ? result : null;
		}

		/// <summary>
		///     查找给定用户标识中给定用户声明的给定属性的值。
		/// </summary>
		/// <param name="identity">用户声明对象。</param>
		/// <param name="claimType">用户声明类型。</param>
		/// <param name="propertyName">属性类型。</param>
		/// <returns>用户标识中给定声明中给定属性的值。如果声明或者值不存在，则返回 <c>null</c>。</returns>
		public static string GetProperty(this ClaimsIdentity identity, string claimType, string propertyName)
		{
			return identity.FindFirst(claimType)?.GetProperty(propertyName);
		}

		/// <summary>
		///     查找给定用户主体中给定用户声明的给定属性的值。
		/// </summary>
		/// <param name="principal">用户声明对象。</param>
		/// <param name="claimType">用户声明类型。</param>
		/// <param name="propertyName">属性类型。</param>
		/// <returns>用户主体中给定声明中给定属性的值。如果声明或者值不存在，则返回 <c>null</c>。</returns>
		public static string GetProperty(this ClaimsPrincipal principal, string claimType, string propertyName)
		{
			return principal.FindFirst(claimType)?.GetProperty(propertyName);
		}

		/// <summary>
		///     包含声其它属性的键名称。
		/// </summary>
		public static class Keys
		{
			/// <summary>
			///     表示声明的描述。描述用于补充声明的值。
			/// </summary>
			public const string Description = nameof(Description);
		}
	}
}