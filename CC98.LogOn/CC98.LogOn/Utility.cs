using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using JetBrains.Annotations;

namespace CC98.LogOn
{
	/// <summary>
	///     提供辅助方法。该类型为静态类型。
	/// </summary>
	public static class Utility
	{
		/// <summary>
		///     获取异常的最终错误消息。
		/// </summary>
		/// <param name="ex">异常对象。</param>
		/// <returns>异常对象的最终错误消息。</returns>
		public static string GetMessage([NotNull] this Exception ex)
		{
			if (ex == null)
				throw new ArgumentNullException(nameof(ex));

			return ex.GetBaseException().Message;
		}

		/// <summary>
		///     获取给定用户主体对象的标识。
		/// </summary>
		/// <param name="principal">用户主体对象。</param>
		/// <returns><paramref name="principal" /> 对象的标识。</returns>
		public static string GetId(this ClaimsPrincipal principal)
		{
			return principal.FindFirstValue(ClaimTypes.NameIdentifier);
		}

		/// <summary>
		/// 判断集合中是否存在给定项目。如果集合为 <c>null</c> 则视为不存在。
		/// </summary>
		/// <typeparam name="T">集合中元素的类型。</typeparam>
		/// <param name="source">要检索的集合。</param>
		/// <param name="value">要检索的项目。</param>
		/// <returns>如果 <paramref name="source"/> 不为 <c>null</c> 并且包含 <paramref name="value"/>，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool NotNullAndContains<T>(this IEnumerable<T> source, T value) => source?.Contains(value) ?? false;

		/// <summary>
		/// 判断两个集合是否具有交集。
		/// </summary>
		/// <typeparam name="T">集合的元素类型。</typeparam>
		/// <param name="source1">集合 1。</param>
		/// <param name="source2">集合 2。</param>
		/// <returns>如果 <paramref name="source1"/> 和 <paramref name="source2"/> 至少有一个公共元素，返回 <c>true</c>；否则返回 <c>false</c>。</returns>
		public static bool IsIntersectedWith<T>(this IEnumerable<T> source1, IEnumerable<T> source2)
		{
			if (source1 == null || source2 == null)
			{
				return false;
			}

			return source1.Intersect(source2).Any();
		}
	}
}