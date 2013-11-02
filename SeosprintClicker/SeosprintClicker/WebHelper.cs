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
using System.Web;

namespace SeosprintClicker
{
    public class WebHelper
    {
        Random randomGenerator = new Random();
        UTF8Encoding encoding = new UTF8Encoding();
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

        public void LoadCapcha(string referer, string[] cookies)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest
                .Create("http://www.seosprint.net/captcha/captcha-ff/captcha.php?sid=" + randomGenerator.NextDouble());
            request.Method = Constants.MethodGet;
            request.UserAgent = Constants.UserAgent;
            request.Host = Constants.DefaultHost;
            request.Accept = Constants.AcceptGet;
            request.Referer = referer;
            var cookieContainer = new CookieContainer();
            for (int i = 0; i < cookies.Length; i++)
                cookieContainer.SetCookies(Constants.DefaultSeosprintUri, cookies[i]);
            request.CookieContainer = cookieContainer;
            using (var response = (HttpWebResponse)request.GetResponse())
            {
                Stream responseStream = response.GetResponseStream();
                using (Image image = Image.FromStream(responseStream))
                {
                    image.Save("captcha.png", ImageFormat.Png);
                }
            }
        }

        public void RequestAndSave(HttpWebRequest request, string fileName)
        {
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                string responseBody = "";
                using (Stream rspStm = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(rspStm))
                    {
                        File.WriteAllText("LOG.txt", "");
                        var success = string.Concat("SUCCESS: Response Description: " + response.StatusDescription +
                         "Response Status Code: " + response.StatusCode);
                        File.AppendAllText("LOG.txt", success);
                        string cookies = request.Headers[HttpRequestHeader.Referer] + "COOKIES:";
                        string headers = "HEADERS:";
                        for (int i = 0; i < response.Cookies.Count; i++)
                        {
                            cookies = string.Concat(cookies, " ", response.Cookies[i].Name, ":", response.Cookies[i].Value);
                        }

                        // Load Header collection into NameValueCollection object.
                        NameValueCollection coll = response.Headers;

                        // Put the names of all keys into a string array.
                        String[] arr1 = coll.AllKeys;
                        for (int loop1 = 0; loop1 < arr1.Length; loop1++)
                        {
                            headers = headers + arr1[loop1] + ": ";
                            // Get all values under this key.
                            String[] arr2 = coll.GetValues(arr1[loop1]);
                            for (int loop2 = 0; loop2 < arr2.Length; loop2++)
                            {
                                headers = headers + ": " + HttpUtility.HtmlEncode(arr2[loop2]);
                            }
                        }
                        File.AppendAllText("LOG.txt", headers);
                        File.AppendAllText("LOG.txt", cookies);
                        responseBody = reader.ReadToEnd();
                    }
                }
                using (var file = File.Create(fileName))
                {
                    file.Write(encoding.GetBytes(responseBody), 0, encoding.GetByteCount(responseBody));
                }
            }
            catch (System.Net.WebException ex)
            {
                var fail = string.Concat("FAIL: Exception message: " + ex.Message +
                       "\nResponse Status Code: " + ex.Status + "\n");
                File.WriteAllText("LOG.txt", fail);
            }
        }

        public HttpWebRequest CreateRequestAndSetSomeStandartHeaders(string URL, string[] cookies, string referer, bool IsPOST)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            if (IsPOST)
            {
                request.Method = Constants.MethodPost;
                request.ContentType = Constants.ContentTypePost;
            }
            else
            {
                request.Method = Constants.MethodGet;
            }
            request.UserAgent = Constants.UserAgent;
            request.Host = Constants.DefaultHost;
            request.Accept = Constants.AcceptGet;
            request.Referer = referer;
            var cookieContainer = new CookieContainer();
            for (int i = 0; i < cookies.Length; i++)
                cookieContainer.SetCookies(Constants.DefaultSeosprintUri, cookies[i]);
            request.CookieContainer = cookieContainer;
            return request;
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
