using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HKLibrary.UTIL;
using HK.Database;
using MySql.Data.MySqlClient;
using LQStructures;
using CrawlerShare;
using CData;


public class DBInterface
{
    // 크롤링 정보 로드 하기
    public static bool GetCrawlerInfo(SqlHelper dbHelper, Int32 nChannelIdx, Int32 PartnerIdx, ref LQCrawlerInfo pLQCrawlerInfo)
    {
        bool bResult = false;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xChannelIdx", nChannelIdx.ToString());
            argdic.Add("xPartnerIdx", PartnerIdx.ToString());

            MySqlDataReader datareader = dbHelper.call_proc("sp_select_Crawler_Info", argdic);
            while (datareader.Read())
            {
                bResult = true;

                pLQCrawlerInfo = new LQCrawlerInfo();
                pLQCrawlerInfo.nIdx_ = Convert.ToInt32(datareader["idx"]);
                pLQCrawlerInfo.Channel_Idx_ = Convert.ToInt32(datareader["Channel_Idx"]);
                pLQCrawlerInfo.ChannelName_ = Convert.ToString(datareader["AuthorityName"]);
                pLQCrawlerInfo.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);
                pLQCrawlerInfo.PartnerName_ = Convert.ToString(datareader["PartnerName"]);

                pLQCrawlerInfo.MainUrl_ = Convert.ToString(datareader["MainUrl"]);         // 메인 URL
                pLQCrawlerInfo.LoginIDTAG_ = Convert.ToString(datareader["LoginIDTAG"]);
                pLQCrawlerInfo.LoginPWTAG_ = Convert.ToString(datareader["LoginPWTAG"]);
                pLQCrawlerInfo.LoginUrl_ = Convert.ToString(datareader["LoginUrl"]);
                pLQCrawlerInfo.LoginParam_ = Convert.ToString(datareader["LoginParam"]);      // 로그인 셋팅값
                pLQCrawlerInfo.LoginID_ = Convert.ToString(datareader["LoginID"]);         // 로그인 아이디
                pLQCrawlerInfo.LoginPW_ = Convert.ToString(datareader["LoginPW"]);         // 로그인 암호
                pLQCrawlerInfo.LoginMethod_ = Convert.ToString(datareader["LoginMethod"]);     // 로그인 방식
                pLQCrawlerInfo.LoginEvent_ = Convert.ToString(datareader["LoginEvent"]);      // 로그인 버튼 이벤트
                pLQCrawlerInfo.LoginCheck_ = Convert.ToString(datareader["LoginCheck"]);
                pLQCrawlerInfo.LoginType_ = Convert.ToChar(datareader["LoginType"]);
                pLQCrawlerInfo.ExcelDownUrl_ = Convert.ToString(datareader["ExcelDownUrl"]);    // 엑셀 다운로드 URL
                pLQCrawlerInfo.ExcelDownParameter_ = Convert.ToString(datareader["ExcelDownParameter"]);
                pLQCrawlerInfo.ExcelDownMethod_ = Convert.ToString(datareader["ExcelDownMethod"]);    // 엑셀 다운로드 방식                    
                pLQCrawlerInfo.ExcelDownRule_ = Convert.ToString(datareader["ExcelDownRule"]);

                pLQCrawlerInfo.UseGoodsUrl_ = Convert.ToString(datareader["UseGoodsUrl"]);
                pLQCrawlerInfo.UseGoodsParam_ = Convert.ToString(datareader["UseGoodsParam"]);
                pLQCrawlerInfo.UseGoodsCheck_ = Convert.ToString(datareader["UseGoodsCheck"]);
                pLQCrawlerInfo.UseGoodsRule_ = Convert.ToString(datareader["UseGoodsRule"]);

                pLQCrawlerInfo.UseUserUrl_ = Convert.ToString(datareader["UseUserUrl"]);
                pLQCrawlerInfo.UseUserParam_ = Convert.ToString(datareader["UseUserParam"]);
                pLQCrawlerInfo.UseUserCheck_ = Convert.ToString(datareader["UseUserCheck"]);

                pLQCrawlerInfo.NUseGoodsUrl_ = Convert.ToString(datareader["NUseGoodsUrl"]);
                pLQCrawlerInfo.NUseGoodsParam_ = Convert.ToString(datareader["NUseGoodsParam"]);
                pLQCrawlerInfo.NUseGoodsCheck_ = Convert.ToString(datareader["NUseGoodsCheck"]);
                pLQCrawlerInfo.NUseGoodsRule_ = Convert.ToString(datareader["NUseGoodsRule"]);

                pLQCrawlerInfo.NUseUserUrl_ = Convert.ToString(datareader["NUseUserUrl"]);
                pLQCrawlerInfo.NUseUserParam_ = Convert.ToString(datareader["NUseUserParam"]);
                pLQCrawlerInfo.NUseUserCheck_ = Convert.ToString(datareader["NUseUserCheck"]);

                pLQCrawlerInfo.RUseUserUrl_ = Convert.ToString(datareader["RUseUserUrl"]);
                pLQCrawlerInfo.RUseUserParam_ = Convert.ToString(datareader["RUseUserParam"]);
                pLQCrawlerInfo.RUseUserCheck_ = Convert.ToString(datareader["RUseUserCheck"]);

                pLQCrawlerInfo.ExData_Start_ = Convert.ToInt32(datareader["ExData_Start"]);
                pLQCrawlerInfo.ExData_Coupncode_ = Convert.ToInt32(datareader["ExData_Coupncode"]);
                pLQCrawlerInfo.ExData_Buydate_ = Convert.ToInt32(datareader["ExData_Buydate"]);
                pLQCrawlerInfo.ExData_Option_ = Convert.ToInt32(datareader["ExData_Option"]);
                pLQCrawlerInfo.ExData_Cancel_ = Convert.ToInt32(datareader["ExData_Cancel"]);
                pLQCrawlerInfo.ExData_Count_ = Convert.ToInt32(datareader["ExData_Count"]);
                pLQCrawlerInfo.ExData_Use_ = Convert.ToInt32(datareader["ExData_Use"]);
                pLQCrawlerInfo.ExData_Buyer_ = Convert.ToInt32(datareader["ExData_Buyer"]);
                pLQCrawlerInfo.ExData_Buyphone_ = Convert.ToInt32(datareader["ExData_Buyphone"]);
                pLQCrawlerInfo.ExData_Price_ = Convert.ToInt32(datareader["ExData_Price"]);
                pLQCrawlerInfo.ExData_UseCheck_ = Convert.ToString(datareader["ExData_UseCheck"]);
                pLQCrawlerInfo.ExData_CancelCheck_ = Convert.ToString(datareader["ExData_CancelCheck"]);

                break;
            }
            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool GetCrawlerInfo - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    // 크롤링 정보 로드 하기
    public static bool GetCrawlerInfoNew(SqlHelper dbHelper, Int32 nChannelIdx, Int32 PartnerIdx, Int32 AuthorityIdx, ref LQCrawlerInfo pLQCrawlerInfo)
    {
        bool bResult = false;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xChannelIdx", nChannelIdx.ToString());
            argdic.Add("xPartnerIdx", PartnerIdx.ToString());
            argdic.Add("xAuthorityIdx", AuthorityIdx.ToString());

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectCrawlerInfo", argdic);
            while (datareader.Read())
            {
                bResult = true;

                pLQCrawlerInfo = new LQCrawlerInfo();
                pLQCrawlerInfo.nIdx_ = Convert.ToInt32(datareader["idx"]);
                pLQCrawlerInfo.Channel_Idx_ = Convert.ToInt32(datareader["Channel_Idx"]);
                pLQCrawlerInfo.ChannelName_ = Convert.ToString(datareader["AuthorityName"]);
                pLQCrawlerInfo.AuthoritySeq_ = Convert.ToInt32(datareader["AuthoritySeq"]);
                pLQCrawlerInfo.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);
                pLQCrawlerInfo.PartnerName_ = Convert.ToString(datareader["PartnerName"]);

                pLQCrawlerInfo.MainUrl_ = Convert.ToString(datareader["MainUrl"]);         // 메인 URL
                pLQCrawlerInfo.LoginIDTAG_ = Convert.ToString(datareader["LoginIDTAG"]);
                pLQCrawlerInfo.LoginPWTAG_ = Convert.ToString(datareader["LoginPWTAG"]);
                pLQCrawlerInfo.LoginUrl_ = Convert.ToString(datareader["LoginUrl"]);
                pLQCrawlerInfo.LoginParam_ = Convert.ToString(datareader["LoginParam"]);      // 로그인 셋팅값
                pLQCrawlerInfo.LoginID_ = Convert.ToString(datareader["LoginID"]);         // 로그인 아이디
                pLQCrawlerInfo.LoginPW_ = Convert.ToString(datareader["LoginPW"]);         // 로그인 암호
                pLQCrawlerInfo.LoginMethod_ = Convert.ToString(datareader["LoginMethod"]);     // 로그인 방식
                pLQCrawlerInfo.LoginEvent_ = Convert.ToString(datareader["LoginEvent"]);      // 로그인 버튼 이벤트
                pLQCrawlerInfo.LoginCheck_ = Convert.ToString(datareader["LoginCheck"]);
                pLQCrawlerInfo.LoginType_ = Convert.ToChar(datareader["LoginType"]);
                pLQCrawlerInfo.ExcelDownUrl_ = Convert.ToString(datareader["ExcelDownUrl"]);    // 엑셀 다운로드 URL
                pLQCrawlerInfo.ExcelDownParameter_ = Convert.ToString(datareader["ExcelDownParameter"]);
                pLQCrawlerInfo.ExcelDownMethod_ = Convert.ToString(datareader["ExcelDownMethod"]);    // 엑셀 다운로드 방식                    
                pLQCrawlerInfo.ExcelDownRule_ = Convert.ToString(datareader["ExcelDownRule"]);

                pLQCrawlerInfo.UseGoodsUrl_ = Convert.ToString(datareader["UseGoodsUrl"]);
                pLQCrawlerInfo.UseGoodsParam_ = Convert.ToString(datareader["UseGoodsParam"]);
                pLQCrawlerInfo.UseGoodsCheck_ = Convert.ToString(datareader["UseGoodsCheck"]);
                pLQCrawlerInfo.UseGoodsRule_ = Convert.ToString(datareader["UseGoodsRule"]);

                pLQCrawlerInfo.UseUserUrl_ = Convert.ToString(datareader["UseUserUrl"]);
                pLQCrawlerInfo.UseUserParam_ = Convert.ToString(datareader["UseUserParam"]);
                pLQCrawlerInfo.UseUserCheck_ = Convert.ToString(datareader["UseUserCheck"]);

                pLQCrawlerInfo.NUseGoodsUrl_ = Convert.ToString(datareader["NUseGoodsUrl"]);
                pLQCrawlerInfo.NUseGoodsParam_ = Convert.ToString(datareader["NUseGoodsParam"]);
                pLQCrawlerInfo.NUseGoodsCheck_ = Convert.ToString(datareader["NUseGoodsCheck"]);
                pLQCrawlerInfo.NUseGoodsRule_ = Convert.ToString(datareader["NUseGoodsRule"]);

                pLQCrawlerInfo.NUseUserUrl_ = Convert.ToString(datareader["NUseUserUrl"]);
                pLQCrawlerInfo.NUseUserParam_ = Convert.ToString(datareader["NUseUserParam"]);
                pLQCrawlerInfo.NUseUserCheck_ = Convert.ToString(datareader["NUseUserCheck"]);

                pLQCrawlerInfo.RUseUserUrl_ = Convert.ToString(datareader["RUseUserUrl"]);
                pLQCrawlerInfo.RUseUserParam_ = Convert.ToString(datareader["RUseUserParam"]);
                pLQCrawlerInfo.RUseUserCheck_ = Convert.ToString(datareader["RUseUserCheck"]);

                pLQCrawlerInfo.ExData_Start_ = Convert.ToInt32(datareader["ExData_Start"]);
                pLQCrawlerInfo.ExData_Coupncode_ = Convert.ToInt32(datareader["ExData_Coupncode"]);
                pLQCrawlerInfo.ExData_Buydate_ = Convert.ToInt32(datareader["ExData_Buydate"]);
                pLQCrawlerInfo.ExData_Option_ = Convert.ToInt32(datareader["ExData_Option"]);
                pLQCrawlerInfo.ExData_Cancel_ = Convert.ToInt32(datareader["ExData_Cancel"]);
                pLQCrawlerInfo.ExData_Count_ = Convert.ToInt32(datareader["ExData_Count"]);
                pLQCrawlerInfo.ExData_Use_ = Convert.ToInt32(datareader["ExData_Use"]);
                pLQCrawlerInfo.ExData_Buyer_ = Convert.ToInt32(datareader["ExData_Buyer"]);
                pLQCrawlerInfo.ExData_Buyphone_ = Convert.ToInt32(datareader["ExData_Buyphone"]);
                pLQCrawlerInfo.ExData_Price_ = Convert.ToInt32(datareader["ExData_Price"]);
                pLQCrawlerInfo.ExData_UseCheck_ = Convert.ToString(datareader["ExData_UseCheck"]);
                pLQCrawlerInfo.ExData_CancelCheck_ = Convert.ToString(datareader["ExData_CancelCheck"]);
                pLQCrawlerInfo.ExData_GoodName_ = Convert.ToInt32(datareader["ExData_Goods"]);                

                break;
            }
            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool GetCrawlerInfoNew - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    // 각각의 상태 정보 로드
    public static bool SelectStateTable(SqlHelper dbHelper)
    {
        DealStateManager.Instance.Init();

        bool bResult = true;

        try
        {
            MySqlDataReader datareader = dbHelper.call_proc("sp_select_StateTable", null);

            while (datareader.Read())
            {
                Int32 nStateType = Convert.ToInt32(datareader["StateType"]);
                string strStateName = Convert.ToString(datareader["StateName"]);
                string strExplain = Convert.ToString(datareader["Explain"]);

                DealStateManager.Instance.Add(nStateType, strStateName, strExplain);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool SelectStateTable - {0}", ex.Message));
            bResult = false;
        }

        return bResult;

    }

    // DB 에서 상품 코드 읽어오기
    public static bool GetGoodsTableWithUID(SqlHelper dbHelper, Int32 nChannelIdx, Int32 nAuthorityIdx, Int32 nUID, ref Dictionary<Int32, ChannelGoodInfo> pInfoList)
    {
        pInfoList.Clear();

        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xChannelSeq", nChannelIdx.ToString());
            argdic.Add("xAuthoritySeq", nAuthorityIdx.ToString());
            argdic.Add("xCrawlerSeq", nUID.ToString());

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectGoodsInfo", argdic);
            string availableData = "";
            while (datareader.Read())
            {
                ChannelGoodInfo pGoodInfo = new ChannelGoodInfo();

                pGoodInfo.Idx_ = Convert.ToInt32(datareader["seq"]);
                pGoodInfo.Goods_Code_ = Convert.ToString(datareader["ChGoodsCode"]);
                pGoodInfo.GoodsName_ = Convert.ToString(datareader["GoodsName"]);
                pGoodInfo.GoodsNickName_ = Convert.ToString(datareader["GoodsNick"]);
                pGoodInfo.GoodsPassType_ = Convert.ToString(datareader["GoodsPassType"]);
                pGoodInfo.GoodsSendType_ = Convert.ToInt32(datareader["GoodsSendType"]); 
                pGoodInfo.sDate_ = Convert.ToString(datareader["GoodsSdate"]);
                pGoodInfo.eDateFormat_ = Convert.ToString(datareader["GoodsEdateFormat"]);
                availableData = Convert.ToString(datareader["AvailableDate"]);
                pGoodInfo.OptionName_ = Convert.ToString(datareader["GoodsOptionName"]);
                pGoodInfo.OptionNickName_ = Convert.ToString(datareader["GoodsNickName"]);
                pGoodInfo.GoodsAttrType_ = Convert.ToInt32(datareader["GoodsAttrType"]);

                if (string.IsNullOrEmpty(pGoodInfo.Goods_Code_) == true)
                {
                    string LogMessage = string.Format("bool GetGoodsTable 상품코드가 지정되어 있지 않아서 상품은 건너 뜁니다.{0}/{1}"
                        , pGoodInfo.Goods_Code_, pGoodInfo.GoodsName_);
                    NewLogManager2.Instance.Log(LogMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(pGoodInfo.OptionNickName_) == true)
                {
                    string LogMessage = string.Format("bool GetGoodsTable 상품 옵션명이 지정되지 않아서 이 상품은 건너 뜁니다.{0}/{1}"
                        , pGoodInfo.Goods_Code_, pGoodInfo.OptionNickName_);
                    NewLogManager2.Instance.Log(LogMessage);
                    continue;
                }

                if (string.IsNullOrEmpty(availableData) == false)
                {
                    if (Regex.IsMatch(availableData, @"^(19|20)\d{2}-(0[1-9]|1[012])-(0[1-9]|[12][0-9]|3[0-1])$") == true)
                    {
                        pGoodInfo.availableDateTime_ = Convert.ToDateTime(availableData);
                        pGoodInfo.Expired_ = true;
                    }
                }
                pInfoList.Add(pGoodInfo.Idx_, pGoodInfo);

            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool GetGoodsTableWithUID - {0}", ex.Message));
            bResult = false;
        }

        return bResult;

    }

    public static bool Insert_tblOrder(SqlHelper dbHelper, Int32 GoodsSeq, Int32 channelSeq, string channelCode, float OrderPrice
        , Int32 OrderCount, string UserId, string UserName, string UserPhone, string State, string goodsNickName, string goodsOrgName
        , string RegDate, ref Int32 OrderSeq, ref string pin_m_num)
    {
        bool bResult = true;
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xchannelSeq", channelSeq);
            argdic.Add("xchannelCode", channelCode);
            argdic.Add("xOrderPrice", OrderPrice);
            argdic.Add("xOrderCount", OrderCount);
            argdic.Add("xUserId", UserId);
            argdic.Add("xUserName", UserName);
            argdic.Add("xUserPhone", UserPhone);
            argdic.Add("xState", State);
            argdic.Add("xGoodsSeq", GoodsSeq);
            argdic.Add("xgoodsNickName", goodsNickName);
            argdic.Add("xgoodsOrgName", goodsOrgName);
            argdic.Add("xRegDate", RegDate);

            MySqlDataReader datareader = dbHelper.call_proc("spNewInsert_chOrder_temp", argdic);

            string strSeq = "";
            while (datareader.Read())
            {
                strSeq = Convert.ToString(datareader["OrderSeq"]); 
                OrderSeq = Convert.ToInt32(strSeq);
                pin_m_num = Convert.ToString(datareader["pin_m_num"]);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Insert_tblOrder - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }


    public static bool Insert_tblOrder_test(SqlHelper dbHelper, Int32 GoodsSeq, Int32 channelSeq, string channelCode, float OrderPrice
        , Int32 OrderCount, string UserId, string UserName, string UserPhone, string State, string goodsNickName, string goodsOrgName
        , string RegDate, ref Int32 OrderSeq, ref string pin_m_num)
    {
        bool bResult = true;
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xchannelSeq", channelSeq);
            argdic.Add("xchannelCode", channelCode);
            argdic.Add("xOrderPrice", OrderPrice);
            argdic.Add("xOrderCount", OrderCount);
            argdic.Add("xUserId", UserId);
            argdic.Add("xUserName", UserName);
            argdic.Add("xUserPhone", UserPhone);
            argdic.Add("xState", State);
            argdic.Add("xGoodsSeq", GoodsSeq);
            argdic.Add("xgoodsNickName", goodsNickName);
            argdic.Add("xgoodsOrgName", goodsOrgName);
            argdic.Add("xRegDate", RegDate);

            MySqlDataReader datareader = dbHelper.call_proc("spNewInsert_chOrder_Free", argdic);

            string strSeq = "";
            while (datareader.Read())
            {
                strSeq = Convert.ToString(datareader["OrderSeq"]);
                OrderSeq = Convert.ToInt32(strSeq);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Insert_tblOrder - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    // 사용 처리 할때 호출 하는 프로시저
    public static bool Update_OrderInfo(SqlHelper dbHelper, Int32 xOrderSeq, string xState)
    {
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xOrderSeq", xOrderSeq);
            argdic.Add("xState", xState);

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateOrderInfo", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Update_OrderInfo - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    // 사용처리 원래대로 되돌리는 프로시저
    public static bool Update_OrderInfo_FixUP(SqlHelper dbHelper, Int32 xOrderSeq, string xState)
    {
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xOrderSeq", xOrderSeq);
            argdic.Add("xState", xState);

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateSyncOrderInfo", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Update_OrderInfo_FixUP - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    // 취소처리 상태 프로시저
    public static bool Update_OrderInfo_Cancel(SqlHelper dbHelper, Int32 xOrderSeq, string xState, string xChannelCode)
    {
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xOrderSeq", xOrderSeq);
            argdic.Add("xState", xState);
            argdic.Add("xOrderChannelCode", xChannelCode);

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateCancelOrderInfo", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Update_OrderInfo_Cancel - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    public static bool Insert_SMS(SqlHelper dbHelper, Int32 xSOrderSeq, Int32 xEOrderSeq)
    {
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xSOrderSeq", xSOrderSeq);
            argdic.Add("xEOrderSeq", xEOrderSeq);

            //MySqlDataReader datareader = dbHelper.call_proc("sp_insert_BARCODE", argdic);
            MySqlDataReader datareader = dbHelper.call_proc("sp_insert_Sms", argdic);
            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Insert_SMS - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    public static bool Insert_tblOrderWr(SqlHelper dbHelper, Int32 GoodsSeq, Int32 channelSeq, string channelCode, float OrderPrice
        , Int32 OrderCount, string UserId, string UserName, string UserPhone, string State, string goodsName, string goodsNick, string goodsNickName, string goodsOrgName
        , string RegDate, ref Int32 OrderSeq)
    {
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xchannelSeq", channelSeq);
            argdic.Add("xchannelCode", channelCode);
            argdic.Add("xOrderPrice", OrderPrice);
            argdic.Add("xOrderCount", OrderCount);
            argdic.Add("xUserId", UserId);
            argdic.Add("xUserName", UserName);
            argdic.Add("xUserPhone", UserPhone);
            argdic.Add("xState", State);
            argdic.Add("xGoodsSeq", GoodsSeq);
            argdic.Add("xgoodsName", goodsName);
            argdic.Add("xgoodsNick", goodsNick);
            argdic.Add("xgoodsNickName", goodsNickName);
            argdic.Add("xgoodsOrgName", goodsOrgName);
            argdic.Add("xRegDate", RegDate);

            MySqlDataReader datareader = dbHelper.call_proc("spNewInsert_OrderWr", argdic);

            string strSeq = "";
            while (datareader.Read())
            {
                strSeq = Convert.ToString(datareader["OrderSeq"]);
                OrderSeq = Convert.ToInt32(strSeq);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Insert_tblOrderWr - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    public static bool Select_tblOrder_With_UID(SqlHelper dbHelper, Int32 xchannelSeq, Int32 xGoodsSeq
       , /*string sDate, string eDate,*/ref Dictionary<string, COrderData> pList)
    {
        
        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xChannelSeq", xchannelSeq);
            //argdic.Add("xAuthorityLoginSeq", xGoodsSeq);
            argdic.Add("xGoodsSeq", xGoodsSeq);
            
            //argdic.Add("xSDate", sDate);
            //argdic.Add("xEDate", eDate);

            //\MySqlDataReader datareader = dbHelper.call_proc("spNewTestSelectOrderInfo", argdic);
            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectOrderInfo", argdic);

            while (datareader.Read())
            {
                COrderData pOrderData = new COrderData();
                pOrderData.seq_ = Convert.ToInt64(datareader["seq"]);
                pOrderData.goodsSeq_ = Convert.ToInt32(datareader["goodsSeq"]);
                pOrderData.memberSeq_ = Convert.ToInt32(datareader["memberSeq"]);
                pOrderData.channelSeq_ = Convert.ToInt32(datareader["channelSeq"]);

                pOrderData.channelOrderCode_ = Convert.ToString(datareader["channelOrderCode"]);
                pOrderData.orderReserveCode_ = Convert.ToString(datareader["orderCode"]);
                pOrderData.orderID_          = Convert.ToString(datareader["orderId"]);
                pOrderData.goodsCode_        = Convert.ToString(datareader["ChGoodsCode"]);

                pOrderData.orderSettlePrice_ = Convert.ToInt32(datareader["orderSettlePrice"]);
                pOrderData.orderName_        = Convert.ToString(datareader["orderName"]);
                pOrderData.orderPhone_       = Convert.ToString(datareader["orderPhone"]);
                //pOrderData.orderMethod_ = Convert.ToString(datareader["orderMethod"]);
                pOrderData.State_            = Convert.ToString(datareader["State"]);


                if (pList.ContainsKey(pOrderData.channelOrderCode_) == false)
                {
                    pList.Add(pOrderData.channelOrderCode_, pOrderData);
                }
                else
                {
                    NewLogManager2.Instance.Log(string.Format("Error Select_tblOrder_With_UID Same OrderCode {0}", pOrderData.channelOrderCode_));
                }
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Select_tblOrder_With_UID - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }


    public static bool Insert_Order_Channel(SqlHelper dbHelper, Int32 GoodsSeq, Int32 channelSeq, string channelCode, float OrderPrice
       , string UserName, string UserPhone, string State, string RegDate, ref Int32 OrderSeq, ref string pin_m_num)
    {
        bool bResult = true;
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xchannelSeq",  channelSeq);
            argdic.Add("xchannelCode", channelCode);
            argdic.Add("xOrderPrice",  OrderPrice);
            argdic.Add("xUserName",    UserName);
            argdic.Add("xUserPhone",   UserPhone);
            argdic.Add("xState",       State);
            argdic.Add("xGoodsSeq",    GoodsSeq);
            argdic.Add("xRegDate",     RegDate);

            MySqlDataReader datareader = dbHelper.call_proc("SP_INSERT_ORDER", argdic);

            string strSeq = "";
            while (datareader.Read())
            {
                strSeq = Convert.ToString(datareader["OrderSeq"]);
                OrderSeq = Convert.ToInt32(strSeq);
                pin_m_num = Convert.ToString(datareader["pin_m_num"]);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Insert_tblOrder - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }

    public static bool Select_Order_List(SqlHelper dbHelper, Int32 xchannelSeq, Int32 xAuthoritySeq, Int32 xCrawlerSeq
       ,ref Dictionary<string, COrderData> pList)
    {

        bool bResult = true;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xChannelSeq", xchannelSeq);
            argdic.Add("xAuthorityLoginSeq", xAuthoritySeq);
            argdic.Add("xCrawlerSeq", xCrawlerSeq);

            MySqlDataReader datareader = dbHelper.call_proc("SP_SELECT_ORDER", argdic);

            while (datareader.Read())
            {
                COrderData pOrderData = new COrderData();
                pOrderData.seq_ = Convert.ToInt64(datareader["seq"]);
                pOrderData.goodsSeq_ = Convert.ToInt32(datareader["goodsSeq"]);
                pOrderData.memberSeq_ = Convert.ToInt32(datareader["memberSeq"]);
                pOrderData.channelSeq_ = Convert.ToInt32(datareader["channelSeq"]);

                pOrderData.channelOrderCode_ = Convert.ToString(datareader["channelOrderCode"]);
                pOrderData.orderReserveCode_ = Convert.ToString(datareader["orderCode"]);
                pOrderData.orderID_ = Convert.ToString(datareader["orderId"]);
                pOrderData.goodsCode_ = Convert.ToString(datareader["ChGoodsCode"]);

                pOrderData.orderSettlePrice_ = Convert.ToInt32(datareader["orderSettlePrice"]);
                pOrderData.orderName_ = Convert.ToString(datareader["orderName"]);
                pOrderData.orderPhone_ = Convert.ToString(datareader["orderPhone"]);
                //pOrderData.orderMethod_ = Convert.ToString(datareader["orderMethod"]);
                pOrderData.State_ = Convert.ToString(datareader["State"]);


                if (pList.ContainsKey(pOrderData.channelOrderCode_) == false)
                {
                    pList.Add(pOrderData.channelOrderCode_, pOrderData);
                }
                else
                {
                    NewLogManager2.Instance.Log(string.Format("Error Select_tblOrder_With_UID Same OrderCode {0}", pOrderData.channelOrderCode_));
                }
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            NewLogManager2.Instance.Log(string.Format("Error bool Select_tblOrder_With_UID - {0}", ex.Message));
            bResult = false;
        }

        return bResult;
    }
}
