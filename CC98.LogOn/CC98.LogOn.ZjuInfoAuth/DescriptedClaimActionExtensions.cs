using System;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 为  <see cref="DescriptedJsonKeyClaimAction"/> 相关的操作提供扩展方法。该类型为静态类型。
	/// </summary>
	public static class DescriptedClaimActionExtensions
	{
		/// <summary>
		/// 用于添加 <see cref="DescriptedJsonKeyClaimAction"/> 的扩展方法。
		/// </summary>
		/// <param name="claimActions">定义所有声明操作的集合。</param>
		/// <param name="claimType">声明的类型。</param>
		/// <param name="valueType">声明的值的类型。</param>
		/// <param name="jsonKey">声明数据对应的 JSON 键。</param>
		/// <param name="descriptionJsonKey">描述数据对应的 JSON 键。</param>
		public static void MapJsonWithDescription(this ClaimActionCollection claimActions, string claimType, string valueType,
			string jsonKey, string descriptionJsonKey = null)
		{
			claimActions.Add(new DescriptedJsonKeyClaimAction(claimType, valueType, jsonKey, descriptionJsonKey));
		}
	}
}