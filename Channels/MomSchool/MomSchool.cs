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
    public class MomSchool : BaseChannel
    {
        // 로그인 Web
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

                //if (htmlBuffer.IndexOf(LQCrawlerInfo_.LoginCheck_) < 0)
                //    return false;

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
                //ChannelGoodInfo p = new ChannelGoodInfo();
                //p.Goods_Code_ = "test";
                //GoodsInfoList_.Add(9999,p);
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
                    if (GoodsDownInfo_.ContainsKey(pGoodInfo.Goods_Code_) == false)
                    {
                        try
                        {
                            string method = LQCrawlerInfo_.ExcelDownMethod_;
                            string url = LQCrawlerInfo_.ExcelDownUrl_;
                            url = url.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);

                            string sendparameter = LQCrawlerInfo_.ExcelDownParameter_;

                            DateTime beforeData = dtNow.AddDays(-30);

                            string eDate_year = string.Format("{0:D4}", dtNow.Year);
                            string eDate_month = string.Format("{0:D2}", dtNow.Month);
                            string eDate_day = string.Format("{0:D2}", dtNow.Day);

                            string sDate_year = string.Format("{0:D4}", beforeData.Year);
                            string sDate_month = string.Format("{0:D2}", beforeData.Month);
                            string sDate_day = string.Format("{0:D2}", beforeData.Day);

                            sendparameter = sendparameter.Replace("{sDay_Y}", sDate_year);
                            sendparameter = sendparameter.Replace("{sDay_M}", sDate_month);
                            sendparameter = sendparameter.Replace("{sDay_D}", sDate_day);
                            sendparameter = sendparameter.Replace("{eDay_Y}", eDate_year);
                            sendparameter = sendparameter.Replace("{eDay_M}", eDate_month);
                            sendparameter = sendparameter.Replace("{eDay_D}", eDate_day);

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

                Int32 ExData_OpCount_1 = 5;
                Int32 ExData_OpCount_2 = 7;
                Int32 ExData_OpCount_3 = 9;

                Int32[] ArrBuyCnt = new Int32[3] { 0, 0, 0 };

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
                        pExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsNick_ = pChannelGoodInfo.GoodsName_;
                        pExcelData.goodsCode_ = pChannelGoodInfo.Goods_Code_;

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

                        tRange = ws.Cells[nCurrentRow, ExData_OpCount_1];
                        ArrBuyCnt[0] = Convert.ToInt32(tRange.Value2);
                        tRange = ws.Cells[nCurrentRow, ExData_OpCount_2];
                        ArrBuyCnt[1] = Convert.ToInt32(tRange.Value2);
                        tRange = ws.Cells[nCurrentRow, ExData_OpCount_3];
                        ArrBuyCnt[2] = Convert.ToInt32(tRange.Value2);

                        SplitDealAndInsertExcelData(pExcelData, ArrBuyCnt);
                        //SplitDealAndInsertExcelData(pExcelData, comparesitename);

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
                Int32 nCurrentRow = 5;
                Int32 CouponColumn = 7;
                Int32 StateColumn = 13;

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;

                        CCancelData pCCancelData = new CCancelData();
                        pCCancelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        pCCancelData.CancelCount_ = 1;

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

                        tRange = ws.Cells[nCurrentRow, StateColumn];
                        pCCancelData.State_ = Convert.ToString(tRange.Value2);

                        Excel_Cancel_List_.Add(pCCancelData.channelOrderCode_, pCCancelData);
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

        void SplitDealAndInsertExcelData(COrderData pExcelData, Int32[] arr)
        {
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            Int32 nTotalcount = 0;
            for (Int32 i = 0; i < arr.Length; i++)
            {
                for (Int32 j = 0; j < arr[i]; j++)
                {
                    nTotalcount++;
                    COrderData tempExcelData = new COrderData();
                    tempExcelData.CopyFrom(pExcelData);
                    tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);
                    tempExcelData.ExData_Option_ = pExcelData.ExData_Option_;
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
        // 하나의 딜을 여러개로 나눌 필요가 있는가? 있다면 나눠서 넣고 없다면 그냥 넣는다.        
        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            Excel_List_.Add(pExcelData.channelOrderCode_, pExcelData);

            return 1;
        }

        // 웹에서 사용처리
        public override bool Web_Use()
        {
            // 맘스쿨은 사용처리가 없다.

            return true;
        }

        public override bool OpenMarketChangeState()
        {
            return true;
        }

        bool GetUseTicketInfo(string couponcode, string goodscode, ref string ticketcode)
        {
            try
            {
                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = LQCrawlerInfo_.UseGoodsParam_;
                strparam = strparam.Replace("{CouponCode}", couponcode);
                strparam = strparam.Replace("{GoodsCode}", goodscode);

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

                ticketcode = oe[0].Groups["CouponCode2"].ToString();
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
                Int32 n = cpcode.IndexOf('-');
                string cp_sub = "";
                if (n < 0)
                {
                    GetUseTicketInfo(cpcode, goodscode, ref cp_sub);
                }
                else
                {
                    cp_sub = cpcode.Substring(n + 1, 9);
                }

                useparam = useparam.Replace("{CouponCode}", cpcode);
                useparam = useparam.Replace("{CouponCode2}", cp_sub);
                useparam = useparam.Replace("{GoodsCode}", goodscode);

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
                        pInfo = GetGoodInfoByGoodOptionName(pData.Value.ExData_GoodsName_, pData.Value.ExData_Option_);

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

                        // 레저큐에서 예약을 완료한 상태 웹에 사용 처리를 해야한다.
                        if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                        {
                            pDBData.BuyDate_ = pData.Value.BuyDate_;
                            WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                        else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                        {
                            pDBData.BuyDate_ = pData.Value.BuyDate_;
                            WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
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

        public override bool CheckIsCancel()
        {
            try
            {
                foreach (var pData in Excel_Cancel_List_)
                {
                    if (pData.Value.State_ != "취소완료")
                        continue;

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
                bool bContinue = true;
                Int32 PageCount = 1;

                string url = @"https://wing.coupang.com/dailyDeal/refundConfirms.pang";
                string param = @"cancelId=&orderId=&returnInvoiceNumber=&originInvoiceNumber=&returnDeliveryId=&pageNum={Page}&searchOk=true&page={Page}&download=Y&coupangSrl={GoodsCode}&isDelivery=N&coupangSrlList={GoodsCode}_N&statusType=";
                string checkend = "해당하는 항목이 없습니다";

                DateTime dtNow = DateTime.Now;
                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                string downString = "";
                Dictionary<string, string> DoneList = new Dictionary<string, string>();
                foreach (var pData in GoodsInfoList_)
                {
                    if (DoneList.ContainsKey(pData.Value.Goods_Code_) == false)
                    {
                        bContinue = true;
                        PageCount = 1;
                        while (bContinue)
                        {
                            string sendparam = param.Replace("{Page}", Convert.ToString(PageCount));
                            sendparam = sendparam.Replace("{GoodsCode}", pData.Value.Goods_Code_);
                            downString = string.Format(@"{0}\Cancel_{1}_{2}_{3}.xls"
                                , makefolder, pData.Value.Goods_Code_, Convert.ToString(dtNow.Ticks), PageCount);

                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", url, sendparam, Cookie_, null, null, 180000);

                            TextReader textReader = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding(pResponse.CharacterSet));
                            string htmlBuffer = textReader.ReadToEnd();

                            if (htmlBuffer.IndexOf(checkend) >= 0)
                            {
                                bContinue = false;
                                textReader.Close();
                                textReader.Dispose();
                                break;
                            }

                            HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                            textReader.Close();
                            textReader.Dispose();

                            string GoodsCode = string.Format("{0}_{1}", pData.Value.Goods_Code_, PageCount);
                            CancelDownInfo_.Add(GoodsCode, downString);

                            PageCount++;
                        }

                        DoneList.Add(pData.Value.Goods_Code_, pData.Value.Goods_Code_);
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error Web_DownLoad_CancelList {0}", ex.Message));
                return false;
            }

            return true;
        }
    }
}
