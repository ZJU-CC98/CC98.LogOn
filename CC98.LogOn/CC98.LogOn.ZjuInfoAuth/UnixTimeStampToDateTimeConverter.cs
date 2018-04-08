using System;
using Newtonsoft.Json;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供从 Linux 时间戳转换到当前时间的转换器。
	/// </summary>
	public class UnixTimeStampToDateTimeConverter : JsonConverter
	{
		/// <summary>
		/// Unix 时间戳开始时间
		/// </summary>
		private static readonly DateTime StartTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		/// <inheritdoc />
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var realValue = (DateTime)value;
			writer.WriteRawValue((realValue - StartTime).TotalMilliseconds.ToString("F"));
		}

		/// <inheritdoc />
		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null)
			{
				return null;
			}

			var realValue = (long)reader.Value;
			return StartTime.AddMilliseconds(realValue);
		}

		/// <inheritdoc />
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTime);
		}
	}
}