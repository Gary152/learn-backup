using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            TestGet();

            Console.ReadKey();
        }

        public static async void TestGet()
        {
            var list = await GetFavorites();
            Console.WriteLine("请选择：");

            for (int i = 0; i < list.Count; i++)
            {
                Console.WriteLine(i + 1 + ". " + list[i].title);
            }

            var index = int.Parse(Console.ReadLine() ?? "");

            List<dynamic> videos = await GetVideos(list[index - 1].id);

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
            httpClient.DefaultRequestHeaders.Add("cookie", "SESSDATA=59acb0c8%2C1685150710%2Cf3bb2%2Ab1");

            var jsonResult = await httpClient.GetStringAsync("https://api.bilibili.com/x/v3/fav/folder/list4navigate");
            var json = JsonDocument.Parse(jsonResult);
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
            httpClient.DefaultRequestHeaders.Add("cookie", "SESSDATA=59acb0c8%2C1685150710%2Cf3bb2%2Ab1");

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

        private static void Http_start()
        {
            HttpClient client = new HttpClient();
            client.Timeout = new TimeSpan(0, 2, 0);//保持2分钟
            client.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.131 Safari/537.36");

            var videosoucode = HttpGetAsync("https://www.cnnpn.cn/article/29829.html", client);
            HtmlDocument videoDoc = new HtmlDocument();
            videoDoc.LoadHtml(videosoucode.Result);
            var contentNode = videoDoc.DocumentNode.SelectSingleNode("//div[@class='content']");
            var videoNode = contentNode.SelectSingleNode("video");
            var scriptNode = contentNode.SelectSingleNode("script");
            Console.WriteLine(scriptNode.OuterHtml);
            var scriptcode = scriptNode.InnerText;
            var fileIDBeginIndex = scriptcode.IndexOf("fileID") + 9;
            var fileIDEndIndex = scriptcode.IndexOf("'", fileIDBeginIndex);
            var appIDBeginIndex = scriptcode.IndexOf("appID") + 8;
            var appIDEndIndex = scriptcode.IndexOf("'", appIDBeginIndex);
            var fileID = scriptcode.Substring(fileIDBeginIndex, fileIDEndIndex - fileIDBeginIndex);
            var appID = scriptcode.Substring(appIDBeginIndex, appIDEndIndex - appIDBeginIndex);
            Console.WriteLine(fileID + "----" + appID);
            var videoJsonUrl = string.Format("https://playvideo.qcloud.com/getplayinfo/v2/{0}/{1}", appID, fileID);
            var videoinfo = HttpGetAsync(videoJsonUrl, client);
            var document = JsonDocument.Parse(videoinfo.Result);
            var videoFaceUrl = document.RootElement.GetProperty("coverInfo").GetProperty("coverUrl").GetString();
            var videoUrl = document.RootElement.GetProperty("videoInfo").GetProperty("sourceVideo").GetProperty("url").GetString();
            contentNode.RemoveChild(scriptNode);
            var newsNewContentStr = contentNode.InnerHtml;
            newsNewContentStr = newsNewContentStr.Replace("></video>", string.Format(" controls poster=\"{0}\" src=\"{1}\"></video>", videoFaceUrl, videoUrl));

            Console.WriteLine(videoFaceUrl + "----" + videoUrl);
        }
        private static async Task<string> HttpGetAsync(string url, HttpClient client)
        {
            Thread.Sleep(1000);//防止https认为是攻击
            var response = await client?.GetAsync(url)!;
            return await response.Content.ReadAsStringAsync();
        }

        #endregion
    }
}