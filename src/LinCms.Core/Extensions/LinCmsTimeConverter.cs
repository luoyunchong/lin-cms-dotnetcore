using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LinCms.Extensions
{
    /// <summary>
    /// 配合LinCMS中的时间戳 后台只返回 1562904163734
    /// </summary>
    public class LinCmsTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double javaScriptTicks;
            if (value is DateTime dateTime)
            {
                javaScriptTicks = ConvertDateTimeInt(dateTime);
            }
            else
            {
                if (!(value is DateTimeOffset dateTimeOffset))
                    throw new JsonSerializationException("Expected date object value.");
                javaScriptTicks = ConvertDateTimeInt(dateTimeOffset.ToUniversalTime().UtcDateTime);

            }
            writer.WriteValue(javaScriptTicks);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString() == "")
            {
                return null;
            }

            if (reader.TokenType == JsonToken.Integer)
            {
                return ConvertIntDateTime(double.Parse(reader.Value.ToString()));
            }

            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
            {
                DateTime.TryParse(reader.Value.ToString(), out DateTime readerTime);
                return readerTime;
            }
            if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
            {
                DateTimeOffset.TryParse(reader.Value.ToString(), out var readerTime);
                return readerTime;
            }

            return null;
        }

        public static DateTime ConvertIntDateTime(double milliseconds)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(milliseconds);
        }

        /// <summary>
        /// 日期转换为时间戳（时间戳单位毫秒）
        /// </summary>
        /// <returns></returns>
        public static double ConvertDateTimeInt(DateTime dateTime)
        {
            if (dateTime.Year == 1)
            {
                return 0;
            }
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
        }
    }
}
