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
    class LQCrawlerGoodByeSelly : LQCrawlerBase
    {
        string str_sitecompare_ = "";
        string str_use_url_1_ = "";
        string str_use_param_1_ = "";
        string str_use_check_1_ = "";

        string str_use_url_2_ = "";
        string str_use_param_2_ = "";
        string str_use_check_2_ = "";

        string str_use_url_3_ = "";
        string str_use_param_3_ = "";
        string str_use_check_3_ = "";

        public void SetUseInfo(string comparesitename, string useurl1, string useparam1, string usecheck1
          , string useurl2, string useparam2, string usecheck2, string useurl3, string useparam3, string usecheck3)
        {
            str_sitecompare_ = comparesitename;
            str_use_url_1_ = useurl1;
            str_use_param_1_ = useparam1;
            str_use_check_1_ = usecheck1;

            str_use_url_2_ = useurl2;
            str_use_param_2_ = useparam2;
            str_use_check_2_ = usecheck2;

            str_use_url_3_ = useurl3;
            str_use_param_3_ = useparam3;
            str_use_check_3_ = usecheck3; 

        }

        public override bool First_UseData(Int32 goodsSeq, string cpcode, string goodscode)
        {
            //return true;   // 사용처리 이베이는 막아두자.

            //// 웹 호출을 통해서 사용처리한다.
            string ticketcode = "";
            if (GetUseTicketInfo(goodsSeq, cpcode, ref ticketcode) == false)
               return false;

            //사용처리 스텝1
            if (use_step_1(ticketcode, cpcode) == false)
                return false;

            //사용처리 스텝2
            if (use_step_2(ticketcode, cpcode) == false)
                return false;

            //사용처리 스텝3
            if (use_step_3(ticketcode, cpcode) == false)
                return false;

             return true;
        }

        // 상품 사용 처리 티켓번호 얻어오기
        bool GetUseTicketInfo(Int32 goodsSeq, string cpcode, ref string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            Dictionary<Int32, ChannelGoodInfo> pInfoList = CrawlerManager.Instance.GetGoodsInfo();

            ChannelGoodInfo pGoodInfo = pInfoList[goodsSeq];

            if (pGoodInfo == null)
            {
                string Message = string.Format("GetUseTicketInfo 매칭되는 코드가 없다.{0}/{1}{2}", goodsSeq, cpcode, ticketcode);
                LogManager.Instance.Log(Message);
                return false;
            }

            DateTime dtNow = DateTime.Now;
            string eDate = "";
            if (pGoodInfo.eDateFormat_ != null)
            {
                eDate = string.Format(pGoodInfo.eDateFormat_, dtNow.Year, dtNow.Month, dtNow.Day);
            }


            string strurl = pCrawlerInfo.UseGoodsUrl_;
            string strparam = pCrawlerInfo.UseGoodsParam_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];
            strparam = strparam.Replace("{CouponCode}", cpcode);
            strparam = strparam.Replace("{sDate}", pGoodInfo.sDate_);
            strparam = strparam.Replace("{eDate}", eDate);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("GET", strurl, strparam, cookie_);

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
            if (oe.Count != 0)
            {
                ticketcode = oe[0].Groups["TicketCode"].ToString();
            }
            else
            {
                return false;
            }
            return true;
        }

        bool use_step_1(string ticketcode, string cpcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = str_use_url_1_;
            string useparam = str_use_param_1_;
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

            if (htmlBuffer.IndexOf(str_use_check_1_) < 0)
            {
                LogManager.Instance.Log("public override bool use_step_1(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }


        bool use_step_2(string ticketcode, string cpcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = str_use_url_2_;
            string useparam = str_use_param_2_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];

            useparam = useparam.Replace("{TicketCode}", ticketcode);
            useparam = useparam.Replace("{CouponCode}", cpcode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(str_use_check_2_) < 0)
            {
                LogManager.Instance.Log("public override bool use_step_2(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }


        bool use_step_3(string ticketcode, string cpcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = str_use_url_3_;
            string useparam = str_use_param_3_;
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

            if (htmlBuffer.IndexOf(str_use_check_2_) < 0)
            {
                LogManager.Instance.Log("public override bool use_step_2(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }

        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            //            string regstring = @"(?<OptionName>\S+),\S+(?<Count>\d+)개";
            string regstring = @"(?<OptionName>\S+)\(\S+\)(?<Count>\d+)개";

            string[] optionarray = System.Text.RegularExpressions.Regex.Split(optionstring, "\n");

            foreach (string curoption in optionarray)
            {
                optionstring = curoption.Replace(" ", "");
                Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(optionstring);

                foreach (Match mat in oe)
                {
                    GroupCollection group = mat.Groups;
                    optionname = Convert.ToString(group["OptionName"].Value);
                    optionname = Regex.Replace(optionname, @"[^a-zA-Z0-9가-힣]", "");
                    nBuycount = Convert.ToInt32(group["Count"].Value);

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
