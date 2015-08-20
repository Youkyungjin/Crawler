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
    public class EzWell : BaseChannel
    {
        string str_use_url_1_ = @"http://partneradmin.ezwel.com/cpadm/shop/order/updateOrderAll.ez?changeMode=B";
        string str_use_param_1_ = @"kind=submit&currentPage=1&chk=csp&applYear=2014&goodsNm=&orderStatus=&orderDt1=20140928&orderDt2=20141226&modiDt1=&modiDt2=&orderNum=&sndNm=&dlvrHopeDt1=&dlvrHopeDt2=&dlvrStatus=&clientType=&orderStatusVal=1002&dlvrStatusVal=1002&checkOrder={CouponCode}_ERM&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001";
        string str_use_check_1_ = @"ezwel";

        string str_use_url_2_ = @"http://partneradmin.ezwel.com/cpadm/shop/order/updateOrderAll.ez?changeMode=C";
        string str_use_param_2_ = @"kind=submit&currentPage=1&chk=csp&applYear=2014&goodsNm=&orderStatus=&orderDt1=20140928&orderDt2=20141226&modiDt1=&modiDt2=&orderNum=&sndNm=&dlvrHopeDt1=&dlvrHopeDt2=&dlvrStatus=&clientType=&checkOrder={CouponCode}_ERM&orderStatusVal=1002&dlvrStatusVal=1002&orderStatusVal=1002&dlvrStatusVal=1002&orderStatusVal=1002&dlvrStatusVal=1002&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001&orderStatusVal=1002&dlvrStatusVal=1001";
        string str_use_check_2_ = @"ezwel";

        // 로그인 Web
        public override bool Web_Login()
        {
            Cookie_ = new CookieContainer();

            // 1차 쿠키 받아오는곳
            try
            {
                string loginurl = "https://partneradmin.ezwel.com/cpadm/login/loginForm.ez";

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("GET", loginurl, "", Cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding("EUC-KR"));
                string htmlBuffer = r.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            // 2차 로그인 처리
            try
            {
                string loginurl = "https://partneradmin.ezwel.com/cpadm/login/newLoginCheckAction.ez";
                string loginparameter = "&userId={0}&password={1}";
                loginparameter = string.Format(loginparameter, LQCrawlerInfo_.LoginID_, LQCrawlerInfo_.LoginPW_);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", loginurl, loginparameter, Cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding("EUC-KR"));
                string htmlBuffer = r.ReadToEnd();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            // 실제 로그인
            try
            {
                string loginurl = LQCrawlerInfo_.LoginUrl_;
                string loginstring = LQCrawlerInfo_.LoginParam_.Replace("{LoginID}", LQCrawlerInfo_.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", LQCrawlerInfo_.LoginPW_);
                //byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(LQCrawlerInfo_.LoginMethod_, loginurl, loginstring, Cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
//                string htmlBuffer = r.ReadToEnd();

                //if (htmlBuffer.IndexOf(pCrawler.LoginCheck_) < 0)
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
                                DateTime beforeData = dtNow.AddDays(-10);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                                eDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                                sData = string.Format("{0:D4}{1:D2}{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
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
                Int32 ExData_BasicColumn = 22;

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
                        

                        tRange = ws.Cells[nCurrentRow, ExData_BasicColumn];
                        string State = Convert.ToString(tRange.Value2);
                        if (State == "완료취소")
                        {
                            nCurrentRow++;
                            continue;
                        }
                        Int32 tempgoodSeq = -1;
                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;
                        pExcelData.authoritySeq_ = LQCrawlerInfo_.AuthoritySeq_;
                        pExcelData.goodsSeq_ = tempgoodSeq;
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
                            {
                                // 돈에 , 가 있으면 제거하자.
                                tempString = Convert.ToString(tRange.Value2);
                                tempString = tempString.Replace(",", "");
                                pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                            }
                        }

                        tRange = ws.Cells[nCurrentRow, ExData_BuyDate];

                        double temp = Convert.ToDouble(tRange.Value2);
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
                Int32 CouponColumn = 2;
                Int32 CancelCountColumn = 18;
                Int32 StateColumn = 22;

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


                        for (Int32 i = 1; i <= pCCancelData.CancelCount_; i++)
                        {
                            CCancelData tempExcelData = new CCancelData();
                            tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pCCancelData.channelOrderCode_, i);
                            tempExcelData.State_ = pCCancelData.State_;
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
                NewLogManager2.Instance.Log(string.Format("Error public override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }
            
            return true;
        }
        // 웹에서 사용처리
        public override bool Web_Use()
        {
            // 웹 사용 처리가 이지웰(L)은 있구 이지웰(W)는 없다.
            if (LQCrawlerInfo_.AuthoritySeq_ == 26)
            {
                try
                {
                    ProcessStateManager.Instance.NeedWebProcessCount_ = WebProcess_List_.Count;
                    foreach (var pData in WebProcess_List_)
                    {

                        if (DBSelected_List_.ContainsKey(pData.Key) == true)
                        {
                            if (Use_Deal(pData.Value.goodsSeq_, pData.Value.channelOrderCode_, pData.Value.goodsCode_) == true)
                            {
                                CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                                pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
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
               }
               catch (System.Exception ex)
               {
                    NewLogManager2.Instance.Log(string.Format("Error override bool Web_Use() - {0}", ex.Message));
                    return false;
               }
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
            if (LQCrawlerInfo_.AuthoritySeq_ == 26)
            {
                if (use_step_1(cpcode) == false)
                    return false;

                if (use_step_2(cpcode) == false)
                    return false;
            }            
                return true;
        }

        // 사용 처리 1단계
        bool use_step_1(string cpcode)
        {
            try
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
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool use_step_1 - {0}", ex.Message));
                return false;
            }


            return true;
        }

        bool use_step_2(string cpcode)
        {
            try
            {
                string useurl = str_use_url_2_;
                string useparam = str_use_param_2_;

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
                string sDate = "";
                if (pGoodInfo.eDateFormat_ != null)
                {
                    eDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                    sDate = pGoodInfo.sDate_;
                    sDate = sDate.Replace("-", "");
                }


                string strurl = LQCrawlerInfo_.UseUserUrl_;
                string strparam = LQCrawlerInfo_.UseUserParam_;
                string[] cpcodeArray = cpcode.Split('_');
                cpcode = cpcodeArray[0];
                strparam = strparam.Replace("{CouponCode}", cpcode);
                strparam = strparam.Replace("{sDate}", sDate);
                strparam = strparam.Replace("{eDate}", eDate);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, Cookie_);

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
                NewLogManager2.Instance.Log(string.Format("Error override bool GetUseTicketInfo() - {0}", ex.Message));
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
            string regstring = "";

            if (pExcelData.authoritySeq_ != 26)
            {
                regstring = @"(?<OptionName>\S+),\S+수량(?<Count>\d+)개";
            }
            else
            {
                regstring = @"(?<OptionName>\S+)";
            }


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
            }

            return nTotalcount;
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
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckNeedUseWeb - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        // 취소 엑셀 파싱해서 리스트에 담자.
        public override bool ExcelParsing_Cancel()
        {
            foreach (var pData in CancelDownInfo_)
            {
                Internal_ExcelCancel_Parsing(pData.Value);
            }

            return true;
        }

        public override bool CheckIsCancel()
        {
            try
            {
                foreach (var pData in Excel_Cancel_List_)
                {
                    if (pData.Value.State_ != "완료취소")
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
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckIsCancel - {0}", ex.Message));
                return false;
            }
            
            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                DateTime beforeData = dtNow.AddDays(-50);
                DateTime CbeforeDate = dtNow.AddDays(-7);
                string eDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                string sDate = string.Format("{0:D4}{1:D2}{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                string yDate = string.Format("{0:D4}", dtNow.Year);
                string ceDate = string.Format("{0:D4}{1:D2}{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                string csDate = string.Format("{0:D4}{1:D2}{2:D2}", CbeforeDate.Year, CbeforeDate.Month, CbeforeDate.Day);
                


                string method = "POST";
                string url = @"http://partneradmin.ezwel.com/cpadm/shop/order/orderListExcel.ez";
                string param = "";

                if (LQCrawlerInfo_.AuthoritySeq_ != 26)
                {
                    param = @"kind=submit&currentPage=1&chk=csp&applYear={yDate}&goodsNm=&orderStatus=1003&orderDt1={sDate}&orderDt2={eDate}&modiDt1={csDate}&modiDt2={ceDate}&orderNum=&sndNm=&dlvrHopeDt1=&dlvrHopeDt2=&dlvrStatus=&clientType=&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001";

                }
                else
                {
                    param = @"kind=submit&currentPage=1&chk=csp&applYear={yDate}&goodsNm=&orderStatus=1003&orderDt1={sDate}&orderDt2={eDate}&modiDt1={csDate}&modiDt2={ceDate}&orderNum=&sndNm=&dlvrHopeDt1=&dlvrHopeDt2=&dlvrStatus=&clientType=&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001&orderStatusVal=1003&dlvrStatusVal=1001";
                }


                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                string sendparam = param.Replace("{sDate}", sDate);
                sendparam = sendparam.Replace("{yDate}", yDate);
                sendparam = sendparam.Replace("{eDate}", eDate);
                sendparam = sendparam.Replace("{csDate}", csDate);
                sendparam = sendparam.Replace("{ceDate}", ceDate);
                string downString = string.Format(@"{0}\Cancel_{1}.xls"
                    , makefolder, Convert.ToString(dtNow.Ticks));

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

                CancelDownInfo_.Add("CANCEL", downString);

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
