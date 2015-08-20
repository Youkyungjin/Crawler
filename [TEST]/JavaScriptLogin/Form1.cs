using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Web;
using CrawlerShare;
using HKLibrary;
using System.Windows.Forms;
using mshtml;
using HKLibrary.WEB;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

//using System.Web.UI.Sc;

namespace JavaScriptLogin
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
            string url,
            string cookieName,
            StringBuilder cookieData,
            ref int size,
            Int32 dwFlags,
            IntPtr lpReserved);

        private const Int32 InternetCookieHttponly = 0x2000;

        public static CookieContainer GetUriCookieContainer(Uri uri)
        {
            CookieContainer cookies = null;
            // Determine the size of the cookie  
            int datasize = 8192 * 16;
            StringBuilder cookieData = new StringBuilder(datasize);
            if (!InternetGetCookieEx(uri.ToString(), null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
            {
                if (datasize < 0)
                    return null;
                // Allocate stringbuilder large enough to hold the cookie  
                cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(
                    uri.ToString(),
                    null, cookieData,
                    ref datasize,
                    InternetCookieHttponly,
                    IntPtr.Zero))
                    return null;
            }
            if (cookieData.Length > 0)
            {
                cookies = new CookieContainer();
                cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
                //MessageBox.Show(cookieData.ToString());  
            }
            return cookies;
        }  

        private void button1_Click(object sender, EventArgs e)
        {
            LogManager.Instance.SetLogFile("test.txt");

            WebBrowser wb = new WebBrowser();
            
            wb.ScrollBarsEnabled = false;
            wb.ScriptErrorsSuppressed = true;
            //wb.Navigate("https://login.soffice.11st.co.kr/login/LoginOk.tmalls");
            wb.Navigate("https://login.soffice.11st.co.kr/login/Login.tmall?returnURL=http%3A%2F%2Fsoffice.11st.co.kr%2F");
            while (wb.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }
            
            //wb.Document.Cookie

            //mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)wb.Document;
            //LogManager.Instance.Log(doc.Get);
            string de = wb.DocumentText;
            //LogManager.Instance.Log(de);
            HtmlElement p = wb.Document.GetElementById("loginName");
            //            HtmlElementCollection p2 = wb.Document.GetElementsByTagName("encryptedLoginName");
            HtmlElement p3 = wb.Document.GetElementById("passWord");//.GetAttribute("passWord");
            string gw = p.GetAttribute("encryptedLoginName");
            p.SetAttribute("value", "leisureq");
            p3.SetAttribute("value", "today007");
            //Int32 i = wb.Document.Forms.Count;

            //LogManager.Instance.Log(wb.Document.DomDocument.ToString());
            //Object obj = wb.Document.InvokeScript("alret", new object[] { "today007" });
            //Object obj = wb.Document.InvokeScript("RSAEncrypt", new object[] { "today007" });
            string temp = p.GetAttribute("value");
            Object obj = wb.Document.InvokeScript("checkForm", new object[] { "today007" });

            //Object obj2 = wb.Document.InvokeScript("encrpyt", new object[] { "today007" });
            temp = p.GetAttribute("value");



            HtmlElement p4 = wb.Document.GetElementById("encryptedLoginName");
            HtmlElement p5 = wb.Document.GetElementById("encryptedPassWord");
            string v = p4.GetAttribute("value");
            string v2 = p5.GetAttribute("value");

            if (obj != null)
            {
                string ne = obj.ToString();
                MessageBox.Show(ne);
            }

            //if (obj2 != null)
            //{
            //    string ne = obj.ToString();
            //    MessageBox.Show(ne);
            //}


            // 쿠키 만들기
            CookieContainer cookie_ = new CookieContainer();

            foreach (string cookie in wb.Document.Cookie.Split(';'))
            {

                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string domain = @"login.soffice.11st.co.kr";
                //cookie_.Add(new Cookie(name, value));
                string path = "/";
                //string domain = ".google.com"; //change to your domain name
                Cookie pC = new Cookie(name.Trim(), value.Trim(), path, domain);
                cookie_.Add(pC);
            }

            //string refff = @"https://login.soffice.11st.co.kr/login/Login.tmall?returnURL=http%3A%2F%2Fsoffice.11st.co.kr%2F";

            //{
            //    string callurl = @"https://ds.11st.co.kr/NetInsight/text/11st/11st_sub/sub@subGNB";
            //    string callparam = @"noCache=20141002053623&category=0";

            //    HttpWebResponse pResponseo = HKHttpWebRequest.ReqHttpRequest("GET", callurl, callparam, cookie_, refff);

            //}

            gw = p.GetAttribute("encryptedLoginName");
            gw = p.GetAttribute("passWord");


            MessageBox.Show("0");

            //1
            {
                string url = @"https://soffice.11st.co.kr/escrow/OrderingLogistics.tmall";
                

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("GET", url, null, cookie_);
            }

            MessageBox.Show("1");

            //1
            {
                string url = @"https://soffice.11st.co.kr/escrow/UnapprovedOrder.tmall?method=getUnapprovedOrderTotal";
                string param = @"listType=orderingConfirm&isAbrdSellerYn=&isItalyAgencyYn=";

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("POST", url, param, cookie_);
            }
            MessageBox.Show("2");

            ////2
            //{
            //    string url = @"https://soffice.11st.co.kr/marketing/SellerMenuAction.tmall";
            //    string param = "method=getMenuNoticePopup&dispSpceNo=1549604&cookie_chk=Y&close_btn=Y";



            //    HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("POST", url, param, cookie_);
            //}

            MessageBox.Show("3");

            //2
            {
                string url = @"https://soffice.11st.co.kr/marketing/SellerMenuAction.tmall";
                string param = @"method=getMenuNoticePopup&dispSpceNo=1549604&cookie_chk=Y&close_btn=Y";

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("POST", url, param, cookie_);
            }

            MessageBox.Show("4");
            //3
            {
                string url = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getOrderLogisticsList&listType=orderingLogistics";
                string param = @"start=0&limit=100&shDateType=01&shDateFrom=20140707&shDateTo=20141006&shBuyerType=&shBuyerText=&shProductStat=202&shDelayReport=&shPurchaseConfirm=&shGblDlv=N&prdNo=&shStckNo=&shOrderType=on&addrSeq=&isAbrdSellerYn=&abrdOrdPrdStat=&isItalyAgencyYn=&shErrYN=";

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("POST", url, param, cookie_);
            }

            MessageBox.Show("5");

            //4
            {
                string url = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getOrderLogisticsList";
                string param = @"listType=orderingTotal&shDateType=01&shDateFrom=20140707&shDateTo=20141006&shBuyerType=&shBuyerText=&shProductStat=202&shDelayReport=&shPurchaseConfirm=&shGblDlv=&shOrderType=on&shStckNo=&prdNo=&addrSeq=&isAbrdSellerYn=&abrdOrdPrdStat=&isItalyAgencyYn=&shErrYN=";

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("POST", url, param, cookie_);
            }

            MessageBox.Show("6");

            //wb.
            //5
            {
                string url = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcelColumn&isAbrdSellerYn=&excelID=35930_202&isItalyAgencyYn=";                

                HttpWebResponse pResponse7 = HKHttpWebRequest.ReqHttpRequest("GET", url, null, cookie_);
            }

            MessageBox.Show("7");

            //6
            {
                string downurl = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
                string gdownparam = @"excelColumnList=0%2F1%2F2%2F3%2F4%2F5%2F6%2F7%2F8%2F9%2F10%2F11%2F12%2F13%2F14%2F15%2F16%2F17%2F18%2F19%2F20%2F21%2F22%2F23%2F24%2F25%2F26%2F27%2F28%2F29%2F30%2F31%2F32%2F33%2F34%2F35%2F36%2F37%2F38%2F39%2F40%2F41%2F42%2F43%2F44%2F45%2F46%2F47%2F48%2F49%2F50%2F51%2F52%2F53%2F54%2F56%2F57%2F58%2F59%2F60%2F61%2F62%2F63%2F64%2F65&excelDownType=oldExcel&abrdOrdPrdStat=&excelShGblDlv=N&shBuyerType=&shBuyerText=&shErrYN=&shProductStat=202&abrdOrdPrdStat420=&abrdOrdPrdStat301=&abrdOrdPrdStat401=&shOrderType=on&addrSeq=&shDateType=01&shDateFrom=2014%2F07%2F07&shDateTo=2014%2F10%2F06&searchDt=8&shDelayReport=&shPurchaseConfirm=&shGblDlv=&dlvMthdCd=%B9%E8%BC%DB%C7%CA%BF%E4%BE%F8%C0%BD&dlvCd=00&pagePerSize=100&listType=orderingConfirm&delaySendDt=&delaySendRsnCd=&delaySendRsn=&orderConfrim=&shStckNo=&prdNo=&hiddenStatusOrder=&hiddenShProductStat=&hiddenCheck=&hiddenprdNo=&hiddenshStckNo=";

                HttpWebResponse pResponse2 = HKHttpWebRequest.ReqHttpRequest("POST", downurl, gdownparam, cookie_);

                string downString = @"d:\ekri.xls";

                if (pResponse2.CharacterSet == "" || pResponse2.CharacterSet == "euc-kr" || pResponse2.CharacterSet == "EUC-KR")
                {
                    FileStream fs = File.OpenWrite(downString);

                    string d = pResponse2.CharacterSet;
                    Stream responsestream = pResponse2.GetResponseStream();
                    byte[] buffer = new byte[2048];

                    long totalBytesRead = 0;
                    int bytesRead;

                    while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        fs.Write(buffer, 0, bytesRead);
                    }
                    fs.Close();
                    fs.Dispose();
                }
                else
                {
                    TextReader textReader = (TextReader)new StreamReader(pResponse2.GetResponseStream(), Encoding.GetEncoding(pResponse2.CharacterSet));
                    string htmlBuffer = textReader.ReadToEnd();
                    HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                    textReader.Close();
                    textReader.Dispose();
                }
            }


        }


        WebBrowser wb_ = null;

        private void button2_Click(object sender, EventArgs e)
        {
            DateTime dt = System.DateTime.Now;
            string LogFileName = string.Format("CrawlerLog_{0:D4}{1:D2}{2:D2}.txt", dt.Year, dt.Month, dt.Day);


            LogManager.Instance.SetLogFile(LogFileName);

            LogManager.Instance.Log("START !!!!!!!!!!!!!!!!!!!!!!!!");

            wb_ = new WebBrowser();
            wb_.FileDownload += WebBrowser1_FileDownload;
            wb_.Navigated += WebBrowser1_Navigatged;
            wb_.Navigating+= WebBrowser1_Navigating;
            wb_.DocumentCompleted += webBrowser1_DocumentCompleted;

            wb_.ScrollBarsEnabled = false;
            wb_.ScriptErrorsSuppressed = true;
            //wb_.Navigate("https://login.soffice.11st.co.kr/login/LoginOk.tmalls");
            wb_.Navigate("https://login.soffice.11st.co.kr/login/Login.tmall?returnURL=http%3A%2F%2Fsoffice.11st.co.kr%2F");
            while (wb_.ReadyState != WebBrowserReadyState.Complete) { Application.DoEvents(); }

            //wb_.Document.Cookie

            //mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)wb_.Document;
            //LogManager.Instance.Log(doc.Get);
            string de = wb_.DocumentText;
            //LogManager.Instance.Log(de);
            HtmlElement p = wb_.Document.GetElementById("loginName");
            //            HtmlElementCollection p2 = wb_.Document.GetElementsByTagName("encryptedLoginName");
            HtmlElement p3 = wb_.Document.GetElementById("passWord");//.GetAttribute("passWord");
            string gw = p.GetAttribute("encryptedLoginName");
            p.SetAttribute("value", "leisureq");
            p3.SetAttribute("value", "today007");
            //Int32 i = wb_.Document.Forms.Count;

            //LogManager.Instance.Log(wb_.Document.DomDocument.ToString());
            //Object obj = wb_.Document.InvokeScript("alret", new object[] { "today007" });
            //Object obj = wb_.Document.InvokeScript("RSAEncrypt", new object[] { "today007" });
            string temp = p.GetAttribute("value");
            Object obj = wb_.Document.InvokeScript("checkForm", new object[] { "today007" });

            //Object obj2 = wb_.Document.InvokeScript("encrpyt", new object[] { "today007" });
            temp = p.GetAttribute("value");



            HtmlElement p4 = wb_.Document.GetElementById("encryptedLoginName");
            HtmlElement p5 = wb_.Document.GetElementById("encryptedPassWord");
            string v = p4.GetAttribute("value");
            string v2 = p5.GetAttribute("value");

            if (obj != null)
            {
                string ne = obj.ToString();
                MessageBox.Show(ne);
            }

            //if (obj2 != null)
            //{
            //    string ne = obj.ToString();
            //    MessageBox.Show(ne);
            //}


            
            

            //string refff = @"https://login.soffice.11st.co.kr/login/Login.tmall?returnURL=http%3A%2F%2Fsoffice.11st.co.kr%2F";

            //{
            //    string callurl = @"https://ds.11st.co.kr/NetInsight/text/11st/11st_sub/sub@subGNB";
            //    string callparam = @"noCache=20141002053623&category=0";

            //    HttpWebResponse pResponseo = HKHttpWebRequest.ReqHttpRequest("GET", callurl, callparam, cookie_, refff);

            //}

            gw = p.GetAttribute("encryptedLoginName");
            gw = p.GetAttribute("passWord");


            //
            return;
            MessageBox.Show("0");
            //1
            {
                wb_.Navigate("https://soffice.11st.co.kr/escrow/OrderingLogistics.tmall");
            }

            //MessageBox.Show("1");

            return;
            //1
            {
              //  CookieContainer cookie_ = GetUriCookieContainer(wb_.Url);
                string downurl = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
                string gdownparam = @"excelColumnList=0%2F1%2F2%2F3%2F4%2F5%2F6%2F7%2F8%2F9%2F10%2F11%2F12%2F13%2F14%2F15%2F16%2F17%2F18%2F19%2F20%2F21%2F22%2F23%2F24%2F25%2F26%2F27%2F28%2F29%2F30%2F31%2F32%2F33%2F34%2F35%2F36%2F37%2F38%2F39%2F40%2F41%2F42%2F43%2F44%2F45%2F46%2F47%2F48%2F49%2F50%2F51%2F52%2F53%2F54%2F56%2F57%2F58%2F59%2F60%2F61%2F62%2F63%2F64%2F65&excelDownType=oldExcel&abrdOrdPrdStat=&excelShGblDlv=N&shBuyerType=&shBuyerText=&shErrYN=&shProductStat=202&abrdOrdPrdStat420=&abrdOrdPrdStat301=&abrdOrdPrdStat401=&shOrderType=on&addrSeq=&shDateType=01&shDateFrom=2014%2F07%2F07&shDateTo=2014%2F10%2F06&searchDt=8&shDelayReport=&shPurchaseConfirm=&shGblDlv=&dlvMthdCd=%B9%E8%BC%DB%C7%CA%BF%E4%BE%F8%C0%BD&dlvCd=00&pagePerSize=100&listType=orderingConfirm&delaySendDt=&delaySendRsnCd=&delaySendRsn=&orderConfrim=&shStckNo=&prdNo=&hiddenStatusOrder=&hiddenShProductStat=&hiddenCheck=&hiddenprdNo=&hiddenshStckNo=";

                //string strPostData = string.Format("id={0}&pw={1}", "idvalue", "passwordvalue");

                byte[] postData = Encoding.Default.GetBytes(gdownparam);
                wb_.Navigate(downurl, null, postData, "Content-Type: application/x-www-form-urlencoded");
                
                //wb_.Navigate("https://soffice.11st.co.kr/escrow/OrderingLogistics.tmall", );
                //string url = @"https://soffice.11st.co.kr/escrow/UnapprovedOrder.tmall?method=getUnapprovedOrderTotal";
                //string param = @"listType=orderingConfirm&isAbrdSellerYn=&isItalyAgencyYn=";

                //HttpWebResponse pResponse2 = HKHttpWebRequest.ReqHttpRequest("POST", downurl, gdownparam, cookie_);

                //string downString = @"d:\ekri.xls";

                //if (pResponse2.CharacterSet == "" || pResponse2.CharacterSet == "euc-kr" || pResponse2.CharacterSet == "EUC-KR")
                //{
                //    FileStream fs = File.OpenWrite(downString);

                //    string d = pResponse2.CharacterSet;
                //    Stream responsestream = pResponse2.GetResponseStream();
                //    byte[] buffer = new byte[2048];

                //    long totalBytesRead = 0;
                //    int bytesRead;

                //    while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        totalBytesRead += bytesRead;
                //        fs.Write(buffer, 0, bytesRead);
                //    }
                //    fs.Close();
                //    fs.Dispose();
                //}
                //else
                //{
                //    TextReader textReader = (TextReader)new StreamReader(pResponse2.GetResponseStream(), Encoding.GetEncoding(pResponse2.CharacterSet));
                //    string htmlBuffer = textReader.ReadToEnd();
                //    HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                //    textReader.Close();
                //    textReader.Dispose();
                //}
            }
            //MessageBox.Show("2");
        }

        private void WebBrowser1_FileDownload(Object sender, EventArgs e)
        {
            //LogManager.Instance.Log(string.Format("download {0}", e.ToString()));
            
            //e.GetType();
            //WebBrowserFile

            

            //WebBrowserNavigatingEventArgs eve
            //MessageBox.Show("You are in the WebBrowser.FileDownload event.");
            //LogManager.Instance.Log()

        }

        private void WebBrowser1_Navigatged(Object sender, WebBrowserNavigatedEventArgs e)
        {
            LogManager.Instance.Log(string.Format("Navigated {0}", e.Url.ToString()));
            //if (e.Url.ToString().IndexOf("escrow/OrderingLogisticsAction.tmall") >= 0)
            //{

            //    MessageBox.Show("WebBrowser1_Navigatged");

            //    wb_.Stop();
            //    return;
            //}
            //e.Url.ToString();
            
            //LogManager.Instance.Log(e.Url.ToString());
            //WebBrowserNavigatingEventArgs
            //e.GetType();
            //WebBrowserNavigatedEventHandler p = (WebBrowserNavigatedEventHandler)e;

            //MessageBox.Show("You are in the WebBrowser.WebBrowser1_Navigatged event.");

        }

        bool bTryDownload_ = false;
        private void WebBrowser1_Navigating(Object sender, WebBrowserNavigatingEventArgs e)
        {
            //LogManager.Instance.Log(string.Format("WebBrowser1_Navigating {0}", e.Url.ToString()));

         
            
            
            //e.Url.
            
            //LogManager.Instance.Log(string.Format("WebBrowser1_Navigating {0}", e.Url.ToString()));
            //LogManager.Instance.Log(string.Format("WebBrowser1_Navigating {0}", e.Url.IsFile));
            //MessageBox.Show("You are in the WebBrowser.WebBrowser1_Navigating event.");

        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            MessageBox.Show("File downloaded");
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            // Better use the e parameter to get the url.
            // ... This makes the method more generic and reusable.
            if (e.Url.ToString().IndexOf("about:blank") >= 0)
                return;
            LogManager.Instance.Log(string.Format("webBrowser1_DocumentCompleted {0}", e.Url.ToString()));
            //LogManager.Instance.Log(string.Format("webBrowser1_DocumentCompleted ab {0}", e.Url.AbsoluteUri));
            //LogManager.Instance.Log(string.Format("webBrowser1_DocumentCompleted ori {0}", e.Url.OriginalString));
            

            
            //if (string.Compare(e.Url.ToString(), @"https://soffice.11st.co.kr/escrow/OrderingLogistics.tmall") == 0)

            if (e.Url.ToString().IndexOf("https://login.soffice.11st.co.kr/login/LoginOk") >= 0)
            {
                LogManager.Instance.Log("Login 성공");
                OrderSomeThing();
            }   
            else if (e.Url.ToString().IndexOf("escrow/OrderingLogistics.tmall") >= 0)
            {
                LogManager.Instance.Log("AAAAAAAAA !!!!!!!!!!");
                //MessageBox.Show("AAAAAAAAA");
                //download();
            }
            //else if (e.Url.ToString().IndexOf("escrow/OrderingLogisticsAction.tmall") >= 0)
            //{
            //    LogManager.Instance.Log(wb_.Document.ToString());
                
            //    LogManager.Instance.Log("BBBBBBBBB !!!!!!!!!!");
            //    MessageBox.Show("BBBBBBBBB");
            //}
            
        }

        void OrderSomeThing()
        {
            download();
        }

        void download()
        {
            LogManager.Instance.Log(string.Format("download"));


            //string downurl = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
            //string gdownparam = @"excelColumnList=0%2F1%2F2%2F3%2F4%2F5%2F6%2F7%2F8%2F9%2F10%2F11%2F12%2F13%2F14%2F15%2F16%2F17%2F18%2F19%2F20%2F21%2F22%2F23%2F24%2F25%2F26%2F27%2F28%2F29%2F30%2F31%2F32%2F33%2F34%2F35%2F36%2F37%2F38%2F39%2F40%2F41%2F42%2F43%2F44%2F45%2F46%2F47%2F48%2F49%2F50%2F51%2F52%2F53%2F54%2F56%2F57%2F58%2F59%2F60%2F61%2F62%2F63%2F64%2F65&excelDownType=oldExcel&abrdOrdPrdStat=&excelShGblDlv=N&shBuyerType=&shBuyerText=&shErrYN=&shProductStat=202&abrdOrdPrdStat420=&abrdOrdPrdStat301=&abrdOrdPrdStat401=&shOrderType=on&addrSeq=&shDateType=01&shDateFrom=2014%2F07%2F07&shDateTo=2014%2F10%2F06&searchDt=8&shDelayReport=&shPurchaseConfirm=&shGblDlv=&dlvMthdCd=%B9%E8%BC%DB%C7%CA%BF%E4%BE%F8%C0%BD&dlvCd=00&pagePerSize=100&listType=orderingConfirm&delaySendDt=&delaySendRsnCd=&delaySendRsn=&orderConfrim=&shStckNo=&prdNo=&hiddenStatusOrder=&hiddenShProductStat=&hiddenCheck=&hiddenprdNo=&hiddenshStckNo=";

            //byte[] postData = Encoding.Default.GetBytes(gdownparam);
            //wb_.Navigate(downurl, null, postData, "Content-Type: application/x-www-form-urlencoded");


            //CookieContainer cookie_ = GetUriCookieContainer(wb_.Url);
            
            CookieContainer cookie_ = new CookieContainer();
            foreach (string cookie in wb_.Document.Cookie.Split(';'))
            {
                
                string name = cookie.Split('=')[0];
                string value = cookie.Substring(name.Length + 1);
                string domain = @"soffice.11st.co.kr";
                string wow = wb_.Document.Domain;
                //cookie_.Add(new Cookie(name, value));
                string path = "/";
                //string domain = ".google.com"; //change to your domain name
                Cookie pC = new Cookie(name.Trim(), value.Trim(), path, domain);
                cookie_.Add(pC);
            }

            string downurl = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
            string gdownparam = @"excelColumnList=0%2F1%2F2%2F3%2F4%2F5%2F6%2F7%2F8%2F9%2F10%2F11%2F12%2F13%2F14%2F15%2F16%2F17%2F18%2F19%2F20%2F21%2F22%2F23%2F24%2F25%2F26%2F27%2F28%2F29%2F30%2F31%2F32%2F33%2F34%2F35%2F36%2F37%2F38%2F39%2F40%2F41%2F42%2F43%2F44%2F45%2F46%2F47%2F48%2F49%2F50%2F51%2F52%2F53%2F54%2F56%2F57%2F58%2F59%2F60%2F61%2F62%2F63%2F64%2F65&excelDownType=oldExcel&abrdOrdPrdStat=&excelShGblDlv=N&shBuyerType=&shBuyerText=&shErrYN=&shProductStat=202&abrdOrdPrdStat420=&abrdOrdPrdStat301=&abrdOrdPrdStat401=&shOrderType=on&addrSeq=&shDateType=01&shDateFrom=2014%2F07%2F07&shDateTo=2014%2F10%2F06&searchDt=8&shDelayReport=&shPurchaseConfirm=&shGblDlv=&dlvMthdCd=%B9%E8%BC%DB%C7%CA%BF%E4%BE%F8%C0%BD&dlvCd=00&pagePerSize=100&listType=orderingConfirm&delaySendDt=&delaySendRsnCd=&delaySendRsn=&orderConfrim=&shStckNo=&prdNo=&hiddenStatusOrder=&hiddenShProductStat=&hiddenCheck=&hiddenprdNo=&hiddenshStckNo=";
            HttpWebResponse pResponse2 = HKHttpWebRequest.ReqHttpRequestTest("POST", downurl, gdownparam, cookie_);
            //HttpWebResponse pResponse2 = HKHttpWebRequest.ReqHttpRequest("POST", downurl, gdownparam, cookie_);

            
            string downString = @"d:\ekri.xls";

            if (pResponse2.CharacterSet == "" || pResponse2.CharacterSet == "euc-kr" || pResponse2.CharacterSet == "EUC-KR")
            {
                FileStream fs = File.OpenWrite(downString);

                string d = pResponse2.CharacterSet;
                Stream responsestream = pResponse2.GetResponseStream();
                byte[] buffer = new byte[2048];

                long totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    totalBytesRead += bytesRead;
                    fs.Write(buffer, 0, bytesRead);
                }
                fs.Close();
                fs.Dispose();
            }
            else
            {
                TextReader textReader = (TextReader)new StreamReader(pResponse2.GetResponseStream(), Encoding.GetEncoding(pResponse2.CharacterSet));
                string htmlBuffer = textReader.ReadToEnd();
                HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                textReader.Close();
                textReader.Dispose();
            }

            wb_.Stop();
        }
        //void DownLodedFile()
        //{
        //}
    }
}
