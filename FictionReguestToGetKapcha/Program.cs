using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace FictionReguestToGetKapcha
{
    public class Program
    {
        public static HtmlDocument html = new HtmlDocument();
        public static WebHelper helper = new WebHelper();

        static void Main(string[] args)
        {
            
            Console.WriteLine("----------------------Request 1 Start---------------------");
            HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(Constants.UrlSeosprint);
            request1.Method = Constants.MethodGet;
            request1.Accept = Constants.AcceptGet;
            request1.Host = Constants.DefaultHost;
            request1.UserAgent = Constants.UserAgent;
            request1.Proxy = null;
            var response = (HttpWebResponse)request1.GetResponse();

            HtmlDocument htmlDoc = helper.GetHtmlDocument(response);
            string btnLoginRegExp = "(?<=class=\"btnlogin\" href=\")(.*)(?=\")";
            var BtnIdQ1 = helper.GetStringByRegExp(htmlDoc, btnLoginRegExp);
            var allCookies = response.Headers[HttpResponseHeader.SetCookie].Split(';').ToList();
           

            Console.WriteLine("------------------------Query1Data---------------------------");
            string UrlQ1 = string.Concat(Constants.UrlSeosprint, BtnIdQ1);
            CookieContainer mainCookieContainer = new CookieContainer();
            var uri = new Uri(Constants.UrlSeosprint);
            mainCookieContainer.SetCookies(uri, allCookies[0].Trim());
            mainCookieContainer.SetCookies(uri, allCookies[1].Trim());
            string CookieQ1 = string.Empty;
            foreach (var item in allCookies)
            {
                CookieQ1 += item+" ";
            }

            string RefererQ1 = string.Concat(Constants.UrlSeosprint, "/");
            Console.WriteLine("Next Query Url:{0}\nCookies:{1}\nReferer:{2}\nBtnId:{3}", UrlQ1, CookieQ1, RefererQ1, BtnIdQ1);


            Console.WriteLine("---------------------Request 2 Start---------------------");
            HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(UrlQ1);
            request2.Method = Constants.MethodGet;
            request2.Accept = Constants.AcceptGet;
            request2.Host = Constants.DefaultHost;
            request2.UserAgent = Constants.UserAgent;
            request2.KeepAlive = Constants.KeepAlive;
            request2.Referer = RefererQ1;
            request2.CookieContainer = mainCookieContainer;

            var response2 = (HttpWebResponse)request2.GetResponse();
            HtmlDocument htmlDoc2 = helper.GetHtmlDocument(response2);
            var allCookies2 = response2.Headers[HttpResponseHeader.SetCookie].Split(';').ToList();
            response2.Close();

            var asklcntRegExp = "(?<=name=\"asklcnt\" value=\")(.*)(?=\")";
            var asklcnt = helper.GetStringByRegExp(htmlDoc2, asklcntRegExp);
            string LoginUrl = string.Concat(Constants.UrlSeosprint, "/proc-service/us-login.php");
            //string CookieQ2 = CookieQ1 + allCookies2[0];
            string RefererQ2 = UrlQ1;
            Console.WriteLine("Login Url:{0}\nReferer:{1}\nasklcnt:{2}", LoginUrl, RefererQ2, asklcnt);
           
            var CookieQ2 = string.Empty;
            foreach (var item in allCookies2)
            {
                CookieQ2 += item + " ";
            }

            mainCookieContainer.Add(response2.Cookies[0]);
            Console.WriteLine("Куки в контейнере созданном:{0}\nКуки котоыре пришли:{1}", mainCookieContainer.GetCookies(new Uri(Constants.UrlSeosprint))[3].Name, CookieQ2);
   
            helper.LoadCapcha(RefererQ2, mainCookieContainer);

            Console.WriteLine("---------------------Request 3 Start---------------------");
           // HttpWebRequest request2 = (HttpWebRequest)WebRequest.Create(UrlQ1);
            Console.ReadKey();
        }
    }
}
