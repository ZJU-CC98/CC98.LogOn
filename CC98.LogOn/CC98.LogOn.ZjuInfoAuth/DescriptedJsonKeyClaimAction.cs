using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 对于具有描述的声明，同时提取它的值和描述。
	/// </summary>
	/// <inheritdoc />
	public class DescriptedJsonKeyClaimAction : JsonKeyClaimAction
	{
		/// <summary>
		/// 描述对象的 JSON 键名称。
		/// </summary>
		public string DescriptionJsonKey { get; }

		/// <inheritdoc />
		public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
		{

			var claimValue = userData.Value<string>(JsonKey);
			var claimDescription = string.IsNullOrEmpty(DescriptionJsonKey) ? null : userData.Value<string>(DescriptionJsonKey);

			if (string.IsNullOrEmpty(claimValue))
			{
				return;
			}

			var claim = new Claim(ClaimType, claimValue, ValueType, issuer);
			if (!string.IsNullOrEmpty(claimDescription))
			{
				claim.Properties.Add(ClaimProperties.Keys.Description, claimDescription);
			}

			identity.AddClaim(claim);
		}

		public DescriptedJsonKeyClaimAction(string claimType, string valueType, string jsonKey, string descriptionJsonKey)
			: base(claimType, valueType, jsonKey)
		{
			DescriptionJsonKey = descriptionJsonKey;
		}
	}
}