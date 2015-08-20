using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HK.Database;
using MySql.Data.MySqlClient;

public enum CRAWLER_MODE
{
    NONE = 0,
    INSERT,     // 취합( 신규 데이터를 Insert )
    PROCESS,    // 처리( 데이터의 변경 사항을 Update )
    CHECK,      // 반환(체크한다)
}

public class CrawlerData
{
    public Int32 seq_ = 0;              // 모니터 시퀀스
    public string CrawlerName_ = "";    // 크롤러 이름
    public Int32 ChannelSeq_ = 0;       // 채널 시퀀스
    public string ChannelName_ = "";    // 채널 이름
    public Int32 AuthoritySeq_ = 0;     // 권리사 시퀀스
    public string AuthorityName_ = "";  // 권리사 이름

    public Int32 AuthorityLoginSeq_ = 0;        // 권리사 로그인 시퀀스
    public string AuthorityLoginName_ = "";     // 권리사 로그인 이름

    public Int32 CrawlerSeq_ = 0;       // 크롤러 시퀀스
    public Int32 Mode_ = 0;             // 동작 모드(CRAWLER_MODE)
    public string IP_ = "";             // 아이피
    public string Memo_ = "";           // 메모
    public string CrawlerState_ = "";          // 크롤링 상태
    public string UpdateDate_ = "";    // 최근 동작 상태
}

public class AdminInfoData
{
    public Int32 Seq_ = 0;
    public string ID_ = "";
    public string PW_ = "";
    public string adminName_ = "";
    public string Mobile_ = "";
    public Int32 IsMaster_ = 0;
    public string permission_ = "";
}

// 크롤러 정보 매니저
public class CInfoManager : BaseSingleton<CInfoManager>
{
    SqlHelper pMySqlDB_ = null;
    Dictionary<Int32, CrawlerData> List_ = new Dictionary<Int32, CrawlerData>();
    public AdminInfoData AdminInfoData_ = new AdminInfoData();

    public bool ConnectDB()
    {
        try
        {
            pMySqlDB_ = new SqlHelper();

            pMySqlDB_.Connect(CMIniManager.Instance.method_, CMIniManager.Instance.dbip_, CMIniManager.Instance.dbport_, CMIniManager.Instance.dbname_
                , CMIniManager.Instance.dbaccount_, CMIniManager.Instance.dbpw_, CMIniManager.Instance.sshhostname_
                , CMIniManager.Instance.sshuser_, CMIniManager.Instance.sshpw_);
        }
        catch (System.Exception ex)
        {
            pMySqlDB_ = null;
            return false;
        }
        
        return true;
    }

    public void InitDB()
    {
        if (pMySqlDB_ != null)
        {
            pMySqlDB_.Close();
        }
    }

    public SqlHelper DB()
    {
        return pMySqlDB_;
    }

    public void InitList()
    {
        List_.Clear();
    }

    public Dictionary<Int32, CrawlerData> GetList()
    {
        return List_;
    }

    public CrawlerData GetCrawlerData(Int32 seq)
    {
        if (List_.ContainsKey(seq) == true)
            return List_[seq];

        return null;
    }

    public bool DelCrawlerData(Int32 seq)
    {
        if (List_.ContainsKey(seq) == true)
        {
            List_.Remove(seq);
            return true;
        }

        return false;
    }
    
}

