using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;

using System.Web;
using System.Net;
using HKLibrary.WEB;

using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;
using CrawlerShare;

namespace CrawlerShare
{
    
    public class LQCrawlerBase
    {
        protected CookieContainer cookie_ = null;

        // 크롤링 
        public virtual void Crawling(BackgroundWorker pWorker)
        {
            LogManager.Instance.Log(string.Format("<<< 크롤링 시작 {0}>>>", CrawlerManager.Instance.CrawlingCount()));
            pWorker.ReportProgress(1);
            bool bResult = true;

            // DB 접속
            SqlHelper pMySqlDB = new SqlHelper();
            pMySqlDB.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_
                , CrawlerInfoManager.Instance.sshuser_, CrawlerInfoManager.Instance.sshpw_);

            // DB에서 채널 정보 로드하기
            pWorker.ReportProgress(2);
            //bResult = DBFunctions.GetCrawlerInfo_Authority(pMySqlDB, CrawlerInfoManager.Instance.channelidx_, CrawlerInfoManager.Instance.partneridx_);
            bResult = DBFunctions.GetCrawlerInfo_Authority(pMySqlDB, CrawlerInfoManager.Instance.channelidx_, CrawlerInfoManager.Instance.partneridx_, CrawlerInfoManager.Instance.authorityseq_);
            if (bResult == false)
            {
                pWorker.ReportProgress(9);
                OrderManager.Instance.Init();
                LogManager.Instance.Log("<<< 크롤링 실패 : GetCrawlerInfo >>>");
                return;
            }
            // 주문별 상태 로드하기            
            bResult = DBFunctions.SelectStateTable(pMySqlDB);
            if (bResult == false)
            {
                pWorker.ReportProgress(9);
                OrderManager.Instance.Init();
                LogManager.Instance.Log("<<< 크롤링 실패 : SelectStateTable >>>");
                return;
            }

            // DB에서 현재 가지고 있는 상품 정보 로드하기
            pWorker.ReportProgress(3);
            bResult = DBFunctions.GetGoodsTable(pMySqlDB, CrawlerInfoManager.Instance.channelseq_, CrawlerInfoManager.Instance.authorityseq_);
            if (bResult == false)
            {
                pWorker.ReportProgress(9);
                OrderManager.Instance.Init();
                LogManager.Instance.Log("<<< 크롤링 실패 : GetGoodsTable >>>");
                return;
            }

            // DB에서 현재 Insert된 상품 정보 로드 하기
            pWorker.ReportProgress(4);
            OrderManager.Instance.Init();
            bResult = DBFunctions.Select_tblOrder(pMySqlDB, CrawlerInfoManager.Instance.channelseq_);
            bResult = DBFunctions.Select_tblOrderWr(pMySqlDB, CrawlerInfoManager.Instance.channelseq_);

            if (bResult == false)
            {
                pWorker.ReportProgress(9);
                OrderManager.Instance.Init();
                LogManager.Instance.Log("<<< 크롤링 실패 : Select_tblOrder >>>");
                return;
            }

            CrawlerManager.Instance.GetResultData().DBSelected_ = OrderManager.Instance.GetOrderList().Count;

            // 채널 로그인
            pWorker.ReportProgress(5);
            bResult = Login();

            // 채널에서 상품 판매 정보 다운로드 // 데이터 가공
            pWorker.ReportProgress(6);
            bResult = DownloadExcelAndDataMake();
            if (bResult == false)
            {
                pWorker.ReportProgress(9);
                OrderManager.Instance.Init();
                LogManager.Instance.Log("<<< 크롤링 실패 >>>");
                return;
            }
            bResult = Combine_DB_And_Excel(false);
            //bResult = Process_RefundData(pMySqlDB);  // 당일 취소건에 대한것. 일단 막아두자.
            
            bResult = Process_ExpiredData();

            // 채널에 상품처리// 채널에 취소처리  // 채널에 반품처리
            pWorker.ReportProgress(7);
            bResult = Process_Use_Cancel_Refund();

            // 다운로드 받은파일 삭제
            //deletedownloadfile();

            if (pMySqlDB.Close() == false)
            {
                LogManager.Instance.Log("<<< 삭제 실패 1 >>>");
            }
            pMySqlDB = null;

