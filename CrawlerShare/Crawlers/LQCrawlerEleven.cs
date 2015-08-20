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
    class LQCrawlerEleven : LQCrawlerBase
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
            Int32 ExData_SecondCouponcode = 0;

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
                ExData_SecondCouponcode = 4; //이지웰은 주문번호가 다른 상품을 같이 사는경우 옵션별로 나뉘어지지 않고 똑같다
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

                    tempString = tRange.Value2;
                    if (tempString == null)
                    {
                        break;
                    }

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

                    tRange = ws.Cells[nCurrentRow, ExData_SecondCouponcode];
                    if (tRange == null)
                        break;
                    pExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, Convert.ToString(tRange.Value2));

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
                    if (pCrawlerInfo.Channel_Idx_ == 9 || pCrawlerInfo.Channel_Idx_ == 14 || pCrawlerInfo.Channel_Idx_ == 15)
                    {
                        double temp = Convert.ToDouble(tRange.Value2);
                        DateTime dta = DateTime.FromOADate(temp);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");
                    }
                    else
                    {
                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                    }

                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('.', '-');
                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('/', '-');
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

        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            //            string regstring = @"(?<OptionName>\S+),\S+(?<Count>\d+)개";
            string regstring = @"(?<OptionName>\S+)-\S+";

            optionstring = optionstring.Replace(" ", "");
            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(optionstring);

            foreach (Match mat in oe)
            {
                GroupCollection group = mat.Groups;
                optionname = Convert.ToString(group["OptionName"].Value);
                optionname = Regex.Replace(optionname, @"[^a-zA-Z0-9가-힣]", "");
                nBuycount = pExcelData.BuyCount_;

                for (Int32 i = 0; i < nBuycount; i++)
                {
                    nTotalcount++;
                    tblOrderData tempExcelData = new tblOrderData();
                    tempExcelData.CopyFrom(pExcelData);
                    tempExcelData.bFindInExcel_ = true;
                    tempExcelData.ExData_Option_ = optionname;
                    tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);
                    OrderManager.Instance.AddExcelData(tempExcelData);
                }
            }


            return nTotalcount;
        }

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }
    }
}
