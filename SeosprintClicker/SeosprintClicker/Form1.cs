using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;

namespace SeosprintClicker
{
    public partial class Form1 : Form
    {
        public static HtmlAgilityPack.HtmlDocument html = new HtmlAgilityPack.HtmlDocument();
        public static WebHelper helper = new WebHelper();
        public static UTF8Encoding encodingUTF8 = new UTF8Encoding();
        public string log_cnt;
        public string EnterRefererQ2;
        #region CookieInfoFields
        public string phpsessCookie;

        #endregion
        public Form1()
        {
            InitializeComponent();
            PreEnterInitialization();
        }

        private void Enter_Click(object sender, EventArgs e)
        {
            string captchaCode = TextBoxCaptcha.Text;
            if (!string.IsNullOrWhiteSpace(captchaCode))
            {
                //---------------------POST Request To LOG IN---------------------
                HttpWebRequest request1 = helper.CreateRequestAndSetSomeStandartHeaders(Constants.UrlLogin,
                    new string[] { phpsessCookie }, EnterRefererQ2, true);
                #region Доп. куки
                //cookieContainer.SetCookies(uri, langCookie);
                //cookieContainer.SetCookies(uri, helloCookie);
                //cookieContainer.SetCookies(uri, visitorCookie);
                #endregion
                string parameters = new StringBuilder()
                                         .Append("log_email=" + Constants.Login)
                                         .Append("&log_pass=" + Constants.Password)
                                         .Append("&log_code=" + captchaCode)
                                         .Append("&log_cnt=" + log_cnt)
                                         .Append("&log_svd=0")
                                         .ToString();

                request1.ContentLength = encodingUTF8.GetByteCount(parameters);
                using (Stream requestStream = request1.GetRequestStream())
                {
                    requestStream.Write(encodingUTF8.GetBytes(parameters), 0,
                        encodingUTF8.GetByteCount(parameters));
                }
                helper.RequestAndSave(request1, "PostLoginQuery.txt");

                //---------------------Get Start Page AFTER LOGIN---------------------
                HttpWebRequest request2 = helper.CreateRequestAndSetSomeStandartHeaders(Constants.UrlSeosprint,
                   new string[] { phpsessCookie }, EnterRefererQ2, false);
                #region Доп. куки
                // visitorCookie = "visitor=2604341";
                // cookieContainer2.SetCookies(uri, langCookie);
                //cookieContainer2.SetCookies(uri, "hello=223.282");
                //cookieContainer2.SetCookies(uri, visitorCookie);
                #endregion
                helper.RequestAndSave(request2, "GetLoginQuery.txt");
                Status.Text = File.ReadAllText("PostLoginQuery.txt");
            }
            else
                Status.Text = "Капча не введена.";
        }

        private void PreEnterInitialization()
        {
            try
            {              
                //---------------------Request 1 Start---------------------
                HttpWebRequest request1 = helper.CreateRequestAndSetSomeStandartHeaders(Constants.UrlSeosprint,
                   new string[] { }, string.Empty, false);
                var response1 = (HttpWebResponse)request1.GetResponse();
                HtmlAgilityPack.HtmlDocument htmlDoc = helper.GetHtmlDocument(response1);
                string btnLoginRegExp = "(?<=class=\"btnlogin\" href=\")(.*)(?=\")";
                
                //-----------------------Query1Data-----------------------
                var BtnIdQ1 = helper.GetStringByRegExp(htmlDoc, btnLoginRegExp);
                var allCookies = response1.Headers[HttpResponseHeader.SetCookie].Split(';').ToList();
                string UrlQ1 = string.Concat(Constants.UrlSeosprint, BtnIdQ1);
                phpsessCookie = allCookies[0].Trim();
                string RefererQ1 = string.Concat(Constants.UrlSeosprint, "/");
                response1.Close();

                //---------------------Request 2 Start---------------------
                HttpWebRequest request2 = helper.CreateRequestAndSetSomeStandartHeaders(UrlQ1,
                 new string[] { phpsessCookie }, RefererQ1, false);
                var response2 = (HttpWebResponse)request2.GetResponse();
                HtmlAgilityPack.HtmlDocument htmlDoc2 = helper.GetHtmlDocument(response2);
                var asklcntRegExp = "(?<=name=\"asklcnt\" value=\")(.*)(?=\")";
                response2.Close();

                //-----------------------Query2Data-----------------------
                log_cnt = helper.GetStringByRegExp(htmlDoc2, asklcntRegExp);
                EnterRefererQ2 = UrlQ1;

                helper.LoadCapcha(EnterRefererQ2, new string[] { phpsessCookie });
                var image = Image.FromFile("captcha.png");
                Captcha.Height = image.Height;
                Captcha.Width = image.Width;
                Captcha.Image = image;
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }
    }
}
