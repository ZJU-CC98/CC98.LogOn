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
	public class DescriptedCustomJsonClaimAction : CustomJsonClaimAction
	{

		public Func<JObject, string> DescriptionResolver { get; }

		/// <inheritdoc />
		public override void Run(JObject userData, ClaimsIdentity identity, string issuer)
		{
			if (Resolver == null)
			{
				return;
			}

			var claimValue = Resolver(userData);
			var claimDescription = DescriptionResolver?.Invoke(userData);

			if (string.IsNullOrEmpty(claimValue))
			{
				return;
			}

			var claim = new Claim(ClaimType, claimValue, ValueType, issuer);
			if (!string.IsNullOrEmpty(claimDescription))
			{
				claim.Properties.Add(ClaimProperties.Keys.Description, claimDescription);
			}
		}

		/// <inheritdoc />
		public DescriptedCustomJsonClaimAction(string claimType, string valueType, Func<JObject, string> resolver, Func<JObject, string> descriptionResolver) : base(claimType, valueType, resolver)
		{
			DescriptionResolver = descriptionResolver;
		}
	}
}