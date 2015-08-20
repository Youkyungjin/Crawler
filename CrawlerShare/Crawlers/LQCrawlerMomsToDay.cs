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
    public class LQCrawlerMomsToDay : LQCrawlerBase
    {
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



                    Int32 tempgoodSeq = -1;
                    tblOrderData pExcelData = new tblOrderData();
                    pExcelData.channelSeq_ = pCrawlerInfo.Channel_Idx_;
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
                    pExcelData.ExData_Cancel_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.ExData_Cancel_ == null) pExcelData.ExData_Cancel_ = ""; 

                    tRange = ws.Cells[nCurrentRow, ExData_Use];
                    pExcelData.ExData_Use_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.ExData_Use_ == null) pExcelData.ExData_Use_ = "";

                    tRange = ws.Cells[nCurrentRow, ExData_Buyphone];
                    pExcelData.orderPhone_ = Convert.ToString(tRange.Value2);
                    if (pExcelData.orderPhone_ == null) pExcelData.orderPhone_ = "";

                    pExcelData.orderPhone_ = pExcelData.orderPhone_.Replace("'", "");
                    pExcelData.orderPhone_ = pExcelData.orderPhone_.Replace(")", "-");
                    pExcelData.orderPhone_ = Regex.Replace(pExcelData.orderPhone_, @"^(01[016789]{1}|02|0[3-9]{1}[0-9]{1})-?([0-9]{3,4})-?([0-9]{4})$", @"$1-$2-$3");

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


                    double temp = Convert.ToDouble(tRange.Value2);
                    DateTime dta = DateTime.FromOADate(temp);
                    pExcelData.BuyDate_ = dta.ToString("u");
                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");

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


        public override void MakeDBData(tblOrderData pExcelData)
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();

            // DB에 저장되어 있던 값이 아니면 들어온 값이라면
            if (pOrderList.ContainsKey(pExcelData.channelOrderCode_) == false)
            {
                // 상태값 변경
                if (pExcelData.ExData_Use_.Contains(pCrawler.ExData_UseCheck_))
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                }
                else if (pExcelData.ExData_Cancel_ == pCrawler.ExData_CancelCheck_)
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND];
                }
                else if (pExcelData.ExData_Use_ == "정산완료" || pExcelData.ExData_Cancel_ == "정산완료")
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                }
                else
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];
                }

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

                if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                {// 예약한 상태 사용처리 해야함
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USER_WANT_REFUND])
                {// 사용 취소를 요청한 상태 사용취소처리 해야함
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY])
                {// DB 에는 구매만 한 상태인데, 엑셀에는 취소 요청을 해둔 상태라면 환불이다.
                    if (pExcelData.ExData_Cancel_ == pCrawler.ExData_CancelCheck_)
                    {
                        pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USER_WANT_REFUND];
                    }
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED])
                {
                    // 사용 처리된 상태인데, 뭔가 해줘야 할게 있는가?
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND])
                {// 환불이 끝난 상태 아무것도 하지 않는다.
                }
                else
                {
                    //pCrawler.ExData_UseCheck_
                    if (pExcelData.ExData_Use_.IndexOf(pCrawler.ExData_UseCheck_) >= 0)
                    {
                        pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        pDBData.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                        LogManager.Instance.Log(string.Format("채널에서 이미 사용처리 되어 있어서 DB 값만 바꾸자.{0}", pDBData.channelOrderCode_));
                    }
                    else
                    {
                        string log = string.Format("C:{0}, CP:{1}, S:{2}, {3}, {4} "
                        , pDBData.channelSeq_, pDBData.channelOrderCode_, pDBData.State_, pExcelData.ExData_Cancel_, pExcelData.ExData_Use_);
                        LogManager.Instance.Log(log);
                    }
                }
            }
        }
    }


}
 