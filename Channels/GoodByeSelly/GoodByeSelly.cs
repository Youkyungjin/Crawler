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


namespace Channels
{
    public class GoodByeSelly : BaseChannel
    {
        //string str_sitecompare_ = "굿바이셀리";
        string str_use_url_1_ = @"http://www.goodbuyselly.com/shop/set_trans_ready_proc";
        string str_use_param_1_ = @"order_srl={CouponCode}";
        string str_use_check_1_ = @"success";

        string str_use_url_2_ = @"http://www.goodbuyselly.com/shop/set_trans_proc";
        string str_use_param_2_ = @"order_srl={CouponCode}&pay_srl={TicketCode}&invoice_no=0000&total_trans=N&trans_method=E&trans_comp=&trans_method_etc=직접 전달";
        string str_use_check_2_ = @"0#@#";

        string str_use_url_3_ = @"http://www.goodbuyselly.com/shop/set_trans_complete_proc";
        string str_use_param_3_ = @"order_srl={CouponCode}";
        //string str_use_check_3_ = @"success";

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

        public override bool Web_DownLoadExcel()
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

                string downString = makefolder;
                downString += "\\";
                downString += pGoodInfo.Goods_Code_;
                downString += "_";
                downString += Convert.ToString(dtNow.Ticks);
                downString += ".xls";

                // 이미 다운로드가 끝난 파일이라면 다시 다운로드 하지 않는다.
                if (GoodsDownInfo_.ContainsKey("0000") == false)
                {
                    try
                    {
                        string method = LQCrawlerInfo_.ExcelDownMethod_;
                        string url = LQCrawlerInfo_.ExcelDownUrl_;
                        string sendparameter = LQCrawlerInfo_.ExcelDownParameter_;

                        string eDate = "";
                        string sData = "";
                        if (pGoodInfo.eDateFormat_ != null)
                        {
                            DateTime beforeData = dtNow.AddDays(-31);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                            eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                            sData = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                        }

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
                    break;
                }
                else
                {
                    ProcessStateManager.Instance.PassDownLoadCount_++;
                }
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

                if (nCurrentRow > 0)
                    ProcessStateManager.Instance.NeedParsingCount_ += (ws.UsedRange.Rows.Count - (nCurrentRow - 1));

                string beforeOrderName = "";    // 이전 주문자명
                string beforeOrderPhone = "";   // 이전 주문 전화번호
                string beforeOrderDate = "";    // 이전 주문 날짜
                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, ExData_Option];
                        if (tRange == null)
                            break;

                        tempString = tRange.Value2;
                        if (tempString == null)
                        {
                            break;
                        }

                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;       // 채널 시퀀스
                        pExcelData.goodsSeq_ = -1;                                  // 상품 시퀀스
                        pExcelData.ExData_Option_ = tempString;                     // 옵션명
                        pExcelData.ExData_OptionOriginal_ = tempString;             // 원래 옵션명

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
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Trim();

                        tRange = ws.Cells[nCurrentRow, ExData_Buyer];
                        pExcelData.orderName_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.orderName_ == null)
                        {
                            pExcelData.orderName_ = beforeOrderName;
                        }
                        else
                        {
                            beforeOrderName = pExcelData.orderName_;
                        }

