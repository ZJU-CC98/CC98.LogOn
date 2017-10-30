using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CC98.LogOn
{
	/// <summary>
	/// 提供对于数据的部分覆盖功能。该类型为静态类型。
	/// </summary>
	public static class DataPatchExtensions
	{
		/// <summary>
		/// 对给定的对象进行部分数据覆盖，并同时指定覆盖时包含和排除的属性。
		/// </summary>
		/// <typeparam name="T">要覆盖的数据的类型。</typeparam>
		/// <typeparam name="TInclude">用于表示 <paramref name="includes"/> 表达式的参数类型。</typeparam>
		/// <typeparam name="TExclude">用于表示 <paramref name="excludes"/> 表达式的参数类型。</typeparam>
		/// <param name="source">要进行覆盖操作的对象，该对象包含一个或多个需要被覆盖的属性。</param>
		/// <param name="patch">提供覆盖值的对象，该对象包含一个或多个将用于覆盖 <paramref name="source"/> 中同名属性值的属性。</param>
		/// <param name="includes">如果该参数不为 <c>null</c>，则只有该参数对应的表达式的返回类型中包含的属性才会被覆盖。</param>
		/// <param name="excludes">如果该参数不为 <c>null</c>，则该参数对应的表达式的返回类型中包含的属性将不会被覆盖。</param>
		/// <param name="ignoreCase">指定在检索和对比属性名称时是否区分大小写，默认值为 <c>false</c>。</param>
		public static void Patch<T, TInclude, TExclude>(this T source, object patch,
			Expression<Func<T, TInclude>> includes, Expression<Func<T, TExclude>> excludes, bool ignoreCase = false)
		{

			var includeNames = typeof(TInclude).GetPropertyNamesForType(PropertyAccessRequirement.None);
			var excludeNames = typeof(TExclude).GetPropertyNamesForType(PropertyAccessRequirement.None);

			Patch(source, patch, includeNames, excludeNames, ignoreCase);
		}

		/// <summary>
		/// 对给定的对象进行部分数据覆盖，并同时指定覆盖时包含的属性。
		/// </summary>
		/// <typeparam name="T">要覆盖的数据的类型。</typeparam>
		/// <typeparam name="TInclude">用于表示 <paramref name="includes"/> 表达式的参数类型。</typeparam>
		/// <param name="source">要进行覆盖操作的对象，该对象包含一个或多个需要被覆盖的属性。</param>
		/// <param name="patch">提供覆盖值的对象，该对象包含一个或多个将用于覆盖 <paramref name="source"/> 中同名属性值的属性。</param>
		/// <param name="includes">如果该参数不为 <c>null</c>，则只有该参数对应的表达式的返回类型中包含的属性才会被覆盖。</param>
		/// <param name="ignoreCase">指定在检索和对比属性名称时是否区分大小写，默认值为 <c>false</c>。</param>
		public static void PatchInclude<T, TInclude>(this T source, object patch,
			Expression<Func<T, TInclude>> includes, bool ignoreCase = false)
		{

			var includeNames = typeof(TInclude).GetPropertyNamesForType(PropertyAccessRequirement.None);
			Patch(source, patch, includeNames, null, ignoreCase);
		}

		/// <summary>
		/// 对给定的对象进行部分数据覆盖，并同时指定覆盖时排除的属性。
		/// </summary>
		/// <typeparam name="T">要覆盖的数据的类型。</typeparam>
		/// <typeparam name="TExclude">用于表示 <paramref name="excludes"/> 表达式的参数类型。</typeparam>
		/// <param name="source">要进行覆盖操作的对象，该对象包含一个或多个需要被覆盖的属性。</param>
		/// <param name="patch">提供覆盖值的对象，该对象包含一个或多个将用于覆盖 <paramref name="source"/> 中同名属性值的属性。</param>
		/// <param name="excludes">如果该参数不为 <c>null</c>，则该参数对应的表达式的返回类型中包含的属性将不会被覆盖。</param>
		/// <param name="ignoreCase">指定在检索和对比属性名称时是否区分大小写，默认值为 <c>false</c>。</param>
		public static void PatchExclude<T, TExclude>(this T source, object patch, Expression<Func<T, TExclude>> excludes, bool ignoreCase = false)
		{
			var excludeNames = typeof(TExclude).GetPropertyNamesForType(PropertyAccessRequirement.None);
			Patch(source, patch, null, excludeNames, ignoreCase);
		}

		/// <summary>
		/// 对给定的对象进行部分数据覆盖。
		/// </summary>
		/// <param name="source">要进行覆盖操作的对象，该对象包含一个或多个需要被覆盖的属性。</param>
		/// <param name="patch">提供覆盖值的对象，该对象包含一个或多个将用于覆盖 <paramref name="source"/> 中同名属性值的属性。</param>
		/// <param name="includes">如果该参数不为 <c>null</c>，则只有该参数中给定名称的属性才将执行覆盖。</param>
		/// <param name="excludes">如果该参数不为 <c>null</c>，则该参数中给定名称的属性将不会被覆盖。</param>
		/// <param name="ignoreCase">指定在检索和对比属性名称时是否区分大小写。</param>
		public static void Patch(this object source, object patch, IEnumerable<string> includes = null, IEnumerable<string> excludes = null, bool ignoreCase = false)
		{
			var comparer = ignoreCase ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal;
			var propertySet = new HashSet<string>(comparer);

			// 加入原始属性
			propertySet.UnionWith(source.GetType().GetPropertyNamesForType(PropertyAccessRequirement.Write));

			// 和目标属性进行交叉
			propertySet.IntersectWith(patch.GetType().GetPropertyNamesForType(PropertyAccessRequirement.Read));

			// Include 限制
			if (includes != null)
			{
				propertySet.IntersectWith(includes);
			}

			// Exclude 限制
			if (excludes != null)
			{
				propertySet.ExceptWith(excludes);
			}

			PatchCore(source, patch, propertySet.ToArray(), ignoreCase);
		}

		/// <summary>
		/// 单个属性修补的核心方法。
		/// </summary>
		/// <param name="source">原始对象。</param>
		/// <param name="patch">修补对象。</param>
		/// <param name="propertyNames">属性名。</param>
		/// <param name="ignoreCase">是否忽略大小写。</param>
		private static void PatchCore(object source, object patch, IEnumerable<string> propertyNames, bool ignoreCase)
		{
			foreach (var propertyName in propertyNames)
			{
				PatchPropertyCore(source, patch, propertyName, ignoreCase);
			}
		}


		/// <summary>
		/// 检索一个类型中所有合格的属性。
		/// </summary>
		/// <param name="type">要检索的类型。</param>
		/// <param name="requirement">对属性的访问性要求。</param>
		/// <returns></returns>
		private static IEnumerable<string> GetPropertyNamesForType(this IReflect type, PropertyAccessRequirement requirement)
		{
			var properties = (IEnumerable<PropertyInfo>)type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

			// 条件判断

			if (requirement.HasFlag(PropertyAccessRequirement.Read))
			{
				properties = properties.Where(i => i.CanRead);
			}

			if (requirement.HasFlag(PropertyAccessRequirement.Write))
			{
				properties = properties.Where(i => i.CanWrite);
			}

			return properties.Select(i => i.Name);
		}

		/// <summary>
		/// 执行属性修改的核心方法。
		/// </summary>
		/// <param name="source">要修补的原始对象。</param>
		/// <param name="patch">提供新值的修补对象。</param>
		/// <param name="propertyName">要修补的属性名称。</param>
		/// <param name="ignoreCase">是否忽略大小写。</param>
		private static void PatchPropertyCore(object source, object patch, string propertyName, bool ignoreCase)
		{
			var sourceProp = source.GetProperty(propertyName, ignoreCase);
			var patchProp = patch.GetProperty(propertyName, ignoreCase);

			if (sourceProp == null || patchProp == null || !sourceProp.CanWrite || !patchProp.CanRead)
			{
				throw new InvalidOperationException($"对象属性 '{propertyName}' 修补失败。请确保原始对象具有该属性，并且该属性可写入；修补对象也具有该属性，并且该属性可读取。");
			}

			sourceProp.SetValue(source, patchProp.GetValue(patch));
		}

		/// <summary>
		/// 提取对象的属性信息。
		/// </summary>
		/// <param name="obj">要提取的对象。</param>
		/// <param name="propertyName">要提取的属性名。</param>
		/// <param name="ignoreCase">是否忽略大小写。</param>
		/// <returns>表示属性信息的 <see cref="PropertyInfo"/> 对象。</returns>
		private static PropertyInfo GetProperty(this object obj, string propertyName, bool ignoreCase)
		{
			var flags = BindingFlags.Instance | BindingFlags.Public;

			// 不区分大小写标志
			if (ignoreCase)
			{
				flags |= BindingFlags.IgnoreCase;
			}

			return obj.GetType().GetProperty(propertyName, flags);
			;
		}

		/// <summary>
		/// 定义属性的访问要求。
		/// </summary>
		[Flags]
		public enum PropertyAccessRequirement
		{
			/// <summary>
			/// 允许所有属性。
			/// </summary>
			None = 0,
			/// <summary>
			/// 要求属性必须可读。
			/// </summary>
			Read = 0x1,
			/// <summary>
			/// 要求属性必须可写。
			/// </summary>
			Write = 0x2,
			/// <summary>
			/// 要求属性必须同时可读写。
			/// </summary>
			ReadWrite = Read | Write
		}
	}
}
