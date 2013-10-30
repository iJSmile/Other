using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using System.Drawing;
using System.Drawing.Imaging;

namespace FictionReguestToGetKapcha
{
    public class WebHelper
    {
        Random randomGenerator = new Random();
        /// <returns> Возвращает объект HtmlDocument</returns>
        public HtmlDocument GetHtmlDocument(WebResponse response)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.Load(response.GetResponseStream());
            return htmlDoc;
        }

        /// <returns> Возвращает строку по совпадению или string.Empty если совпадений не нашлось</returns>
        public string GetStringByRegExp(HtmlDocument htmlDoc, string pattern)
        {
            var html = htmlDoc.DocumentNode.InnerHtml;
            Regex regExp = new Regex(pattern);
            Match match = regExp.Match(html);
            if (match.Success)
                return match.Value;
            return string.Empty;
        }

        public MatchCollection GetAllStringByRegExp(string text, string pattern)
        {
            Regex regExp = new Regex(pattern);
            MatchCollection matchCollection = regExp.Matches(text);
            return matchCollection;
        }

        public void LoadCapcha(string referer, CookieContainer cookie)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create("http://www.seosprint.net/captcha/captcha-ff/captcha.php?sid=" + randomGenerator.NextDouble());
            request.Method = Constants.MethodGet;
            request.UserAgent = Constants.UserAgent;
            request.Host = Constants.DefaultHost;
            request.Accept = Constants.AcceptGet;
            request.Referer = referer;
            request.CookieContainer = cookie;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (Image image = Image.FromStream(responseStream))
                {
                    image.Save("captcha.png", ImageFormat.Png);
                }
            }
        }

        public void BrowseFile(string filePath)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.Arguments = filePath;
            startInfo.FileName = Constants.pathToBrowser;

            Process.Start(startInfo);
        }
    }
}
