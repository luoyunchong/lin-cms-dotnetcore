using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace LinCms.Zero.Common
{
    public class LinCmsUtils
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

        /// <summary>
        /// 获取操作系统的名字
        /// </summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public static string GetOsNameByUserAgent(string userAgent)
        {
            string osVersion = "未知";

            if (userAgent.Contains("NT 10.0"))
            {
                osVersion = "Windows 10";
            }
            else if (userAgent.Contains("NT 6.3"))
            {
                osVersion = "Windows 8.1";
            }
            else if (userAgent.Contains("NT 6.2"))
            {
                osVersion = "Windows 8";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.1"))
            {
                osVersion = "Windows 7";
            }
            else if (userAgent.Contains("NT 6.0"))
            {
                osVersion = "Windows Vista/Server 2008";
            }
            else if (userAgent.Contains("NT 5.2"))
            {
                osVersion = userAgent.Contains("64") ? "Windows XP" : "Windows Server 2003";
            }
            else if (userAgent.Contains("NT 5.1"))
            {
                osVersion = "Windows XP";
            }
            else if (userAgent.Contains("NT 5"))
            {
                osVersion = "Windows 2000";
            }
            else if (userAgent.Contains("NT 4"))
            {
                osVersion = "Windows NT4";
            }
            else if (userAgent.Contains("Me"))
            {
                osVersion = "Windows Me";
            }
            else if (userAgent.Contains("98"))
            {
                osVersion = "Windows 98";
            }
            else if (userAgent.Contains("95"))
            {
                osVersion = "Windows 95";
            }
            else if (userAgent.Contains("Mac"))
            {
                osVersion = "Mac";
            }
            else if (userAgent.Contains("Unix"))
            {
                osVersion = "UNIX";
            }
            else if (userAgent.Contains("Linux"))
            {
                osVersion = "Linux";
            }
            else if (userAgent.Contains("SunOS"))
            {
                osVersion = "SunOS";
            }
            return osVersion;
        }


        /// <summary>
        /// https://cloud.tencent.com/developer/article/1152362
        /// 获取本机IP、地区及运营商 
        /// </summary>
        /// <param name="ipAddres"></param>
        /// <returns></returns>
        public static IpQueryResult IpQueryCity(string ipAddres)
        {
            if (ipAddres == "::1" || ipAddres == "127.0.0.1")
            {
                return new IpQueryResult()
                {
                    data = "本地ip",
                    errno = 0
                };
            }
            if (!IPAddress.TryParse(ipAddres, out IPAddress ip))
            {
                return null;
            }

            Uri url = new Uri("http://ip.360.cn/IPQuery/ipquery?ip=" + ipAddres);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = 1000 * 5;
            request.KeepAlive = false;
            request.Method = "GET";
            WebResponse resoponse = request.GetResponse();
            string pageHtml;
            // ReSharper disable once AssignNullToNotNullAttribute
            using (var stream = new StreamReader(resoponse.GetResponseStream()))
            {
                pageHtml = stream.ReadToEnd();
            }
            IpQueryResult result = JsonConvert.DeserializeObject<IpQueryResult>(pageHtml);

            return result;
        }


    }
}
