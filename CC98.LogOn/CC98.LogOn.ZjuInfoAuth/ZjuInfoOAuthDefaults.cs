using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大统一身份验证相关选项的默认值。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoOAuthDefaults
	{
		/// <summary>
		/// 提供 <see cref="ZjuInfoOAuthOptions.AuthorizationEndpoint"/> 属性的默认值。该字段为常量。
		/// </summary>
		public const string AuthorizationEndpoint = "https://zjuam.zju.edu.cn/cas/oauth2.0/authorize";

		/// <summary>
		/// 提供 <see cref="ZjuInfoOAuthOptions.TokenEndpoint"/> 属性的默认值。该字段为常量。
		/// </summary>
		public const string TokenEndpoint = "https://zjuam.zju.edu.cn/cas/oauth2.0/accessToken";

		/// <summary>
		/// 提供 <see cref="ZjuInfoOAuthOptions.UserInformationEndpoint"/> 属性的默认值。该字段为常量。
		/// </summary>
		public const string UserInformationEndpoint = "https://zjuam.zju.edu.cn/cas/oauth2.0/profile";

		/// <summary>
		/// 浙大统一身份认证的默认验证架构名称。该字段为常量。
		/// </summary>
		public const string AuthenticationScheme = "ZjuInfo";

		/// <summary>
		/// 浙大统一身份认证的默认显示名称。
		/// </summary>
		public const string DisplayName = "浙大通行证";
	}
}