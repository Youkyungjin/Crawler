using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Common;

namespace Channels
{
    public class CINIManager : BaseSingleton<CINIManager>
    {
        public Int32 UID_ = -1;    // 크롤러의 고유번호
        public Int32 MODE_ = -1;    // 크롤러 동작 방식

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


        // Manager 관련
        public string managerip_ = "";
        public Int32 managerport_ = 0;

        // 채널, 기타 관련 
        public Int32 partneridx_ = -1;
        public Int32 channelidx_ = -1;
        public Int32 channelseq_ = -1;
        public Int32 authorityseq_ = -1;
        public Int32 crawlingtick_ = -1;
        public bool deletedownfile_ = false;        
        // 체커 관련
        public Int32 checkerport_ = -1;

        
        public bool LoadIni(string inifilepath)
        {
            bool bResult = true;
            try
            {
                INIControlor ic = new INIControlor(inifilepath);
                string tempstring = ic.GetIniValue2("Crawler", "uid");
                UID_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("Crawler", "mode");
                MODE_ = Convert.ToInt32(tempstring);
                if (MODE_ < 0 || MODE_ > 3)
                {
                    MODE_ = 0;
                    return false;
                }

                dbip_ = ic.GetIniValue2("Database", "ip");
                dbport_ = ic.GetIniValue2("Database", "port");
                dbname_ = ic.GetIniValue2("Database", "name");
                dbaccount_ = ic.GetIniValue2("Database", "account");
                dbpw_ = ic.GetIniValue2("Database", "pw");
                method_ = ic.GetIniValue2("Database", "method");
                sshhostname_ = ic.GetIniValue2("Database", "sshhostname");
                sshuser_ = ic.GetIniValue2("Database", "sshuser");
                sshpw_ = ic.GetIniValue2("Database", "sshpw");

                managerip_ = ic.GetIniValue2("Manager", "ip");
                tempstring = ic.GetIniValue2("Manager", "port");
                managerport_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "partneridx");
                partneridx_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "channelidx");
                channelidx_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "channelseq");
                channelseq_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "authorityseq");
                authorityseq_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "crawlingtick");
                crawlingtick_ = Convert.ToInt32(tempstring);

                tempstring = ic.GetIniValue2("ETC", "deletedownfile");
                deletedownfile_ = Convert.ToBoolean(tempstring);

                tempstring = ic.GetIniValue2("CHECKER", "checkerport");
                checkerport_ = Convert.ToInt32(tempstring);
            }
            catch
            {
                bResult = false;
            }
            
            return bResult;
        }

    }
}
