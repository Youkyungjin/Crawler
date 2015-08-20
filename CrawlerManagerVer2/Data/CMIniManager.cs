using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMS.Common;

public class CMIniManager : BaseSingleton<CMIniManager>
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

    // 기타
    public Int32 checkbox_ = 0;
    public string loginid_ = "";

    public Int32 refreshtick_ = -1;

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

            string tempstring = ic.GetIniValue2("ETC", "refreshtick");
            refreshtick_ = Convert.ToInt32(tempstring);

            tempstring = ic.GetIniValue2("ETC", "checkbox");
            checkbox_ = Convert.ToInt32(tempstring);
            loginid_ = ic.GetIniValue2("ETC", "loginid");

            bLoad_ = true;
        }
        catch
        {
            bResult = false;
            bLoad_ = false;
        }

        return bResult;
    }

    public bool SaveCheckBoxAndID(string inifilepath, string ischeckbox, string loginid)
    {
        try
        {
            INIControlor ic = new INIControlor(inifilepath);

            ic.SetIniValue("ETC", "checkbox", ischeckbox);
            ic.SetIniValue("ETC", "loginid", loginid);

            checkbox_ = Convert.ToInt32(ischeckbox);
            loginid_ = loginid;
        }
        catch
        {
            return false;
        }

        return true;
    }

}
