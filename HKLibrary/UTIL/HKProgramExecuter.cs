using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace HK.Util
{
    public class ExecuteResult
    {
        string strMessage_;
        Int32 ID_;

        public ExecuteResult()
        {
            strMessage_ = "ok";
            ID_ = -1;
        }

        public string Message
        {
            get { return strMessage_; }
            set { strMessage_ = value; }
        }

        public Int32 ID
        {
            get { return ID_; }
            set { ID_ = value; }
        }

    }

    public class HKProgramExecuter
    {
        public static ExecuteResult StartProgram(string exepath)
        {
            ExecuteResult result = new ExecuteResult();

            try
            {
                System.Diagnostics.Process p = Process.Start(exepath);
                result.ID = p.Id;
            }
            catch (System.Exception ex)
            {
                result.ID = -1;
                result.Message = ex.ToString();
            }

            return result;
        }

        public static Int32 CheckRunningProgramByProcessName(string processname)
        {
            Int32 nResult = 0;
            Process[] processList = Process.GetProcessesByName(processname);

            foreach (Process p in processList)
            {
                nResult++;
            }

            return nResult;
        }

        public static Process CheckRunningProgramByProcessID(Int32 processID)
        {
            Process programprocess = Process.GetProcessById(processID);
            
            return programprocess;
        }

        public static Int32 StopProgram(string processname)
        {
            Int32 nResult = 0;
            Process[] processList = Process.GetProcessesByName(processname);

            foreach (Process p in processList)
            {
                p.Kill();
                nResult++;
            }

            return nResult;
        }

        public static bool StopProgramByProcessID(Int32 processID)
        {
            try
            {
                Process programprocess = Process.GetProcessById(processID);
                if (programprocess == null)
                    return false;

                programprocess.Kill();
            }
            catch (System.Exception ex)
            {
                return false;
            }


            return true;
        }
    }
}
