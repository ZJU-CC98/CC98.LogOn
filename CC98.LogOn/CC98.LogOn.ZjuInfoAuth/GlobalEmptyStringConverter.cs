using System;
using System.Collections.Generic;
using System.Resources;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CC98.LogOn.ZjuInfoAuth
{
	public class NullableTypeEmptyStringConverter<T> : JsonConverter<T?>
		where T : struct
	{
		public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
		{
			var str = reader.GetString();
			if (string.IsNullOrEmpty(str))
			{
				return default;
			}

			// DateTime 特殊处理，因为直接将内容进行JSON解析会出现语法错误
			if (typeof(T) == typeof(DateTime))
			{
				return (T)(object)DateTime.Parse(str);
			}

			return JsonSerializer.Deserialize<T>(str, options);
		}

		public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
		{
			JsonSerializer.Serialize(writer, value);
		}
	}

	public class NullableTypeEmptyStringConverterFactory : JsonConverterFactory
	{
		public override bool CanConvert(Type typeToConvert)
		{
			return typeToConvert.IsGenericType && typeToConvert.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
		{
			var baseType = typeToConvert.GetGenericArguments()[0];

			var realType = typeof(NullableTypeEmptyStringConverter<>).MakeGenericType(baseType);
			return (JsonConverter)Activator.CreateInstance(realType);
		}
	}
}