            // DB 에 Insert or Update
            pWorker.ReportProgress(8);
            SqlHelper pMySqlDB2 = new SqlHelper();
            pMySqlDB2.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_
                , CrawlerInfoManager.Instance.sshuser_, CrawlerInfoManager.Instance.sshpw_);
            Process_DB(pMySqlDB2);

            if (pMySqlDB2.Close() == false)
            {
                LogManager.Instance.Log("<<< 삭제 실패 2 >>>");
            }
            pMySqlDB2 = null;

            // 완료
            CrawlerManager.Instance.AddCrawlingCount();
            pWorker.ReportProgress(9);
            LogManager.Instance.Log("<<< 크롤링 완료 >>>");
        }

        // 로그인
        public virtual bool Login()
        {
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();
            cookie_ = new CookieContainer();

            try
            {
                string loginurl = pCrawler.LoginUrl_;
                string loginstring = pCrawler.LoginParam_.Replace("{LoginID}", pCrawler.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", pCrawler.LoginPW_);
                byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(pCrawler.LoginMethod_, loginurl, loginstring, cookie_);
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(pCrawler.LoginCheck_) < 0)
                    return false;

            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(ex.Message);
                return false;
            }

            return true;
        }

        public bool Combine_DB_And_Excel(bool bWithGoodsName)
        {
            Dictionary<string, tblOrderData> pExcelList = OrderManager.Instance.GetExcelOrderList();
            Dictionary<string, tblOrderData> pDBList = OrderManager.Instance.GetOrderList();
            Dictionary<string, tblOrderData> pWrongList = OrderManager.Instance.GetWrongOrderList();

            Dictionary<Int32, ChannelGoodInfo> pGoodsList = CrawlerManager.Instance.GetGoodsInfo();

            foreach (var pData in pExcelList)
            {
                tblOrderData pExcelData = pData.Value;
                ChannelGoodInfo pChannelGoodInfo = null;
                
                if (bWithGoodsName == true)
                {
                    pChannelGoodInfo = CrawlerManager.Instance.GetGoodSeqByOptionNameAndGoodName(pExcelData.ExData_Option_, pExcelData.ExData_GoodsName_);
                }
                else
                {
                    pChannelGoodInfo = CrawlerManager.Instance.GetGoodInfoByOptionName(pExcelData.ExData_Option_);
                }

                if (pChannelGoodInfo == null)                
                {// 매칭되는 상품명이 없음.
                    if (pDBList.ContainsKey(pExcelData.channelOrderCode_) == false
                        && pWrongList.ContainsKey(pExcelData.channelOrderCode_) == false)
                    {// 근데 DB에 주문해둔것에 있다면 있다가 사라진거니까 아무것도 하지 말고 넘어가자.
                        pExcelData.goodsSeq_ = 0;
                        pExcelData.NeedDBProc_ = tblOrderData.NeedDBProc.Insert;
                        pWrongList.Add(pExcelData.channelOrderCode_, pExcelData);
                    }

                    continue;
                }

                pExcelData.ExData_GoodsName_ = pChannelGoodInfo.GoodsName_;
                pExcelData.goodsSeq_ = pChannelGoodInfo.Idx_;                
                pExcelData.goodsCode_ = pChannelGoodInfo.Goods_Code_;

                if (IsNeedCheck(pExcelData) == true)
                {
                    MakeDBData(pExcelData);
                }
            }

            pExcelList.Clear(); // 더이상 액셀 데이터를 쓸일이 없으니 삭제하자.

            return true;
        }

        // 상품 판매 다운로드하고 데이터 정리
        public virtual bool DownloadExcelAndDataMake()
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
                            DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                            eDate = string.Format(pGoodInfo.eDateFormat_, dtNow.Year, dtNow.Month, dtNow.Day);
                            sData = string.Format(pGoodInfo.eDateFormat_, beforeData.Year, beforeData.Month, beforeData.Day);
                        }

                        sendparameter = sendparameter.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);
                        sendparameter = sendparameter.Replace("{sDate}", sData);
                        sendparameter = sendparameter.Replace("{eDate}", eDate);

                        HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, cookie_, null, null, 180000);

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
                        LogManager.Instance.Log(ex.Message);
                        continue;
                    }

                    GoodsDownInfo.Add(pGoodInfo.Goods_Code_, downString);
                }

                LoadExcelAndInsertList(GoodsDownInfo[pGoodInfo.Goods_Code_], pGoodInfo.GoodsAttrType_, false, pGoodInfo.GoodsName_);
            }

            return true;
        }

        // 상품 정보를 DB 에서 읽어와서 그것에 따라서 상품명을 매칭하는 함수.
        public virtual bool LoadExcelAndInsertList(string filepath, Int32 GoodsAttrType, bool bFixedType, string goodsname)
        {
            LQStructures.LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();

            Microsoft.Office.Interop.Excel.Application ap = null;
            Workbook wb = null;
            Worksheet ws = null;
            HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);


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

            if (bFixedType == true)
            {// 레저큐 양식일때는 고정값으로
                nCurrentRow = 2;
                ExData_Option = 4;
                ExData_Coupncode = 3;
                ExData_Buyer = 1;
                ExData_Cancel = 6;
                ExData_Use = 6;
                ExData_Buyphone = 2;
                ExData_Price = 5;
                ExData_BuyDate = 7;
                ExData_BuyCount = 8;
            }
            else
            {
                nCurrentRow = pCrawlerInfo.ExData_Start_;
                ExData_Option = pCrawlerInfo.ExData_Option_;
                ExData_Coupncode = pCrawlerInfo.ExData_Coupncode_;
                ExData_Buyer = pCrawlerInfo.ExData_Buyer_;
                ExData_Cancel = pCrawlerInfo.ExData_Cancel_;
                ExData_Use = pCrawlerInfo.ExData_Use_;
                ExData_Buyphone = pCrawlerInfo.ExData_Buyphone_;
                ExData_Price = pCrawlerInfo.ExData_Price_;
                ExData_BuyDate = pCrawlerInfo.ExData_Buydate_;
                ExData_BuyCount = pCrawlerInfo.ExData_Count_;

                // 티몬을 위한 변경
                if (GoodsAttrType == 1)
                {
                    nCurrentRow = 3;
                    ExData_Option = 6;
                    ExData_Coupncode = 3;
                    ExData_Buyer = 1;
                    ExData_Cancel = 8;
                    ExData_Use = 8;
                    ExData_Buyphone = 2;
                    ExData_Price = 7;
                    ExData_BuyDate = 9;
                }
            }
            
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
                    tblOrderData pExcelData = new tblOrderData();
                    pExcelData.channelSeq_ = pCrawlerInfo.Channel_Idx_;
                    pExcelData.authoritySeq_ = pCrawlerInfo.AuthoritySeq_;                    
                    //pExcelData.goodsCode_ = pGoodInfo.Goods_Code_;
                    pExcelData.goodsSeq_ = tempgoodSeq;
                    pExcelData.ExData_Option_ = tempString;
                    pExcelData.ExData_OptionOriginal_ = tempString;
                    if (string.IsNullOrEmpty(goodsname) == false)
                    {
                        pExcelData.ExData_GoodsName_ = goodsname;
                    }

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
                    if (pCrawlerInfo.Channel_Idx_ == 9  ||pCrawlerInfo.Channel_Idx_ == 14 || pCrawlerInfo.Channel_Idx_ == 15
                        || pCrawlerInfo.Channel_Idx_ == 18 || pCrawlerInfo.Channel_Idx_ == 23)
                    {
                        double temp = Convert.ToDouble(tRange.Value2);
                        DateTime dta = DateTime.FromOADate(temp);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");
                    }
                    else if (pCrawlerInfo.Channel_Idx_ == 22 )
                    {
                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                        pExcelData.BuyDate_ = pExcelData.BuyDate_ + " 00:00:00";
                    }
                    else
                    {
                        pExcelData.BuyDate_ = Convert.ToString(tRange.Value2);
                    }

                    pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('.', '-');
                    
                    if (ExData_BuyCount != 0)// 구매갯수를 따로 뽑아야 하는 채널에서만
                    {
                        tRange = ws.Cells[nCurrentRow, ExData_BuyCount];
                        pExcelData.BuyCount_ =  Convert.ToInt32(tRange.Value2);
                    }

                    SplitDealAndInsertExcelData(pExcelData, comparesitename);

                }
                catch (System.Exception ex)
                {
                    LogManager.Instance.Log(string.Format("엑셀 파싱 에러 : {0}", ex.Message));
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

        // 하나의 딜을 여러개로 나눌 필요가 있는가? 있다면 나눠서 넣고 없다면 그냥 넣는다.
        // 일반적인 경우에 1개만 한다.
        public virtual Int32 SplitDealAndInsertExcelData(tblOrderData pExcelData, string comparesitename = "")
        {
            pExcelData.bFindInExcel_ = true;
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            OrderManager.Instance.AddExcelData(pExcelData);

            return 1;
        }

        // 체크해야 하는 데이터인가? 채널별로 하지 않아도 되는것이 있다.
        public virtual bool IsNeedCheck(tblOrderData pExcelData)
        {
            // 이미 리스트에 들어있고
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            if (pOrderList.ContainsKey(pExcelData.channelOrderCode_) == true)
            {
                tblOrderData pData = pOrderList[pExcelData.channelOrderCode_];
                if (pData.bProcessed_ == true)
                    return false;
            }

            return true;
        }

        public virtual void MakeDBData(tblOrderData pExcelData)
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();

            // DB에 저장되어 있던 값이 아니면 들어온 값이라면
            if (pOrderList.ContainsKey(pExcelData.channelOrderCode_) == false)
            {
                // 상태값 변경
                if (pExcelData.ExData_Use_ == pCrawler.ExData_UseCheck_)
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                }
                else if (pExcelData.ExData_Cancel_ == pCrawler.ExData_CancelCheck_)
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND];
                }
                else if (pExcelData.ExData_Use_ == "정산완료" || pExcelData.ExData_Cancel_ == "정산완료")
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                }
                else
                {
                    pExcelData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];
                }

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

                if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                {// 예약한 상태 사용처리 해야함
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USER_WANT_REFUND])
                {// 사용 취소를 요청한 상태 사용취소처리 해야함
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY])
                {// DB 에는 구매만 한 상태인데, 엑셀에는 취소 요청을 해둔 상태라면 환불이다.
                    if (pExcelData.ExData_Cancel_ == pCrawler.ExData_CancelCheck_)
                    {
                        pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USER_WANT_REFUND];
                    }
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED])
                {
                    // 사용 처리된 상태인데, 뭔가 해줘야 할게 있는가?
                }
                else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND])
                {// 환불이 끝난 상태 아무것도 하지 않는다.
                }
                else
                {
                    //pCrawler.ExData_UseCheck_
                    if (pExcelData.ExData_Use_.IndexOf(pCrawler.ExData_UseCheck_) >= 0)
                    {
                        pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        pDBData.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                        LogManager.Instance.Log(string.Format("채널에서 이미 사용처리 되어 있어서 DB 값만 바꾸자.{0}", pDBData.channelOrderCode_));
                    }
                    else
                    {
                        string log = string.Format("C:{0}, CP:{1}, S:{2}, {3}, {4} "
                        , pDBData.channelSeq_, pDBData.channelOrderCode_, pDBData.State_, pExcelData.ExData_Cancel_, pExcelData.ExData_Use_);
                        LogManager.Instance.Log(log);
                    }
                }
            }
        }

        // DB에는 있으나 엑셀에서 찾지 못한것들 처리
        public virtual bool Process_RefundData(SqlHelper MySqlDB)
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            Dictionary<Int32, ChannelGoodInfo> pInfoList = CrawlerManager.Instance.GetGoodsInfo();
            foreach (var pData in pOrderList)
            {
                tblOrderData p = pData.Value;

                if (p.bFindInExcel_ == false)
                {
                    if (pInfoList.ContainsKey(p.goodsSeq_) == true)
                    {
                        if (p.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY])
                        {
                        // bool bResult = DBFunctions.Update_CancelOrderInfo(MySqlDB, (Int32)p.seq_, p.channelOrderCode_);
                        }
                        //else if (p.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                        //{
                        //    p.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.BLACK];
                        //}
                        else
                        {
                            continue;
                        }

                        //p.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                    }
                }
            }

            return true;
        }

        // 유효기간이 지난 딜은 모두 사용처리로 바꿔준다.
        // 이 함수에서 예약됨으로 바꿔두면 사용처리한다.
        public virtual bool Process_ExpiredData()
        {
            try
            {
                DateTime curDateTime = DateTime.Now;
                Dictionary<Int32, ChannelGoodInfo> pGoodsInfo = CrawlerManager.Instance.GetGoodsInfo();
                Dictionary<string, tblOrderData> pDBOrderList = OrderManager.Instance.GetOrderList();

                foreach (var pData in pDBOrderList)
                {
                    tblOrderData p = pData.Value;
                    if (pGoodsInfo.ContainsKey(p.goodsSeq_) == false)
                    {
                        LogManager.Instance.Log(string.Format("Process_ExpiredData : 상품키가 없다. {0}", p.goodsSeq_));
                        continue;
                    }

                    ChannelGoodInfo pgoodinfo = pGoodsInfo[p.goodsSeq_];

                    if (p.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY] && pgoodinfo != null)
                    {
                        if (pgoodinfo.Expired_ == true && curDateTime > pgoodinfo.availableDateTime_)
                        {
                            p.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED];
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(string.Format("Process_ExpiredData() 에서 에러났쪄엄 {0}", ex.Message));
                return false;
            }


            return true;
        }
        // DB 에 처리
        public void Process_DB(SqlHelper MySqlDB)
        {
            Int32 nTempSeq = 0;
            Int32 nStartSeq = 0;
            Int32 nEndSeq = 0;
            string emptyCheckCode = "";

            // 매칭이 정상적인 데이터 넣기
             Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            foreach (var pData in pOrderList)
            { 
                tblOrderData pOrder = pData.Value;
                      

                if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.Insert)
                {
                     bool bResult = DBFunctions.Insert_tblOrder(MySqlDB, pOrder.goodsSeq_, pOrder.channelSeq_
                        , pOrder.channelOrderCode_, pOrder.orderSettlePrice_, 1, pOrder.orderID_, pOrder.orderName_
                        , pOrder.orderPhone_, pOrder.State_, pOrder.ExData_Option_, pOrder.ExData_OptionOriginal_
                        , pOrder.BuyDate_, ref nTempSeq);
                    LogManager.Instance.Log(string.Format("DB Insert {0}", pOrder.channelOrderCode_));
                 
                    if (bResult == true)
                    {
                        CrawlerManager.Instance.GetResultData().Inserted_++;
                    }
                    else
                    {
                        CrawlerManager.Instance.GetResultData().ErrorCount_++;
                        CrawlerManager.Instance.GetResultData().TotalErrorCount_++;
                    }

                    if (nTempSeq > 0)
                    {
                        if (nStartSeq == 0)
                        {
                            nEndSeq = nStartSeq = nTempSeq;
                        }
                        else
                        {
                            nEndSeq = nTempSeq;
                        }
                    }

                    if (pOrder.channelSeq_ == 11 || pOrder.channelSeq_ == 12 || pOrder.channelSeq_ == 14 || pOrder.authoritySeq_ == 26)
                    { 

                        int goodsSeq = 0;
                        string cpcode = "";
                        string goodscode = "";

                        goodsSeq  = pOrder.goodsSeq_;
                        cpcode    = pOrder.channelOrderCode_;
                        goodscode = pOrder.goodsCode_;


                        if (cpcode.Contains(emptyCheckCode) != true || emptyCheckCode =="")
                        {//이베이(옥션,지마켓),굿바이셀리는 선 사용처리
                            First_UseData(goodsSeq, cpcode, goodscode);
                        }
                        string[] cpcodeArray = cpcode.Split('_');
                        emptyCheckCode = cpcodeArray[0];
                    }
                }
                else if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.Update)
                {

                    bool bResult = DBFunctions.Update_OrderInfo(MySqlDB, (Int32)pOrder.seq_, pOrder.State_);
                    LogManager.Instance.Log(string.Format("DB Update {0}", pOrder.channelOrderCode_));
                    if (bResult == true)
                    {
                        CrawlerManager.Instance.GetResultData().Updated_++;
                    }
                    else
                    {
                        CrawlerManager.Instance.GetResultData().ErrorCount_++;
                        CrawlerManager.Instance.GetResultData().TotalErrorCount_++;
                    }
                }
            }
           


            bool bSMSOn = true;

            if (nStartSeq > 0 && nEndSeq > 0 && bSMSOn == true)
            {
                DBFunctions.Insert_SMS(MySqlDB, nStartSeq, nEndSeq);
            }

            // 매칭이 비정상적인 데이터 넣기
            Dictionary<string, tblOrderData> pWrongOrderList = OrderManager.Instance.GetWrongOrderList();
            foreach (var pData in pWrongOrderList)
            {
                tblOrderData pOrder = pData.Value;
                if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.Insert)
                {
                    bool bResult = DBFunctions.Insert_tblOrder(MySqlDB, pOrder.goodsSeq_, pOrder.channelSeq_
                        , pOrder.channelOrderCode_, pOrder.orderSettlePrice_, 1, pOrder.orderID_, pOrder.orderName_
                        , pOrder.orderPhone_, pOrder.State_, pOrder.ExData_Option_, pOrder.ExData_OptionOriginal_
                        , pOrder.BuyDate_, ref nTempSeq);
                    LogManager.Instance.Log(string.Format("DB Wrong Insert {0}", pOrder.channelOrderCode_));
                }

            }
        }
        
        // 엑셀데이터 받자마자 사용처리를 해야하는 채널이 있음 EX: 이베이,굿바이셀리

        public virtual bool First_UseData(Int32 goodsSeq, string cpcode, string goodscode)
        {
            // 웹 호출을 통해서 사용처리한다.
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.UseUserUrl_;
            string useparam = pCrawlerInfo.UseUserParam_;

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

        // 상품 사용/취소 처리
        public bool Process_Use_Cancel_Refund()
        {
            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetOrderList();
            foreach (var pData in pOrderList)
            {
                tblOrderData pOrder = pData.Value;

                if (pOrder.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                {
                    if (Use_Deal(pOrder.goodsSeq_, pOrder.channelOrderCode_, pOrder.goodsCode_) == true)
                    {
                        CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                        if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.None)
                        {
                            pOrder.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                        }

                        pOrder.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                    }
                }
                else if (pOrder.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                {
                    if (Use_Deal(pOrder.goodsSeq_, pOrder.channelOrderCode_, pOrder.goodsCode_) == true)
                    {
                        CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                        if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.None)
                        {
                            pOrder.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                        }

                        pOrder.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                    }
                }
                // 취소는 크롤러에서 처리하지 않음.
                //else if (pOrder.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.NEED_CANCEL_USE])
                //{
                //    if (Cancel_Use(pOrder.channelOrderCode_, pOrder.goodsCode_) == true)
                //    {
                //        if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.None)
                //        {
                //            pOrder.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                //        }
                //        pOrder.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];
                //        CrawlerManager.Instance.GetResultData().TotalCancelDeal_++;
                //    }
                //}
                // 환불 요청은 크롤러에서 처리하지 않음.
                //else if (pOrder.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USER_WANT_REFUND])
                //{
                //    if (Refund(pOrder.channelOrderCode_) == true)
                //    {
                //        if (pOrder.NeedDBProc_ == tblOrderData.NeedDBProc.None)
                //        {
                //            pOrder.NeedDBProc_ = tblOrderData.NeedDBProc.Update;
                //        }
                //        pOrder.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND];
                //        CrawlerManager.Instance.GetResultData().TotalRefundDeal_++;
                //    }
                //}
            }

            return true;
        }

        // 상품 사용처리
        public virtual bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            // 웹 호출을 통해서 사용처리한다.
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.UseUserUrl_;
            string useparam = pCrawlerInfo.UseUserParam_;

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

        // 상품 사용 취소 처리
        public virtual bool Cancel_Use(string cpcode, string goodscode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.NUseUserUrl_;
            string useparam = pCrawlerInfo.NUseUserParam_;

            useparam = useparam.Replace("{CouponCode}", cpcode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);

            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.NUseUserCheck_) < 0)
            {
                LogManager.Instance.Log("public virtual bool Cancel_Use(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }

        // 환불 승인 처리
        public virtual bool Refund(string cpcode)
        {
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = pCrawlerInfo.RUseUserUrl_;
            string useparam = pCrawlerInfo.RUseUserParam_;

            useparam = useparam.Replace("{CouponCode}", cpcode);

            LogManager.Instance.Log(useurl);
            LogManager.Instance.Log(useparam);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, cookie_);
            if (pResponse == null)
                return false;

            TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
            string htmlBuffer = r.ReadToEnd();

            if (htmlBuffer.IndexOf(pCrawlerInfo.RUseUserCheck_) < 0)
            {
                LogManager.Instance.Log("public virtual bool Refund(string cpcode) " + htmlBuffer);
                return false;
            }

            return true;
        }
    }
}
