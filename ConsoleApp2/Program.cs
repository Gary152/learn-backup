using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    internal class Program
    {
        private static readonly string Key = " SESSDATA=56634153%2C1685525557%2Cc3bca%2Ac1";

        public static void Main(string[] args)
        {
            //HttpTest();

            Console.WriteLine("执行完毕，按任意键结束。");
            Console.ReadKey();
        }

        #region Favorites

        public static void TestGet()
        {
            var list = GetFavorites().Result;
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
                txt += $"<dl><dt><a href=\"https://www.bilibili.com/video/{item.bvid}\">" +
                    $"<img src=\"{item.cover}\" width=\"192\" height=\"108\" /></a></dt>" +
                    $"<dd><a href=\"#\" onclick=\"F1('{item.bvid}')\">{item.title}" +
                    $"</a></dd></dl>";
            }
            StreamReader sr = new StreamReader("test.html");
            var model = sr.ReadToEnd();
            sr.Close();
            model = model.Replace("{{content}}", txt);
            StreamWriter sw = new StreamWriter(list[index - 1].title + ".html");
            sw.WriteLine(model);
            sw.Close();
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

        #region Regex

        public void TestRegex()
        {
            string str = "Is is the cost of of gasoline going up up";
            var res = Regex.Matches(str, @"\b(?<name>[a-zA-Z]+) \1");
            foreach (Match item in res)
            {
                var a = item.Groups["name"];
                Console.WriteLine(a);
            }
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