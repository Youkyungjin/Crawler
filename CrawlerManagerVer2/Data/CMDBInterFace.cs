using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HK.Database;
using MySql.Data.MySqlClient;


public class CMDBInterFace
{
    // 매니저 로그인
    public static bool LoginManager(SqlHelper dbHelper, string id, string pw)
    {
        Int32 SelectCount = 0;
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xAdminId", id);
            argdic.Add("xAdminPw", pw);

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectAdminLogin", argdic);
            while (datareader.Read())
            {
                CInfoManager.Instance.AdminInfoData_.Seq_ = Convert.ToInt32(datareader["Seq"]);
                CInfoManager.Instance.AdminInfoData_.adminName_ = Convert.ToString(datareader["adminName"]);
                CInfoManager.Instance.AdminInfoData_.Mobile_ = Convert.ToString(datareader["Mobile"]);

                SelectCount++;
                break;
            }
            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        if (SelectCount == 0)
            return false;

        return true;
    }

    // 크롤링 정보 로드
    public static bool GetAllCrawlerMonitor(SqlHelper dbHelper)
    {
        try
        {
            CInfoManager.Instance.InitList();
            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectCrawlerMonitor", null);

            while (datareader.Read())
            {
                CrawlerData pCrawlerData = new CrawlerData();

                if (datareader["seq"] != null)
                    pCrawlerData.seq_ = Convert.ToInt32(datareader["seq"]);

                if (datareader["CrawlerName"] != null && datareader["CrawlerName"] != DBNull.Value)
                    pCrawlerData.CrawlerName_ = Convert.ToString(datareader["CrawlerName"]);

                if (datareader["ChannelSeq"] != null && datareader["ChannelSeq"] != DBNull.Value)
                    pCrawlerData.ChannelSeq_ = Convert.ToInt32(datareader["ChannelSeq"]);

                if (datareader["ChannelName"] != null && datareader["ChannelName"] != DBNull.Value)
                    pCrawlerData.ChannelName_ = Convert.ToString(datareader["ChannelName"]);

                if (datareader["AuthoritySeq"] != null && datareader["AuthoritySeq"] != DBNull.Value)
                    pCrawlerData.AuthoritySeq_ = Convert.ToInt32(datareader["AuthoritySeq"]);

                if (datareader["AuthorityName"] != null && datareader["AuthorityName"] != DBNull.Value)
                    pCrawlerData.AuthorityName_ = Convert.ToString(datareader["AuthorityName"]);

                if (datareader["AuthorityLoginSeq"] != null && datareader["AuthorityLoginSeq"] != DBNull.Value)
                    pCrawlerData.AuthorityLoginSeq_ = Convert.ToInt32(datareader["AuthorityLoginSeq"]);

                if (datareader["AuthorityLoginName"] != null && datareader["AuthorityLoginName"] != DBNull.Value)
                    pCrawlerData.AuthorityLoginName_ = Convert.ToString(datareader["AuthorityLoginName"]);

                if (datareader["CrawlerSeq"] != null && datareader["CrawlerSeq"] != DBNull.Value)
                    pCrawlerData.CrawlerSeq_ = Convert.ToInt32(datareader["CrawlerSeq"]);

                if (datareader["Mode"] != null && datareader["Mode"] != DBNull.Value)
                    pCrawlerData.Mode_ = Convert.ToInt32(datareader["Mode"]);

                if (datareader["Memo"] != null && datareader["Memo"] != DBNull.Value)
                    pCrawlerData.Memo_ = Convert.ToString(datareader["Memo"]);

                if (datareader["IP"] != null && datareader["IP"] != DBNull.Value)
                    pCrawlerData.IP_ = Convert.ToString(datareader["IP"]);

                if (datareader["CrawlerState"] != null && datareader["CrawlerState"] != DBNull.Value)
                    pCrawlerData.CrawlerState_ = Convert.ToString(datareader["CrawlerState"]);

                if (datareader["UpdateDate"] != null && datareader["UpdateDate"] != DBNull.Value)
                    pCrawlerData.UpdateDate_ = Convert.ToString(datareader["UpdateDate"]);

                CInfoManager.Instance.GetList().Add(pCrawlerData.seq_, pCrawlerData);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 상품 정보 로드하기
    public static bool GetGoodsTable(SqlHelper dbHelper)
    {
        try
        {
            GoodsManager.Instance.InitList();

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectGoods", null);

            while (datareader.Read())
            {
                CGoodsData pCGoodsData = new CGoodsData();
                if (datareader["seq"] != DBNull.Value)
                    pCGoodsData.Seq_ = Convert.ToInt32(datareader["seq"]);

                if (datareader["GoodsCode"] != DBNull.Value)
                    pCGoodsData.Goods_Code_ = Convert.ToString(datareader["GoodsCode"]);

                if (datareader["GoodsName"] != DBNull.Value)
                    pCGoodsData.GoodsName_ = Convert.ToString(datareader["GoodsName"]);

                if (datareader["GoodsNick"] != DBNull.Value)
                    pCGoodsData.GoodsNickName_ = Convert.ToString(datareader["GoodsNick"]);

                if (datareader["GoodsOptionName"] != DBNull.Value)
                    pCGoodsData.OptionName_ = Convert.ToString(datareader["GoodsOptionName"]);

                if (datareader["GoodsNickName"] != DBNull.Value)
                    pCGoodsData.OptionNickName_ = Convert.ToString(datareader["GoodsNickName"]);

                if (datareader["ChannelSeq"] != DBNull.Value)
                    pCGoodsData.ChannelSeq_ = Convert.ToInt32(datareader["ChannelSeq"]);

                if (datareader["AuthoritySeq"] != DBNull.Value)
                    pCGoodsData.AuthoritySeq_ = Convert.ToInt32(datareader["AuthoritySeq"]);

                if (datareader["AuthorityLoginSeq"] != DBNull.Value)
                    pCGoodsData.AuthorityLoginSeq_ = Convert.ToInt32(datareader["AuthorityLoginSeq"]);

                if (datareader["CrawlerSeq"] != DBNull.Value)
                    pCGoodsData.CrawlerSeq_ = Convert.ToInt32(datareader["CrawlerSeq"]);

                if (datareader["State"] != DBNull.Value)
                    pCGoodsData.State_ = Convert.ToString(datareader["State"]);

                GoodsManager.Instance.GetList().Add(pCGoodsData.Seq_, pCGoodsData);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 권리사 정보 로드
    public static bool GetAuthorityList(SqlHelper dbHelper)
    {
        try
        {
            AuthorityManager.Instance.InitList();

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectAuthority", null);

            Int32 ComboBoxIndex = 1;
            while (datareader.Read())
            {
                AuthorityInfoData pAuthorityInfoData = new AuthorityInfoData();

                if (datareader["seq"] != DBNull.Value)
                    pAuthorityInfoData.seq_ = Convert.ToInt32(datareader["seq"]);

                if (datareader["partnerName"] != DBNull.Value)
                    pAuthorityInfoData.partnerName_ = Convert.ToString(datareader["partnerName"]);

                //if (datareader["ChannelSeq"] != DBNull.Value)
                //    pAuthorityInfoData.ChannelSeq_ = Convert.ToInt32(datareader["ChannelSeq"]);

                //if (datareader["ChannelName"] != DBNull.Value)
                //    pAuthorityInfoData.ChannelName_ = Convert.ToString(datareader["ChannelName"]);

                //if (datareader["PartnerSeq"] != DBNull.Value)
                //    pAuthorityInfoData.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);

                //if (datareader["AuthorityName"] != DBNull.Value)
                //    pAuthorityInfoData.AuthorityName_ = Convert.ToString(datareader["AuthorityName"]);

                //if (datareader["Name"] != DBNull.Value)
                //    pAuthorityInfoData.AuthorityName_Identity_ = Convert.ToString(datareader["Name"]);

                //if (datareader["ID"] != DBNull.Value)
                //    pAuthorityInfoData.ID_ = Convert.ToString(datareader["ID"]);

                pAuthorityInfoData.ComboIndex_ = ComboBoxIndex++;

                AuthorityManager.Instance.GetList().Add(pAuthorityInfoData.seq_, pAuthorityInfoData);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 채널 정보 로드
    public static bool GetChannelList(SqlHelper dbHelper)
    {
        try
        {
            ChannelManager.Instance.InitList();

            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectChannel", null);

            Int32 ComboIndex = 1;
            while (datareader.Read())
            {
                ChannelInfoData pChannelInfoData = new ChannelInfoData();

                if (datareader["seq"] != DBNull.Value)
                    pChannelInfoData.seq_ = Convert.ToInt32(datareader["seq"]);

                if (datareader["ChannelCode"] != DBNull.Value)
                    pChannelInfoData.ChannelCode_ = Convert.ToString(datareader["ChannelCode"]);

                if (datareader["ChannelName"] != DBNull.Value)
                    pChannelInfoData.ChannelName_ = Convert.ToString(datareader["ChannelName"]);

                pChannelInfoData.ComboIndex_ = ComboIndex++;    // 콤보 박스에서 사용되는 인덱스
                ChannelManager.Instance.GetList().Add(pChannelInfoData.seq_, pChannelInfoData);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 모니터 정보 변경    
    public static bool UpdateCrawlerMonitorInfo(SqlHelper dbHelper, Int32 xMonitorSeq, string xCrawlerName, Int32 xChannelSeq, string xChannelName
        , Int32 xAuthoritySeq, string xAuthorityName, Int32 xCrawlerSeq, Int32 xMode, string xCrawler_OnOff, string xCrawler_location, Int32 xCrawlerCheckTime
        , Int32 xDBUpdateTime, string xMemo, Int32 xAuthorityLoginSeq, string xAuthorityLoginName)
    {
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xMonitorSeq", xMonitorSeq.ToString());
            argdic.Add("xCrawlerName", xCrawlerName);
            argdic.Add("xChannelSeq", xChannelSeq.ToString());
            argdic.Add("xChannelName", xChannelName);
            argdic.Add("xAuthoritySeq", xAuthoritySeq.ToString());
            argdic.Add("xAuthorityName", xAuthorityName);
            argdic.Add("xAuthorityLoginSeq", xAuthorityLoginSeq.ToString());
            argdic.Add("xAuthorityLoginName", xAuthorityLoginName);
            argdic.Add("xCrawlerSeq", xCrawlerSeq.ToString());
            argdic.Add("xMode", xMode.ToString());
            argdic.Add("xCrawler_OnOff", xCrawler_OnOff.ToString());
            argdic.Add("xCrawler_location", xCrawler_location);
            argdic.Add("xCrawlerCheckTime", xCrawlerCheckTime.ToString());
            argdic.Add("xDBUpdateTime", xDBUpdateTime.ToString());
            argdic.Add("xMemo", xMemo);

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateCrawlerMonitorInfo", argdic);
            
            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 상품 크로러 번호 변경
    public static bool UpdateGoodsCrawlerSeq(SqlHelper dbHelper, Int32 xGoodsSeq, Int32 xCrawlerSeq)
    {
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xGoodsSeq", xGoodsSeq.ToString());
            argdic.Add("xCrawlerSeq", xCrawlerSeq.ToString());

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateGoodsCrawler", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 상품 옵션명, 옵션 닉네임 변경
    public static bool UpdateGoodsNickName(SqlHelper dbHelper, Int32 xGoodsSeq, string xGoodsNickName, string xGoodsOptionNickName)
    {
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xGoodsSeq", xGoodsSeq.ToString());
            argdic.Add("xGoodsNickName", xGoodsNickName);
            argdic.Add("xGoodsOptionNickName", xGoodsOptionNickName);

            MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateGoodsNickName", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 크롤러 모니터 정보 하나 추가
    public static bool InsertCrawlerMonitorInfo(SqlHelper dbHelper, string xIpAddress, Int32 xPort, ref Int32 MonitorSeq)
    {
        MonitorSeq = 0;

        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xIpAddress", xIpAddress);
            argdic.Add("xPort", xPort.ToString());

            MySqlDataReader datareader = dbHelper.call_proc("spNewInsertCrawlerMonitor", argdic);
            while (datareader.Read())
            {
                MonitorSeq = Convert.ToInt32(datareader["MonitorSeq"]);
                break;
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        if (MonitorSeq == 0)
            return false;

        return true;
    }

    // 크롤러 모니터 삭제
    public static bool DeleteCrawlerMonitorInfo(SqlHelper dbHelper, Int32 xMonitorSeq)
    {
        try
        {
            Dictionary<string, object> argdic = new Dictionary<string, object>();
            argdic.Add("xMonitorSeq", xMonitorSeq);

            MySqlDataReader datareader = dbHelper.call_proc("spNewDeleteCrawlerMonitorInfo", argdic);

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }

    // 권리사 로그인 정보 로딩
    public static bool GetAuthorityLoginList(SqlHelper dbHelper)
    {
        try
        {
            AuthorityLoginManager.Instance.InitList();
            MySqlDataReader datareader = dbHelper.call_proc("spNewSelectAuthorityLogin", null);

            Int32 ComboBoxIndex = 1;
            while (datareader.Read())
            {
                AuthorityLoginInfoData pAuthorityLoginInfoData = new AuthorityLoginInfoData();

                if (datareader["seq"] != DBNull.Value)
                    pAuthorityLoginInfoData.seq_ = Convert.ToInt32(datareader["seq"]);

                if (datareader["ChannelSeq"] != DBNull.Value)
                    pAuthorityLoginInfoData.ChannelSeq_ = Convert.ToInt32(datareader["ChannelSeq"]);

                if (datareader["ChannelName"] != DBNull.Value)
                    pAuthorityLoginInfoData.ChannelName_ = Convert.ToString(datareader["ChannelName"]);

                if (datareader["PartnerSeq"] != DBNull.Value)
                    pAuthorityLoginInfoData.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);

                if (datareader["AuthorityName"] != DBNull.Value)
                    pAuthorityLoginInfoData.AuthorityName_ = Convert.ToString(datareader["AuthorityName"]);

                if (datareader["Name"] != DBNull.Value)
                    pAuthorityLoginInfoData.Name_ = Convert.ToString(datareader["Name"]);

                pAuthorityLoginInfoData.ComboIndex_ = ComboBoxIndex++;

                AuthorityLoginManager.Instance.GetList().Add(pAuthorityLoginInfoData.seq_, pAuthorityLoginInfoData);
            }

            datareader.Close();
            datareader.Dispose();
            datareader = null;
        }
        catch (System.Exception ex)
        {
            return false;
        }

        return true;
    }


}

