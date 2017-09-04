using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CC98.LogOn
{
	/// <summary>
	/// 提供为存储数据提供的辅助方法。该类型为静态类型。
	/// </summary>
	public static class StoreValueUtility
	{
		/// <summary>
		/// 将存储区的单个字符串拆分为实际使用的多个字符串。
		/// </summary>
		/// <param name="value">要拆分的多个字符串。</param>
		/// <returns>实际使用的多个字符串。</returns>
		public static string[] SplitForStore(this string value)
		{
			return value == null ? new string[0] : value.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		}

		/// <summary>
		/// 将使用的多个字符串合并为存储区使用的单个字符串。
		/// </summary>
		/// <param name="values">要存储的多个字符串。</param>
		/// <returns>合并后的单个字符串。</returns>
		public static string JoinForStore(this IEnumerable<string> values)
		{
			return string.Join('\n', values);
		}

		/// <summary>
		/// 将复杂对象通过 JSON 保存为存储格式。
		/// </summary>
		/// <param name="value">要存储的对象。</param>
		/// <returns>用于保存的 JSON 格式字符串。</returns>
		public static string StoreToJson(this object value)
		{
			return JsonConvert.SerializeObject(value);
		}

		/// <summary>
		/// 从 JSON 存储内容中读取复杂对象。
		/// </summary>
		/// <typeparam name="T">要读取的对象的类型。</typeparam>
		/// <param name="value">保存复杂对象的 JSON 字符串。</param>
		/// <returns>从字符串中提取的复杂对象。</returns>
		public static T ExtractFromJson<T>(this string value)
		{
			return JsonConvert.DeserializeObject<T>(value);
		}
	}
}
