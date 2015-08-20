using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using Microsoft.Office.Interop.Excel;
using System.Text.RegularExpressions;
using HKLibrary.WEB;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using CrawlerShare;

namespace CrawlerShare
{
    public class LQCrawlerCoupang : LQCrawlerBase
    {
        // 상품 정보를 DB 에서 읽어와서 그것에 따라서 상품명을 매칭하는 함수.
        public override bool LoadExcelAndInsertList(string filepath, Int32 GoodsAttrType, bool bFixedType, string goodsname)
        {
            LQStructures.LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();

            Microsoft.Office.Interop.Excel.Application ap = null;
            Workbook wb = null;
            Worksheet ws = null;
            HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);


            Range tRange = null;
            string tempString = "";
            string comparesitename = "";

            Int32 nCurrentRow = 0;
            Int32 ExData_Option = 0;
            Int32 ExData_Coupncode = 0;
            Int32 ExData_Buyer = 0;
            Int32 ExData_Cancel = 0;

            Int32 ExData_Use = 0;
            Int32 ExData_Buyphone = 0;
            Int32 ExData_Price = 0;
            Int32 ExData_BuyDate = 0;
            Int32 ExData_BuyCount = 0;

            if (bFixedType == true)
            {// 레저큐 양식일때는 고정값으로
                nCurrentRow = 2;
                ExData_Option = 4;
                ExData_Coupncode = 3;
                ExData_Buyer = 1;
                ExData_Cancel = 6;
                ExData_Use = 6;
                ExData_Buyphone = 2;
                ExData_Price = 5;
                ExData_BuyDate = 7;
                ExData_BuyCount = 8;
            }
            else
            {
                nCurrentRow = pCrawlerInfo.ExData_Start_;
                ExData_Option = pCrawlerInfo.ExData_Option_;
                ExData_Coupncode = pCrawlerInfo.ExData_Coupncode_;
                ExData_Buyer = pCrawlerInfo.ExData_Buyer_;
                ExData_Cancel = pCrawlerInfo.ExData_Cancel_;
                ExData_Use = pCrawlerInfo.ExData_Use_;
                ExData_Buyphone = pCrawlerInfo.ExData_Buyphone_;
                ExData_Price = pCrawlerInfo.ExData_Price_;
                ExData_BuyDate = pCrawlerInfo.ExData_Buydate_;
                ExData_BuyCount = pCrawlerInfo.ExData_Count_;

                // 티몬을 위한 변경
                if (GoodsAttrType == 1)
                {
                    nCurrentRow = 3;
                    ExData_Option = 6;
                    ExData_Coupncode = 3;
                    ExData_Buyer = 1;
                    ExData_Cancel = 8;
                    ExData_Use = 8;
                    ExData_Buyphone = 2;
                    ExData_Price = 7;
                    ExData_BuyDate = 9;
                }
            }

            while (true)
            {
                try
                {
                    tRange = ws.Cells[nCurrentRow, 1];
                    comparesitename = Convert.ToString(tRange.Value2);

                    tRange = ws.Cells[nCurrentRow, ExData_Option];
                    if (tRange == null)
                        break;

                    tempString = tRange.Value2;
                    if (tempString == null)
                    {
                        break;
                    }

                    Int32 tempgoodSeq = -1;
                    tblOrderData pExcelData = new tblOrderData();
                    pExcelData.channelSeq_ = pCrawlerInfo.Channel_Idx_;
                    pExcelData.authoritySeq_ = pCrawlerInfo.AuthoritySeq_;
                    //pExcelData.goodsCode_ = pGoodInfo.Goods_Code_;
                    pExcelData.goodsSeq_ = tempgoodSeq;
                    pExcelData.ExData_Option_ = tempString;
                    pExcelData.ExData_OptionOriginal_ = tempString;
                    if (string.IsNullOrEmpty(goodsname) == false)
                    {
                        pExcelData.ExData_GoodsName_ = goodsname;
                    }

                    tRange = ws.Cells[nCurrentRow, ExData_Coupncode];
                    if (tRange == null)
                        break;

                    pExcelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.channelOrderCode_ == null)
                        break;
                    pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Replace("'", "");
                    pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Trim();   // 공백 제거

                    tRange = ws.Cells[nCurrentRow, ExData_Buyer];
                    pExcelData.orderName_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.orderName_ == null) pExcelData.orderName_ = "";

                    tRange = ws.Cells[nCurrentRow, ExData_Cancel];
                    pExcelData.ExData_Cancel_ = tRange.Value2;
                    if (pExcelData.ExData_Cancel_ == null) pExcelData.ExData_Cancel_ = "";

                    tRange = ws.Cells[nCurrentRow, ExData_Use];
                    pExcelData.ExData_Use_ = tRange.Value2;
                    if (pExcelData.ExData_Use_ == null) pExcelData.ExData_Use_ = "";

