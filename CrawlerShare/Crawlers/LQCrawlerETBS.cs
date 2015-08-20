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

namespace CrawlerShare
{
    class LQCrawlerETBS : LQCrawlerBase
    {
        string str_down_url_1_ = "";
        string str_down_param_1_ = "";
        string str_down_check_1_ = "";

        public void SetUseInfo(string useurl1, string useparam1, string usecheck1)
        {
            str_down_url_1_ = useurl1;
            str_down_param_1_ = useparam1;
            str_down_check_1_ = usecheck1;
        }

        public bool Down_stap1(ref string secureKey, string sData, string eDate)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            Dictionary<Int32, ChannelGoodInfo> pInfoList = CrawlerManager.Instance.GetGoodsInfo();

            DateTime dtNow = DateTime.Now;

            string strurl = str_down_url_1_;
            string strparam = @"sch_date_type=ORDER_DATE&sch_rpt_status=&sch_sel_svcd_name=%BC%AD%BA%F1%BD%BA+%C0%FC%C3%BC&afterLogURL=%2Fwl%2Fservlets%2Ftbs.pmt.servlets.PayMainBackServlet%3Faction%3Dlist&type=I&curPage=1&sch_value=&sch_sel_vendor=leisureq&sch_sel_cmpy=&sch_sel_method_name=%B0%E1%C1%A6%B9%E6%B9%FD%C0%FC%C3%BC&sch_sel_method=&sch_to_order_date=2014-07-24&sch_fr_order_date=2014-07-24&xls=Y&sch_field=USER_NAME&sch_ord_status=&sch_pmt_status=&sch_sel_svcd=&sch_item=&ACCESS_TYPE=XLS&ACCESS_REASON=%BE%F7%B9%AB%BF%EB&ACCESS_ADMIN=E&DATA_MASK_YN=Y&ACCESS_AGREE=";//str_down_param_1_;
            string regstring = @"<input&nbsp;type=""hidden""&nbsp;name=""ACCESS_NO""&nbsp;value=""(?<SecurityKey>\S+)"">""";
            
            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(str_down_check_1_) < 0)
            {
                LogManager.Instance.Log(htmlBuffer);
                return false;
            }

            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(htmlBuffer);

            secureKey = oe[0].Groups["TicketCode"].ToString();

            return true;
        }

        public bool Down_stap2(ref string secureKey)
        {
            Dictionary<string, string> GoodsDownInfo = OrderManager.Instance.GetGoodsList();
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();
            DateTime dtNow = DateTime.Now;

            // 하위 폴더 만들기
            string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
            makefolder += "\\";
            makefolder += pCrawler.Channel_Idx_.ToString();
            makefolder += "\\";
            makefolder += dtNow.ToShortDateString();
            HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

            Dictionary<Int32, ChannelGoodInfo> pInfoList = CrawlerManager.Instance.GetGoodsInfo();

            foreach (var pData in pInfoList)
            {
                ChannelGoodInfo pGoodInfo = pData.Value;

                string downString = makefolder;
                downString += "\\";
                downString += pGoodInfo.Goods_Code_;
                downString += "_";
                downString += Convert.ToString(dtNow.Ticks);
                downString += ".xls";

                // 이미 다운로드가 끝난 파일이라면 다시 다운로드 하지 않는다.
                if (GoodsDownInfo.ContainsKey(pGoodInfo.Goods_Code_) == false)
                {
                    try
                    {
                        string method = pCrawler.ExcelDownMethod_;
                        string url = pCrawler.ExcelDownUrl_;
                        url = url.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);

                        string sendparameter = pCrawler.ExcelDownParameter_;

                        string eDate = "";
                        string sData = "";
                        if (pGoodInfo.eDateFormat_ != null)
                        {
                            DateTime beforeData = dtNow.AddMonths(-1);  // 3달 이상은 검색이 안되는 사이트가 있다.
                            eDate = string.Format(pGoodInfo.eDateFormat_, dtNow.Year, dtNow.Month, dtNow.Day);
                            sData = string.Format(pGoodInfo.eDateFormat_, beforeData.Year, beforeData.Month, beforeData.Day);
                        }

                        sendparameter = sendparameter.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);
                        sendparameter = sendparameter.Replace("{sDate}", sData);
                        sendparameter = sendparameter.Replace("{eDate}", eDate);

                        HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, cookie_, null, null, 180000);

                        
                        if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr")
                        {
                            FileStream fs = File.OpenWrite(downString);

                            string d = pResponse.CharacterSet;
                            Stream responsestream = pResponse.GetResponseStream();
                            byte[] buffer = new byte[2048];

                            long totalBytesRead = 0;
                            int bytesRead;

                            while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                totalBytesRead += bytesRead;
                                fs.Write(buffer, 0, bytesRead);
                            }
                            fs.Close();
                            fs.Dispose();
                        }
                        else
                        {
                            TextReader textReader = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding(pResponse.CharacterSet));
                            string htmlBuffer = textReader.ReadToEnd();
                            HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                            textReader.Close();
                            textReader.Dispose();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        LogManager.Instance.Log(ex.Message);
                        continue;
                    }

                    GoodsDownInfo.Add(pGoodInfo.Goods_Code_, downString);
                }

                LoadExcelAndInsertList(GoodsDownInfo[pGoodInfo.Goods_Code_], pGoodInfo.GoodsAttrType_, false, pGoodInfo.GoodsName_);
            }

            return true;
        }

        public override bool DownloadExcelAndDataMake()
        {
            string secureKey = "";
            string eDate = "";
            string sData = "";

            DateTime dtNow = DateTime.Now;
            DateTime beforeData = dtNow.AddMonths(-1);  // 3달 이상은 검색이 안되는 사이트가 있다.
            eDate = string.Format(@"{0}-{1}-{2}", dtNow.Year, dtNow.ToString("MM"), dtNow.Day);
            sData = string.Format(@"{0}-{1}-{2}", beforeData.Year, beforeData.ToString("MM"), beforeData.Day);

            /*if (Down_stap1(ref secureKey, sData, eDate) == false)
                return false;

            if (Down_stap2(ref secureKey) == false)
                return false;
            */
            return true;
        }
      
        public override Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            //            string regstring = @"(?<OptionName>\S+),\S+(?<Count>\d+)개";
            string regstring = @"(?<GoodsName>\S+)/(?<OptionName>\S+)";

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



        public override bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            return true;   // 사용처리 이베이는 막아두자.

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
