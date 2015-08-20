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
    class LQCrawlerOnedayMom : LQCrawlerBase
    {
        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string tempString = "";
            string optionname = "";
            string regstring = @"(?<OptionName>\S+)\((?<Price>\S+)원\)";
            //string regstring = @"(?<OptionName>\S+)\(\S+\)(?<Count>\d+)개";


            optionstring = optionstring.Replace(" ", "");
            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(optionstring);

            foreach (Match mat in oe)
            {
                GroupCollection group = mat.Groups;
                optionname = Convert.ToString(group["OptionName"].Value);
                optionname = Regex.Replace(optionname, @"[^a-zA-Z0-9가-힣]", "");
                tempString = Convert.ToString(group["Price"].Value);
                nBuycount = pExcelData.BuyCount_;

                if (tempString != null)
                {// 돈에 , 가 있으면 제거하자.
                    tempString = tempString.Replace(",", "");
                    pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                }


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

        public override bool First_UseData(Int32 goodsSeq, string cpcode, string goodscode)
        {
            // 웹 호출을 통해서 사용처리한다.
            
            /* 2014-07-31 사용처리가 그지같아서 막아놓음
             * 3개사서 2개 사용처리할려면 웹 URL파라미터값으로 2값으로 넘겨줘야함
             **/
            /*
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
             string useurl = pCrawlerInfo.UseUserUrl_;
             string useparam = pCrawlerInfo.UseUserParam_;

             string[] cpcodeArray = cpcode.Split('_');
             cpcode = cpcodeArray[0];

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
                 LogManager.Instance.Log("public virtual bool Use_Deal(string cpcode) " + htmlBuffer);
                 return false;
             }
             return true;
             */
            return false;
        }

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }
    }
}
