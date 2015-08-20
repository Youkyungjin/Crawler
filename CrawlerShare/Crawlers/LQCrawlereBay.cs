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
    public class LQCrawlereBay : LQCrawlerBase
    {
        string str_sitecompare_ = "";
        string str_use_url_1_ = "";
        string str_use_param_1_ = "";
        string str_use_check_1_ = "";

        string str_use_url_2_ = "";
        string str_use_param_2_ = "";
        string str_use_check_2_ = "";

        // 체크해야 하는 데이터인가? 채널별로 하지 않아도 되는것이 있다.
        public override bool IsNeedCheck(tblOrderData pExcelData)
        {
            return true;
        }

        public void SetUseInfo(string comparesitename, string useurl1, string useparam1, string usecheck1
            , string useurl2, string useparam2, string usecheck2)
        {
            str_sitecompare_ = comparesitename;
            str_use_url_1_ = useurl1;
            str_use_param_1_ = useparam1;
            str_use_check_1_ = usecheck1;

            str_use_url_2_ = useurl2;
            str_use_param_2_ = useparam2;
            str_use_check_2_ = usecheck2;
        }

        public override bool First_UseData(Int32 goodsSeq, string cpcode, string goodscode)
        {
            //// 웹 호출을 통해서 사용처리한다.
            string ticketcode = "";
            string blackCode = "";
            if (GetUseTicketInfo(goodsSeq, cpcode, ref ticketcode, ref blackCode) == false)
                return false;

            if (use_step_1(ticketcode) == false)
                return false;


            if (use_step_2(ticketcode) == false)
                return false;


            return true;
        }

        // 상품 사용 처리 티켓번호 얻어오기
        bool GetUseTicketInfo(Int32 goodsSeq, string cpcode, ref string ticketcode, ref string blackCode)
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

        bool use_step_1(string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = str_use_url_1_;
            string useparam = str_use_param_1_;

            useparam = useparam.Replace("{TicketCode}", ticketcode);

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

        bool use_step_2(string ticketcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = str_use_url_2_;
            string useparam = str_use_param_2_;

            useparam = useparam.Replace("{TicketCode}", ticketcode);

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

        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            return true;   // 사용처리 이베이는 막아두자.

            //// 웹 호출을 통해서 사용처리한다.
            //string ticketcode = "";
            //if (GetUseTicketInfo(goodsSeq, cpcode, ref ticketcode) == false)
            //    return false;

            //if (use_step_1(ticketcode) == false)
            //    return false;


            //if (use_step_2(ticketcode) == false)
            //    return false;


            //return true;
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
            string regstring = @"(?<OptionName>\S+)/\S+/(?<Count>\d+)개|(?<OptionName>\S+)/(?<Count>\d+)개";

            // 옵션명 개수 빼기
            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(optionstring);

            optionname = Convert.ToString(Regex.Replace(oe[0].Groups["OptionName"].Value, @"[^a-zA-Z0-9가-힣]", ""));
            nBuycount = Convert.ToInt32(oe[0].Groups["Count"].Value);
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

        public override bool Process_RefundData(SqlHelper MySqlDB)
        {
            return true;
        }

    }
}
