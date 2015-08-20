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
    public class GSShop : BaseChannel
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
                            string sendparameter = LQCrawlerInfo_.ExcelDownParameter_;
                            // DB 의 값이 달라서 임시로 넣어둔값
                            //string method = "GET";
                            //string url = @"https://withgs.gsshop.com/dlv/mobilCpnAutoRfnManulProc/filedown";
                            //string sendparameter = @"flag=&ecDirdlvOboxYn=&dirdlvRelsInfoImprovTgtYn=N&downPsblYn=null&intgSrchLinkOrdNo=&dateTime={eDate}+12%3A56%3A50&soldOut=&prdCd=&itemCd=&chkOrgSupCd=1027095&chkSupCd=1027095&userDownLoadYn=0&mobilCpnAutoRfnSupYn=&qryTerm=B&fromSearchDtm={sDate}&toSearchDtm={eDate}&srchCond=A&srchText=&ordSt=1&useYn=0&fileDownGbn=";
                            string useragent = @"User-Agent: Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
                            string eDate = "";
                            string sData = "";
                            if (pGoodInfo.eDateFormat_ != null)
                            {
                                DateTime beforeData = dtNow.AddDays(-6);
                                eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                                sData = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                            }

                            sendparameter = sendparameter.Replace("{sDate}", sData);
                            sendparameter = sendparameter.Replace("{eDate}", eDate);

                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, Cookie_, null, useragent, 180000);

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
                Int32 ExData_Coupncode2 = 5;
                Int32 ExData_Buyer = LQCrawlerInfo_.ExData_Buyer_;
                Int32 ExData_Cancel = LQCrawlerInfo_.ExData_Cancel_;
                Int32 ExData_Use = LQCrawlerInfo_.ExData_Use_;
                Int32 ExData_Buyphone = LQCrawlerInfo_.ExData_Buyphone_;
                Int32 ExData_Price = LQCrawlerInfo_.ExData_Price_;
                Int32 ExData_BuyDate = LQCrawlerInfo_.ExData_Buydate_;
                Int32 ExData_BuyCount = LQCrawlerInfo_.ExData_Count_;
                Int32 ExData_GoodsName = LQCrawlerInfo_.ExData_GoodName_;

                if (pChannelGoodInfo.GoodsAttrType_ == 1)
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

                if (nCurrentRow > 0)
                    ProcessStateManager.Instance.NeedParsingCount_ += (ws.UsedRange.Rows.Count - (nCurrentRow - 1));

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[5, 1];
                        comparesitename = Convert.ToString(tRange.Value2);

                        tRange = ws.Cells[nCurrentRow, ExData_Option];
                        if (tRange == null)
                            break;

                        tempString = Convert.ToString(tRange.Value2);
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


                        tRange = ws.Cells[nCurrentRow, ExData_GoodsName];
                        pExcelData.ExData_GoodsName_ = Convert.ToString(tRange.Value2);
                        pExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsName_;

                        tRange = ws.Cells[nCurrentRow, ExData_Coupncode];
                        if (tRange == null)
                            break;

                        pExcelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.channelOrderCode_ == null)
                            break;
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Replace("'", "");
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Trim();

                        // 2주문 아이템 번호
                        tRange = ws.Cells[nCurrentRow, ExData_Coupncode2];
                        string orderitemnum = Convert.ToString(tRange.Value2);
                        orderitemnum = orderitemnum.Trim();
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_ + "_" + orderitemnum; 

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

                        double temp = Convert.ToDouble(tRange.Value2);
                        DateTime dta = DateTime.FromOADate(temp);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");

                        //pExcelData.BuyDate_ = Convert.ToDouble(tRange.Value2);
                        //pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace('.', '-');

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
                HKExcelHelper.GetWorkSheetFromText(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                Int32 nCurrentRow = 2;
                Int32 CouponColumn = 1;

                Int32 wow = ws.UsedRange.Rows.Count;

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
                NewLogManager2.Instance.Log(string.Format("Error public override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }

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
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_Use - {0}", ex.Message));
                return false;
            }

            return true;
        }

        public override bool OpenMarketChangeState()
        {
            return true;
        }

        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            pExcelData.ExData_Option_ = Regex.Replace(pExcelData.ExData_Option_, @"[^a-zA-Z0-9가-힣]", "");
            pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsNick_, @"[^a-zA-Z0-9가-힣]", "");

            if (Excel_List_.ContainsKey(pExcelData.channelOrderCode_) == false)
            {
                Excel_List_.Add(pExcelData.channelOrderCode_, pExcelData);
            }
            return 1;
        }

        public bool Use_Deal(Int32 goodsSeq, string cpcode, string goodscode)
        {
            try
            {
                //string useurl = LQCrawlerInfo_.UseUserUrl_;
                //string useparam = LQCrawlerInfo_.UseUserParam_;

                string useurl = @"https://withgs.gsshop.com/dlv/mobilCpnAutoRfnManulProc/saveTicktInfo";
                string useparam = @"0_gr_id=0&0_checked=1&0_ordSt=%EB%AF%B8%EC%B2%98%EB%A6%AC&0_exchRtpYn=&0_sordNo=2084278775&0_ordNo={CouponCode}&0_ordItemNo={TicketCode}&0_ordPrsn=%EC%9C%A0%EA%B2%BD%EC%A7%84&0_charCpnRcvCelphn=010****4317&0_charCpnSndCelphn=010****4317&0_cpnPinNoVal=1599-8370&0_cpnNoVal=1599-8370&0_charIssueDt=2014-12-09&0_cpnValidDtm=2015-02-22%2023%3A59%3A59&0_useYn=N&0_custUseDt=&0_relsFshDt=&0_dlvFshDt=&0_saleAwareDt=&0_prdCd=15054184&0_prdNm=%EA%B4%91%EC%A7%84%EA%B5%AC%20%EC%96%B4%EB%A6%B0%EC%9D%B4%ED%9A%8C%EA%B4%80&0_attrPrdCd=15054184001&0_attrPrdNm=&0_intrntPrdNm=%5B%EA%B4%91%EC%A7%84%EA%B5%AC%5D%EC%96%B4%EB%A6%B0%EC%9D%B4%ED%9A%8C%EA%B4%80%20%EB%88%88%EC%8D%B0%EB%A7%A4%EC%9E%A5%20%EB%B9%99%EC%96%B4%EC%9E%A1%EC%9D%B4%EC%B2%B4%ED%97%98%2F%EB%88%88%EB%86%80%EC%9D%B4%EB%8F%99%EC%82%B0%2F%EC%96%B4%EB%A6%B0%EC%9D%B4%EB%8C%80%EA%B3%B5%EC%9B%90%2F%EC%96%B4%EB%A6%84%EC%8D%B0%EB%A7%A4%2F%EA%B2%A8%EC%9A%B8%EB%A0%88%EC%A0%B8%20%EC%B2%B4%ED%97%98&0_sordQty=1&0_supPrdCd=a-12345678910&0_saleTmp=7000&0_supGivAmt=6650&0_autoRfnRt=70&0_smlchgShrRt=50&0_prdRegCpnValidDtm=20150222&0_prdRegCpnValidTerm=&0_!nativeeditor_status=updated&0_noProcCnt=6&0_ordCnlCnt=0&0_dlvFshCnt=0&1_gr_id=1&1_checked=0&1_ordSt=%EB%AF%B8%EC%B2%98%EB%A6%AC&1_exchRtpYn=&1_sordNo=2084278778&1_ordNo=676065839&1_ordItemNo=1&1_ordPrsn=%EC%9C%A0%EA%B2%BD%EC%A7%84&1_charCpnRcvCelphn=010****4317&1_charCpnSndCelphn=010****4317&1_cpnPinNoVal=1599-8370&1_cpnNoVal=1599-8370&1_charIssueDt=2014-12-09&1_cpnValidDtm=2015-02-22%2023%3A59%3A59&1_useYn=N&1_custUseDt=&1_relsFshDt=&1_dlvFshDt=&1_saleAwareDt=&1_prdCd=15054184&1_prdNm=%EA%B4%91%EC%A7%84%EA%B5%AC%20%EC%96%B4%EB%A6%B0%EC%9D%B4%ED%9A%8C%EA%B4%80&1_attrPrdCd=15054184001&1_attrPrdNm=&1_intrntPrdNm=%5B%EA%B4%91%EC%A7%84%EA%B5%AC%5D%EC%96%B4%EB%A6%B0%EC%9D%B4%ED%9A%8C%EA%B4%80%20%EB%88%88%EC%8D%B0%EB%A7%A4%EC%9E%A5%20%EB%B9%99%EC%96%B4%EC%9E%A1%EC%9D%B4%EC%B2%B4%ED%97%98%2F%EB%88%88%EB%86%80%EC%9D%B4%EB%8F%99%EC%82%B0%2F%EC%96%B4%EB%A6%B0%EC%9D%B4%EB%8C%80%EA%B3%B5%EC%9B%90%2F%EC%96%B4%EB%A6%84%EC%8D%B0%EB%A7%A4%2F%EA%B2%A8%EC%9A%B8%EB%A0%88%EC%A0%B8%20%EC%B2%B4%ED%97%98&1_sordQty=1&1_supPrdCd=a-12345678910&1_saleTmp=7000&1_supGivAmt=6650&1_autoRfnRt=70&1_smlchgShrRt=50&1_prdRegCpnValidDtm=20150222&1_prdRegCpnValidTerm=&1_!nativeeditor_status=updated&1_noProcCnt=6&1_ordCnlCnt=0&1_dlvFshCnt=0&ids=0%2C1";

                string ticketcode = "";
                if (GetUseTicketInfo(cpcode, ref ticketcode) == false)
                    return false;

                useparam = useparam.Replace("{GoodsCode}", goodscode);
                useparam = useparam.Replace("{TicketCode}", ticketcode);
                useparam = useparam.Replace("{CouponCode}", cpcode);

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
                NewLogManager2.Instance.Log(string.Format("Error public override bool Use_Deal - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 상품 사용 처리 티켓번호 얻어오기
        bool GetUseTicketInfo(string couponcode, ref string ticketcode)
        {
            try
            {
                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = LQCrawlerInfo_.UseGoodsParam_;
                strparam = strparam.Replace("{CouponCode}", couponcode);

                NewLogManager2.Instance.Log(strurl);
                NewLogManager2.Instance.Log(strparam);

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
                string tempCouponCode = "";
                Regex re = new Regex(LQCrawlerInfo_.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);
                for (int i = 0; i < oe.Count; i++)
                {
                    tempCouponCode = oe[i].Groups["CouponCode1"].ToString();
                    tempCouponCode = tempCouponCode + oe[i].Groups["CouponCode2"].ToString();

                    if (tempCouponCode == couponcode)
                    {
                        ticketcode = oe[i].Groups["TicketCode"].ToString();
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool GetUseTicketInfo - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 상품 사용 취소 처리 티켓번호 얻어오기
        bool GetUseCancelInfo(string couponcode, ref string ticketcode)
        {
            try
            {
                string strurl = LQCrawlerInfo_.NUseGoodsUrl_;
                string strparam = LQCrawlerInfo_.NUseGoodsParam_;
                strparam = strparam.Replace("{CouponCode}", couponcode);

                NewLogManager2.Instance.Log(strurl);
                NewLogManager2.Instance.Log(strparam);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.NUseGoodsCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;
                }

                Regex re = new Regex(LQCrawlerInfo_.NUseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);

                ticketcode = oe[0].Groups["TicketCode"].ToString();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool GetUseTicketInfo - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 상품 사용 취소 처리
        public bool Cancel_Use(string cpcode, string goodscode)
        {
            try
            {
                string useurl = LQCrawlerInfo_.NUseUserUrl_;
                string useparam = LQCrawlerInfo_.NUseUserParam_;

                string ticketcode = "";
                if (GetUseCancelInfo(cpcode, ref ticketcode) == false)
                    return false;

                useparam = useparam.Replace("{GoodsCode}", goodscode);
                useparam = useparam.Replace("{TicketCode}", ticketcode);
                useparam = useparam.Replace("{CouponCode}", cpcode);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_);

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.NUseUserCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Cancel_Use - {0}", ex.Message));
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
                NewLogManager2.Instance.Log(string.Format("Error bool CheckNeedUseWeb - {0}", ex.Message));
                return false;
            }

            return true;
        }

        public override bool CheckIsCancel()
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

            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                string method = "GET";
                string url = @"https://ps.ticketmonster.co.kr/daily/cancellist";
                string param = @"main_deal_srl={GoodsCode}&branch_srl=&start_date={sDate}&end_date={eDate}&searchKey=&searchVal=&excel=Y";

                DateTime dtNow = DateTime.Now;

                string eDate = "";
                string sDate = "";

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
                        if (pData.Value.eDateFormat_ != null)
                        {
                            DateTime beforeData = dtNow.AddDays(-7);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
                            eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                            sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                        }

                        string sendparam = param.Replace("{sDate}", sDate);
                        sendparam = sendparam.Replace("{eDate}", eDate);
                        sendparam = sendparam.Replace("{GoodsCode}", pData.Value.Goods_Code_);
                        string downString = string.Format(@"{0}\Cancel_{1}_{2}.xls"
                            , makefolder, pData.Value.Goods_Code_, Convert.ToString(dtNow.Ticks));

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

                        CancelDownInfo_.Add(pData.Value.Goods_Code_, downString);
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool Web_DownLoad_CancelList - {0}", ex.Message));
                return false;
            }

            return true;
        }
    }
}