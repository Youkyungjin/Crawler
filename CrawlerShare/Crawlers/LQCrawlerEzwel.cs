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

using HKLibrary.WEB;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;
using CrawlerShare;

namespace CrawlerShare
{
    class LQCrawlerEzwel : LQCrawlerBase
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

        public virtual bool First_UseData(Int32 goodsSeq, string cpcode, string goodscode)
        {
            // 웹 호출을 통해서 사용처리한다.
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
        }

        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            //레저큐 이즈웰 정규식
            string regstring = "";
            if (pExcelData.authoritySeq_ != 26)
            {
                regstring = @"(?<OptionName>\S+),\S+수량(?<Count>\d+)개";
            }
            else
            {
                regstring = @"(?<OptionName>\S+)";
            }
            
            //오타이어 정규식
            //string regstring = @"(?<OptionName>\S+):수량(?<Count>\d+)개"; 

            string[] optionarray = System.Text.RegularExpressions.Regex.Split(optionstring, "(],)");

            foreach (string curoption in optionarray)
            {
                optionstring = curoption.Replace("],", "]");
                optionstring = optionstring.Replace(" ", "");
                Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(optionstring);

                foreach (Match mat in oe)
                {
                    GroupCollection group = mat.Groups; 
                    optionname = Convert.ToString(group["OptionName"].Value);
                    optionname = Regex.Replace(optionname, @"[^a-zA-Z0-9가-힣]", "");

                    if (pExcelData.BuyCount_ != 0)
                    {
                        nBuycount = pExcelData.BuyCount_;
                    }
                    else
                    {
                        nBuycount = Convert.ToInt32(group["Count"].Value);
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
            }

            return nTotalcount;
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
