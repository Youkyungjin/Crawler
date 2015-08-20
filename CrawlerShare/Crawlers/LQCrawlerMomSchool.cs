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
    public class LQCrawlerMomSchool : LQCrawlerBase
    {
        string str_sitecompare_ = "";

        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            if (string.IsNullOrEmpty(comparesitename) == true)
                return 0;

            if (comparesitename.IndexOf(str_sitecompare_) < 0)
                return 0;
            //string optionstring = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            string optionstring = Regex.Replace(pExcelData.ExData_Option_, @" ", "");
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            
            optionname = Convert.ToString(Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", ""));
            nBuycount = pExcelData.BuyCount_;
            for (Int32 i = 0; i < nBuycount; i++)
            {
                nTotalcount++;
                tblOrderData tempExcelData = new tblOrderData();
                tempExcelData.CopyFrom(pExcelData);
                tempExcelData.ExData_Option_ = optionname;
                tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);
                tempExcelData.bFindInExcel_ = true;
                OrderManager.Instance.AddExcelData(tempExcelData);
            }

            return nTotalcount;
        }

        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            return true;   //굿바이셀리는 막아두자

            //// 웹 호출을 통해서 사용처리한다.
            //string ticketcode = "";
            //if (GetUseTicketInfo(goodsSeq, cpcode, ref ticketcode) == false)
            //  return false;

            //if (use_step_1(ticketcode) == false)
            //    return false;


            //if (use_step_2(ticketcode) == false)
            //    return false;


            //return true;
        }

        public override void MakeDBData(tblOrderData pExcelData)
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();

            // DB에 저장되어 있던 값이 아니면 들어온 값이라면
            if (pOrderList.ContainsKey(pExcelData.channelOrderCode_) == false)
            {
                pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];

                // 신규 데이터라면 일단 DB에 넣는다.
                pExcelData.NeedDBProc_ = tblOrderData.NeedDBProc.Insert;
                pOrderList.Add(pExcelData.channelOrderCode_, pExcelData);
            }
            else
            {
                tblOrderData pDBData = pOrderList[pExcelData.channelOrderCode_];

                if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY])
                {
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED])
                {
                    // 사용 처리된 상태인데, 뭔가 해줘야 할게 있는가?
                }
            }
        }

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }
    }
}
