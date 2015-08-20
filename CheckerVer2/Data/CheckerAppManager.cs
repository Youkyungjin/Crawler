using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HK.Database;
using MySql.Data.MySqlClient;

namespace CheckerVer2.Data
{
    public enum CHECKER_STATE
    {
        INIT = 0,               // 초기화
        ERROR,                  // 에러난 상황.
        WAIT_MONITOR_TABLE_SET, // 크롤러 모니터링 테이블 셋팅 기다리는중
        REPORT_DB,              // 정상 작동 하면서 DB 에 Update 하고 있는중.
    }

    public class CheckerAppManager : BaseSingleton<CheckerAppManager>
    {
        SqlHelper pMySqlDB_ = null;

        public bool ConnectDB()
        {
            try
            {
                pMySqlDB_ = new SqlHelper();

                pMySqlDB_.Connect(CheckerINIManager.Instance.method_, CheckerINIManager.Instance.dbip_, CheckerINIManager.Instance.dbport_, CheckerINIManager.Instance.dbname_
                    , CheckerINIManager.Instance.dbaccount_, CheckerINIManager.Instance.dbpw_, CheckerINIManager.Instance.sshhostname_
                    , CheckerINIManager.Instance.sshuser_, CheckerINIManager.Instance.sshpw_);
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

        #region 상태 관련
        // 마지막 에러 스트링
        public string ErrorString_ = "";        
        // 현재 상태
        public CHECKER_STATE CHECKER_STATE_ = CHECKER_STATE.INIT;        

        #endregion

        #region 크롤러 정보

        public Int32 AuthoritySeq_ = 0;
        public Int32 ChannelSeq_ = 0;
        public Int32 CrawlerSeq_ = 0;
        public Int32 channelidx_ = 0;
        public Int32 Mode_ = 0;

        #endregion
    }
}
