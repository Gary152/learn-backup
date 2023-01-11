using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    public class Other
    {

        #region Aes加解密

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <param name="bytes">内容</param>
        /// <returns>该内容对应的哈希值</returns>
        public static string GetHashcode(byte[] bytes)
        {
            var rawFileHash = BitConverter.ToString(SHA1.Create().ComputeHash(bytes));
            var fileHash = rawFileHash.Replace("-", string.Empty).ToLower();
            return fileHash;
        }

        /// <summary>
        /// 通过string的key获取指定的符合条件的btye[]key
        /// </summary>
        /// <param name="strkey">密码</param>
        /// <returns>符合加密条件的key值</returns>
        public static byte[] GetByteKey(string strkey)
        {
            var arr = Encoding.UTF8.GetBytes(strkey);
            var newArr = new byte[32];
            if (newArr.Length >= arr.Length)
                arr.CopyTo(newArr, 0);
            else
                newArr = arr.Take(32).ToArray();

            return newArr;
        }

        /// <summary>
        /// 使用AES CBC模式进行加密
        /// </summary>
        /// <param name="content">明文内容</param>
        /// <param name="key">密码</param>
        /// <returns>密文</returns>
        public static string AesEncrypt(string content, string key)
        {
            var contentArray = Encoding.UTF8.GetBytes(content);
            var keyArray = GetByteKey(key);

            var aes = Aes.Create();
            //aes.Mode = CipherMode.ECB; //不需要IV的模式
            aes.Key = keyArray;
            aes.IV = new byte[16];

            var encrypt = aes.CreateEncryptor();
            var resultArray = encrypt.TransformFinalBlock(contentArray, 0, contentArray.Length);

            return Convert.ToBase64String(resultArray);
        }

        /// <summary>
        /// 使用AES CBC模式进行解密
        /// </summary>
        /// <param name="content">密文内容</param>
        /// <param name="key">密码</param>
        /// <returns>明文</returns>
        public static string AesDecrypt(string content, string key)
        {
            var contentArray = Convert.FromBase64String(content);
            var keyArray = GetByteKey(key);

            var aes = Aes.Create();
            //aes.Mode = CipherMode.ECB; //不需要IV的模式
            aes.Key = keyArray;
            aes.IV = new byte[16];

            var encrypt = aes.CreateDecryptor();
            var resultArray = encrypt.TransformFinalBlock(contentArray, 0, contentArray.Length);

            return Encoding.UTF8.GetString(resultArray);
        }

        #endregion

        #region Favorites

        private const string Key = "SESSDATA=84d5129b%2C1688950050%2C9cab9%2A11";

        public static void TestGet()
        {
            List<dynamic> list = GetFavorites().Result;
            Console.WriteLine("请选择：");

            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(i + 1 + ". " + list[i].title);
            }

            Console.WriteLine("请输入序号");
            var index = int.Parse(Console.ReadLine() ?? "");

            List<dynamic> videos = GetVideos(list[index - 1].id).Result;

            var txt = "";
            foreach (var item in videos)
            {
                txt += $"<dl><dt><a href=\"https://www.bilibili.com/video/{item.bvid}\">\n" +
                    $"<img src=\"{item.cover}\" width=\"192\" height=\"108\" /></a></dt>\n" +
                    $"<dd><a href=\"#\" onclick=\"F1('{item.bvid}')\">{item.title}\n" +
                    $"</a></dd></dl>\n";
            }
            using (StreamReader sr = new StreamReader("test.html"))
            {
                var model = sr.ReadToEnd();
                model = model.Replace("{{content}}", txt);

                string dirPath = Path.Combine(Directory.GetCurrentDirectory(), "html");
                var filePath = Path.Combine(dirPath, list[index - 1].title + ".html");
                using (StreamWriter sw = new StreamWriter(filePath, false))  //写入不追加
                {
                    sw.WriteLine(model);
                }

                //打开保存目录
                _ = Process.Start("explorer.exe", dirPath);
            }
        }

        public static async Task<List<dynamic>> GetFavorites()
        {
            HttpClient httpClient = new();
            httpClient.Timeout = new TimeSpan(0, 3, 0);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.42");
            httpClient.DefaultRequestHeaders.Add("cookie", Key);

            var jsonResult = await httpClient.GetStringAsync("https://api.bilibili.com/x/v3/fav/folder/list4navigate");
            var json = JsonDocument.Parse(jsonResult);
            var succeed = json.RootElement.TryGetProperty("code", out var code);
            if (!succeed)
            {
                throw new Exception("请求出现未知响应");
            }
            if (code.ToString() == "-101")
            {
                Console.WriteLine("登录失效");
                throw new Exception("登录失效");
            }

            var list = json.RootElement.GetProperty("data")[0].GetProperty("mediaListResponse").GetProperty("list");

            var items = new List<dynamic>();
            for (int i = 0; i < list.GetArrayLength(); i++)
            {
                var title = list[i].GetProperty("title").ToString();
                var id = list[i].GetProperty("id").ToString();
                if (title == null || id == null) continue;
                items.Add(new { title, id });
            }

            return items;
        }

        public static async Task<List<dynamic>> GetVideos(string id)
        {
            HttpClient httpClient = new();
            httpClient.Timeout = new TimeSpan(0, 3, 0);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.42");
            httpClient.DefaultRequestHeaders.Add("cookie", Key);

            var jsonResult = await httpClient.GetStringAsync($"https://api.bilibili.com/x/v3/fav/resource/list?media_id={id}&pn=1&ps=20&keyword=&order=mtime&type=0&tid=0&platform=web&jsonp=jsonp");
            var json = JsonDocument.Parse(jsonResult);
            var list = json.RootElement.GetProperty("data").GetProperty("medias");

            var items = new List<dynamic>();
            for (int i = 0; i < list.GetArrayLength(); i++)
            {
                var title = list[i].GetProperty("title").ToString();
                var bvid = list[i].GetProperty("bvid").ToString();
                var cover = list[i].GetProperty("cover").ToString();
                if (title == null || id == null) continue;
                items.Add(new { title, bvid, cover });
            }

            return items;
        }

        #endregion

        #region Html

        private static void HttpTest(string url = "https://www.baidu.com")
        {
            HttpClient httpClient = new();
            httpClient.Timeout = new TimeSpan(0, 3, 0);
            httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/107.0.0.0 Safari/537.36 Edg/107.0.1418.42");
            httpClient.DefaultRequestHeaders.Add("cookie", Key);

            var videosoucode = httpClient.GetStringAsync(url);
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(videosoucode.Result);
            var uNode = doc.DocumentNode.SelectSingleNode("//div[@id='u']");//xpath
            Console.WriteLine(uNode.OuterHtml);//本节点html码
            Console.WriteLine();
            Console.WriteLine(uNode.InnerHtml);//子节点html码
            Console.WriteLine();
            Console.WriteLine(uNode.InnerText);//纯文本
            Console.WriteLine();
            var aNode = uNode.SelectSingleNode("a");
            Console.WriteLine(aNode.InnerText);
        }

        #endregion
    }
}
