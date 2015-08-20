using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.IO;

namespace HKLibrary.WEB
{
    public class HKHttpWebRequest
    {
        public static HttpWebResponse ReqHttpRequestTest(string method, string url, string sendString
            , CookieContainer cookie, string refferr = null, Int32 TimeOut = 60000)
        {
            HttpWebResponse pResult = null;

            try
            {
                if (method == "POST")
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    if (refferr != null)
                        req.Referer = refferr;
                    req.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
                    req.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.124 Safari/537.36";
                    req.Host = "soffice.11st.co.kr";
                    req.Method = method;                    
                    req.CookieContainer = cookie;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Timeout = TimeOut;
                    //            req.Referer = "http://partneradmin.ezwel.com/cpadm/shop/order/orderList.ez";

                    if (sendString != null)
                    {
                        byte[] sendData = UTF8Encoding.UTF8.GetBytes(sendString);
                        req.ContentLength = sendData.Length;

                        Stream requestStream = req.GetRequestStream();
                        requestStream.Write(sendData, 0, sendData.Length);
                        requestStream.Close();
                    }

                    pResult = (HttpWebResponse)req.GetResponse();
                }
                else// GET 방식이다.
                {
                    string temp_url = url + "?" + sendString;
                    //string temp_url = string.Format("{0}?{1}", url, sendString);
                    Uri uri = new Uri(temp_url);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                    req.Method = method;
                    req.CookieContainer = cookie;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Timeout = TimeOut;
                    pResult = (HttpWebResponse)req.GetResponse();
                }
            }
            catch (System.Exception ex)
            {
                pResult = null;
            }

            return pResult;
        }

        public static HttpWebResponse WaitReqHttpRequest(string method, string url, string sendString, CookieContainer cookie, string refferr
            , Int32 TimeOut, AsyncCallback CallBack, object obj, string host = null)
        {
            HttpWebResponse pResult = null;

            try
            {
                if (method == "POST")
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    
                    if (refferr != null)
                        req.Referer = refferr;

                    req.Method = method;
                    req.CookieContainer = cookie;
                    req.ContentType = "application/x-www-form-urlencoded";
                    if (refferr != null)
                        req.Referer = refferr;
                    req.Timeout = TimeOut;
                    if (host != null)
                        req.Host = host;

                    //req.Connection = "keep-alive";

                    if (sendString != null)
                    {
                        byte[] sendData = UTF8Encoding.UTF8.GetBytes(sendString);
                        req.ContentLength = sendData.Length;                         
                        Stream requestStream = req.GetRequestStream();
                        requestStream.Write(sendData, 0, sendData.Length);
                        requestStream.Close();
                    }

                    //WebResponse response = await request.GetResponseAsync();

                    pResult = (HttpWebResponse)req.BeginGetResponse(CallBack, obj);
                }
                else// GET 방식이다.
                {
                    string temp_url = url;
                    if (string.IsNullOrEmpty(sendString) == false)
                        temp_url = url + "?" + sendString;

                    //string temp_url = string.Format("{0}?{1}", url, sendString);
                    Uri uri = new Uri(temp_url);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                    req.Method = method;
                    //req.Connection = "keep-alive";
                    req.CookieContainer = cookie;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.Timeout = TimeOut;

                    if (refferr != null)
                        req.Referer = refferr;

                    if (host != null)
                        req.Host = host;

                    pResult = (HttpWebResponse)req.BeginGetResponse(CallBack, obj);
                }
            }
            catch (System.Exception ex)
            {
                pResult = null;
            }

            return pResult;
        }

       
        public static HttpWebResponse ReqHttpRequest(string method, string url, string sendString, CookieContainer cookie, string refferr = null
            , string UserAgent = null, Int32 TimeOut = 60000, string contentType = "" )
        {
            HttpWebResponse pResult = null;
            
            try
            {
                if (method == "POST")
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                    if (refferr != null)
                        req.Referer = refferr;

                    req.Method = method;
                    req.CookieContainer = cookie;
                    if (contentType == "json")
                    {
                        req.ContentType = "application/json";
                    }
                    else
                    {
                        req.ContentType = "application/x-www-form-urlencoded";
                    }

                    if (refferr != null)
                        req.Referer = refferr;
                    req.Timeout = TimeOut;

                    if (UserAgent != null)
                        req.UserAgent = UserAgent;

                    if (sendString != null)
                    {
                        byte[] sendData = UTF8Encoding.UTF8.GetBytes(sendString);
                        req.ContentLength = sendData.Length;

                        Stream requestStream = req.GetRequestStream();
                        requestStream.Write(sendData, 0, sendData.Length);
                        requestStream.Close();
                    }

                    pResult = (HttpWebResponse)req.GetResponse();
                }
                else// GET 방식이다.
                {
                    string temp_url = url;
                    if (string.IsNullOrEmpty(sendString) == false)
                        temp_url = url + "?" + sendString;

                    Uri uri = new Uri(temp_url);
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(uri);
                    req.Method = method;
                    req.CookieContainer = cookie;
                    req.ContentType = "application/x-www-form-urlencoded";
                    if (UserAgent != null)
                        req.UserAgent = UserAgent;
                    req.Timeout = TimeOut;
                    if (refferr != null)
                        req.Referer = refferr;

                    pResult = (HttpWebResponse)req.GetResponse();
                }
            }
            catch (System.Exception ex)
            {
                pResult = null;
            }
            
            return pResult;
        } 
    }
}
