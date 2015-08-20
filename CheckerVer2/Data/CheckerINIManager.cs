using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Common;

namespace CheckerVer2.Data
{
    public class CheckerINIManager : BaseSingleton<CheckerINIManager>
    {
        // DB 관련
        public string dbip_ = "39.115.210.134";
        public string dbport_ = "3306";
        public string dbname_ = "crawler";
        public string dbaccount_ = "lq";
        public string dbpw_ = "1234qwer";

        //ssh 관련
        public string method_ = "";
        public string sshhostname_ = "";
        public string sshuser_ = "";
        public string sshpw_ = "";

        public string ExePath_ = "";
        public Int32 MonitorSeq_ = 0;   // 모니터링 시퀀스
        public Int32 CheckTime_ = 10000;
        public Int32 WaitCrawlerTime_ = 600000;
        public Int32 ListenCrawlerPort_ = 0;
        public bool bLoad_ = false;
        

        public bool LoadIni(string inifilepath)
        {
            bool bResult = true;
            try
            {
                INIControlor ic = new INIControlor(inifilepath);

                dbip_ = ic.GetIniValue2("Database", "ip");
                dbport_ = ic.GetIniValue2("Database", "port");
                dbname_ = ic.GetIniValue2("Database", "name");
                dbaccount_ = ic.GetIniValue2("Database", "account");
                dbpw_ = ic.GetIniValue2("Database", "pw");
                method_ = ic.GetIniValue2("Database", "method");
                sshhostname_ = ic.GetIniValue2("Database", "sshhostname");
                sshuser_ = ic.GetIniValue2("Database", "sshuser");
                sshpw_ = ic.GetIniValue2("Database", "sshpw");

                string tempstring = ic.GetIniValue2("Checker", "MonitorSeq");
                MonitorSeq_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("Checker", "CheckTime");
                CheckTime_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("Checker", "WaitCrawlerTime");
                WaitCrawlerTime_ = Convert.ToInt32(tempstring); 

                tempstring = ic.GetIniValue2("Checker", "ListenCrawlerPort");
                ListenCrawlerPort_ = Convert.ToInt32(tempstring);

                ExePath_ = ic.GetIniValue2("Checker", "exefullpath");

                bLoad_ = true;
            }
            catch
            {
                bResult = false;
                bLoad_ = false;
            }

            return bResult;
        }


        #region 모니터링 Seq 수정
        public bool UpdateMonitorSeq(string inifilepath, Int32 MonitorSeq)
        {
            try
            {
                INIControlor ic = new INIControlor(inifilepath);
                ic.SetIniValue("Checker", "MonitorSeq", MonitorSeq.ToString());
                MonitorSeq_ = MonitorSeq;
            }
            catch
            {
                return false;
            }

            return true;
        }
        #endregion

        #region 크롤러 INI 파일 수정
        public bool bUpdate_ = false;
        public bool UpdateCrawlerINI(string inifilepath, Int32 Auth, Int32 ChannelSeq, Int32 CrawlerSeq
            , Int32 mode)
        {
            try
            {
                INIControlor ic = new INIControlor(inifilepath);

                bUpdate_ = ic.SetIniValue("ETC", "authorityseq", Auth.ToString());
                bUpdate_ = ic.SetIniValue("Crawler", "uid", CrawlerSeq.ToString());
                bUpdate_ = ic.SetIniValue("ETC", "channelseq", ChannelSeq.ToString());
                bUpdate_ = ic.SetIniValue("ETC", "channelidx", ChannelSeq.ToString());
                bUpdate_ = ic.SetIniValue("Crawler", "mode", mode.ToString());
            }
            catch
            {
                bUpdate_ = false;
                return false;
            }

            bUpdate_ = true;

            return true;
        }
        #endregion
    }
}
