using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供对于浙大统一身份认证 RestFul 模式的实现。
	/// </summary>
	public class ZjuInfoRestFulHandler : AuthenticationHandler<ZjuInfoRestFulAuthenticationOptions>
	{
		/// <inheritdoc />
		public ZjuInfoRestFulHandler(IOptionsMonitor<ZjuInfoRestFulAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
		{
		}

		/// <inheritdoc />
		protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
		{
			var authenticationData = new Dictionary<string, string>
			{
				["appkey"] = Options.AppKey,
				["appsecret"] = Options.AppSecret,
				["username"] = Request.Form["UserName"],
				["password"] = Request.Form["Password"]
			};

			var authResponse = await Options.BackChannel.PostAsync(Options.AuthenticationEndpoint, new FormUrlEncodedContent(authenticationData));
			var authContent = await authResponse.Content.ReadAsStringAsync();

			// 代码检测
			if (!authResponse.IsSuccessStatusCode)
			{
				Logger.LogError("这大通行证身份验证服务器在验证阶段无法正常返回响应。响应代码:{0}，内容:{1}", authResponse.StatusCode, authContent);
				return AuthenticateResult.Fail("浙大通行证无法正常进行身份验证");
			}

			var result = JsonConvert.DeserializeObject<ZjuInfoRestFulAuthenticationResult>(authContent);

			// 发生错误
			if (result.ErrorCode != 0)
			{
				return AuthenticateResult.Fail(result.ErrorMessage);
			}

			var profileData = new Dictionary<string, string>
			{
				["appkey"] = Options.AppKey,
				["appsecret"] = Options.AppSecret,
				["token"] = result.Token
			};

			var profileResponse = await Options.BackChannel.PostAsync(Options.ProfileEndpoint, new FormUrlEncodedContent(profileData));
			var profileContent = await profileResponse.Content.ReadAsStringAsync();

			if (!profileResponse.IsSuccessStatusCode)
			{
				Logger.LogError("这大通行证身份验证服务器在获取个人信息阶段无法正常返回响应。响应代码:{0}，内容:{1}", profileResponse.StatusCode, profileContent);
				return AuthenticateResult.Fail("浙大通行证无法正常进行身份验证");
			}

			var jObj = JObject.Parse(profileContent);

			// 是否具有错误对象
			if (jObj.Property("errorcode") != null)
			{
				return AuthenticateResult.Fail(jObj.Value<string>("errormsg"));
			}

			// 处理所有令牌操作
			var identity = new ClaimsIdentity();
			foreach (var claimAction in Options.ClaimActions)
			{
				claimAction.Run(jObj, identity, Scheme.Name);
			}

			// 操作结果
			return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), null, Scheme.Name));
		}
	}

	/// <summary>
	/// 表示浙大通行证进行身份验证时产生的结果。
	/// </summary>
	internal class ZjuInfoRestFulAuthenticationResult
	{
		/// <summary>
		/// 错误代码。如果为 0 表示未发生错误。
		/// </summary>
		[JsonProperty("errorcode")]
		public int ErrorCode { get; set; }
		/// <summary>
		/// 错误描述信息。
		/// </summary>
		[JsonProperty("errormsg")]
		public string ErrorMessage { get; set; }
		/// <summary>
		/// 成功时返回的令牌对象。
		/// </summary>
		[JsonProperty("token")]
		public string Token { get; set; }
	}

	/// <inheritdoc />
	/// <summary>
	/// 定义浙大统一身份认证 RestFul 的相关选项。
	/// </summary>
	public class ZjuInfoRestFulAuthenticationOptions : AuthenticationSchemeOptions
	{
		/// <summary>
		/// 获取或设置用于执行身份验证的终结点地址。
		/// </summary>
		public string AuthenticationEndpoint { get; set; }

		/// <summary>
		/// 获取或设置用于检索用户信息的终结点地址。
		/// </summary>
		public string ProfileEndpoint { get; set; }

		/// <summary>
		/// 获取或设置用于访问服务的应用程序标识。
		/// </summary>
		public string AppKey { get; set; }

		/// <summary>
		/// 获取或设置用于访问服务的应用程序机密。
		/// </summary>
		public string AppSecret { get; set; }

		/// <summary>
		/// 身份验证使用的后台处理程序。
		/// </summary>
		public HttpClient BackChannel { get; set; }

		public ClaimActionCollection ClaimActions { get; } = new ClaimActionCollection();

		public ZjuInfoRestFulAuthenticationOptions()
		{
			AuthenticationEndpoint = ZjuInfoRestFulDefaults.AuthenticationEndpoint;
			ProfileEndpoint = ZjuInfoRestFulDefaults.ProfileEndpoint;
			BackChannel = new HttpClient();

			ClaimActions.ConfigureZjuInfoClaims();
		}
	}

	/// <summary>
	/// 提供 <see cref="ZjuInfoRestFulAuthenticationOptions"/> 对象的默认值。该类型为静态类型。
	/// </summary>
	public static class ZjuInfoRestFulDefaults
	{
		/// <summary>
		/// 获取 <see cref="ZjuInfoRestFulAuthenticationOptions.AuthenticationEndpoint"/> 属性的默认值。该字段为常量。
		/// </summary>
		public const string AuthenticationEndpoint = "http://zuinfo.zju.edu.cn/v2/identify.zf";
		/// <summary>
		/// 获取 <see cref="ZjuInfoRestFulAuthenticationOptions.ProfileEndpoint"/> 属性的默认值。该字段为常量。
		/// </summary>
		public const string ProfileEndpoint = "http://zuinfo.zju.edu.cn/v2/session.zf";
	}
}
