using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FictionReguestToGetKapcha
{
    public static class Constants
    {
        //Url-s on site
        public static string UrlSeosprint = "http://www.seosprint.net";

        //Request Info
        public static string Login = "tellyra@gmail.com";
        public static string Password = "38lMlPLFiS";
        public static string UserAgent = "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/30.0.1599.101 Safari/537.36";
        public static string AcceptGet = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
        public static string AcceptPost = "*/*";
        public static string AcceptLanguage = "ru-RU,ru;q=0.8,en-US;q=0.6,en;q=0.4";
        public static bool KeepAlive = true;
        public static string ContentTypePost = "application/x-www-form-urlencoded";
        public static string DefaultHost = "www.seosprint.net";
        public static string MethodGet = "GET";
        public static string MethodPost = "POST";
        //Other Info
        public static Encoding encodeUTF8 = System.Text.Encoding.GetEncoding("UTF-8");
        public static string pathToBrowser = @"c:\Users\iJSmile\AppData\Local\Google\Chrome\Application\chrome.exe";

        public static string GITHUBTEST = "testgithub";
    }
}
