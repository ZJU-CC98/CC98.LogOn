using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 为使用 <see cref="ZjuInfoOAuthHandler"/> 提供扩展方法。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoOAuthExtensions
	{
		public static AuthenticationBuilder AddZjuInfo(this AuthenticationBuilder builder)
			=> builder.AddZjuInfo(ZjuInfoOAuthDefaults.AuthenticationScheme, _ => { });

		public static AuthenticationBuilder AddZjuInfo(this AuthenticationBuilder builder, Action<ZjuInfoOAuthOptions> configureOptions)
			=> builder.AddZjuInfo(ZjuInfoOAuthDefaults.AuthenticationScheme, configureOptions);

		public static AuthenticationBuilder AddZjuInfo(this AuthenticationBuilder builder, string authenticationScheme, Action<ZjuInfoOAuthOptions> configureOptions)
			=> builder.AddZjuInfo(authenticationScheme, ZjuInfoOAuthDefaults.DisplayName, configureOptions);

		public static AuthenticationBuilder AddZjuInfo(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ZjuInfoOAuthOptions> configureOptions)
			=> builder.AddOAuth<ZjuInfoOAuthOptions, ZjuInfoOAuthHandler>(authenticationScheme, displayName, configureOptions);

	}
}