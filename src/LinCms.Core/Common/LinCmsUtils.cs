using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using LinCms.Extensions;
using Microsoft.AspNetCore.Http;

namespace LinCms.Common
{
    public class LinCmsUtils
    {
        /// <summary>
        /// 继承HashAlgorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static string GetHash<T>(Stream stream) where T : HashAlgorithm
        {
            StringBuilder sb = new StringBuilder();

            MethodInfo create = typeof(T).GetMethod("Create", Array.Empty<Type>());
            using (T crypt = (T)create.Invoke(null, null))
            {
                if (crypt != null)
                {
                    byte[] hashBytes = crypt.ComputeHash(stream);
                    foreach (byte bt in hashBytes)
                    {
                        sb.Append(bt.ToString("x2"));
                    }
                }
            }
            return sb.ToString();
        }

        public static string GetRequest(HttpContext httpContext)
        {
            return httpContext.Request.Method + " " + httpContext.Request.Path;
        }

        public static string GetTimeDifferNow(DateTime dt)
        {
            TimeSpan span = DateTime.Now - dt;

            if (span.TotalDays > 60)
            {
                return dt.ToDateString();
            }

            if (span.TotalDays > 30)
            {
                return "1个月前";
            }

            if (span.TotalDays > 14)
            {
                return "2周前";
            }

            if (span.TotalDays > 7)
            {
                return "1周前";
            }

            if (span.TotalDays > 1)
            {
                return $"{(int)Math.Floor(span.TotalDays)}天前";
            }

            if (span.TotalHours > 1)
            {
                return $"{(int)Math.Floor(span.TotalHours)}小时前";
            }

            if (span.TotalMinutes > 1)
            {
                return $"{(int)Math.Floor(span.TotalMinutes)}分钟前";
            }

            if (span.TotalSeconds >= 1)
            {
                return $"{(int)Math.Floor(span.TotalSeconds)}秒前";
            }

            return "1秒前";
        }

        /// <summary>
        /// 校验手机号码是否符合标准。
        /// </summary>
        /// <param name="mobile"></param>
        /// <returns></returns>
        public static bool ValidateMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile))
                return false;
            return Regex.IsMatch(mobile, @"^(13|14|15|16|18|19|17)\d{9}$");
        }

    }
}
