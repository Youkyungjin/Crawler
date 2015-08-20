using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using HKLibrary.UTIL;
using CrawlerShare;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;
using CData;
using System.Net;
using HKLibrary.WEB;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace Channels
{
    public class Street11 : BaseChannel
    {
        bool bLoginSucceed_ = false;
        WebBrowser wb_ = null;
        // 로그인 Web
        public override bool Web_Login()
        {
            bLoginSucceed_ = false;
            Cookie_ = new CookieContainer();

            Thread nDetailThread = new Thread(new ThreadStart(Internal_Login));
            nDetailThread.SetApartmentState(ApartmentState.STA);
            nDetailThread.Start();

            Int32 waitTimeout = 10000;
            while (waitTimeout > 0)
            { 
                System.Windows.Forms.Application.DoEvents();
                Thread.Sleep(100);
                waitTimeout -= 100;

                if (bLoginSucceed_ == true)
                    break;
            }

            return bLoginSucceed_;
        }

        void Internal_Login()
        {
            try
            {
                wb_ = new WebBrowser();
                wb_.ScrollBarsEnabled = false;
                wb_.ScriptErrorsSuppressed = true;
                wb_.DocumentCompleted += webBrowser1_DocumentCompleted;
                if (LQCrawlerInfo_.AuthoritySeq_ == 17)
                {
                    wb_.Navigate(@"https://login.soffice.11st.co.kr/login/Login.tmall?returnURL=http%3A%2F%2Fsoffice.11st.co.kr%2F");
                }
                else
                {
                    wb_.Navigate(@"https://login.partner.11st.co.kr/login/Login.tmall");
                }
                

                while (wb_.ReadyState != WebBrowserReadyState.Complete)
                {
                    System.Windows.Forms.Application.DoEvents();
                }

                HtmlElement el1 = wb_.Document.GetElementById("loginName");
                HtmlElement el2 = wb_.Document.GetElementById("passWord");


                el1.SetAttribute("value", LQCrawlerInfo_.LoginID_);
                el2.SetAttribute("value", LQCrawlerInfo_.LoginPW_);

                wb_.Document.InvokeScript("checkForm");

                Int32 waitTimeout = 10000;
                while (waitTimeout > 0)
                {
                    System.Windows.Forms.Application.DoEvents();
                    Thread.Sleep(100);
                    waitTimeout -= 100;

                    if (bLoginSucceed_ == true)
                        break;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Errro Internal_Login - {0}", ex.Message));
            }
        }
        // WebBrowser 방식을 쓸때 로그인시 끝까지 기다리기 위해서.
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            //NewLogManager2.Instance.Log(e.Url.ToString());

            if (e.Url.ToString().IndexOf("about:blank") >= 0)
                return;
            //if (e.Url.ToString().IndexOf("https://login.soffice.11st.co.kr/login/LoginOk") >= 0)
            if (e.Url.ToString().IndexOf("LoginOk") >= 0)
            {
                foreach (string cookie in wb_.Document.Cookie.Split(';'))
                {
                    string name = cookie.Split('=')[0];
                    string value = cookie.Substring(name.Length + 1);
                    string domain = @"soffice.11st.co.kr";
                    string wow = wb_.Document.Domain;
                    string path = "/";
                    Cookie pC = new Cookie(name.Trim(), value.Trim(), path, domain);
                    Cookie_.Add(pC);
                }

                wb_.Document.ExecCommand("ClearAuthenticationCache", false, null);
                wb_.Navigate(@"about:blank");
                wb_.Dispose();
                wb_ = null;
                GC.Collect();
                bLoginSucceed_ = true;
            }
            else if (e.Url.ToString().IndexOf("partner.11st.co.kr/Index.tmall") >= 0)
            {
                foreach (string cookie in wb_.Document.Cookie.Split(';'))
                {
                    string name = cookie.Split('=')[0];
                    string value = cookie.Substring(name.Length + 1);
                    string domain = @"partner.11st.co.kr";
                    string wow = wb_.Document.Domain;
                    string path = "/";
                    Cookie pC = new Cookie(name.Trim(), value.Trim(), path, domain);
                    Cookie_.Add(pC);
                }

                wb_.Document.ExecCommand("ClearAuthenticationCache", false, null);
                wb_.Navigate(@"about:blank");
                wb_.Dispose();
                wb_ = null;
                GC.Collect();
                bLoginSucceed_ = true;
            }
        }
        // 엑셀 다운로드
        public override bool Web_DownLoadExcel()
        {
            try
            {
                ProcessStateManager.Instance.NeedDownLoadCount_ = GoodsInfoList_.Count;
                DateTime dtNow = DateTime.Now;

                // 하위 폴더 만들기
                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                foreach (var pData in GoodsInfoList_)
                {
                    ChannelGoodInfo pGoodInfo = pData.Value;
                    pGoodInfo.Goods_Code_ = "0000";
                    string downString = makefolder;
                    downString += "\\";
                    downString += pGoodInfo.Goods_Code_;
                    downString += "_";
                    downString += Convert.ToString(dtNow.Ticks);
                    downString += ".xls";

                    // 이미 다운로드가 끝난 파일이라면 다시 다운로드 하지 않는다.
                    if (GoodsDownInfo_.ContainsKey(pGoodInfo.Goods_Code_) == false)
                    {
                        try
                        {
                            string method = LQCrawlerInfo_.ExcelDownMethod_;
                            string url = "";
                            if (LQCrawlerInfo_.AuthoritySeq_ == 17) { 
                                 url = @"https://soffice.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
                            }
                            else
                            {
                                 url = @"https://partner.11st.co.kr/escrow/OrderingLogisticsAction.tmall?method=getLogisticsForExcel&isItalyAgencyYn=&isAbrdSellerYn=&listType=orderingLogistics";
                            }

                            string sendparameter = @"excelColumnList=0/1/2/3/4/5/6/7/8/9/10/11/12/13/14/15/16/17/18/19/20/21/22/23/24/25/26/27/28/29/30/31/32/33/34/35/36/37/38/39/40/41/42/43/44/45/46/47/48/49/50/51/52/53/54/56/57/58/59/60/61/62/63/64/65&excelDownType=oldExcel&abrdOrdPrdStat=&excelShGblDlv=N&shBuyerType=&shBuyerText=&shErrYN=&shProductStat=202&abrdOrdPrdStat420=&abrdOrdPrdStat301=&abrdOrdPrdStat401=&shOrderType=on&addrSeq=&shDateType=01&shDateFrom={sDate}&shDateTo={eDate}&searchDt=8&shDelayReport=&shPurchaseConfirm=&shGblDlv=&dlvMthdCd=%B9%E8%BC%DB%C7%CA%BF%E4%BE%F8%C0%BD&dlvCd=00&pagePerSize=100&listType=orderingConfirm&delaySendDt=&delaySendRsnCd=&delaySendRsn=&orderConfrim=&shStckNo=&prdNo=&hiddenStatusOrder=&hiddenShProductStat=&hiddenCheck=&hiddenprdNo=&hiddenshStckNo=";

                            string eDate = "";
                            string sData = "";
                            if (pGoodInfo.eDateFormat_ != null)
                            {
                                DateTime beforeData = dtNow.AddDays(-7);
                                eDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                                sData = string.Format("{0:D4}{1:D2}{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                            }

                            sendparameter = sendparameter.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);
                            sendparameter = sendparameter.Replace("{sDate}", sData);
                            sendparameter = sendparameter.Replace("{eDate}", eDate);

                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, Cookie_, null, null, 180000);

                            if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr" || pResponse.CharacterSet == "EUC-KR")
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
                            NewLogManager2.Instance.Log(ex.Message);
                            continue;
                        }

                        GoodsDownInfo_.Add(pGoodInfo.Goods_Code_, downString);
                        ProcessStateManager.Instance.CurDownLoadCount_++;
                    }
                    else
                    {
                        ProcessStateManager.Instance.PassDownLoadCount_++;
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_DownLoadExcel() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        protected override bool Internal_Excel_Parsing(ChannelGoodInfo pChannelGoodInfo)
        {
            try
            {
                if (GoodsDownInfo_.ContainsKey(pChannelGoodInfo.Goods_Code_) == false)
                {
                    NewLogManager2.Instance.Log(string.Format("!! 상품 코드 엑셀이 없습니다. - {0}", pChannelGoodInfo.Goods_Code_));
                    return false;
                }

                string filepath = GoodsDownInfo_[pChannelGoodInfo.Goods_Code_];
                Microsoft.Office.Interop.Excel.Application ap = null;
                Workbook wb = null;
                Worksheet ws = null;
                HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                string tempString = "";
                string comparesitename = "";

                Int32 nCurrentRow = LQCrawlerInfo_.ExData_Start_;
                Int32 ExData_Option = LQCrawlerInfo_.ExData_Option_;
                Int32 ExData_Coupncode = LQCrawlerInfo_.ExData_Coupncode_;
                Int32 ExData_Buyer = LQCrawlerInfo_.ExData_Buyer_;
                Int32 ExData_Cancel = LQCrawlerInfo_.ExData_Cancel_;
                Int32 ExData_Use = LQCrawlerInfo_.ExData_Use_;
                Int32 ExData_Buyphone = LQCrawlerInfo_.ExData_Buyphone_;
                Int32 ExData_Price = LQCrawlerInfo_.ExData_Price_;
                Int32 ExData_BuyDate = LQCrawlerInfo_.ExData_Buydate_;
                Int32 ExData_BuyCount = LQCrawlerInfo_.ExData_Count_;
                Int32 ExData_GoodsName = LQCrawlerInfo_.ExData_GoodName_;
                Int32 ExData_Coupncode2 = 4;    // 11번가만 가지고 있는 주문 순번 쿠폰 코드가 고유하지 않아서 이것과 합쳐야만 고유해진다.

                ProcessStateManager.Instance.NeedParsingCount_ += ws.UsedRange.Rows.Count;

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

                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;
                        pExcelData.goodsSeq_ = pChannelGoodInfo.Idx_;
                        pExcelData.ExData_Option_ = tempString;
                        pExcelData.ExData_OptionOriginal_ = tempString;
                        tRange = ws.Cells[nCurrentRow, ExData_GoodsName];
                        pExcelData.ExData_GoodsName_ = tRange.Value2;
                        pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", "");
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
                        if (LQCrawlerInfo_.Channel_Idx_ == 9 || LQCrawlerInfo_.Channel_Idx_ == 14 || LQCrawlerInfo_.Channel_Idx_ == 15 || LQCrawlerInfo_.Channel_Idx_ == 18)
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
                        NewLogManager2.Instance.Log(string.Format("엑셀 파싱 에러 : {0}", ex.Message));
                        break;
                        //nCurrentRow++;
                        //continue;
                    }

                    ProcessStateManager.Instance.CurParsingCount_++;
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
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool Internal_Excel_Parsing - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool ExcelParsing_Cancel()
        {
            Internal_ExcelCancel_Parsing(CancelDownInfo_["C"]);
            Internal_ExcelCancel_Parsing(CancelDownInfo_["R"]);
            return true;
        }

        protected override bool Internal_ExcelCancel_Parsing(string filepath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application ap = null;
                Workbook wb = null;
                Worksheet ws = null;
                HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                Int32 nCurrentRow = 7;
                Int32 StateColumn = 2;
                Int32 CouponColumn = 3;
                Int32 CouponColumn2 = 4;
                Int32 CancelCountColumn = 9;
                

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;

                        CCancelData pCCancelData = new CCancelData();
                        pCCancelData.channelOrderCode_ = Convert.ToString(tRange.Value2);

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

                        tRange = ws.Cells[nCurrentRow, StateColumn];
                        if (tRange == null)
                            break;
                        pCCancelData.State_ = Convert.ToString(tRange.Value2);


                        tRange = ws.Cells[nCurrentRow, CouponColumn2];
                        if (tRange == null)
                            break;

                        pCCancelData.channelOrderCode_ = string.Format("{0}_{1}", pCCancelData.channelOrderCode_, Convert.ToString(tRange.Value2));

                        tRange = ws.Cells[nCurrentRow, CancelCountColumn];
                        pCCancelData.CancelCount_ = Convert.ToInt32(tRange.Value2);


                        for (int i = 1; i <= pCCancelData.CancelCount_; i++)
                        {
                            CCancelData tempExcelData = new CCancelData();
                            tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pCCancelData.channelOrderCode_, i);
                            tempExcelData.CancelCount_ = 1;
                            tempExcelData.State_ = pCCancelData.State_;

                            Excel_Cancel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
                        }

                        
                    }
                    catch (System.Exception ex)
                    {
                        NewLogManager2.Instance.Log(string.Format("Internal_ExcelCancel_Parsing 엑셀 파싱 에러 : {0}/{1}", filepath, ex.Message));
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
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error protected override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }

            return true;
        }
        
        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            string regstring = @"(?<OptionName>\S+)-\S+개";            
            optionstring = optionstring.Replace(" ", "");
            pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", "");            
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
                    COrderData tempExcelData = new COrderData();
                    tempExcelData.CopyFrom(pExcelData);
                    tempExcelData.ExData_Option_ = optionname;
                    tempExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsName_;
                    tempExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsNick_;
                    tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);

                    if (Excel_List_.ContainsKey(tempExcelData.channelOrderCode_) == false)
                    {
                        Excel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
                    }
                }
            }

            return nTotalcount;
        }
        // 웹에서 사용처리
        public override bool Web_Use()
        {
            try
            {
                ProcessStateManager.Instance.NeedWebProcessCount_ = WebProcess_List_.Count;
                foreach (var pData in WebProcess_List_)
                {

                    if (DBSelected_List_.ContainsKey(pData.Key))
                    {
                        if (Use_Deal(pData.Value) == true)
                        {
                            CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                            // 2014.11.23 사용처리 변경
                            // DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                            ProcessStateManager.Instance.CurWebProcessCount_++;
                        }
                        else
                        {
                            ProcessStateManager.Instance.FailedWebProcessCount_++;
                        }
                    }
                 
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_Use() - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        // 오픈 마켓들은 바로 사용처리를 해줬기 때문에 DB 에 AR, UR 이 있으면 그냥 A, U 로 변경한다.
        public override bool OpenMarketChangeState()
        {
            try
            {
                foreach (var pData in DBSelected_List_)
                {
                    if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                    {
                        pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                        if (DBProccess_List_.ContainsKey(pData.Value.channelOrderCode_) == false)
                            DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                    }
                    else if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                    {
                        pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        if (DBProccess_List_.ContainsKey(pData.Value.channelOrderCode_) == false)
                            DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool OpenMarketChangeState - {0}", ex.Message));
                return false;
            }

            return true;
        }

        bool Use_Deal(COrderData pCOrderData)
        {   
            try
            {
                string couponcode = pCOrderData.channelOrderCode_;
                Int32 LIndex = couponcode.LastIndexOf('_');
                couponcode = couponcode.Substring(0, LIndex);
                string delivery_no = "";
                string ticket_code = "";

                DateTime eDateTime = Convert.ToDateTime(pCOrderData.BuyDate_);
                DateTime sDateTime = eDateTime.AddDays(-7);

                string eDate = string.Format("{0:D4}{1:D2}{2:D2}", eDateTime.Year, eDateTime.Month, eDateTime.Day);
                string sDate = string.Format("{0:D4}{1:D2}{2:D2}", sDateTime.Year, sDateTime.Month, sDateTime.Day);

                // 웹 호출을 통해서 사용처리한다.
                string useurl = LQCrawlerInfo_.UseUserUrl_;
                string useparam = LQCrawlerInfo_.UseUserParam_;
                string[] cpcodeArray = couponcode.Split('_');
                couponcode = cpcodeArray[0];
                GetUseTicketInfo(couponcode, sDate, eDate, ref ticket_code, ref delivery_no);
                
                useparam = useparam.Replace("{Delivery_no}", delivery_no);
                useparam = useparam.Replace("{GoodsCode}", couponcode);
                useparam = useparam.Replace("{TicketCode}", ticket_code);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.UseUserCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Use_Deal(COrderData pCOrderData) - {0}", ex.Message));
                return false;
            }

            return true;
        }

        bool GetUseTicketInfo(string couponcode, string sDate, string geDate, ref string ticketcode, ref string delivery_no)
        {
            try
            {
                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = LQCrawlerInfo_.UseGoodsParam_;
                strparam = strparam.Replace("{CouponCode}", couponcode);
                strparam = strparam.Replace("{sDate}", sDate);
                strparam = strparam.Replace("{eDate}", geDate);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.UseGoodsCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;
                }

                htmlBuffer = htmlBuffer.Replace(" ", "&nbsp");
                htmlBuffer = htmlBuffer.Replace("　", "&nbsp");

                Regex re = new Regex(LQCrawlerInfo_.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);

                ticketcode = oe[0].Groups["TicketCode"].ToString();
                delivery_no = oe[0].Groups["Delivery_no"].ToString();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool GetUseTicketInfo - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 웹에서 사용처리 해야 할게 있는지 체크
        public override bool CheckNeedUseWeb()
        {
            try
            {
                foreach (var pData in Excel_List_)
                {
                    ChannelGoodInfo pInfo = null;

                    if (LQCrawlerInfo_.ExData_GoodName_ == 0)
                        pInfo = GetGoodInfoByGoodCodeAndOptionName(pData.Value.goodsCode_, pData.Value.ExData_Option_);
                    else
                        pInfo = GetGoodInfoByGoodOptionName(pData.Value.ExData_GoodsNick_, pData.Value.ExData_Option_);

                    if (pInfo == null)
                        continue;

                    pData.Value.ExData_GoodsName_ = pInfo.GoodsName_;
                    pData.Value.goodsSeq_ = pInfo.Idx_;
                    pData.Value.goodsCode_ = pInfo.Goods_Code_;

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];
                        if (pData.Value.State_ == pDBData.State_)
                            continue;

                        /**
                         * 2014.12.16 사용처리 변경
                         * 레저큐에 데이터가 있으면 무조건 사용처리
                        */
                        pDBData.BuyDate_ = pData.Value.BuyDate_;
                        WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                       
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckNeedUseWeb() - {0}", ex.Message));
                return false;
            }
            
            return true;
        }
        // 취소된것이 있는치 체크
        public override bool CheckIsCancel()
        {
            try
            {
                foreach (var pData in Excel_Cancel_List_)
                {
                    if (pData.Value.State_ == "반품완료" || pData.Value.State_ == "취소완료"){}
                    else { continue; }

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        if (DBProccess_List_.ContainsKey(pData.Key) == true)
                        {
                            NewLogManager2.Instance.Log(string.Format("CheckIsCancel DB 처리에 두가지가 다 들어가 있다.{0}", pData.Key));
                            continue;
                        }

                        COrderData pCOrderData = DBSelected_List_[pData.Key];

                        if (pCOrderData.State_ != DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL])
                        {
                            pCOrderData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL];
                            DBCancel_List_.Add(pCOrderData.channelOrderCode_, pCOrderData);
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool CheckIsCancel( - {0}", ex.Message));
                return false;
            }

            

            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                DateTime beforeData = dtNow.AddDays(-7);
                string eDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                string sDate = string.Format("{0:D4}{1:D2}{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);

                {
                    string url = "";
                    if (LQCrawlerInfo_.AuthoritySeq_ == 17)
                    {
                        url = @"https://soffice.11st.co.kr/escrow/AuthSellerClaimManager.tmall?method=getClaimExcelList";
                    }
                    else
                    {
                        url = @"https://partner.11st.co.kr/escrow/AuthSellerClaimManager.tmall?method=getClaimExcelList";
                    }
                    string method = "POST";
                    string param = @"clmOccrTyp=01&currentPageNo=1&lastSearchKind=02&smartSearchClmStat=&searchVer=02&townSellerYn=&key=searchALL&keyValue=&clmStat=106&shDateType=reqDt&shDateFrom={sDate}&shDateTo={eDate}&searchDt=8";



                    string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                    makefolder += "\\";
                    makefolder += CINIManager.Instance.channelseq_;
                    makefolder += "\\";
                    makefolder += dtNow.ToShortDateString();
                    HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);
                    string sendparam = param.Replace("{sDate}", sDate);
                    sendparam = sendparam.Replace("{eDate}", eDate);
                    string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                        , makefolder, "C", Convert.ToString(dtNow.Ticks));

                    HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparam, Cookie_, null, null, 180000);

                    if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr" || pResponse.CharacterSet == "EUC-KR")
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

                    CancelDownInfo_.Add("C", downString);
                }

                {
                    string url = "";
                    if (LQCrawlerInfo_.AuthoritySeq_ == 17)
                    {
                        url = @"https://soffice.11st.co.kr/escrow/OrderCancelManageList.tmall?method=getSellListToExcel&ver=2nd";
                    }
                    else
                    {
                        url = @"https://partner.11st.co.kr/escrow/OrderCancelManageList.tmall?method=getSellListToExcel&ver=2nd";
                    }
                    string method = "POST";
                    string param = @"listType=cancel&pagePerSize=500&currentPageNo=1&ordPrdCnSeq=&ordNoList=&ordPrdSeqList=&ordPrdCnSeqList=&searchFlag=&shBuyerType=&shBuyerText=&key=02&shDateType=07&shDateFrom={sDate}&shDateTo={eDate}&searchDt=8&dataGrid=0";

                    string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                    makefolder += "\\";
                    makefolder += CINIManager.Instance.channelseq_;
                    makefolder += "\\";
                    makefolder += dtNow.ToShortDateString();
                    HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);
                    string sendparam = param.Replace("{sDate}", sDate);
                    sendparam = sendparam.Replace("{eDate}", eDate);
                    string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                        , makefolder, "R", Convert.ToString(dtNow.Ticks));

                    HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparam, Cookie_, null, null, 180000);

                    if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr" || pResponse.CharacterSet == "EUC-KR")
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

                    CancelDownInfo_.Add("R", downString);
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_DownLoad_CancelList() - {0}", ex.Message));
                return false;
            }

            return true;
        }
    }
}

