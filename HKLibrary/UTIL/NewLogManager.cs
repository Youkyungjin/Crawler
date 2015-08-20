using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HKLibrary.UTIL
{
    public class NewLogManager2 : BaseSingleton<NewLogManager2>
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
