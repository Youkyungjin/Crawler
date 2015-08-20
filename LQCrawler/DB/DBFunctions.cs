//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.RegularExpressions;

//using HK.Database;
//using MySql.Data.MySqlClient;
//using LQCrawler.Data;
//using LQStructures;
//using CrawlerShare;

//namespace LQCrawler.DB
//{
//    public class DBFunctions
//    {
//        // 크롤링 정보 로드 하기
//        public static bool GetCrawlerInfo(SqlHelper dbHelper, Int32 nChannelIdx, Int32 PartnerIdx)
//        {
//            bool bResult = false;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xChannelIdx", nChannelIdx.ToString());
//                argdic.Add("xPartnerIdx", PartnerIdx.ToString());

//                MySqlDataReader datareader = dbHelper.call_proc("sp_select_Crawler_Info", argdic);
//                while (datareader.Read())
//                {
//                    bResult = true;

//                    LQCrawlerInfo pInfo = new LQCrawlerInfo();
//                    pInfo.nIdx_ = Convert.ToInt32(datareader["idx"]);
//                    pInfo.Channel_Idx_ = Convert.ToInt32(datareader["Channel_Idx"]);
//                    pInfo.ChannelName_ = Convert.ToString(datareader["Channel_Name"]);
//                    pInfo.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);
//                    pInfo.PartnerName_ = Convert.ToString(datareader["PartnerName"]);

//                    pInfo.MainUrl_ = Convert.ToString(datareader["MainUrl"]);         // 메인 URL
//                    pInfo.LoginIDTAG_ = Convert.ToString(datareader["LoginIDTAG"]);
//                    pInfo.LoginPWTAG_ = Convert.ToString(datareader["LoginPWTAG"]);
//                    pInfo.LoginUrl_ = Convert.ToString(datareader["LoginUrl"]);
//                    pInfo.LoginParam_ = Convert.ToString(datareader["LoginParam"]);      // 로그인 셋팅값
//                    pInfo.LoginID_ = Convert.ToString(datareader["LoginID"]);         // 로그인 아이디
//                    pInfo.LoginPW_ = Convert.ToString(datareader["LoginPW"]);         // 로그인 암호
//                    pInfo.LoginMethod_ = Convert.ToString(datareader["LoginMethod"]);     // 로그인 방식
//                    pInfo.LoginEvent_ = Convert.ToString(datareader["LoginEvent"]);      // 로그인 버튼 이벤트
//                    pInfo.LoginCheck_ = Convert.ToString(datareader["LoginCheck"]);
//                    pInfo.LoginType_ = Convert.ToChar(datareader["LoginType"]);
//                    pInfo.ExcelDownUrl_ = Convert.ToString(datareader["ExcelDownUrl"]);    // 엑셀 다운로드 URL
//                    pInfo.ExcelDownParameter_ = Convert.ToString(datareader["ExcelDownParameter"]);
//                    pInfo.ExcelDownMethod_ = Convert.ToString(datareader["ExcelDownMethod"]);    // 엑셀 다운로드 방식                    
//                    pInfo.ExcelDownRule_ = Convert.ToString(datareader["ExcelDownRule"]);

//                    pInfo.UseGoodsUrl_ = Convert.ToString(datareader["UseGoodsUrl"]);
//                    pInfo.UseGoodsParam_ = Convert.ToString(datareader["UseGoodsParam"]);
//                    pInfo.UseGoodsCheck_ = Convert.ToString(datareader["UseGoodsCheck"]);
//                    pInfo.UseGoodsRule_ = Convert.ToString(datareader["UseGoodsRule"]);

//                    pInfo.UseUserUrl_ = Convert.ToString(datareader["UseUserUrl"]);
//                    pInfo.UseUserParam_ = Convert.ToString(datareader["UseUserParam"]);
//                    pInfo.UseUserCheck_ = Convert.ToString(datareader["UseUserCheck"]);

