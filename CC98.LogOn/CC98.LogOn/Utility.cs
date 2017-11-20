using System;
using System.Security.Claims;
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
		/// 获取给定用户主体对象的标识。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns><paramref name="principal"/> 对象的标识。</returns>
	    public static string GetId(this ClaimsPrincipal principal)
	    {
		    return principal.FindFirstValue(ClaimTypes.NameIdentifier);
	    }
    }
}

