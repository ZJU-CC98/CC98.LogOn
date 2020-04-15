using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Claims;
using System.Text.Json;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     对于具有描述的声明，同时提取它的值和描述。
	/// </summary>
	/// <inheritdoc />
	public class ZjuInfoJsonClaimAction : ClaimAction
	{
		/// <summary>
		/// 获取通信证关联到的属性映射的集合。
		/// </summary>
		public ICollection<ZjuInfoJsonClaimMapping> Mappings { get; } = new Collection<ZjuInfoJsonClaimMapping>();

		/// <inheritdoc />
		public override void Run(JsonElement userData, ClaimsIdentity identity, string issuer)
		{
			// attributes 节点保存了核心数据
			var attributesNode = userData.GetProperty("attributes");

			/*
			 * 浙大通行证返回的数据将每个用户属性作为单独数组元素存放，格式如下：
			 * [
			 *   { 属性1: 值1 },
			 *   { 属性2: 值2 },
			 *   ...
			 * ]
			 *
			 * 下面的代码用于将其转换为字典
			 */
			var dataDic = new Dictionary<string, string>();

			foreach (var element in attributesNode.EnumerateArray())
			{
				foreach (var prop in element.EnumerateObject())
				{
					var propName = prop.Name;
					var propValue = prop.Value.ToString();

					dataDic.Add(propName, propValue);
				}
			}

			// 对映射表中的关系进行
			foreach (var mapping in Mappings)
			{
				// 未定义值，忽略该映射
				if (string.IsNullOrEmpty(mapping.ClaimValueName))
				{
					continue;
				}

				// 提取值
				var claimValue = dataDic.GetValueOrDefault(mapping.ClaimValueName);

				// 值不存在，则忽略该声明
				if (string.IsNullOrEmpty(claimValue))
				{
					continue;
				}

				var claim = new Claim(mapping.ClaimType,claimValue, mapping.ValueType, issuer);

				// 如果还定义了描述，则增加描述。
				if (!string.IsNullOrEmpty(mapping.ClaimDescriptionName))
				{
					var claimDescription = dataDic.GetValueOrDefault(mapping.ClaimDescriptionName);
					if (!string.IsNullOrEmpty(claimDescription))
					{
						claim.Properties.Add(ClaimProperties.Keys.Description, claimDescription);

					}
				}

				identity.AddClaim(claim);

			}
		}

		/// <summary>
		/// 初始化 <see cref="ZjuInfoJsonClaimAction"/> 对象的新实例。
		/// </summary>
		public ZjuInfoJsonClaimAction() : base(null, null)
		{
		}

	}
}