                        tRange = ws.Cells[nCurrentRow, ExData_Cancel];
                        pExcelData.ExData_Cancel_ = tRange.Value2;
                        if (pExcelData.ExData_Cancel_ == null) pExcelData.ExData_Cancel_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Use];
                        pExcelData.ExData_Use_ = tRange.Value2;
                        if (pExcelData.ExData_Use_ == null) pExcelData.ExData_Use_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Buyphone];
                        pExcelData.orderPhone_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.orderPhone_ == null)
                        {
                            pExcelData.orderPhone_ = beforeOrderPhone;
                        }
                        else
                        {
                            beforeOrderPhone = pExcelData.orderPhone_;
                        }

                        pExcelData.orderPhone_ = pExcelData.orderPhone_.Replace("'", "");

                        if (ExData_Price != 0)
                        {
                            tRange = ws.Cells[nCurrentRow, ExData_Price];

                            if (tRange.Value2 != null)
                            {
                                tempString = Convert.ToString(tRange.Value2);
                                tempString = tempString.Replace(",", "");
                                pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                            }
                        }

                        tRange = ws.Cells[nCurrentRow, ExData_BuyDate];

                        if (tRange.Value2 == null)
                        {
                            pExcelData.BuyDate_ = beforeOrderDate;
                        }
                        else
                        {
                            double temp = Convert.ToDouble(tRange.Value2);
                            DateTime dta = DateTime.FromOADate(temp);
                            pExcelData.BuyDate_ = dta.ToString("u");
                            pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");
                            beforeOrderDate = pExcelData.BuyDate_;
                        }

                        if (ExData_BuyCount != 0)
                        {
                            tRange = ws.Cells[nCurrentRow, ExData_BuyCount];
                            pExcelData.BuyCount_ = Convert.ToInt32(tRange.Value2);
                        }

                        SplitDealAndInsertExcelData(pExcelData);
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
                NewLogManager2.Instance.Log(string.Format("Error public override bool Internal_Excel_Parsing - {0}", ex.Message));
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
            Microsoft.Office.Interop.Excel.Application ap = null;
            Workbook wb = null;
            Worksheet ws = null;
            HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

            Range tRange = null;
            Int32 nCurrentRow = 3;
            Int32 CouponColumn = 4;
            Int32 CancelCountColumn = 10;
            Int32 StateColumn = 14;

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

                    // 혹시 중복해서 엑셀파일을 다운로드 받았다면 중복이 있을수 있다. 이건 그냥 지나쳐야 한다.
                    if (Excel_Cancel_List_.ContainsKey(pCCancelData.channelOrderCode_) == true)
                        break;

                    tRange = ws.Cells[nCurrentRow, CancelCountColumn];
                    pCCancelData.CancelCount_ = Convert.ToInt32(tRange.Value2);

                    tRange = ws.Cells[nCurrentRow, StateColumn];
                    pCCancelData.State_ = Convert.ToString(tRange.Value2);

                    for (int i = 1; i <= pCCancelData.CancelCount_; i++)
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

            return true;
        }
        // 웹에서 사용처리
        public override bool Web_Use()
        {
            try
            {
                ProcessStateManager.Instance.NeedWebProcessCount_ = WebProcess_List_.Count;
                foreach (var pData in WebProcess_List_)
                {
                  if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
                  {
                     CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                     //pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                     // 2014.11.23 사용처리 변경
                     //DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);                        
                     ProcessStateManager.Instance.CurWebProcessCount_++;
                  }
                  else
                  {
                     ProcessStateManager.Instance.FailedWebProcessCount_++;
                  }
                  
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Web_Use - {0}", ex.Message));
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

        public bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
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

        bool GetUseTicketInfo(Int32 goodsSeq, string cpcode, ref string ticketcode)
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
            if (pGoodInfo.eDateFormat_ != null)
            {
                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
            }

            string strurl = LQCrawlerInfo_.UseGoodsUrl_;
            string strparam = LQCrawlerInfo_.UseGoodsParam_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];
            strparam = strparam.Replace("{CouponCode}", cpcode);
            strparam = strparam.Replace("{sDate}", pGoodInfo.sDate_);
            strparam = strparam.Replace("{eDate}", eDate);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("GET", strurl, strparam, Cookie_);

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
            return true;
        }

        bool use_step_1(string ticketcode, string cpcode)
        {
            string useurl = str_use_url_1_;
            string useparam = str_use_param_1_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];

            useparam = useparam.Replace("{CouponCode}", cpcode);

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

            return true;
        }

        bool use_step_2(string ticketcode, string cpcode)
        {
            string useurl = str_use_url_2_;
            string useparam = str_use_param_2_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];

            useparam = useparam.Replace("{TicketCode}", ticketcode);
            useparam = useparam.Replace("{CouponCode}", cpcode);

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

            return true;
        }

        bool use_step_3(string ticketcode, string cpcode)
        {
            string useurl = str_use_url_3_;
            string useparam = str_use_param_3_;
            string[] cpcodeArray = cpcode.Split('_');
            cpcode = cpcodeArray[0];

            useparam = useparam.Replace("{CouponCode}", cpcode);

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

            return true;
        }

        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            pExcelData.ExData_GoodsNick_ = Convert.ToString(Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", ""));
            string regstring = @"(?<OptionName>\S+)\(\S+\)(?<Count>\d+)개|(?<OptionName>\S+)(?<Count>\d+)개";
      
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
                        COrderData tempExcelData = new COrderData();
                        tempExcelData.CopyFrom(pExcelData);
                        tempExcelData.ExData_Option_ = optionname;
                        tempExcelData.ExData_Use_ = pExcelData.ExData_Use_;
                        tempExcelData.ExData_Cancel_ = pExcelData.ExData_Cancel_;
                        tempExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsName_;
                        tempExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsNick_;
                        tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);

                        if (Excel_List_.ContainsKey(tempExcelData.channelOrderCode_) == false)
                        {
                            Excel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
                        }
                    }
                }
            }

            return nTotalcount;
        }

        // 웹에서 사용처리 해야 할게 있는지 체크
        public override bool CheckNeedUseWeb()
        {
            foreach (var pData in Excel_List_)
            {
                if (DBSelected_List_.ContainsKey(pData.Key) == true)
                {
                    COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];
                    if (pData.Value.State_ == pDBData.State_)
                        continue;

                    pData.Value.goodsPassType = pDBData.goodsPassType;
                    pData.Value.ExData_GoodsName_ = pDBData.ExData_GoodsName_;
                    pData.Value.goodsSeq_ = pDBData.goodsSeq_;
                    pData.Value.goodsCode_ = pDBData.goodsCode_;

                    if (pData.Value.ExData_Use_ == "결제완료") { 
                        pDBData.BuyDate_ = pData.Value.BuyDate_;
                        WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                    }
                }
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
                NewLogManager2.Instance.Log(string.Format("Error override bool CheckIsCancel( - {0}", ex.Message));
                return false;
            }
            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                string method = "GET";
                string url1 = @"http://www.goodbuyselly.com/shop/order_list_excel?state=C&type=&start_date={sDate}&end_date={eDate}";
                string param = @"";

                DateTime dtNow = DateTime.Now;

                string eDate = "";
                string sDate = "";

                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                DateTime beforeData = dtNow.AddDays(-7); 
                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);

                string tempurl = url1.Replace("{sDate}", sDate);
                tempurl = tempurl.Replace("{eDate}", eDate);
                string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                    , makefolder, "C", Convert.ToString(dtNow.Ticks));

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, tempurl, param, Cookie_, null, null, 180000);

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
                string method = "GET";
                string url1 = @"http://www.goodbuyselly.com/shop/order_list_excel?state=I&type=&start_date={sDate}&end_date={eDate}";
                string param = @"";

                DateTime dtNow = DateTime.Now;

                string eDate = "";
                string sDate = "";

                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                DateTime beforeData = dtNow.AddDays(-7);
                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);

                string tempurl = url1.Replace("{sDate}", sDate);
                tempurl = tempurl.Replace("{eDate}", eDate);
                string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                    , makefolder, "R", Convert.ToString(dtNow.Ticks));

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, tempurl, param, Cookie_, null, null, 180000);

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
