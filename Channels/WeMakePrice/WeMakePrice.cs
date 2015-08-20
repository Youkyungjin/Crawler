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
    public class WeMakePrice : BaseChannel
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

                if (actionType != "insert")
                {
                    return true;
                }
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

                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, Cookie_, null, null, 3600000);

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
                            NewLogManager2.Instance.Log(ex.Message + " - " + pGoodInfo.Goods_Code_);
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
                if (actionType != "insert")
                {
                    return true;
                }
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

        protected override bool Internal_ExcelCancel_Parsing(string filepath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application ap = null;
                Workbook wb = null;
                Worksheet ws = null;
                HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                Int32 nCurrentRow = 3;
                Int32 CouponColumn = 14;

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;

                        CCancelData pCCancelData = new CCancelData();
                        pCCancelData.channelOrderCode_ = tRange.Value2;
                        pCCancelData.CancelCount_ = 1;

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

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
                NewLogManager2.Instance.Log(string.Format("Error override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool CheckNeedFixUp()
        {
            foreach (var pData in Excel_List_)
            {
                if (DBSelected_List_.ContainsKey(pData.Key) == true)
                {
                    COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];

                    if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED])
                    {
                        if (pData.Value.ExData_Use_.IndexOf(LQCrawlerInfo_.ExData_UseCheck_) < 0)
                        {
                            pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED];
                            DBProccess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                    else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A])
                    {
                        if (pData.Value.ExData_Use_.IndexOf(LQCrawlerInfo_.ExData_UseCheck_) < 0)
                        {
                            pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR];
                            DBProccess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                }
            }

            return true;
        }
        // 웹에서 사용처리
        public override bool Web_Use()
        {
            ProcessStateManager.Instance.NeedWebProcessCount_ = WebProcess_List_.Count;
            foreach (var pData in WebProcess_List_)
            {
                if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                {
                    if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
                    {
                        CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                        pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                        DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                        ProcessStateManager.Instance.CurWebProcessCount_++;
                    }
                    else
                    {
                        ProcessStateManager.Instance.FailedWebProcessCount_++;
                    }
                }
                else if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                {
                    if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
                    {
                        CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                        pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                        ProcessStateManager.Instance.CurWebProcessCount_++;
                    }
                    else
                    {
                        ProcessStateManager.Instance.FailedWebProcessCount_++;
                    }
                }
            }

            return true;
        }
        
        public override bool OpenMarketChangeState()
        {
            return true;
        }

        // 상품 사용처리
        bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            try
            {
                string useurl = LQCrawlerInfo_.UseUserUrl_;
                string useparam = LQCrawlerInfo_.UseUserParam_;

                useparam = useparam.Replace("{CouponCode}", cpcode);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();
                

                string useCheck = @"\uc2e4\ud328\ud558\uc600\uc2b5\ub2c8\ub2e4.\uc774\ubbf8 \uc0ac\uc6a9\ub41c \ud2f0\ucf13\uc785\ub2c8\ub2e4.";
                if (htmlBuffer.IndexOf(LQCrawlerInfo_.UseUserCheck_) < 0 && htmlBuffer.IndexOf(useCheck) < 0)
                {
                    NewLogManager2.Instance.Log("public virtual bool Use_Deal(string cpcode) " + htmlBuffer);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Use_Deal - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            Excel_List_.Add(pExcelData.channelOrderCode_, pExcelData);

            return 1;
        }

        // 웹에서 사용처리 해야 할게 있는지 체크
        public override bool CheckNeedUseWeb()
        {
            try
            {
                foreach (var pData in DBSelected_List_)
                {
                    /*  ChannelGoodInfo pInfo = null;
    
                    if (LQCrawlerInfo_.ExData_GoodName_ == 0)
                         pInfo = GetGoodInfoByGoodCodeAndOptionName(pData.Value.goodsCode_, pData.Value.ExData_Option_);
                     else
                         pInfo = GetGoodInfoByGoodOptionName(pData.Value.ExData_GoodsName_, pData.Value.ExData_Option_);

                     if (pInfo == null)
                         continue;
                     */

                    pData.Value.ExData_GoodsName_ = pData.Value.ExData_GoodsName_;
                    pData.Value.goodsSeq_ = pData.Value.goodsSeq_;
                    pData.Value.goodsCode_ = pData.Value.goodsCode_;

                    if (pData.Value.channelOrderCode_ == "4SMBFCRQ7K3D" || pData.Value.channelOrderCode_ == "4UAEDSKHEDQM" || pData.Value.channelOrderCode_ == "55WOW493OVMY")
                    {
                        Int32 a = 32;
                    }

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];
                        
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
                NewLogManager2.Instance.Log(string.Format("Error bool CheckNeedUseWeb - {0}", ex.Message));
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
                NewLogManager2.Instance.Log(string.Format("Error bool CheckIsCancel - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                string method = "GET";
                string url = @"http://biz.wemakeprice.com/dealer/deal_list/do_xsl_download/{GoodsCode}/1";

                DateTime dtNow = DateTime.Now;

                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                foreach (var pData in GoodsInfoList_)
                {
                    if (CancelDownInfo_.ContainsKey(pData.Value.Goods_Code_) == false)
                    {
                        string sendurl = url.Replace("{GoodsCode}", pData.Value.Goods_Code_);
                        string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                            , makefolder, pData.Value.Goods_Code_, Convert.ToString(dtNow.Ticks));

                        HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, sendurl, "", Cookie_, null, null, 180000);

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

                        CancelDownInfo_.Add(pData.Value.Goods_Code_, downString);
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Web_DownLoad_CancelList - {0}", ex.Message));
                return false;
            }
            
            return true;
        }
    }
}
