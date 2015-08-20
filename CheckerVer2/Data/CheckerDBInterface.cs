using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HK.Database;
using MySql.Data.MySqlClient;


namespace CheckerVer2.Data
{
    public class CheckerDBInterface
    {
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

        public static bool UpdateCrawlerMonitorInfo(SqlHelper dbHelper, Int32 xMonitorSeq, string xIpAddress, Int32 xPort, string xCrawlerState
            , ref Int32 Result, ref Int32 ChannelSeq, ref Int32 AuthoritySeq, ref Int32 Mode, ref Int32 CrawlerSeq)
        {
            Result = 0;
            try
            {
                Dictionary<string, object> argdic = new Dictionary<string, object>();
                argdic.Add("xMonitorSeq", xMonitorSeq.ToString());
                argdic.Add("xIpAddress", xIpAddress);
                argdic.Add("xPort", xPort.ToString());
                argdic.Add("xCrawlerState", xCrawlerState);

                MySqlDataReader datareader = dbHelper.call_proc("spNewUpdateCrawlerMonitor", argdic);

                while (datareader.Read())
                {
                    Result = Convert.ToInt32(datareader["RESULT"]);
                    if (Result == 0)
                    {
                        break;
                    }

                    if (datareader["AuthoritySeq"] != DBNull.Value)
                        AuthoritySeq = Convert.ToInt32(datareader["AuthoritySeq"]);
                    if (datareader["ChannelSeq"] != DBNull.Value)
                        ChannelSeq = Convert.ToInt32(datareader["ChannelSeq"]);
                    if (datareader["Mode"] != DBNull.Value)
                        Mode = Convert.ToInt32(datareader["Mode"]);
                    if (datareader["CrawlerSeq"] != DBNull.Value)
                        CrawlerSeq = Convert.ToInt32(datareader["CrawlerSeq"]);
                    
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

            return true;
        }

        public static bool InsertCrawlerRestartLog(SqlHelper dbHelper, string xlogMessage, Int32 xCrawlerSeq, Int32 xCrawlerMonitorSeq
            , Int32 xChannelSeq, Int32 xAuthoritySeq, string xIssueDate)
        {
            try
            {
                Dictionary<string, object> argdic = new Dictionary<string, object>();
                argdic.Add("xlogMessage", xlogMessage);
                argdic.Add("xCrawlerSeq", xCrawlerSeq.ToString());
                argdic.Add("xCrawlerMonitorSeq", xCrawlerMonitorSeq.ToString());
                argdic.Add("xChannelSeq", xChannelSeq.ToString());
                argdic.Add("xAuthoritySeq", xAuthoritySeq.ToString());
                argdic.Add("xIssueDate", xIssueDate);

                MySqlDataReader datareader = dbHelper.call_proc("spNewCrawlerInsertLog", argdic);

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
        //spNewCrawlerInsertLog`(IN xlogMessage TEXT,IN xCrawlerSeq INT,IN xCrawlerMonitorSeq INT,IN xChannelSeq INT,IN xAuthoritySeq INT,IN xIssueDate varchar(20))
    }
}

