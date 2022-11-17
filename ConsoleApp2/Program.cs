using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ConsoleApp2
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string str = "Is is the cost of of gasoline going up up";
            var re = Regex.Match(str, @"\b(?<name>[a-zA-Z]+) \1");
            while (re.Success)
            {

                Console.WriteLine(re.Value);
                re=re.NextMatch();
            }
        }

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

        #region TimeSpan

        private static void TimeSpan_start()
        {
            Regex regex = new Regex(@"^\d\d:\d\d:\d\d$");
            Console.WriteLine("请输入TimeSpan");
            string str = Console.ReadLine()!;
            Match match = regex.Match(str);
            double s = GetTimeSpanValue(match).TotalMilliseconds;
            Console.WriteLine(s);
        }
        private static TimeSpan GetTimeSpanValue(Match match)
        {
            if (match.Success)
            {
                bool isTS = TimeSpan.TryParse(match.Value, out var outTS);
                if (isTS)
                {
                    return outTS;
                }
                else
                {
                    return GetTimeSpanValue(match.NextMatch());
                }
            }
            else
            {
                return TimeSpan.Zero;
            }
        }

        #endregion
    }
}