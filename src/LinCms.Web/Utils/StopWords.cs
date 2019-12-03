using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinCms.Web.Utils
{
    public static class StopWords
    {

        static readonly ConcurrentDictionary<string, bool> FunNlpDataSensitive = new ConcurrentDictionary<string, bool>();
        static readonly ConcurrentDictionary<int, string> ReplaceNewValue = new ConcurrentDictionary<int, string>();

        private const string KeywordsPath = "wwwroot/_Illegal/IllegalKeywords.txt";
        private const string UrlsPath = "wwwroot/_Illegal/IllegalUrls.txt";


        static StopWords()
        {
            LoadDataFromFile();
        }

        public static void LoadDataFromFile()
        {
            string words1 = File.ReadAllText(Path.GetFullPath(KeywordsPath), Encoding.UTF8);
            string words2 = File.ReadAllText(Path.GetFullPath(UrlsPath), Encoding.UTF8);
            LoadDataFromText(words1);
            LoadDataFromText(words2);
        }


        public static void LoadDataFromText(string text)
        {
            int oldcount = FunNlpDataSensitive.Count;
            foreach (string wd in text.Split('\n'))
            {
                string keykey = wd.Trim().Trim('\r', '\n').Trim();
                if (string.IsNullOrEmpty(keykey)) continue;
                FunNlpDataSensitive.TryAdd(keykey, true);
                if (ReplaceNewValue.ContainsKey(keykey.Length) == false)
                    ReplaceNewValue.TryAdd(keykey.Length, "".PadRight(keykey.Length, '*'));
            }
            Console.WriteLine($"敏感词加载完毕，增加数量：{FunNlpDataSensitive.Count - oldcount}");
        }


        /// <summary>
        /// 替换所有敏感词为 *
        /// </summary>
        /// <param name="that"></param>
        /// <returns></returns>
        public static string ReplaceStopWords(this string that)
        {
            foreach (var wd in FunNlpDataSensitive.Keys)
                that = that.Replace(wd, ReplaceNewValue.TryGetValue(wd.Length, out var tryval) ? tryval : "".PadRight(wd.Length, '*'));
            return that;
        }
    }
}
