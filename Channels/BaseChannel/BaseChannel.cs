using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using System.Drawing;
using HKLibrary.UTIL;
using CrawlerShare;
using BusinessRefinery.Barcode;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;
using CData;
using System.Net;
using System.Drawing.Imaging;
using HKLibrary.WEB;
using BarcodeLib;
using Tamir.SharpSsh;


namespace Channels
{
    public abstract class BaseChannel
    {
        public static DateTime dtnow = DateTime.Now; 
        protected CookieContainer Cookie_ = null;
        protected LQCrawlerInfo LQCrawlerInfo_ = null;
        protected Dictionary<Int32, ChannelGoodInfo> GoodsInfoList_ = new Dictionary<Int32, ChannelGoodInfo>(); // 상품 정보
        protected Dictionary<string, string> GoodsDownInfo_ = new Dictionary<string, string>();                 // 다운로드 받은 엑셀 파일 위치 정보
        protected Dictionary<string, COrderData> DBSelected_List_ = new Dictionary<string, COrderData>();       // DB 에서 다운로드 받은것
        protected Dictionary<string, COrderData> Excel_List_ = new Dictionary<string, COrderData>();            // 엑셀에 들어 있던 데이터
        protected Dictionary<string, COrderData> WebProcess_List_ = new Dictionary<string, COrderData>();       // 웹에서 사용처리 해야하는것
        protected Dictionary<string, COrderData> DBProccess_List_ = new Dictionary<string, COrderData>();       // DB에 처리 해야 하는것
        protected Dictionary<string, COrderData> ExcelPass_List = new Dictionary<string, COrderData>();         // 날짜에 패스된 데이터
        protected Dictionary<string, COrderData> DBProccess_List_Wrong_ = new Dictionary<string, COrderData>();       // 매칭되지 않아서 DB 에 Insert 해야 하는것

        protected Dictionary<string, string> CancelDownInfo_ = new Dictionary<string, string>();          // 다운로드 받은 취소 파일 위치 정보
        protected Dictionary<string, CCancelData> Excel_Cancel_List_ = new Dictionary<string, CCancelData>();       // 취소 엑셀에 있던것.
        protected Dictionary<string, COrderData> DBCancel_List_ = new Dictionary<string, COrderData>();   // 취소 처리를 위해 DB에 Update 해야 하는것
        public string actionType;

        
        #region 모든 채널에서 공통으로 사용하는 함수
        
