using System;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 为  <see cref="DescriptedCustomJsonClaimAction"/> 相关的操作提供扩展方法。该类型为静态类型。
	/// </summary>
	public static class DescriptedClaimActionExtensions
	{
		/// <summary>
		/// 添加
		/// </summary>
		/// <param name="claimActions">定义所有声明操作的集合。</param>
		/// <param name="claimType">声明的类型。</param>
		/// <param name="valueType">声明的值的类型。</param>
		/// <param name="valueResolver">从自定义 <see cref="JObject"/> 对象中提取声明的值的委托。</param>
		/// <param name="descriptionResolver">从自定义 <see cref="JObject"/> 对象中提取声明的描述的委托。</param>
		public static void MapCustomJsonWithDescription(this ClaimActionCollection claimActions, string claimType, string valueType,
			Func<JObject, string> valueResolver, Func<JObject, string> descriptionResolver = null)
		{
			claimActions.Add(new DescriptedCustomJsonClaimAction(claimType, valueType, valueResolver, descriptionResolver));
		}
	}
}