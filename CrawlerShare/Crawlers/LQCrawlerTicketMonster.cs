using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using System.Web;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

using HKLibrary.WEB;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using CrawlerShare;

namespace CrawlerShare
{
    public class LQCrawlerTicketMonster : LQCrawlerBase
    {
        // 상품 사용 처리 티켓번호 얻어오기
        bool GetUseTicketInfo(string couponcode, ref string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string strurl = pCrawlerInfo.UseGoodsUrl_;
            string strparam = pCrawlerInfo.UseGoodsParam_;
            strparam = strparam.Replace("{CouponCode}", couponcode);

            LogManager.Instance.Log(strurl);
            LogManager.Instance.Log(strparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.UseGoodsCheck_) < 0)
            {
                LogManager.Instance.Log(htmlBuffer);
                return false;
            }
            string tempCouponCode = "";
            Regex re = new Regex(pCrawlerInfo.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(htmlBuffer);
            for (int i = 0; i < oe.Count; i++)
            {
                tempCouponCode = oe[i].Groups["CouponCode1"].ToString();
                tempCouponCode = tempCouponCode + oe[i].Groups["CouponCode2"].ToString();

                if (tempCouponCode == couponcode)
                {
                    ticketcode = oe[i].Groups["TicketCode"].ToString();
                }
            }



            return true;
        }

        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.UseUserUrl_;
            string useparam = pCrawlerInfo.UseUserParam_;

            string ticketcode = "";
            if (GetUseTicketInfo(cpcode, ref ticketcode) == false)
                return false;

            useparam = useparam.Replace("{GoodsCode}", goodscode);
            useparam = useparam.Replace("{TicketCode}", ticketcode);
            useparam = useparam.Replace("{CouponCode}", cpcode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.UseUserCheck_) < 0)
            {
                LogManager.Instance.Log(htmlBuffer);
                return false;

            }
            return true;
        }

        // 상품 사용 취소 처리 티켓번호 얻어오기
        bool GetUseCancelInfo(string couponcode, ref string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string strurl = pCrawlerInfo.NUseGoodsUrl_;
            string strparam = pCrawlerInfo.NUseGoodsParam_;
            strparam = strparam.Replace("{CouponCode}", couponcode);

            LogManager.Instance.Log(strurl);
            LogManager.Instance.Log(strparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.NUseGoodsCheck_) < 0)
            {
                LogManager.Instance.Log(htmlBuffer);
                return false;
            }

            Regex re = new Regex(pCrawlerInfo.NUseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(htmlBuffer);

            ticketcode = oe[0].Groups["TicketCode"].ToString();

            return true;
        }

        // 상품 사용 취소 처리
        public override bool Cancel_Use(string cpcode, string goodscode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.NUseUserUrl_;
            string useparam = pCrawlerInfo.NUseUserParam_;

            string ticketcode = "";
            if (GetUseCancelInfo(cpcode, ref ticketcode) == false)
                return false;

            useparam = useparam.Replace("{GoodsCode}", goodscode);
            useparam = useparam.Replace("{TicketCode}", ticketcode);
            useparam = useparam.Replace("{CouponCode}", cpcode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.NUseUserCheck_) < 0)
            {
                LogManager.Instance.Log(htmlBuffer);
                return false;

            }
            return true;
        }

        public override bool Refund(string cpcode)
        {
            return false;
        }
    }
}