        // 초기화 작업
        public bool Init()
        {
            GoodsInfoList_.Clear();            
            GoodsDownInfo_.Clear();
            DBSelected_List_.Clear();
            Excel_List_.Clear();
            WebProcess_List_.Clear();
            DBProccess_List_.Clear();


            DBProccess_List_Wrong_.Clear();


            CancelDownInfo_.Clear();
            Excel_Cancel_List_.Clear();
            DBCancel_List_.Clear();
            dtnow = DateTime.Now; 
            return true;
        }
        // 다운 로드 받은 파일 삭제
        public void DeleteDownloadedFile()
        {
            foreach (var pData in GoodsDownInfo_)
            {
                if (System.IO.File.Exists(pData.Value) == true)
                {
                    try
                    {
                        System.IO.File.Delete(pData.Value);
                    }
                    catch (System.IO.IOException ex)
                    {
                        NewLogManager2.Instance.Log("System.IO.File.Exists(downString) " + ex.Message);
                        continue;
                    }
                }
            }

            foreach (var pData in CancelDownInfo_)
            {
                if (System.IO.File.Exists(pData.Value) == true)
                {
                    try
                    {
                        System.IO.File.Delete(pData.Value);
                    }
                    catch (System.IO.IOException ex)
                    {
                        NewLogManager2.Instance.Log("System.IO.File.Exists(downString) " + ex.Message);
                        continue;
                    }
                }
            }
        }
        // DB 에서 정보 로드 하기
        public bool DB_GetInfos()
        {
            bool bResult = true;

            try
            {
                SqlHelper pMySqlDB = new SqlHelper();
                
                pMySqlDB.Connect(CINIManager.Instance.method_, CINIManager.Instance.dbip_, CINIManager.Instance.dbport_, CINIManager.Instance.dbname_
                    , CINIManager.Instance.dbaccount_, CINIManager.Instance.dbpw_, CINIManager.Instance.sshhostname_
                    , CINIManager.Instance.sshuser_, CINIManager.Instance.sshpw_);
                
                bResult = DB_GetChannelInfo(pMySqlDB);

                if (bResult)
                    ProcessStateManager.Instance.ChannelName_ = LQCrawlerInfo_.ChannelName_;

                if (bResult)
                    bResult = DB_GetStateTable(pMySqlDB);

                if (bResult)
                    bResult = DB_GetGoodsInfo(pMySqlDB);

                if (bResult)
                    bResult = DB_SelectData(pMySqlDB);

                pMySqlDB.Close();
                pMySqlDB = null;
                
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public bool DB_GetInfos() {0}", ex.Message));
                bResult = false;
            }

            return bResult;
        }
        // DB 에서 채널 정보 로드
        bool DB_GetChannelInfo(SqlHelper pDBHelper)
        {
            //bool bResult = DBInterface.GetCrawlerInfo(pDBHelper, CINIManager.Instance.channelidx_, CINIManager.Instance.partneridx_, ref LQCrawlerInfo_);
            bool bResult = DBInterface.GetCrawlerInfoNew(pDBHelper, CINIManager.Instance.channelidx_, CINIManager.Instance.partneridx_
                , CINIManager.Instance.authorityseq_, ref LQCrawlerInfo_);
            return bResult;
        }
        // 자신이 담당할 상품 Load
        bool DB_GetStateTable(SqlHelper pDBHelper)
        {
            bool bResult = DBInterface.SelectStateTable(pDBHelper);
            return bResult;
        }
        // 자신이 담당할 상품 Load
        bool DB_GetGoodsInfo(SqlHelper pDBHelper)
        {
            bool bResult = DBInterface.GetGoodsTableWithUID(pDBHelper, CINIManager.Instance.channelidx_, CINIManager.Instance.authorityseq_
                , CINIManager.Instance.UID_, ref GoodsInfoList_);
            return bResult;
        }
        // 기존 주문 내역 Load
        bool DB_SelectData(SqlHelper pDBHelper)
        {
            bool bResult = true;
            string eDate = "";
            string sDate = "";

            string DateFormat_ = "{0}-{1}-{2} {3}:{4}:{5}";

            DBSelected_List_.Clear();
            DateTime beforeData = dtnow.AddDays(-6);  // 이지웰 건수가 많으면 데이터를 못들고옴, 10일전 건수만 들고오게 함
            //yourdate.ToString("yyyyMMddHHmmss");

            //eDate = dtnow.ToString("yyyy-MM-dd 23:59:59");
            //sDate = beforeData.ToString("yyyy-MM-dd 00:00:00");
            //eDate = string.Format(DateFormat_, dtnow.Year, dtnow.Month, dtnow.Day, "23", "59", "59");
            //sData = string.Format(DateFormat_, beforeData.Year, beforeData.Month, beforeData.Day, "00", "00", "00");

            bResult = DBInterface.Select_Order_List(pDBHelper, CINIManager.Instance.channelidx_, CINIManager.Instance.authorityseq_, CINIManager.Instance.UID_, ref DBSelected_List_);


            //foreach (var pData in GoodsInfoList_)
            //{
            //    bResult = DBInterface.Select_tblOrder_With_UID(pDBHelper, CINIManager.Instance.channelidx_, pData.Value.Idx_ , ref DBSelected_List_);
                
                // bResult = DBInterface.Select_tblOrder_With_UID(pDBHelper, CINIManager.Instance.channelidx_, pData.Value.Idx_, sDate, eDate, ref DBSelected_List_);
            //}
            
            return bResult;
        }
        public Int32 GetOrderListByCouponCode(string coupon_code, ref Dictionary<string, COrderData> OrderList)
        {
            Int32 Result = 0;

            foreach (var pData in DBSelected_List_)
            {
                if (pData.Key.IndexOf(coupon_code) >= 0)
                {
                    OrderList.Add(pData.Key, pData.Value);
                    Result++;
                }
            }

            return Result;
        }

