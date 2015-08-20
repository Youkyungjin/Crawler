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
    public class WeekOn : BaseChannel
    {
        // 로그인 Web
        public override bool Web_Login()
        {
            Cookie_ = new CookieContainer();
            try
            {

                //첫번째 메인페이지에서 암호화 키를 받아온다
                string loginurl = LQCrawlerInfo_.LoginUrl_;
                string loginstring = LQCrawlerInfo_.LoginParam_.Replace("{LoginID}", LQCrawlerInfo_.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", LQCrawlerInfo_.LoginPW_);
                byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(LQCrawlerInfo_.LoginMethod_, loginurl, loginstring, Cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf("로그인 성공") < 0)
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
                            string sData = "";
                            if (pGoodInfo.eDateFormat_ != null)
                            {
                                DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                                eDate = string.Format(pGoodInfo.eDateFormat_, dtNow.Year, dtNow.Month, dtNow.Day);
                                sData = string.Format(pGoodInfo.eDateFormat_, beforeData.Year, beforeData.Month, beforeData.Day);
                            }

                            sendparameter = sendparameter.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);
                            sendparameter = sendparameter.Replace("{sDate}", sData);
                            sendparameter = sendparameter.Replace("{eDate}", eDate);

                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, Cookie_);

                            
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

                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;
                        pExcelData.goodsSeq_ = pChannelGoodInfo.Idx_;
                        pExcelData.ExData_Option_ = tempString;
                        pExcelData.ExData_OptionOriginal_ = tempString;
                        pExcelData.goodsCode_ = pChannelGoodInfo.Goods_Code_;

                        tRange = ws.Cells[nCurrentRow, ExData_Coupncode];
                        if (tRange == null)
                            break;

                        pExcelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.channelOrderCode_ == null)
                            break;
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Replace("'", "");
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Trim();   // 공백 제거

                        tRange = ws.Cells[nCurrentRow, ExData_GoodsName];
                        pExcelData.ExData_GoodsName_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.ExData_GoodsName_ == null) break;

                        tRange = ws.Cells[nCurrentRow, ExData_Buyer];
                        pExcelData.orderName_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.orderName_ == null) pExcelData.orderName_ = "";
                        
                        tRange = ws.Cells[nCurrentRow, ExData_Cancel];
                        pExcelData.ExData_Cancel_ = Convert.ToString(tRange.Value);
                        if (pExcelData.ExData_Cancel_ == null) pExcelData.ExData_Cancel_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Use];
                        pExcelData.ExData_Use_ = Convert.ToString(tRange.Value);
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
                                tempString = tempString.Replace("원", "");
                                pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                            }
                        }

                        tRange = ws.Cells[nCurrentRow, ExData_BuyDate];

                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.BuyDate_ == null)
                            break;
                        Double temp = Convert.ToDouble(tRange.Value2);
                        DateTime dta = DateTime.FromOADate(temp);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");

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
                NewLogManager2.Instance.Log(string.Format("Error protected override bool Internal_Excel_Parsing - {0}", ex.Message));
                return false;
            }

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
                Int32 nCurrentRow = 2;
                Int32 CouponColumn = 14;
                Int32 OrderCnt = 10;
                Int32 StateColumn = 15;

                while (true)
                {
                    try
                    {
                     
                        CCancelData pCCancelData = new CCancelData();

                        tRange = ws.Cells[nCurrentRow, StateColumn];
                        if (tRange == null)
                            break;
                        pCCancelData.State_ = Convert.ToString(tRange.Value2);
                        if (tRange.Value2 == null)
                            break;
                        if (pCCancelData.State_ != "결제취소")
                        {
                            nCurrentRow++;
                            continue;   // 옥션것만 하자.
                        }

                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;
                        pCCancelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

                        
                        tRange = ws.Cells[nCurrentRow, OrderCnt];
                        pCCancelData.CancelCount_ = Convert.ToInt32(tRange.Value2);
                        if (tRange == null)
                            break;

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
        // 하나의 딜을 여러개로 나눌 필요가 있는가? 있다면 나눠서 넣고 없다면 그냥 넣는다.        
        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", "");            
            nBuycount = pExcelData.BuyCount_;

            for (Int32 i = 0; i < nBuycount; i++)
            {
                nTotalcount++;
                COrderData tempExcelData = new COrderData();
                tempExcelData.CopyFrom(pExcelData);
                tempExcelData.ExData_Option_ = pExcelData.ExData_Option_;
                tempExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsName_;
                tempExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsNick_;
                tempExcelData.ExData_Use_ = pExcelData.ExData_Use_;
                tempExcelData.ExData_Cancel_ = pExcelData.ExData_Cancel_;
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
                    //if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY])
                    //{
                    if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
                    {
                        CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                        
                        // 2014.11.23 사용처리 변경
                        //DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                        ProcessStateManager.Instance.CurWebProcessCount_++;
                    }
                    else
                    {
                        ProcessStateManager.Instance.FailedWebProcessCount_++;
                    }
                    //}
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_Use() - {0}", ex.Message));
                return false;
            }

            return true;
        }

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

        bool GetUseTicketInfo(string couponcode, string goodscode, ref string ticketcode)
        {
            try
            {
                string[] arrayCoupon;
                arrayCoupon = couponcode.Split('_');

                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = LQCrawlerInfo_.UseGoodsParam_;
                strparam = strparam.Replace("{CouponCode}", arrayCoupon[0]);
                LQCrawlerInfo_.UseGoodsRule_ = LQCrawlerInfo_.UseGoodsRule_.Replace("{CouponCode}", arrayCoupon[0]);

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
                htmlBuffer = htmlBuffer.Replace(" ", "&nbsp;");
                Regex re = new Regex(LQCrawlerInfo_.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);


                for(int i = 1; i <= oe.Count; i++)
                {
                    //if (Convert.ToInt32(arrayCoupon[1]) == i)
                    //{
                        ticketcode = oe[i - 1].Groups["CouponCode2"].ToString();
                    //}
                }

                
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool GetUseTicketInfo - {0}", ex.Message));
                return false;
            }

            return true;
        }

        bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            try
            {
                string useurl = LQCrawlerInfo_.UseUserUrl_;
                string useparam = LQCrawlerInfo_.UseUserParam_;

                // 두번째 쿠폰 코드 찾기
                string cp_sub = "";
                GetUseTicketInfo(cpcode, goodscode, ref cp_sub);


                string[] arrayCoupon = cpcode.Split('_');

                useparam = useparam.Replace("{CouponCode2}", cp_sub);
          
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
                NewLogManager2.Instance.Log(string.Format("Error bool Use_Deal( 사용처리 에러남 - {0}", ex.Message));
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
                   
                    pData.Value.goodsPassType = pInfo.GoodsPassType_;
                    pData.Value.ExData_GoodsName_ = pInfo.GoodsName_;
                    pData.Value.goodsSeq_ = pInfo.Idx_;
                    pData.Value.goodsCode_ = pInfo.Goods_Code_;

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        
                        COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];
                        if (pData.Value.State_ == pDBData.State_)
                            continue;


                        if (pData.Value.ExData_Use_ == "0000-00-00 00:00:00" && pData.Value.ExData_Cancel_ == "0000-00-00 00:00:00")
                        {
                           pDBData.BuyDate_ = pData.Value.BuyDate_;
                           WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool CheckNeedUseWeb( - {0}", ex.Message));
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
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckIsCancel() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        // 취소 엑셀 파싱해서 리스트에 담자.
        public override bool ExcelParsing_Cancel()
        {
            Dictionary<string, string> DoneList_ = new Dictionary<string, string>();

            foreach (var pData in CancelDownInfo_)
            {
                if (DoneList_.ContainsKey(pData.Key) == false)
                {
                    Internal_ExcelCancel_Parsing(pData.Value);

                    DoneList_.Add(pData.Key, pData.Key);
                }
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
                string url = @"http://partner.weekon.co.kr/pay_info_all/excel";
                string param = "searchType=all_search&searchText=&mt_sdate=&mt_edate=";

                string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls", makefolder, "C", Convert.ToString(dtNow.Ticks));

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, param, Cookie_);
                
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
             
                CancelDownInfo_.Add("C", downString);

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