//                    pInfo.NUseGoodsUrl_ = Convert.ToString(datareader["NUseGoodsUrl"]);
//                    pInfo.NUseGoodsParam_ = Convert.ToString(datareader["NUseGoodsParam"]);
//                    pInfo.NUseGoodsCheck_ = Convert.ToString(datareader["NUseGoodsCheck"]);
//                    pInfo.NUseGoodsRule_ = Convert.ToString(datareader["NUseGoodsRule"]);

//                    pInfo.NUseUserUrl_ = Convert.ToString(datareader["NUseUserUrl"]);
//                    pInfo.NUseUserParam_ = Convert.ToString(datareader["NUseUserParam"]);
//                    pInfo.NUseUserCheck_ = Convert.ToString(datareader["NUseUserCheck"]);

//                    pInfo.RUseUserUrl_ = Convert.ToString(datareader["RUseUserUrl"]);
//                    pInfo.RUseUserParam_ = Convert.ToString(datareader["RUseUserParam"]);
//                    pInfo.RUseUserCheck_ = Convert.ToString(datareader["RUseUserCheck"]);

//                    pInfo.ExData_Start_ = Convert.ToInt32(datareader["ExData_Start"]);
//                    pInfo.ExData_Coupncode_ = Convert.ToInt32(datareader["ExData_Coupncode"]);
//                    pInfo.ExData_Buydate_ = Convert.ToInt32(datareader["ExData_Buydate"]);
//                    pInfo.ExData_Option_ = Convert.ToInt32(datareader["ExData_Option"]);
//                    pInfo.ExData_Cancel_ = Convert.ToInt32(datareader["ExData_Cancel"]);
//                    pInfo.ExData_Use_ = Convert.ToInt32(datareader["ExData_Use"]);
//                    pInfo.ExData_Buyer_ = Convert.ToInt32(datareader["ExData_Buyer"]);
//                    pInfo.ExData_Buyphone_ = Convert.ToInt32(datareader["ExData_Buyphone"]);
//                    pInfo.ExData_Price_ = Convert.ToInt32(datareader["ExData_Price"]);
//                    pInfo.ExData_UseCheck_ = Convert.ToString(datareader["ExData_UseCheck"]);
//                    pInfo.ExData_CancelCheck_ = Convert.ToString(datareader["ExData_CancelCheck"]);

//                    AppManager.Instance.SetCrawlerInfo(pInfo);
//                    break;
//                }
//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;

//        }

//        // DB 에서 상품 코드 읽어오기
//        public static bool GetGoodsTable(SqlHelper dbHelper, Int32 nChannelIdx)
//        {
//            Dictionary<Int32, ChannelGoodInfo> pInfoList = AppManager.Instance.GetGoodsInfo();
//            pInfoList.Clear();

//            bool bResult = true;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xChannelIdx", nChannelIdx.ToString());

//                // 2014.06.05
//                MySqlDataReader datareader = dbHelper.call_proc("sp_select_GoodsInfo", argdic);
//                string availableData = "";
//                while (datareader.Read())
//                {
//                    ChannelGoodInfo pGoodInfo = new ChannelGoodInfo();

//                    pGoodInfo.Idx_ = Convert.ToInt32(datareader["seq"]);
//                    pGoodInfo.Goods_Code_ = Convert.ToString(datareader["ChGoodsCode"]);
//                    pGoodInfo.GoodsName_ = Convert.ToString(datareader["GoodsName"]);
//                    pGoodInfo.sDate_ = Convert.ToString(datareader["GoodsSdate"]);
//                    pGoodInfo.eDateFormat_ = Convert.ToString(datareader["GoodsEdateFormat"]);
//                    availableData = Convert.ToString(datareader["AvailableDate"]);
//                    pGoodInfo.OptionName_ = Convert.ToString(datareader["GoodsOptionName"]);
//                    pGoodInfo.OptionNickName_ = Convert.ToString(datareader["GoodsNickName"]);
//                    pGoodInfo.GoodsAttrType_ = Convert.ToInt32(datareader["GoodsAttrType"]);

