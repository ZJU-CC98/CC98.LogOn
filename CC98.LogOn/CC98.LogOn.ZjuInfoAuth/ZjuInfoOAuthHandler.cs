using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	///     提供浙大通行证身份验证服务。
	/// </summary>
	public class ZjuInfoOAuthHandler : OAuthHandler<ZjuInfoOAuthOptions>
	{
		/// <summary>
		///     初始化一个 <see cref="ZjuInfoOAuthHandler" /> 对象的新实例。
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

		/// <inheritdoc />
		public override async Task<bool> HandleRequestAsync()
		{
			if (await ShouldHandleRequestAsync() && !Request.QueryString.HasValue && Options.RedirectPath != null &&
				Options.RedirectPath.HasValue)
			{
				Context.Response.Redirect(Options.RedirectPath.Value);
				return true;
			}

			return await base.HandleRequestAsync();
		}


		protected override async Task<AuthenticationTicket> CreateTicketAsync(ClaimsIdentity identity,
			AuthenticationProperties properties, OAuthTokenResponse tokens)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, Options.UserInformationEndpoint);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken); // 不能使用 token.TokenType 指定 scheme 因为服务器没有提供

			var response = await Backchannel.SendAsync(request, Context.RequestAborted);
			if (!response.IsSuccessStatusCode)
				throw new HttpRequestException($"获取浙大通行证个人信息时无法服务器无法正常响应。状态代码：{response.StatusCode}");

			using var payload = JsonDocument.Parse(await response.Content.ReadAsByteArrayAsync());
			var regeneratedData = payload.RootElement;

			var context = new OAuthCreatingTicketContext(new ClaimsPrincipal(identity), properties, Context, Scheme, Options,
				Backchannel, tokens, regeneratedData);
			context.RunClaimActions();

			await Events.CreatingTicket(context);
			return new AuthenticationTicket(context.Principal, context.Properties, Scheme.Name);
		}

	}
}