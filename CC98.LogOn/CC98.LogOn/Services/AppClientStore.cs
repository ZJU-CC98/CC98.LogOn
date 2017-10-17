using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using CC98.LogOn.Data;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using JetBrains.Annotations;

namespace CC98.LogOn.Services
{
	/// <inheritdoc />
	/// <summary>
	/// 提供基于数据库的客户端信息检索。
	/// </summary>
	public class AppClientStore : IClientStore
	{
		[UsedImplicitly]
		public AppClientStore(CC98IdentityDbContext dbContext)
		{
			DbContext = dbContext;
		}

		/// <summary>
		/// 获取数据库上下文对象。
		/// </summary>
		private CC98IdentityDbContext DbContext { get; }

		/// <inheritdoc />
		public async Task<Client> FindClientByIdAsync(string clientId)
		{
			if (Guid.TryParse(clientId, out var id))
			{
				var item = await DbContext.Apps.FindAsync(id);
				return ConvertAppToClient(item);
			}

			return null;
		}


		/// <summary>
		/// 将应用信息转换为客户端信息。
		/// </summary>
		/// <param name="app">应用信息。</param>
		/// <returns>客户端信息。</returns>
		public static Client ConvertAppToClient(App app)
		{
			return new Client
			{
				// APP 信息

				// ID
				ClientId = app.Id.ToString("D"),
				// 显示名称
				ClientName = app.DisplayName,
				// 机密，目前只支持一个
				ClientSecrets = new[] { new Secret(app.Secret.ToString("D")) },
				// 图标地址
				LogoUri = app.LogoUri,
				// 介绍地址
				ClientUri = app.WebPageUri,
				// 是否启用
				Enabled = app.IsEnabled,

				// 重定向 URI 集合
				RedirectUris = app.RedirectUris,

				// 允许的 CORS 来源
				AllowedCorsOrigins = app.AllowedScopes,

				// 注销后重定向地址
				PostLogoutRedirectUris = app.PostLogoutRedirectUris,
				// 允许的授权类型
				AllowedGrantTypes = ConvertGrantTypesToStrings(app.GrantTypes),
				// 允许的领域
				AllowedScopes = app.AllowedScopes,

				// 允许记住授权操作
				AllowRememberConsent = true,

				// 默认值

				// 要求授权
				RequireConsent = true,

				// 允许使用 CC98 账户本地登录
				EnableLocalLogin = true,

				// 是否允许通过浏览器获得访问令牌
				AllowAccessTokensViaBrowser = true,
				// 是否允许申请刷新令牌
				AllowOfflineAccess = true,
				// 允许使用的外部标识提供程序，空集合表示不限制
				IdentityProviderRestrictions = new string[0],

				// 标识令牌有效期，默认 5 分钟
				IdentityTokenLifetime = 300,
				// 授权码有效期，默认 1 分钟
				AuthorizationCodeLifetime = 60,
				// 授权确认操作有效期，默认 5 分钟
				ConsentLifetime = 300,
				// 访问令牌有效期，默认 1 小时
				AccessTokenLifetime = 3600,
				// 刷新令牌最长有效期，默认 30 天
				AbsoluteRefreshTokenLifetime = 2592000,
				// 刷新令牌延长有效期，默认 15 天
				SlidingRefreshTokenLifetime = 1296000,
				// 是否允许刷新令牌多次使用

				RefreshTokenUsage = TokenUsage.ReUse,
				// 刷新令牌超时模式为延展模式
				RefreshTokenExpiration = TokenExpiration.Sliding,
				// 使用刷新令牌后是否更新访问令牌
				UpdateAccessTokenClaimsOnRefresh = false,
				// 访问令牌格式，默认为 JWT
				AccessTokenType = AccessTokenType.Jwt,

				// 允许使用的声明，目前不支持
				Claims = new Claim[0],

				// 不要求必须 PK
				RequirePkce = false,
				// 不允许明文传输 PK
				AllowPlainTextPkce = false,

				// 必须提供机密
				RequireClientSecret = false,

				// 不始终发送客户端声明
				AlwaysSendClientClaims = true,
				// 不始终包含用户声明
				AlwaysIncludeUserClaimsInIdToken = true,

				// 是否要求后台注销会话
				BackChannelLogoutSessionRequired = false,
				// 是否要求前台注销会话
				FrontChannelLogoutSessionRequired = false,
				// 后台和前台注销会话地址
				BackChannelLogoutUri = null,
				FrontChannelLogoutUri = null,

				// 始终包含 JWT 标识
				IncludeJwtId = true,
				// 支持的协议类型
				ProtocolType = IdentityServerConstants.ProtocolTypes.OpenIdConnect,
			

			};
		}

		/// <summary>
		/// 将 <see cref="AppGrantTypes"/> 枚举值转换为 <see cref="Client.AllowedGrantTypes"/> 所需的字符串集合。
		/// </summary>
		/// <param name="value">要转换的 <see cref="AppGrantTypes"/> 。</param>
		/// <returns>转换后的授权类型字符串集合。</returns>
		[NotNull]
		[ItemNotNull]
		private static string[] ConvertGrantTypesToStrings(AppGrantTypes value)
		{
			var result = new List<string>();

			if (value.HasFlag(AppGrantTypes.AuthorizationCode))
			{
				result.Add(GrantType.AuthorizationCode);
			}
			if (value.HasFlag(AppGrantTypes.Implicit))
			{
				result.Add(GrantType.Implicit);
			}
			if (value.HasFlag(AppGrantTypes.Hybrid))
			{
				result.Add(GrantType.Hybrid);
			}
			if (value.HasFlag(AppGrantTypes.ClientCredentials))
			{
				result.Add(GrantType.ClientCredentials);
			}
			if (value.HasFlag(AppGrantTypes.ResourceOwnerPassword))
			{
				result.Add(GrantType.ResourceOwnerPassword);
			}

			return result.ToArray();
		}
	}
}

