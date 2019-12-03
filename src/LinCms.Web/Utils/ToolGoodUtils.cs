using System.Collections.Generic;
using System.IO;
using System.Text;
using ToolGood.Words;

namespace LinCms.Web.Utils
{
    /// <summary>
    /// ToolGood.Words类库配合敏感库
    /// </summary>
    public class ToolGoodUtils
    {
        //敏感库只要这二个文件存在即可
        //本地敏感库缓存-https://github.com/toolgood/ToolGood.Words/tree/master/csharp/ToolGood.Words.Test/_Illegal
        //因为需要上传至github并同步gitee,安全起见，所以未上传至git，需要自行下载并复制
        private const string KeywordsPath = "wwwroot/_Illegal/IllegalKeywords.txt";
        private const string UrlsPath = "wwwroot/_Illegal/IllegalUrls.txt";

        private const string InfoPath = "wwwroot/_Illegal/IllegalInfo.txt";
        private const string BitPath = "wwwroot/_Illegal/IllegalBit.iws";

        private static IllegalWordsSearch _search;
        /// <summary>
        /// 本地敏感库,文件修改后，重新创建缓存Bit
        /// </summary>
        /// <returns></returns>
        public static IllegalWordsSearch GetIllegalWordsSearch()
        {
            if (_search == null)
            {
                string ipath = Path.GetFullPath(InfoPath);
                if (File.Exists(ipath) == false)
                {
                    _search = CreateIllegalWordsSearch();
                }
                else
                {
                    var texts = File.ReadAllText(ipath).Split('|');
                    if (new FileInfo(Path.GetFullPath(KeywordsPath)).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") !=
                        texts[0] ||
                        new FileInfo(Path.GetFullPath(UrlsPath)).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") !=
                        texts[1]
                    )
                    {
                        _search = CreateIllegalWordsSearch();
                    }
                    else
                    {
                        var s = new IllegalWordsSearch();
                        s.Load(Path.GetFullPath(BitPath));
                        _search = s;
                    }
                }
            }
            return _search;
        }

        private static IllegalWordsSearch CreateIllegalWordsSearch()
        {
            string[] words1 = File.ReadAllLines(Path.GetFullPath(KeywordsPath), Encoding.UTF8);
            string[] words2 = File.ReadAllLines(Path.GetFullPath(UrlsPath), Encoding.UTF8);
            var words = new List<string>();
            foreach (var item in words1)
            {
                words.Add(item.Trim());
            }
            foreach (var item in words2)
            {
                words.Add(item.Trim());
            }

            var search = new IllegalWordsSearch();
            search.SetKeywords(words);

            search.Save(Path.GetFullPath(BitPath));

            var text = new FileInfo(Path.GetFullPath(KeywordsPath)).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss") + "|"
                                                                                                                  + new FileInfo(Path.GetFullPath(UrlsPath)).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss");
            File.WriteAllText(Path.GetFullPath(InfoPath), text);

            return search;
        }

    }
}
