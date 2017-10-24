using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供浙大通行证身份验证服务。
	/// </summary>
	public class ZjuInfoOAuthHandler : OAuthHandler<ZjuInfoOAuthOptions>
	{
		/// <summary>
		/// 初始化一个 <see cref="ZjuInfoOAuthHandler"/> 对象的新实例。
		/// </summary>
		/// <param name="options">OAuth 验证选项。</param>
		/// <param name="logger">日志记录程序。</param>
		/// <param name="encoder">URL 编码程序。</param>
		/// <param name="clock">系统时钟服务。</param>
		[UsedImplicitly]
		public ZjuInfoOAuthHandler(IOptionsMonitor<ZjuInfoOAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder,
			ISystemClock clock)
			: base(options, logger, encoder, clock)
		{
		}

		protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity, AuthenticationProperties properties, OAuthTokenResponse tokens)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

			var response = await Backchannel.SendAsync(request, Context.RequestAborted);
			if (!response.IsSuccessStatusCode)
			{
				throw new HttpRequestException($"获取浙大通行证个人信息时无法服务器无法正常响应。状态代码：{response.StatusCode}");
			}

			var payload = JObject.Parse(await response.Content.ReadAsStringAsync());
			var regeneratedData = RebuildJsonObject(payload);

			var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options, Backchannel, tokens, regeneratedData);
			context.RunClaimActions();

			await Events.CreatingTicket(context);
			return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
		}

		/// <summary>
		/// 将服务器提供的 JSON 对象数据进行重新组合，以便于后续声明处理程序提取声明数据。
		/// </summary>
		/// <param name="data"></param>
		protected static JObject RebuildJsonObject(JObject data)
		{
			var result = new JObject();

			var attributeNode = data["attributes"];

			foreach (var token in attributeNode)
			{
				if (token is JObject obj)
				{
					foreach (var prop in obj.Properties())
					{
						result[prop.Name] = prop.Value;
					}
				}
			}

			return result;
		}
	}
}
