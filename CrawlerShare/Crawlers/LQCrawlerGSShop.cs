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
    public class LQCrawlerGSShop : LQCrawlerBase
    {
        public override bool Login()
        {
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();
            cookie_ = new CookieContainer();


            // 1차 쿠키 받아오는곳
            try
            {
                string loginurl = "https://partneradmin.ezwel.com/cpadm/login/loginForm.ez";

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("GET", loginurl, "", cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding("EUC-KR"));
                string htmlBuffer = r.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
            }

            // 2차 로그인 처리
            try
            {
                string loginurl = "https://partneradmin.ezwel.com/cpadm/login/newLoginCheckAction.ez";
                string loginparameter = "&userId={0}&password={1}";
                loginparameter = string.Format(loginparameter, pCrawler.LoginID_, pCrawler.LoginPW_);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", loginurl, loginparameter, cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding("EUC-KR"));
                string htmlBuffer = r.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
            }

            // 실제 로그인
            try
            {
                string loginurl = pCrawler.LoginUrl_;
                string loginstring = pCrawler.LoginParam_.Replace("{LoginID}", pCrawler.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", pCrawler.LoginPW_);
                //byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(pCrawler.LoginMethod_, loginurl, loginstring, cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                //if (htmlBuffer.IndexOf(pCrawler.LoginCheck_) < 0)
                //    return false;

            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
                return false;
            }

            return true;
        }



        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            pExcelData.bFindInExcel_ = true;
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            OrderManager.Instance.AddExcelData(pExcelData);
        
            return 1;
        }


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
            Int32 ExData_Coupncode2 = 0;
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
                nCurrentRow       = pCrawlerInfo.ExData_Start_;
                ExData_Option     = pCrawlerInfo.ExData_Option_;
                ExData_Coupncode  = pCrawlerInfo.ExData_Coupncode_;
                ExData_Coupncode2 = 9;
                ExData_Buyer      = pCrawlerInfo.ExData_Buyer_;
                ExData_Cancel     = pCrawlerInfo.ExData_Cancel_;
                ExData_Use        = pCrawlerInfo.ExData_Use_;
                ExData_Buyphone   = pCrawlerInfo.ExData_Buyphone_;
                ExData_Price      = pCrawlerInfo.ExData_Price_;
                ExData_BuyDate    = pCrawlerInfo.ExData_Buydate_;
                ExData_BuyCount   = pCrawlerInfo.ExData_Count_;

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

                    tempString = Convert.ToString(tRange.Value2);
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

                    tRange = ws.Cells[nCurrentRow, ExData_Coupncode2];
                    if (tRange == null)
                        break;
                    string tempCouponCode2 = Convert.ToString(tRange.Value2);

                    pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_ + "_" + tempCouponCode2;

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
                    if (pCrawlerInfo.Channel_Idx_ == 9 || pCrawlerInfo.Channel_Idx_ == 14 || pCrawlerInfo.Channel_Idx_ == 15
                        || pCrawlerInfo.Channel_Idx_ == 18 || pCrawlerInfo.Channel_Idx_ == 23)
                    {
                        double temp = Convert.ToDouble(tRange.Value2);
                        DateTime dta = DateTime.FromOADate(temp);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");
                    }
                    else if (pCrawlerInfo.Channel_Idx_ == 22)
                    {
                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                        pExcelData.BuyDate_ = pExcelData.BuyDate_ + " 00:00:00";
                    }
                    else
                    {
                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                    }

                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('.', '-');

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

        // 이지웰은 그냥 사용처리 하면 된다.
        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            return true;
        }

        public override bool Cancel_Use(string cpcode, string goodscode)
        {
            return false;
        }

        public override bool Refund(string cpcode)
        {
            return false;
        }
              
        public override void MakeDBData(tblOrderData pExcelData)
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();

            // DB에 저장되어 있던 값이 아니면 들어온 값이라면
            if (pOrderList.ContainsKey(pExcelData.channelOrderCode_) == false)
            {
                //if (pExcelData.ExData_Use_ == pCrawler.ExData_UseCheck_)
                //{
                //    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                //}
                //else if (pExcelData.ExData_Cancel_ == pCrawler.ExData_CancelCheck_)
                //{
                //    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL];
                //}
                //else
                //{

                //}

                pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];

                // 신규 데이터라면 일단 DB에 넣는다.
                pExcelData.NeedDBProc_ = tblOrderData.NeedDBProc.Insert;
                pExcelData.bFindInExcel_ = true;
                pExcelData.bProcessed_ = true;
                pOrderList.Add(pExcelData.channelOrderCode_, pExcelData);
            }
            else
            {
                tblOrderData pDBData = pOrderList[pExcelData.channelOrderCode_];
                if (pExcelData.State_ == pDBData.State_)
                    return;

                pDBData.bFindInExcel_ = true;
                pDBData.bProcessed_ = true;
            }
        }

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }
    }
}
