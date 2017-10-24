using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Extensions;
using JetBrains.Annotations;

namespace CC98.LogOn
{
	/// <summary>
	/// 提供辅助方法。该类型为静态类型。
	/// </summary>
	public static class Utility
	{
		/// <summary>
		/// 获取异常的最终错误消息。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>异常对象的最终错误消息。</returns>
		public static string GetMessage([NotNull] this Exception ex)
		{
			if (ex == null)
			{
				throw new ArgumentNullException(nameof(ex));
			}

			return ex.GetBaseException().Message;
		}



		/// <summary>
		/// 判断用户主体是否通过了给定验证方法的验证。
		/// </summary>
		/// <param name="principal">要验证的用户主体。</param>
		/// <param name="authenticationScheme">要判断的验证方法。</param>
		/// <returns>如果 <paramref name="principal"/> 对象的标识中具有 <paramref name="authenticationScheme"/> 所定义的验证信息，则为 <c>true</c>；否则为 <c>false</c>。</returns>
		public static bool IsAuthenticatedWith(this ClaimsPrincipal principal, string authenticationScheme)
		{
			return principal.Identities.Any(i => i.TryGetAuthenticationMethod() == authenticationScheme);
		}

		/// <summary>
		/// 获取用户主体在给定验证方法下的用户名。
		/// </summary>
		/// <param name="principal">要获取用户名的主体对象。</param>
		/// <param name="authenticationScheme">给定的验证方法。</param>
		/// <returns>获取 <paramref name="principal"/> 对象中在 <paramref name="authenticationScheme"/> 方法下定义的用户名。如果未找到该信息，则返回 <c>null</c>。</returns>
		public static string GetNameWith(this ClaimsPrincipal principal, string authenticationScheme)
		{
			return principal.Identities.FirstOrDefault(i => i.TryGetAuthenticationMethod() == authenticationScheme)?.GetName();
		}

		/// <summary>
		/// 尝试找出某个用户标识的 IdentityServer 验证方法。如果失败则返回 <c>null</c>。
		/// </summary>
		/// <param name="identity">用户标识对象。</param>
		/// <returns>用户标识中 IdentityServer</returns>

		private static string TryGetAuthenticationMethod(this IIdentity identity)
		{
			if (identity is ClaimsIdentity claimsIdentity)
			{
				return claimsIdentity.FindFirst(JwtClaimTypes.AuthenticationMethod)?.Value;
			}

			return null;
		}

		/// <summary>
		/// 讲一个标准 <see cref="ClaimsPrincipal"/> 对象转换为 IdentityServer 所使用的对象。
		/// </summary>
		/// <param name="principal">要转换的标准对象。</param>
		/// <param name="provider">身份验证提供程序。</param>
		/// <returns>转换后的对象。</returns>
		public static ClaimsPrincipal CreateIdentityServerPrincipal(this ClaimsPrincipal principal, string provider)
		{
			var authMethods = new[] { provider };

			return IdentityServerPrincipal.Create(principal.FindFirstValueOfAny(ClaimTypes.NameIdentifier, JwtClaimTypes.Subject),
				principal.FindFirstValueOfAny(ClaimTypes.Name, JwtClaimTypes.Name), provider, authMethods, DateTime.UtcNow, principal.Claims.ToArray());
		}

		/// <summary>
		/// 查找给定主体对象中若干声明类型中的第一个值。
		/// </summary>
		/// <param name="principal">主体对象。</param>
		/// <param name="claimTypes">要查找的多个声明的类型。</param>
		/// <returns>声明类型在 <paramref name="claimTypes"/> 中的第一个声明的值。</returns>
		public static string FindFirstValueOfAny(this ClaimsPrincipal principal, params string[] claimTypes)
		{
			return principal.FindFirst(i => claimTypes.Contains(i.Type))?.Value;
		}
	}
}