//                    if (string.IsNullOrEmpty(pGoodInfo.Goods_Code_) == true)
//                    {
//                        string LogMessage = string.Format("bool GetGoodsTable 상품코드가 지정되어 있지 않아서 상품은 건너 뜁니다.{0}/{1}"
//                            , pGoodInfo.Goods_Code_, pGoodInfo.GoodsName_);
//                        LogManager.Instance.Log(LogMessage);
//                        continue;
//                    }

//                    if (string.IsNullOrEmpty(pGoodInfo.OptionNickName_) == true)
//                    {
//                        string LogMessage = string.Format("bool GetGoodsTable 상품 옵션명이 지정되지 않아서 이 상품은 건너 뜁니다.{0}/{1}"
//                            , pGoodInfo.Goods_Code_, pGoodInfo.OptionNickName_);
//                        LogManager.Instance.Log(LogMessage);
//                        continue;
//                    }

//                    if (string.IsNullOrEmpty(availableData) == false)
//                    {
//                        if (Regex.IsMatch(availableData, @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$") == true)
//                        {
//                            pGoodInfo.availableDateTime_ = Convert.ToDateTime(availableData);
//                            pGoodInfo.Expired_ = true;
//                        }
//                    }
//                    pInfoList.Add(pGoodInfo.Idx_, pGoodInfo);

//                }
//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;

//        }

//        public static bool Insert_tblOrder(SqlHelper dbHelper, Int32 GoodsSeq, Int32 channelSeq, string channelCode, float OrderPrice
//            , Int32 OrderCount, string UserId, string UserName, string UserPhone, string State, string goodsNickName, string goodsOrgName
//            , string RegDate, ref Int32 OrderSeq)
//        {
//            bool bResult = true;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xchannelSeq", channelSeq);
//                argdic.Add("xchannelCode", channelCode);
//                argdic.Add("xOrderPrice", OrderPrice);
//                argdic.Add("xOrderCount", OrderCount);
//                argdic.Add("xUserId", UserId);
//                argdic.Add("xUserName", UserName);
//                argdic.Add("xUserPhone", UserPhone);
//                argdic.Add("xState", State);
//                argdic.Add("xGoodsSeq", GoodsSeq);
//                argdic.Add("xgoodsNickName", goodsNickName);
//                argdic.Add("xgoodsOrgName", goodsOrgName);
//                argdic.Add("xRegDate", RegDate);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_insert_chOrder", argdic);

//                string strSeq = "";
//                while (datareader.Read())
//                {
//                    strSeq = Convert.ToString(datareader["OrderSeq"]);
//                    OrderSeq = Convert.ToInt32(strSeq);
//                }

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        public static bool Insert_SMS(SqlHelper dbHelper, Int32 xSOrderSeq, Int32 xEOrderSeq)
//        {
//            bool bResult = true;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xSOrderSeq", xSOrderSeq);
//                argdic.Add("xEOrderSeq", xEOrderSeq);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_insert_Sms", argdic);

//                //string strSeq = "";
//                //while (datareader.Read())
//                //{
//                //    strSeq = Convert.ToString(datareader["OrderSeq"]);
//                //}

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        public static bool Update_OrderInfo(SqlHelper dbHelper, Int32 xOrderSeq, string xState)
//        {
//            bool bResult = true;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xOrderSeq", xOrderSeq);
//                argdic.Add("xState", xState);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_update_OrderInfo", argdic);

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        // DB에서 사용처리 해야하는 테이블 로드
//        public static bool Select_tblOrder(SqlHelper dbHelper, Int32 xchannelSeq)
//        {
//            bool bResult = true;

//            try
//            {
//                // 2014.06.05
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xChannelIdx", xchannelSeq);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_select_OrderInfo", argdic);

