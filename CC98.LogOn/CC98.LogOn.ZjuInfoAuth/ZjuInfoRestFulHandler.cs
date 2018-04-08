using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     提供对于浙大统一身份认证 RestFul 模式的实现。
	/// </summary>
	public class ZjuInfoRestFulHandler : AuthenticationHandler<ZjuInfoRestFulAuthenticationOptions>
	{
		/// <inheritdoc />
		public ZjuInfoRestFulHandler(IOptionsMonitor<ZjuInfoRestFulAuthenticationOptions> options, ILoggerFactory logger,
			UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
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

			var authResponse =
				await Options.BackChannel.PostAsync(Options.AuthenticationEndpoint, new FormUrlEncodedContent(authenticationData));
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
				return AuthenticateResult.Fail(result.ErrorMessage);

			var profileData = new Dictionary<string, string>
			{
				["appkey"] = Options.AppKey,
				["appsecret"] = Options.AppSecret,
				["token"] = result.Token
			};

			var profileResponse =
				await Options.BackChannel.PostAsync(Options.ProfileEndpoint, new FormUrlEncodedContent(profileData));
			var profileContent = await profileResponse.Content.ReadAsStringAsync();

			if (!profileResponse.IsSuccessStatusCode)
			{
				Logger.LogError("这大通行证身份验证服务器在获取个人信息阶段无法正常返回响应。响应代码:{0}，内容:{1}", profileResponse.StatusCode, profileContent);
				return AuthenticateResult.Fail("浙大通行证无法正常进行身份验证");
			}

			var jObj = JObject.Parse(profileContent);

			// 是否具有错误对象
			if (jObj.Property("errorcode") != null)
				return AuthenticateResult.Fail(jObj.Value<string>("errormsg"));

			// 处理所有令牌操作
			var identity = new ClaimsIdentity();
			foreach (var claimAction in Options.ClaimActions)
				claimAction.Run(jObj, identity, Scheme.Name);

			// 操作结果
			return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), null, Scheme.Name));
		}
	}
}