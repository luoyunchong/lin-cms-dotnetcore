using System;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;

namespace LinCms.Zero.Common
{
    public  class LinCmsUtils
    {
        /// <summary>
        /// 通过创建哈希字符串适用于任何 MD5 哈希函数 （在任何平台） 上创建 32 个字符的十六进制格式哈希字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Get32Md5(string source)
        {
            using MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            StringBuilder sBuilder = new StringBuilder();
            foreach (byte t in data)
            {
                sBuilder.Append(t.ToString("x2"));
            }

            string hash = sBuilder.ToString();
            return hash.ToUpper();
        }
        /// <summary>
        /// 获取16位md5加密
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string Get16Md5(string source)
        {
            using MD5 md5Hash = MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(source));
            //转换成字符串，并取9到25位
            string sBuilder = BitConverter.ToString(data, 4, 8);
            //BitConverter转换出来的字符串会在每个字符中间产生一个分隔符，需要去除掉
            sBuilder = sBuilder.Replace("-", "");
            return sBuilder.ToUpper();
        }

        /// <summary>
        /// 继承HashAlgorithm
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static String GetHash<T>(Stream stream) where T : HashAlgorithm
        {
            StringBuilder sb = new StringBuilder();

            MethodInfo create = typeof(T).GetMethod("Create", new Type[] { });
            using (T crypt = (T)create.Invoke(null, null))
            {
                byte[] hashBytes = crypt.ComputeHash(stream);
                foreach (byte bt in hashBytes)
                {
                    sb.Append(bt.ToString("x2"));
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
                return dt.ToShortDateString();
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
    }
}