//                while (datareader.Read())
//                {
//                    tblOrderData pOrderData = new tblOrderData();
//                    pOrderData.NeedDBProc_ = tblOrderData.NeedDBProc.None;
//                    pOrderData.seq_ = Convert.ToInt64(datareader["seq"]);
//                    pOrderData.goodsSeq_ = Convert.ToInt32(datareader["goodsSeq"]);
//                    pOrderData.memberSeq_ = Convert.ToInt32(datareader["memberSeq"]);
//                    pOrderData.channelSeq_ = Convert.ToInt32(datareader["channelSeq"]);

//                    pOrderData.channelOrderCode_ = Convert.ToString(datareader["channelOrderCode"]);
//                    pOrderData.orderReserveCode_ = Convert.ToString(datareader["orderCode"]);
//                    pOrderData.orderID_ = Convert.ToString(datareader["orderId"]);

//                    pOrderData.orderSettlePrice_ = Convert.ToInt32(datareader["orderSettlePrice"]);
//                    pOrderData.orderName_ = Convert.ToString(datareader["orderName"]);
//                    pOrderData.orderPhone_ = Convert.ToString(datareader["orderPhone"]);
//                    //pOrderData.orderMethod_ = Convert.ToString(datareader["orderMethod"]);
//                    pOrderData.State_ = Convert.ToString(datareader["State"]);

//                    OrderManager.Instance.AddOrderData(pOrderData);
//                }

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        public static bool Select_tblOrderWr(SqlHelper dbHelper, Int32 xchannelSeq)
//        {
//            bool bResult = true;

//            try
//            {
//                // 2014.06.05
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xChannelIdx", xchannelSeq);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_select_OrderWrInfo", argdic);

//                while (datareader.Read())
//                {
//                    tblOrderData pOrderData = new tblOrderData();
//                    pOrderData.channelOrderCode_ = Convert.ToString(datareader["channelOrderCode"]);
//                    pOrderData.NeedDBProc_ = tblOrderData.NeedDBProc.None;
//                    OrderManager.Instance.AddWrongData(pOrderData);
//                }

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        // 쿠폰 정보 입력.
//        public static bool Insert_tblWrongOrder(SqlHelper dbHelper, Int32 xchannelSeq, string xchannelOrderCode, float xorderSettlePrice
//            , string xorderName, string xorderOptionName, Int32 xorderCount, string xorderPhone, string xstate)
//        {
//            bool bResult = true;

//            try
//            {
//                Dictionary<string, object> argdic = new Dictionary<string, object>();
//                argdic.Add("xchannelSeq", xchannelSeq);
//                argdic.Add("xchannelOrderCode", xchannelOrderCode);
//                argdic.Add("xorderSettlePrice", xorderSettlePrice);
//                argdic.Add("xorderName", xorderName);
//                argdic.Add("xorderOptionName", xorderOptionName);
//                argdic.Add("xorderPhone", xorderPhone);
//                argdic.Add("xorderCount", xorderCount);
//                argdic.Add("xstate", xstate);

//                MySqlDataReader datareader = dbHelper.call_proc("sp_insert_tblWrongOrder", argdic);
//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;
//        }

//        // 각각의 상태 정보 로드
//        public static bool SelectStateTable(SqlHelper dbHelper)
//        {
//            DealStateManager.Instance.Init();

//            bool bResult = true;

//            try
//            {
//                MySqlDataReader datareader = dbHelper.call_proc("sp_select_StateTable", null);

//                while (datareader.Read())
//                {
//                    Int32 nStateType = Convert.ToInt32(datareader["StateType"]);
//                    string strStateName = Convert.ToString(datareader["StateName"]);
//                    string strExplain = Convert.ToString(datareader["Explain"]);

//                    DealStateManager.Instance.Add(nStateType, strStateName, strExplain);
//                }

//                datareader.Close();
//                datareader.Dispose();
//                datareader = null;
//            }
//            catch (System.Exception ex)
//            {
//                LogManager.Instance.Log(ex.Message);
//                bResult = false;
//            }

//            return bResult;

//        }
//    }
//}
