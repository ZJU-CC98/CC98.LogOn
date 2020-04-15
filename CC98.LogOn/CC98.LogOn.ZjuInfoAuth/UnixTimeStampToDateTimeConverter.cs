using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CC98.LogOn.ZjuInfoAuth
{
	/// <summary>
	/// 提供从 Linux 时间戳转换到当前时间的转换器。
	/// </summary>
	public class UnixTimeStampToDateTimeConverter : JsonConverter<DateTimeOffset>
	{
		public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			return DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64());
		}

		public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
		{
			writer.WriteNumberValue(value.ToUnixTimeMilliseconds());
		}
	}
}