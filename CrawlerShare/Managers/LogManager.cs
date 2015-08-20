using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HKLibrary.UTIL;

namespace CrawlerShare
{
    public class LogManager : BaseSingleton<LogManager>
    {
        string LogFileName_ = string.Format(@"{0}/CrawlerLog.txt", HKFileHelper.GetCurrentDirectory());

        public void SetLogFile(string logfilename)
        {
            LogFileName_ = string.Format(@"{0}/{1}", HKFileHelper.GetCurrentDirectory(), logfilename);
        }

        public void Log(string strlog)
        {
            HKFileHelper.AddToFile(LogFileName_, strlog);
        }
    }
}
