using System.Net.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <inheritdoc />
	/// <summary>
	///     定义浙大统一身份认证 RestFul 的相关选项。
	/// </summary>
	public class ZjuInfoRestFulAuthenticationOptions : AuthenticationSchemeOptions
	{
		public ZjuInfoRestFulAuthenticationOptions()
		{
			AuthenticationEndpoint = ZjuInfoRestFulDefaults.AuthenticationEndpoint;
			ProfileEndpoint = ZjuInfoRestFulDefaults.ProfileEndpoint;
			BackChannel = new HttpClient();

			ClaimActions.ConfigureZjuInfoClaims();
		}

		/// <summary>
		///     获取或设置用于执行身份验证的终结点地址。
		/// </summary>
		public string AuthenticationEndpoint { get; set; }

		/// <summary>
		///     获取或设置用于检索用户信息的终结点地址。
		/// </summary>
		public string ProfileEndpoint { get; set; }

		/// <summary>
		///     获取或设置用于访问服务的应用程序标识。
		/// </summary>
		public string AppKey { get; set; }

		/// <summary>
		///     获取或设置用于访问服务的应用程序机密。
		/// </summary>
		public string AppSecret { get; set; }

		/// <summary>
		///     身份验证使用的后台处理程序。
		/// </summary>
		public HttpClient BackChannel { get; set; }

		public ClaimActionCollection ClaimActions { get; } = new ClaimActionCollection();
	}
}