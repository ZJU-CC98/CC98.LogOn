using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <inheritdoc />
	/// <summary>
	///     定义浙大通行证 OAuth 验证的相关选项。
	/// </summary>
	public class ZjuInfoOAuthOptions : OAuthOptions
	{
		/// <summary>
		///     用默认配置初始化一个 <see cref="ZjuInfoOAuthOptions" /> 对象的新实例。
		/// </summary>
		public ZjuInfoOAuthOptions()
		{
			CallbackPath = new PathString("/signin-zjuinfo");

			AuthorizationEndpoint = ZjuInfoOAuthDefaults.AuthorizationEndpoint;
			TokenEndpoint = ZjuInfoOAuthDefaults.TokenEndpoint;
			UserInformationEndpoint = ZjuInfoOAuthDefaults.UserInformationEndpoint;

			ClaimActions.ConfigureZjuInfoClaims();
		}

		/// <summary>
		///     如果该属性不为<c>null</c>，则当用户直接访问 <see cref="OAuthOptions.CallbackPath" /> 时，将被重定向至给定的地址。
		/// </summary>
		public PathString RedirectPath { get; set; }
	}
}