                    tRange = ws.Cells[nCurrentRow, ExData_Buyphone];
                    pExcelData.orderPhone_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.orderPhone_ == null) pExcelData.orderPhone_ = "";

                    pExcelData.orderPhone_ = pExcelData.orderPhone_.Replace("'", "");


                    if (ExData_Price != 0)
                    {
                        tRange = ws.Cells[nCurrentRow, ExData_Price];

                        if (tRange.Value2 != null)
                        {// 돈에 , 가 있으면 제거하자.
                            tempString = Convert.ToString(tRange.Value2);
                            tempString = tempString.Replace(",", "");
                            pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                        }
                    }

                    tRange = ws.Cells[nCurrentRow, ExData_BuyDate];
                    pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('.', '-');
                    pExcelData.BuyDate_ = Regex.Replace(pExcelData.BuyDate_, @"^([0-9]{4})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})([0-9]{2})$", @"$1-$2-$3 $4:$5:$6");

                    if (ExData_BuyCount != 0)// 구매갯수를 따로 뽑아야 하는 채널에서만
                    {
                        tRange = ws.Cells[nCurrentRow, ExData_BuyCount];
                        pExcelData.BuyCount_ = Convert.ToInt32(tRange.Value2);
                    }

                    SplitDealAndInsertExcelData(pExcelData, comparesitename);

                }
                catch (System.Exception ex)
                {
                    LogManager.Instance.Log(string.Format("엑셀 파싱 에러 : {0}", ex.Message));
                    nCurrentRow++;
                    continue;
                }

                nCurrentRow++;
            }

            wb.Close(false, Type.Missing, Type.Missing);
            ap.Quit();

            Marshal.FinalReleaseComObject(ws);
            Marshal.FinalReleaseComObject(wb);
            Marshal.FinalReleaseComObject(ap);
            ws = null;
            wb = null;
            ap = null;
            GC.Collect();

            return true;
        }

        bool GetUseTicketInfo(string couponcode, string goodscode, ref string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string strurl = pCrawlerInfo.UseGoodsUrl_;
            string strparam = pCrawlerInfo.UseGoodsParam_;
            strparam = strparam.Replace("{CouponCode}", couponcode);
            strparam = strparam.Replace("{GoodsCode}", goodscode);

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

            Regex re = new Regex(pCrawlerInfo.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(htmlBuffer);

            ticketcode = oe[0].Groups["CouponCode2"].ToString();

            return true;
        }

        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            try
            {
                // 웹 호출을 통해서 사용처리한다.
                LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
                string useurl = pCrawlerInfo.UseUserUrl_;
                string useparam = pCrawlerInfo.UseUserParam_;

                // 두번째 쿠폰 코드 찾기
                Int32 n = cpcode.IndexOf('-');
                string cp_sub = "";
                if (n < 0)
                {
                    GetUseTicketInfo(cpcode, goodscode, ref cp_sub);
                }
                else
                {
                    cp_sub = cpcode.Substring(n + 1, 9);
                }

                useparam = useparam.Replace("{CouponCode}", cpcode);
                useparam = useparam.Replace("{CouponCode2}", cp_sub);
                useparam = useparam.Replace("{GoodsCode}", goodscode);

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
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(string.Format("사용처리 에러남 {0}", ex.Message));
                return false;
            }

            return true;
        }

        // 상품 사용 취소 처리
        public override bool Cancel_Use(string cpcode, string goodscode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.NUseUserUrl_;
            string useparam = pCrawlerInfo.NUseUserParam_;

            // 두번째 쿠폰 코드 찾기
            Int32 n = cpcode.IndexOf('-');
            string cp_sub = cpcode.Substring(n + 1, 9);

            useparam = useparam.Replace("{CouponCode}", cpcode);
            useparam = useparam.Replace("{CouponCode2}", cp_sub);
            useparam = useparam.Replace("{GoodsCode}", goodscode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);
            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.NUseUserCheck_) < 0)
            {
                LogManager.Instance.Log("public virtual bool Cancel_Use(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }

        // 환불 승인 처리
        public override bool Refund(string cpcode)
        {
            // 웹 호출을 통해서 사용처리한다.
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.RUseUserUrl_;
            string useparam = pCrawlerInfo.RUseUserParam_;

            // 두번째 쿠폰 코드 찾기
            Int32 n = cpcode.IndexOf('-');
            Int32 nCount = cpcode.Length - (n + 1);
            string cp_sub = cpcode.Substring(n + 1, nCount);

            useparam = useparam.Replace("{CouponCode2}", cp_sub);


            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);
            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.RUseUserCheck_) < 0)
            {
                LogManager.Instance.Log("public virtual bool Refund(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }
    }
}
