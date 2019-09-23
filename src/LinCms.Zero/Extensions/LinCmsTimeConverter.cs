using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;

namespace LinCms.Zero.Extensions
{
    /// <summary>
    /// 配合LinCMS中的时间戳 后台只返回 1562904163734
    /// </summary>
    public class LinCmsTimeConverter : DateTimeConverterBase
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            double javaScriptTicks = 0;
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
            return ConvertIntDateTime(double.Parse(reader.Value.ToString()));
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
            return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
            //return (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