        public Int32 QrCodeImageCreate(string PinCode, int OrderSeq)
        {
            
            try
            {
                Int32 Result = 0;
                QRCode QrCode = new QRCode();

                string makefile = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                string sDirPath;
                sDirPath = makefile + "\\data";
                DirectoryInfo di = new DirectoryInfo(sDirPath);
                if (di.Exists == false)
                {
                    di.Create();
                }

                makefile += "\\";
                makefile += "data";
                makefile += "\\";
                makefile += PinCode;
                makefile += ".jpg";

                string folderName = Convert.ToString(OrderSeq);
                string ftpfileName = folderName + "\\" + PinCode + ".jpg";

                string ftpBasicPath = "/var/www/IMAGE/Web/upload/order/qrcode/";


                /*QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                QrCode qrCode = qrEncoder.Encode(PinCode);
                var renderer = new GraphicsRenderer(new FixedCodeSize(400, QuietZoneModules.Zero), Brushes.Black, Brushes.White);
                MemoryStream ms = new MemoryStream();
                renderer.WriteToStream(qrCode.Matrix, ImageFormat.Png, ms);
                var image = new Bitmap(Image.FromStream(ms), new Size(new Point(200, 200)));
                image.Save(makefile, ImageFormat.Jpeg);
                */
                HKLibrary.comwls.comwls.image_01_FileMake(makefile);
                string ftpUri = "ftp://ledev.leisureq.co.kr:10004/" + ftpBasicPath + folderName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                //request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("qruser", "#qruser1!");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                ftpUri = "ftp://ledev.leisureq.co.kr:10004/" + ftpBasicPath + ftpfileName;
                request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("qruser", "#qruser1!");

                byte[] fileContents = File.ReadAllBytes(makefile);
             
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                response = (FtpWebResponse)request.GetResponse();      
                response.Close();
                return Result;
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

        public Int32 BarCodeImageCreate(string PinCode, int OrderSeq)
        {

            try
            {
                Int32 Result = 0;
                string makefile = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                string sDirPath;
                sDirPath = makefile + "\\data";
                DirectoryInfo di = new DirectoryInfo(sDirPath);
                if (di.Exists == false)
                {
                    di.Create();
                }

                makefile += "\\";
                makefile += "data";
                makefile += "\\";
                makefile += PinCode;
                makefile += ".jpg";

                string folderName = Convert.ToString(OrderSeq);
                string ftpfileName = folderName + "/" + PinCode + ".jpg";

                string ftpBasicPath = "/var/www/IMAGE/Web/upload/order/barcode/";

                
                BarcodeLib.Barcode b = new BarcodeLib.Barcode();
                b.Encode(BarcodeLib.TYPE.CODE128, PinCode);
                b.SaveImage(makefile, BarcodeLib.SaveTypes.JPG);

                //string ftpUri = "" + ftpBasicPath + folderName;
                SshStream ssh = new SshStream("ftp://ledev.leisureq.co.kr:10004/", "lion", "gkffl1!");

                
                
                string ftpUri = "ftp://121.78.127.40:21/" + ftpBasicPath + folderName;
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.MakeDirectory;
                //request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("infobay", "info9887");
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Stream ftpStream = response.GetResponseStream();

                ftpStream.Close();
                response.Close();

                ftpUri = "ftp://121.78.127.40:21/" + ftpBasicPath + ftpfileName;
                request = (FtpWebRequest)WebRequest.Create(ftpUri);
                request.Method = WebRequestMethods.Ftp.UploadFile;

                // This example assumes the FTP site uses anonymous logon.
                request.Credentials = new NetworkCredential("infobay", "info9887");
                byte[] fileContents = File.ReadAllBytes(makefile);
                request.ContentLength = fileContents.Length;

                Stream requestStream = request.GetRequestStream();
                requestStream.Write(fileContents, 0, fileContents.Length);
                requestStream.Close();

                response = (FtpWebResponse)request.GetResponse();
                response.Close();
                return Result;
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

        // DB 에 데이터 Insert
        public bool DB_InsertData()
        {
            try
            {
                SqlHelper pMySqlDB = new SqlHelper();

                pMySqlDB.Connect(CINIManager.Instance.method_, CINIManager.Instance.dbip_, CINIManager.Instance.dbport_, CINIManager.Instance.dbname_
                    , CINIManager.Instance.dbaccount_, CINIManager.Instance.dbpw_, CINIManager.Instance.sshhostname_
                    , CINIManager.Instance.sshuser_, CINIManager.Instance.sshpw_);

                ProcessStateManager.Instance.NeedDBProcessCount_ = DBProccess_List_.Count;
                Int32 nTempSeq = 0;
                Int32 nStartSeq = 0;
                Int32 nEndSeq = 0;
                String nTempPinCode = "";

                foreach (var pData in DBProccess_List_)
                {
                    COrderData pOrder = pData.Value;

                    if (pOrder.goodsPassType == "2")
                    {
                        bool bResult = DBInterface.Insert_tblOrder_test(pMySqlDB, pOrder.goodsSeq_, pOrder.channelSeq_
                             , pOrder.channelOrderCode_, pOrder.orderSettlePrice_, 1, pOrder.orderID_, pOrder.orderName_
                             , pOrder.orderPhone_, pOrder.State_, pOrder.ExData_Option_, pOrder.ExData_OptionOriginal_
                             , pOrder.BuyDate_, ref nTempSeq, ref nTempPinCode);
                    }
                    else
                    {
                        bool bResult = DBInterface.Insert_Order_Channel(pMySqlDB, pOrder.goodsSeq_, pOrder.channelSeq_
                             , pOrder.channelOrderCode_, pOrder.orderSettlePrice_,  pOrder.orderName_
                             , pOrder.orderPhone_, pOrder.State_, pOrder.BuyDate_, ref nTempSeq, ref nTempPinCode);
                    }

                    if (nTempSeq > 0)
                    {
                        if(pOrder.goodsSendType_ == 2){
                            QrCodeImageCreate(nTempPinCode, nTempSeq);
                        }
                        else if (pOrder.goodsSendType_ == 3)
                        {
                            BarCodeImageCreate(nTempPinCode, nTempSeq);
                        }

                        if (nStartSeq == 0)
                        {
                            nEndSeq = nStartSeq = nTempSeq;
                        }
                        else
                        {
                            nEndSeq = nTempSeq;
                        }
                    }
                    
                    ProcessStateManager.Instance.CurDBProcessCount_++;
                }

                bool bSMSOn = true;

                if (nStartSeq > 0 && nEndSeq > 0 && bSMSOn == true)
                {
                    DBInterface.Insert_SMS(pMySqlDB, nStartSeq, nEndSeq);
                }

                // 매칭 되지 않은 데이터 넣기
                foreach (var pData in DBProccess_List_Wrong_)
                {
                    COrderData pOrder = pData.Value;

                    bool bResult = DBInterface.Insert_tblOrderWr(pMySqlDB, pOrder.goodsSeq_, pOrder.channelSeq_
                           , pOrder.channelOrderCode_, pOrder.orderSettlePrice_, 1, pOrder.orderID_, pOrder.orderName_
                           , pOrder.orderPhone_, pOrder.State_, pOrder.ExData_GoodsName_, pOrder.ExData_GoodsNick_, pOrder.ExData_Option_, pOrder.ExData_OptionOriginal_
                           , pOrder.BuyDate_, ref nTempSeq);
                }

                pMySqlDB.Close();
                pMySqlDB = null;
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error DB_InsertData {0}", ex.Message));
                return false;
            }

            return true;
        }
        // DB 주문정보 수정
        public bool DB_UpdateData()
        {
            try
            {
                SqlHelper pMySqlDB = new SqlHelper();

                pMySqlDB.Connect(CINIManager.Instance.method_, CINIManager.Instance.dbip_, CINIManager.Instance.dbport_, CINIManager.Instance.dbname_
                    , CINIManager.Instance.dbaccount_, CINIManager.Instance.dbpw_, CINIManager.Instance.sshhostname_
                    , CINIManager.Instance.sshuser_, CINIManager.Instance.sshpw_);

                ProcessStateManager.Instance.NeedDBProcessCount_ = DBProccess_List_.Count;
                // 일반 상태 변경 관련
                foreach (var pData in DBProccess_List_)
                {
                    COrderData pOrder = pData.Value;

                    DBInterface.Update_OrderInfo(pMySqlDB, (Int32)pOrder.seq_, pOrder.State_);
                    ProcessStateManager.Instance.CurDBProcessCount_++;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error DB_UpdateData {0}", ex.Message));
                return true;
            }

            return true;
        }

        // DB 취소로 변경
        public bool DB_UpdateData_Cancel()
        {
            try
            {
                SqlHelper pMySqlDB = new SqlHelper();

                pMySqlDB.Connect(CINIManager.Instance.method_, CINIManager.Instance.dbip_, CINIManager.Instance.dbport_, CINIManager.Instance.dbname_
                    , CINIManager.Instance.dbaccount_, CINIManager.Instance.dbpw_, CINIManager.Instance.sshhostname_
                    , CINIManager.Instance.sshuser_, CINIManager.Instance.sshpw_);

                ProcessStateManager.Instance.NeedDBProcessCount_ = DBCancel_List_.Count;

                // 취소관련 채널 시퀀스
              

                // 취소처리 관련
                foreach (var pData in DBCancel_List_)
                {
                    COrderData pOrder = pData.Value;

                    DBInterface.Update_OrderInfo_Cancel(pMySqlDB, (Int32)pOrder.seq_, pOrder.State_, pOrder.channelOrderCode_);
                    ProcessStateManager.Instance.CurDBProcessCount_++;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error DB_UpdateData {0}", ex.Message));
                return true;
            }


            return true;
        }
        // 주문정보 잘못된것 정정하는 프로시저
        public bool DB_UpdateData_FixUp()
        {
            try
            {
                SqlHelper pMySqlDB = new SqlHelper();

                pMySqlDB.Connect(CINIManager.Instance.method_, CINIManager.Instance.dbip_, CINIManager.Instance.dbport_, CINIManager.Instance.dbname_
                    , CINIManager.Instance.dbaccount_, CINIManager.Instance.dbpw_, CINIManager.Instance.sshhostname_
                    , CINIManager.Instance.sshuser_, CINIManager.Instance.sshpw_);

                ProcessStateManager.Instance.NeedDBProcessCount_ = DBProccess_List_.Count;
                foreach (var pData in DBProccess_List_)
                {
                    COrderData pOrder = pData.Value;

                    DBInterface.Update_OrderInfo_FixUP(pMySqlDB, (Int32)pOrder.seq_, pOrder.State_);
                    ProcessStateManager.Instance.CurDBProcessCount_++;
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error DB_UpdateData_FixUp {0}", ex.Message));
                return true;
            }


            return true;
        }
        // 상품 정보 찾기
        protected ChannelGoodInfo GetGoodInfoByOptionName(string optionname)
        {
            foreach (var pData in GoodsInfoList_)
            {
                if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0)
                {
                    return pData.Value;
                }
            }

            return null;
        }
        // 상품명과 옵션 명을 넣어서 상품 정보 찾기
        protected ChannelGoodInfo GetGoodInfoByGoodOptionName(string goodname, string optionname)
        {
            foreach (var pData in GoodsInfoList_)
            {
                if (string.Compare(goodname, pData.Value.GoodsNickName_, true) == 0)
                {
                    if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0)
                    {
                        return pData.Value;
                    }
                }
            }

            return null;
        }
        // 상품코드와 옵션 명을 넣어서 상품 정보 찾기
        protected ChannelGoodInfo GetGoodInfoByGoodCodeAndOptionName(string goodsCode, string optionname)
        {
            foreach (var pData in GoodsInfoList_)
            {
                if (string.Compare(goodsCode, pData.Value.Goods_Code_, true) == 0)
                {
                    if (string.Compare(optionname, pData.Value.OptionNickName_, true) == 0)
                    {
                        return pData.Value;
                    }
                }
            }

            return null;
        }
        // 신규 주문인가 체크해서 리스트에 넣는다.
        public virtual bool CheckNewOrder()
        {

            try
            {
                foreach (var pData in Excel_List_)
                {
                    if (DBSelected_List_.ContainsKey(pData.Key) == false)
                    {
                        ChannelGoodInfo pInfo = null;

                        if (LQCrawlerInfo_.ExData_GoodName_ == 0)
                            pInfo = GetGoodInfoByGoodCodeAndOptionName(pData.Value.goodsCode_, pData.Value.ExData_Option_);
                        else
                            pInfo = GetGoodInfoByGoodOptionName(pData.Value.ExData_GoodsNick_, pData.Value.ExData_Option_);

                        if (pInfo == null)
                        {
                            // 매칭이 안됐음. 하나만 Insert 하자 그러면 알아볼것이다.
                            if (DBProccess_List_Wrong_.ContainsKey(pData.Value.ExData_Option_) == false)
                            {
                                pData.Value.goodsSeq_ = 0;
                                DBProccess_List_Wrong_.Add(pData.Value.ExData_Option_, pData.Value);
                            }
                            continue;
                        }

                        pData.Value.goodsPassType = pInfo.GoodsPassType_;
                        pData.Value.goodsSendType_ = pInfo.GoodsSendType_;
                        pData.Value.ExData_GoodsName_ = pInfo.GoodsName_;
                        pData.Value.goodsSeq_ = pInfo.Idx_;
                        pData.Value.goodsCode_ = pInfo.Goods_Code_;


                        if (pData.Value.ExData_Use_.Contains(LQCrawlerInfo_.ExData_UseCheck_) == true)
                        {
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        }
                        else if (pData.Value.ExData_Cancel_.Contains(LQCrawlerInfo_.ExData_CancelCheck_) == true)
                        {
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_REFUND];
                        }
                        else if (pData.Value.ExData_Use_ == "정산완료" || pData.Value.ExData_Cancel_ == "정산완료")
                        {
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                        }
                        else
                        {
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_BUY];
                        }

                        DBProccess_List_.Add(pData.Key, pData.Value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error CheckNewOrder {0}", ex.Message));
                return true;
            }
            
            return true;
        }
        // 웹에서는 사용처리가 되지 않았는데, DB에 는 사용처리로 되어 있다면, DB값을 변경 해준다.
        public virtual bool CheckNeedFixUp()
        {
            foreach (var pData in Excel_List_)
            {
                if (DBSelected_List_.ContainsKey(pData.Key) == true)
                {
                    COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];

                    if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED])
                    {
                        if (pData.Value.ExData_Use_ != LQCrawlerInfo_.ExData_UseCheck_)
                        {
                            pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED];
                            DBProccess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                    else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A])
                    {
                        if (pData.Value.ExData_Use_ != LQCrawlerInfo_.ExData_UseCheck_)
                        {
                            pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR];
                            DBProccess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                }
            }

            return true;
        }
        // 엑셀 파싱해서 리스트에 담자.
        public bool ExcelParsing()
        {
            Dictionary<string, string> DoneList_ = new Dictionary<string, string>();

            foreach (var pData in GoodsInfoList_)
            {
                if (DoneList_.ContainsKey(pData.Value.Goods_Code_) == false)
                {
                    Internal_Excel_Parsing(pData.Value);
                    //Internal_Excel_Parsing(GoodsDownInfo_[pData.Value.Goods_Code_], pData.Value.GoodsAttrType_
                    //, false, pData.Value.GoodsName_);

                    DoneList_.Add(pData.Value.Goods_Code_, pData.Value.Goods_Code_);
                }
            }

            return true;
        }
        // 취소 엑셀 파싱해서 리스트에 담자.
        public virtual bool ExcelParsing_Cancel()
        {
            Dictionary<string, string> DoneList_ = new Dictionary<string, string>();

            foreach (var pData in GoodsInfoList_)
            {
                if (DoneList_.ContainsKey(pData.Value.Goods_Code_) == false)
                {
                    Internal_ExcelCancel_Parsing(CancelDownInfo_[pData.Value.Goods_Code_]);

                    DoneList_.Add(pData.Value.Goods_Code_, pData.Value.Goods_Code_);
                }
            }

            return true;
        }
        // 웹에서 취소가 된 딜이 있는데, DB에는 다른걸로 되어 있다면, DB값을 취소로 변경 해주자.
        public bool CheckNeedFixUP_Cancel()
        {
            foreach (var pData in Excel_List_)
            {
                if (DBSelected_List_.ContainsKey(pData.Key) == true)
                {
                    COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];

                    if (pDBData.State_ != DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL])
                    {
                        if (LQCrawlerInfo_.ExData_CancelCheck_ == pData.Value.ExData_Cancel_)
                        {
                            pDBData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL];
                            DBProccess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                }
            }

            return true;
        }
        #endregion

        public abstract bool CheckNeedUseWeb();
        // 로그인 Web
        public abstract bool Web_Login();
        // 엑셀 다운로드
        public abstract bool Web_DownLoadExcel();
        // 취소된 거래인지 확인
        public abstract bool CheckIsCancel();
        // 엑셀 파싱
        //protected abstract bool Internal_Excel_Parsing(string filepath, Int32 GoodsAttrType, bool bFixedType, string goodsname);
        protected abstract bool Internal_Excel_Parsing(ChannelGoodInfo pChannelGoodInfo);
        // 취소 엑셀 파싱
        protected abstract bool Internal_ExcelCancel_Parsing(string filepath);
        // 웹에서 사용처리
        public abstract bool Web_Use();
        // 엑셀 데이터 리스트에 넣기
        protected abstract Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "");
        // 오픈 마켓들은 바로 사용처리를 해줬기 때문에 DB 에 AR, UR 이 있으면 그냥 A, U 로 변경한다.
        public abstract bool OpenMarketChangeState();
        // 취소 리스트 다운로드
        public abstract bool Web_DownLoad_CancelList();
    }
}
