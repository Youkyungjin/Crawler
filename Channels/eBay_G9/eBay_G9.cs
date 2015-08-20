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
using System.Threading;

namespace Channels
{
    public class eBay_G9 : BaseChannel
    {
        string str_sitecompare_ = "지마켓";
        string str_use_url_1_ = @"https://www.esmplus.com/Escrow/Order/OrderCheck";
        string str_use_param_1_ = @"mID=140935&orderInfo={TicketCode},1,leisureq";
        string str_use_check_1_ = @":true,";

        string str_use_url_2_ = @"https://www.esmplus.com/Escrow/Delivery/SetDoShippingGeneral";
        string str_use_param_2_ = @"mID=140935&deliveryInfo={TicketCode},10032,자체배송,";
        string str_use_check_2_ = @":true,";

        // 로그인
        public override bool Web_Login()
        {
            Cookie_ = new CookieContainer();

            try
            {
                string loginurl = LQCrawlerInfo_.LoginUrl_;
                string loginstring = LQCrawlerInfo_.LoginParam_.Replace("{LoginID}", LQCrawlerInfo_.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", LQCrawlerInfo_.LoginPW_);
                byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(LQCrawlerInfo_.LoginMethod_, loginurl, loginstring, Cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.LoginCheck_) < 0)
                    return false;

            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            return true;
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
                            string url = LQCrawlerInfo_.ExcelDownUrl_;
                            url = url.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);

                            string sendparameter = LQCrawlerInfo_.ExcelDownParameter_;

                            string eDate = "";
                            string sDate = "";
                            if (pGoodInfo.eDateFormat_ != null)
                            {
                                DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                                sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                            }

                            sendparameter = sendparameter.Replace("{sDate}", sDate);
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
                NewLogManager2.Instance.Log(string.Format("Error override bool Web_DownLoadExcel - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 엑셀 파싱
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

                if(HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws) == false)
                    return false;

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
                Int32 ExData_GoodsName = 0;

                nCurrentRow = LQCrawlerInfo_.ExData_Start_;
                ExData_Option = LQCrawlerInfo_.ExData_Option_;
                ExData_Coupncode = LQCrawlerInfo_.ExData_Coupncode_;
                ExData_Buyer = LQCrawlerInfo_.ExData_Buyer_;
                ExData_Cancel = LQCrawlerInfo_.ExData_Cancel_;
                ExData_Use = LQCrawlerInfo_.ExData_Use_;
                ExData_Buyphone = LQCrawlerInfo_.ExData_Buyphone_;
                ExData_Price = LQCrawlerInfo_.ExData_Price_;
                ExData_BuyDate = LQCrawlerInfo_.ExData_Buydate_;
                ExData_BuyCount = LQCrawlerInfo_.ExData_Count_;
                ExData_GoodsName = LQCrawlerInfo_.ExData_GoodName_;

                if (nCurrentRow > 0)
                    ProcessStateManager.Instance.NeedParsingCount_ += (ws.UsedRange.Rows.Count - (nCurrentRow - 1));

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
                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;
                        pExcelData.goodsSeq_ = tempgoodSeq;
                        pExcelData.ExData_Option_ = tempString;
                        pExcelData.ExData_OptionOriginal_ = tempString;

                        tRange = ws.Cells[nCurrentRow, ExData_GoodsName];
                        pExcelData.ExData_GoodsName_ = tRange.Value2;
                        pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", "");

                        tRange = ws.Cells[nCurrentRow, ExData_Coupncode];
                        if (tRange == null)
                            break;

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

                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
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
                Int32 SiteColumn = 1;
                Int32 nCurrentRow = 2;
                Int32 CouponColumn = 2;
                Int32 CancelCountColumn = 16;


                if(filepath.IndexOf("_C_") > 0)
                { 
                    SiteColumn = 1;
                    nCurrentRow = 2;
                    CouponColumn = 2;
                    CancelCountColumn = 16;
                }else{
                    SiteColumn = 1;
                    nCurrentRow = 2;
                    CouponColumn = 3;
                    CancelCountColumn = 13;
                }

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, SiteColumn];
                        if (tRange == null)
                            break;
                        string StringSite = tRange.Value2;
                        if (string.IsNullOrEmpty(StringSite) == true)
                            break;

                        if (StringSite.IndexOf("G") < 0)
                        {
                            nCurrentRow++;
                            continue;   // 옥션것만 넘기기
                        }

                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;

                        CCancelData pCCancelData = new CCancelData();
                        pCCancelData.channelOrderCode_ = Convert.ToString(tRange.Value2);

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

                        tRange = ws.Cells[nCurrentRow, CancelCountColumn];
                        pCCancelData.CancelCount_ = Convert.ToInt32(tRange.Value2);

                        for(int i = 1; i <= pCCancelData.CancelCount_; i++ )
                        {
                            CCancelData tempExcelData = new CCancelData();
                            tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pCCancelData.channelOrderCode_, i);
                            tempExcelData.CancelCount_ = 1;
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
                NewLogManager2.Instance.Log(string.Format("Error override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }
            
            return true;
        }
        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            if (string.IsNullOrEmpty(comparesitename) == true)
                return 0;

            if (comparesitename.IndexOf(str_sitecompare_) < 0)
                return 0;
            
            string optionstring = Regex.Replace(pExcelData.ExData_Option_, @" ", "");
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionnickname = "";
            pExcelData.ExData_GoodsNick_ = Convert.ToString(Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", ""));
            string regstring = @"(?<OptionName>\S+)/\S+/\d+개|(?<OptionName>\S+)/\d+개";

            // 옵션명 개수 빼기
            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(optionstring);

            optionnickname = Convert.ToString(Regex.Replace(oe[0].Groups["OptionName"].Value, @"[^a-zA-Z0-9가-힣]", ""));
            nBuycount = pExcelData.BuyCount_;
            for (Int32 i = 0; i < nBuycount; i++)
            {
                nTotalcount++;
                COrderData tempExcelData = new COrderData();
                tempExcelData.CopyFrom(pExcelData);
                tempExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsName_;
                tempExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsNick_;
                tempExcelData.ExData_Option_ = optionnickname;
                tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);
                if (Excel_List_.ContainsKey(tempExcelData.channelOrderCode_) == false)
                {
                    Excel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
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
                        if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
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
                NewLogManager2.Instance.Log(string.Format("Error override bool Web_Use() - {0}", ex.Message));
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

        bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {   
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
             try
            {
                ChannelGoodInfo pGoodInfo = GoodsInfoList_[goodsSeq];

                if (pGoodInfo == null)
                {
                    string Message = string.Format("GetUseTicketInfo 매칭되는 코드가 없다.{0}/{1}{2}", goodsSeq, cpcode, ticketcode);
                    NewLogManager2.Instance.Log(Message);
                    return false;
                }

                DateTime dtNow = DateTime.Now;
                string eDate = "";
              
                eDate = string.Format(@"{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
               

                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = LQCrawlerInfo_.UseGoodsParam_;
                string[] cpcodeArray = cpcode.Split('_');
                cpcode = cpcodeArray[0];
                strparam = strparam.Replace("{CouponCode}", cpcode);
                strparam = strparam.Replace("{sDate}", pGoodInfo.sDate_);
                strparam = strparam.Replace("{eDate}", eDate);

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

                Regex re = new Regex(LQCrawlerInfo_.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);
                if (oe.Count != 0)
                {
                    ticketcode = oe[0].Groups["TicketCode"].ToString();
                }
                else
                {
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool GetUseTicketInfo() - {0}", ex.Message));
                return false;
            }
            
            return true;
        }
        // 사용 처리 1단계
        bool use_step_1(string ticketcode)
        {
            try
            {
                string useurl = str_use_url_1_;
                string useparam = str_use_param_1_;

                useparam = useparam.Replace("{TicketCode}", ticketcode);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(str_use_check_1_) < 0)
                {
                    NewLogManager2.Instance.Log("public override bool use_step_1(string cpcode) " + htmlBuffer);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool use_step_1 - {0}", ex.Message));
                return false;
            }
            

            return true;
        }
        // 사용 처리 2단계
        bool use_step_2(string ticketcode)
        {
            try
            {
                string useurl = str_use_url_2_;
                string useparam = str_use_param_2_;

                useparam = useparam.Replace("{TicketCode}", ticketcode);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(str_use_check_2_) < 0)
                {
                    NewLogManager2.Instance.Log("public override bool use_step_2(string cpcode) " + htmlBuffer);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool use_step_2 - {0}", ex.Message));
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

                        // 2014.11.23 사용처리 변경
                        // 레저큐에서 예약을 완료한 상태 웹에 사용 처리를 해야한다.
                       
                        pDBData.BuyDate_ = pData.Value.BuyDate_;
                        WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                       
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool CheckNeedUseWeb - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool CheckIsCancel()
        {
            try
            {
                foreach (var pData in Excel_Cancel_List_)
                {
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
                NewLogManager2.Instance.Log(string.Format("Error override bool CheckIsCancel - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                DateTime dtNow = DateTime.Now;

                string eDate = "";
                string sDate = "";

                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();

                DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);

                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                string method = "GET";
                string url = @"https://www.esmplus.com/Escrow/Claim/ExcelDownload";
                string reffer = @"https://www.esmplus.com/Escrow/Claim/ReturnRequestManagement?menuCode=TDM118";
                string param = @"from=ReturnRequest&gridID=GEC012&type=A&searchAccount=TA^140935&searchDateType=&searchSDT={sDate}&searchEDT={eDate}&searchType=RF&searchKey=ON&searchKeyword=&searchStatus=RR&searchAllYn=Y";
                string useragent = @"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";

             
                        string sendparam = param.Replace("{sDate}", sDate);
                        sendparam = sendparam.Replace("{eDate}", eDate);
                        string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                     , makefolder, "C", Convert.ToString(dtNow.Ticks));

                        HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparam, Cookie_, reffer, useragent, 180000);

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
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            try
            {
                DateTime dtNow = DateTime.Now;

                string eDate = "";
                string sDate = "";

                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();

                DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);

                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                string method = "GET";
                string url = @"https://www.esmplus.com/Escrow/Claim/ExcelDownload";
                string reffer = @"https://www.esmplus.com/Escrow/Claim/ReturnRequestManagement?menuCode=TDM118";
                string param = @"from=CancelRequest&gridID=GEC011&type=A&searchAccount=TA&searchDateType=PAD&searchSDT={sDate}&searchEDT={eDate}&searchType=CC&searchKey=ON&searchKeyword=&searchStatus=CC&searchAllYn=N&tabGbn=1";
                string useragent = @"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";

                        string sendparam = param.Replace("{sDate}", sDate);
                        sendparam = sendparam.Replace("{eDate}", eDate);
                        string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                    , makefolder, "R", Convert.ToString(dtNow.Ticks));

                        HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparam, Cookie_, reffer, useragent, 180000);

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
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            return true;
        }
    }
}
