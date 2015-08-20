using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using LQStructures;
using HK.Database;
using CrawlerShare;

namespace LQCrawler.Data
{
    class AppManager : BaseSingleton<AppManager>
    {
        System.Windows.Forms.Timer Crawler_Timer_ = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer UI_Timer_ = new System.Windows.Forms.Timer();
        System.Windows.Forms.Timer Connection_Timer_ = new System.Windows.Forms.Timer();

        public System.Windows.Forms.Timer GetCrawlerTimer()
        {
            return Crawler_Timer_;
        }

        public System.Windows.Forms.Timer GetUIItmer()
        {
            return UI_Timer_;
        }
        
        public System.Windows.Forms.Timer GetConnectionItmer()
        {
            return Connection_Timer_;
        }
    }
}
