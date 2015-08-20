using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web;
using HKLibrary.UTIL;
using CrawlerShare;
using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;
using CData;
using System.Net;
using HKLibrary.WEB;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;
using System.Threading;

namespace Channels
{
    public class Cangoto : BaseChannel
    {
        // 로그인 Web
        public override bool Web_Login()
        {
            Cookie_ = new CookieContainer();

            try
            {
                string loginurl = LQCrawlerInfo_.LoginUrl_;
                string loginstring = "ctl00%24ctl00%24sm=ctl00%24ctl00%24body%24body%24upnlLogin%7Cctl00%24ctl00%24body%24body%24lnkLogin&hdRefererUrl=https%3A%2F%2Finvite.cangoto.kr%2Fmanager%2FListSupply_THEATER.aspx&__EVENTTARGET=ctl00%24ctl00%24body%24body%24lnkLogin&__EVENTARGUMENT=&__VIEWSTATE=%2FwEPDwUKMTY4NTU4MjM0OQ8WAh4MUGFnZVZpZXdEYXRhMpgEAAEAAAD%2F%2F%2F%2F%2FAQAAAAAAAAAMAgAAAEFFU05meDMuV2ViLCBWZXJzaW9uPTEuMC4wLjAsIEN1bHR1cmU9bmV1dHJhbCwgUHVibGljS2V5VG9rZW49bnVsbAUBAAAAGEVTTmZ4My5XZWIuUGFnZS5WaWV3RGF0YQEAAAAFX2RhdGEDHFN5c3RlbS5Db2xsZWN0aW9ucy5IYXNodGFibGUCAAAACQMAAAAEAwAAABxTeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlBwAAAApMb2FkRmFjdG9yB1ZlcnNpb24IQ29tcGFyZXIQSGFzaENvZGVQcm92aWRlcghIYXNoU2l6ZQRLZXlzBlZhbHVlcwAAAwMABQULCBxTeXN0ZW0uQ29sbGVjdGlvbnMuSUNvbXBhcmVyJFN5c3RlbS5Db2xsZWN0aW9ucy5JSGFzaENvZGVQcm92aWRlcgjsUTg%2FAgAAAAoKCwAAAAkEAAAACQUAAAAQBAAAAAIAAAAGBgAAAAxNRVRBX0tFWVdPUkQGBwAAAAlNRVRBX0RFU0MQBQAAAAIAAAAGCAAAACNCMkIg7KCE7J6Q6rKw7KCcIOydtOyDgeuEpO2KuOybjeyKpAYJAAAAQuuMgO2VnOuvvOq1rSDrjIDtkZxCMkIgTWFya2V0UGxhY2Ug7KCE7J6Q6rKw7KCcIOydtOyDgeuEpO2KuOybjeyKpAtkZNoHvca%2FH7z5AEHT%2Bxp%2BUaCtTdgh&__VIEWSTATEGENERATOR=FB1FF1F5&__EVENTVALIDATION=%2FwEWCAL0mu%2BGCAK%2BhoGnDwLU%2B%2B72AQLqoeGbBQKQrqHsAgLD1oHGDQKo9LDPDwLWvMqLAXsjmW6bcHFUTrYqNM75a2sSPala&sortNM=&sortDirection=&ctl00%24ctl00%24body%24body%24txtLoginID={LoginID}&ctl00%24ctl00%24body%24body%24txtLoginPWD={LoginPW}&ctl00%24ctl00%24body%24body%24ucPopPassword%24txtPWD=&ctl00%24ctl00%24body%24body%24ucPopPassword%24txtPWD_Confirm=&ctl00%24ctl00%24body%24body%24hddMsg=%EC%95%84%EC%9D%B4%EB%94%94%20%EB%98%90%EB%8A%94%20%ED%8C%A8%EC%8A%A4%EC%9B%8C%EB%93%9C%EA%B0%80%20%ED%8B%80%EB%A0%B8%EC%8A%B5%EB%8B%88%EB%8B%A4.&__ASYNCPOST=true&";
                
                loginstring = loginstring.Replace("{LoginID}", LQCrawlerInfo_.LoginID_);
                loginstring = loginstring.Replace("{LoginPW}", LQCrawlerInfo_.LoginPW_);
                //byte[] sendData = UTF8Encoding.UTF8.GetBytes(loginstring);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(LQCrawlerInfo_.LoginMethod_, loginurl, loginstring, Cookie_, "", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");
                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                //if (htmlBuffer.IndexOf(LQCrawlerInfo_.LoginCheck_) < 0)
                //   return false;

            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(ex.Message);
                return false;
            }

            return true;
        }
        // 엑셀 다운로드
        public override bool Web_DownLoadExcel()
        {
            try
            {
                ProcessStateManager.Instance.NeedDownLoadCount_ = GoodsInfoList_.Count;
                DateTime dtNow = DateTime.Now;

                // 하위 폴더 만들기
                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                foreach (var pData in GoodsInfoList_)
                {
                    ChannelGoodInfo pGoodInfo = pData.Value;

                    string downString = makefolder;
                    downString += "\\";
                    downString += pGoodInfo.Goods_Code_;
                    downString += "_";
                    downString += Convert.ToString(dtNow.Ticks);
                    downString += ".xls";

                    // 이미 다운로드가 끝난 파일이라면 다시 다운로드 하지 않는다.
                    if (GoodsDownInfo_.ContainsKey(pGoodInfo.Goods_Code_) == false)
                    {
                        try
                        {
                            string method = LQCrawlerInfo_.ExcelDownMethod_;
                            string url = LQCrawlerInfo_.ExcelDownUrl_;
                            url = url.Replace("{GoodsCode}", pGoodInfo.Goods_Code_);

                            string sendparameter = "hdRefererUrl=https%3A%2F%2Finvite.cangoto.kr%2Fauth%2Fuserlogin.aspx%3FreUrl%3D%2Fdefault.aspx&__EVENTTARGET=ctl00%24ctl00%24body%24body%24btnexcel&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=%2FwEPDwULLTE2Njg2MTc5NTAPFgIeDFBhZ2VWaWV3RGF0YTLVBwABAAAA%2F%2F%2F%2F%2FwEAAAAAAAAADAIAAABBRVNOZngzLldlYiwgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwFAQAAABhFU05meDMuV2ViLlBhZ2UuVmlld0RhdGEBAAAABV9kYXRhAxxTeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlAgAAAAkDAAAABAMAAAAcU3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0YWJsZQcAAAAKTG9hZEZhY3RvcgdWZXJzaW9uCENvbXBhcmVyEEhhc2hDb2RlUHJvdmlkZXIISGFzaFNpemUES2V5cwZWYWx1ZXMAAAMDAAUFCwgcU3lzdGVtLkNvbGxlY3Rpb25zLklDb21wYXJlciRTeXN0ZW0uQ29sbGVjdGlvbnMuSUhhc2hDb2RlUHJvdmlkZXII7FE4PxEAAAAKChcAAAAJBAAAAAkFAAAAEAQAAAAQAAAABgYAAAAFRVZFSUQGBwAAAAtTTVNNU0dfVEFJTAYIAAAADE1FVEFfS0VZV09SRAYJAAAACFVzZXJUeXBlBgoAAAAJQURNUGFzc05PBgsAAAAOSXNMb2dpblN1Y2Nlc3MGDAAAAAVFQ09ERQYNAAAABEVBSUQGDgAAAApBRE1BZG1pbk5PBg8AAAAKQURNTG9naW5JRAYQAAAAB01lc3NhZ2UGEQAAAAVURUxOTwYSAAAACU1FVEFfREVTQwYTAAAACFJPTEVUeXBlBhQAAAAIVXNlck5hbWUGFQAAAAdBRE1OYW1lEAUAAAAQAAAACAgjBwAABhYAAAAABhcAAAAjQjJCIOyghOyekOqysOygnCDsnbTsg4HrhKTtirjsm43siqQJGAAAAAoIAQEJFgAAAAgI6QcAAAYaAAAAB2NhbmdvdG8GGwAAAAdoYWxsaTAxBhwAAAAG7ZmV7J24CRYAAAAGHgAAAELrjIDtlZzrr7zqta0g64yA7ZGcQjJCIE1hcmtldFBsYWNlIOyghOyekOqysOygnCDsnbTsg4HrhKTtirjsm43siqQGHwAAAAFEBiAAAAA066Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCBMT1RURSBXT1JMRCBOSUdIVCBQQVJUWQkgAAAADCEAAAA%2FQXBwX0NvZGUsIFZlcnNpb249MC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxsBRgAAAAIVXNlclR5cGUBAAAAB3ZhbHVlX18ACCEAAAACAAAACxYCZg9kFgJmD2QWAgIDD2QWAgIDD2QWBmYPFgIeCWlubmVyaHRtbAU%2B66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCA8YnIvPkxPVFRFIFdPUkxEIE5JR0hUIFBBUlRZPGJyLz5kAgEPFgIfAQUhMTIuMTko6riIKSB%2BIDEyLjE5KOq4iCkgLyAx7J286rCEZAICD2QWBgIFDxBkEBUCBuyEoO2DnS8g66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRUCAC9A66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRQrAwJnZ2RkAgcPEGQQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnZGQCDQ9kFgJmD2QWBgIBDxYCHgdWaXNpYmxlaBYCAgEPFgIeC18hSXRlbUNvdW50AgEWAmYPZBYCZg8VAwoyMDE1LTAxLTA1ATIGMzQsMDAwZAIDDxYCHwMCKBZQZg9kFgRmDxUJLOyEnOycpOqyvSAgICAgICAgICAgICAgICAgICAgICAgICjshJzsnKTqsr0pCzAxMDg4NjY0MzMyDTExODI3MjU3NDgzMzIZMjAxNC0xMi0xOSDsmKTtm4QgMjoxNTo1Mx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7LC47ISdZAIBDxAPFgQeB1Rvb2xUaXAFBTY4MDQxHgtfIURhdGFCb3VuZGcWAh4Ib25jaGFuZ2UFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIBD2QWBGYPFQko7LWc7JuQICAgICAgICAgICAgICAgICAgICAgICAgICAo7LWc7JuQKQswMTAyNjI1ODEyNQ0xMTgyNzU3MzgxMTI1GTIwMTQtMTItMTkg7Jik7KCEIDE6NTI6MTEd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3OTkyHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAgIPZBYEZg8VCSzsnbTshKDtnawgICAgICAgICAgICAgICAgICAgICAgICAo7J207ISg7Z2sKQswMTA4MDA1MjQ4MQ0xMTgyNzU0OTcwNDgxGjIwMTQtMTItMTgg7Jik7ZuEIDEwOjU5OjMxHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2Nzk2MB8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIDD2QWBGYPFQks7KCV7YOc7ISxICAgICAgICAgICAgICAgICAgICAgICAgKOygle2DnOyEsSkLMDEwOTI0NDI3MzINMTE4Mjc5NzQ3ODczMhkyMDE0LTEyLTE4IOyYpO2bhCA4OjQ1OjU1Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzkyMx8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIED2QWBGYPFQks7JWI7KKF66%2B8ICAgICAgICAgICAgICAgICAgICAgICAgKOyViOyiheuvvCkLMDEwNjY0NTU4NjQNMTE4Mjc1OTE0Njg2NBkyMDE0LTEyLTE4IOyYpO2bhCA3OjAwOjUzHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2Nzg2Mh8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIFD2QWBGYPFQks67CV7KeE7ISdICAgICAgICAgICAgICAgICAgICAgICAgKOuwleynhOyEnSkLMDEwOTU2NTMzNDQNMTE4Mjc3MjY0MTM0NBkyMDE0LTEyLTE4IOyYpO2bhCA2OjI2OjE2Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzgyOB8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIGD2QWBGYPFQks7ZmN7ISg7ZicICAgICAgICAgICAgICAgICAgICAgICAgKO2ZjeyEoO2YnCkLMDEwNzE2NjM5MjUNMTE4MjcxNjQzNDkyNRkyMDE0LTEyLTE4IOyYpO2bhCA0OjI0OjEwHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzgxMx8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIHD2QWBGYPFQkLKOydtOynhOyjvCkLMDEwMjk2NjIzNjMNMTE4MjcxODg0NjM2MxkyMDE0LTEyLTE4IOyYpO2bhCAzOjAyOjMzHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbtmZjrtohkAgEPEA8WBB8EBQU2Nzc5Mh8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIID2QWBGYPFQks6rCV7JuQ7YOcICAgICAgICAgICAgICAgICAgICAgICAgKOqwleybkO2DnCkLMDEwMzQ1NDYyMjUNMTE4Mjc2NDQ2MzIyNRoyMDE0LTEyLTE4IOyYpO2bhCAxMjo0NjoxNh3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMQYxNywwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjc3NjcfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCCQ9kFgRmDxUJLOq5gOyaqeybkCAgICAgICAgICAgICAgICAgICAgICAgICjquYDsmqnsm5ApCzAxMDU1NTQ5MzA0DTExODI3OTI2MTQzMDQaMjAxNC0xMi0xOCDsmKTsoIQgMTI6MjM6MjId66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NzI1HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAgoPZBYEZg8VCSzsnbTqs7XsmrAgICAgICAgICAgICAgICAgICAgICAgICAo7J206rO17JqwKQswMTAyMzc5MjA1MA0xMTgyNzU0MTg3MDUwGTIwMTQtMTItMTcg7Jik7ZuEIDk6NTQ6NDcd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATEGMTcsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NzA1HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAgsPZBYEZg8VCSzsmrDslYTrpoQgICAgICAgICAgICAgICAgICAgICAgICAo7Jqw7JWE66aEKQswMTAyODIwMjU0MQ0xMTgyNzkwNzQ1NTQxGTIwMTQtMTItMTcg7Jik7ZuEIDk6NDc6NTcd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATMGNTEsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NzA0HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAgwPZBYEZg8VCSzsmKTshJzsmIEgICAgICAgICAgICAgICAgICAgICAgICAo7Jik7ISc7JiBKQswMTA1MzMwNTAxNA0xMTgyNzYyMDkzMDE0GTIwMTQtMTItMTcg7Jik7ZuEIDg6NTA6Mjkd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3Njk2HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAg0PZBYEZg8VCSzsnbTsp4DsmIEgICAgICAgICAgICAgICAgICAgICAgICAo7J207KeA7JiBKQswMTA4ODAyMDMyOA0xMTgyNzE3Nzk2MzI4GTIwMTQtMTItMTcg7Jik7ZuEIDc6MjU6Mzgd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3Njg0HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAg4PZBYEZg8VCSzquYDsl7Dqsr0gICAgICAgICAgICAgICAgICAgICAgICAo6rmA7Jew6rK9KQswMTAzMDgxODU5MA0xMTgyNzkxODU5NTkwGTIwMTQtMTItMTcg7Jik7ZuEIDc6MTk6MjAd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NjgxHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAg8PZBYEZg8VCT3tnojrnbzsubTsmYDrp4jrpqzsvZQgICAgICAgICAgICAgICAgKO2eiOudvOy5tOyZgCDrp4jrpqzsvZQpCzAxMDUyODA3NDk0DTExODI3NDUyMjU0OTQaMjAxNC0xMi0xNyDsmKTtm4QgMTI6MDE6NDcd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NjE0HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAhAPZBYEZg8VCSztmY3rr7jsoJUgICAgICAgICAgICAgICAgICAgICAgICAo7ZmN66%2B47KCVKQswMTA1NTgzNTcxNg0xMTgyNzYyNTAxNzE2GTIwMTQtMTItMTcg7Jik7KCEIDM6NDc6MDcd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBu2ZmOu2iGQCAQ8QDxYEHwQFBTY3NjAyHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAhEPZBYEZg8VCSzstZztmJXsnbggICAgICAgICAgICAgICAgICAgICAgICAo7LWc7ZiV7J24KQswMTA4MzQ1NTgzMw0xMTgyNzU4NTM1ODMzGTIwMTQtMTItMTcg7Jik7KCEIDE6Mzg6NTQd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3NTk2HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAhIPZBYEZg8VCSzsnbTshKDtmJwgICAgICAgICAgICAgICAgICAgICAgICAo7J207ISg7ZicKQswMTAzMDAyMjMwMg0xMTgyNzc2NTI1MzAyGjIwMTQtMTItMTYg7Jik7ZuEIDExOjU4OjQwHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzU3NB8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAITD2QWBGYPFQks7LWc7ISg7Z2sICAgICAgICAgICAgICAgICAgICAgICAgKOy1nOyEoO2drCkLMDEwNzU5NzMzMTkNMTE4Mjc1MDI5ODMxORkyMDE0LTEyLTE2IOyYpO2bhCA0OjUxOjM2Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzU0Nh8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIUD2QWBGYPFQks7J2066%2B4656AICAgICAgICAgICAgICAgICAgICAgICAgKOydtOuvuOuegCkLMDEwODIzMTU4ODQNMTE4Mjc1MDExMzg4NBkyMDE0LTEyLTE2IOyYpO2bhCAyOjIzOjIwHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzUyMx8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIVD2QWBGYPFQks67OA7IOB7JikICAgICAgICAgICAgICAgICAgICAgICAgKOuzgOyDgeyYpCkLMDEwMzYzOTIxNjUNMTE4Mjc5MDY0MDE2NRkyMDE0LTEyLTE2IOyYpO2bhCAxOjIyOjUxHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzUxNR8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIWD2QWBGYPFQks7KCV7IaM7Z2sICAgICAgICAgICAgICAgICAgICAgICAgKOygleyGjO2drCkLMDEwMjg1OTcyNTANMTE4MjczOTA1NzI1MBkyMDE0LTEyLTE2IOyYpO2bhCAxOjE0OjQyHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQExBjE3LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NzUxNB8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIXD2QWBGYPFQks7Jqw7IOB6regICAgICAgICAgICAgICAgICAgICAgICAgKOyasOyDgeq3oCkLMDEwNzEyMDI5NjENMTE4MjcxNzQxNjk2MRoyMDE0LTEyLTE2IOyYpO2bhCAxMjo0MTozNx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMQYxNywwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjc1MDcfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCGA9kFgRmDxUJLOyghOyYgeynhCAgICAgICAgICAgICAgICAgICAgICAgICjsoITsmIHsp4QpCzAxMDk2NjgzMTY4DTExODI3MTUyNzkxNjgZMjAxNC0xMi0xNSDsmKTtm4QgNDo0MTo1Nx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMQYxNywwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjczNDUfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCGQ9kFgRmDxUJLOycoOqwgOuguSAgICAgICAgICAgICAgICAgICAgICAgICjsnKDqsIDroLkpCzAxMDkxODUzNjY1DTExODI3MzQwNjg2NjUZMjAxNC0xMi0xNSDsmKTtm4QgNDowNzowNh3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMQYxNywwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjczMzgfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCGg9kFgRmDxUJCyjsobDtmITsp4ApCzAxMDM4NjcyOTM5DTExODI3MTAwNzc5MzkZMjAxNC0xMi0xNSDsmKTtm4QgMjowOTo0OR3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjczMTcfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCGw9kFgRmDxUJLOyViOyYgeyEoCAgICAgICAgICAgICAgICAgICAgICAgICjslYjsmIHshKApCzAxMDQ4NDk5Mjg5DTExODI3MjMyMTAyODkaMjAxNC0xMi0xNSDsmKTsoIQgMTI6MDg6MTMd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3MjY4HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAhwPZBYEZg8VCSzsnoTsoJXrr7ggICAgICAgICAgICAgICAgICAgICAgICAo7J6E7KCV66%2B4KQswMTA3NjIyODgzMA0xMTgyNzE1OTAwODMwGTIwMTQtMTItMTQg7Jik7ZuEIDM6MzE6MzYd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATEGMTcsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY3MDg3HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAh0PZBYEZg8VCSzquYDtmJzrr7wgICAgICAgICAgICAgICAgICAgICAgICAo6rmA7Zic66%2B8KQswMTA4NzA1ODAzOQ0xMTgyNzU0ODMyMDM5GTIwMTQtMTItMTQg7Jik7ZuEIDI6MzY6MzUd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATEGMTcsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTY2OTEyHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAh4PZBYEZg8VCSzquYDshLHtm4ggICAgICAgICAgICAgICAgICAgICAgICAo6rmA7ISx7ZuIKQswMTA0MTk0NDA2OQ0xMTgyNzY3OTkxMDY5GjIwMTQtMTItMTMg7Jik7ZuEIDExOjU4OjM5Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NTk4OR8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIfD2QWBGYPFQks6rmA7IS47Jq0ICAgICAgICAgICAgICAgICAgICAgICAgKOq5gOyEuOyatCkLMDEwMjU3MzM0ODkNMTE4Mjc0Mzg2NjQ4ORkyMDE0LTEyLTEzIOyYpO2bhCA5OjExOjUzHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2NTgzMh8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIgD2QWBGYPFQks64W47KeA7JiBICAgICAgICAgICAgICAgICAgICAgICAgKOuFuOyngOyYgSkLMDEwOTYzNTUxMzUNMTE4MjczMzA2MjEzNRoyMDE0LTEyLTEzIOyYpOyghCAxMTo1NjoxMh3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjQ5NjcfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCIQ9kFgRmDxUJLOq5gOyImOqyvSAgICAgICAgICAgICAgICAgICAgICAgICjquYDsiJjqsr0pCzAxMDkwNzczNTU5DTExODI3NjYyMjA1NTkZMjAxNC0xMi0xMiDsmKTtm4QgOToxODo0OB3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMQYxNywwMDAG7ZmY67aIZAIBDxAPFgQfBAUFNjQyOTkfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCIg9kFgRmDxUJLOq5gOyEnO2ZjSAgICAgICAgICAgICAgICAgICAgICAgICjquYDshJztmY0pCzAxMDkzOTUxNzI2DTExODI3NDczMTU3MjYZMjAxNC0xMi0xMiDsmKTtm4QgODozMDozOR3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7JiI7JW9ZAIBDxAPFgQfBAUFNjQyNjIfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCIw9kFgRmDxUJLOy1nOuCqOuvuCAgICAgICAgICAgICAgICAgICAgICAgICjstZzrgqjrr7gpCzAxMDI0MDY4MTg5DTExODI3MzcyMjgxODkaMjAxNC0xMi0xMiDsmKTsoIQgMTA6NTQ6Mjcd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBu2ZmOu2iGQCAQ8QDxYEHwQFBTY0MDQxHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAiQPZBYEZg8VCSzquYDshozsnoQgICAgICAgICAgICAgICAgICAgICAgICAo6rmA7IaM7J6EKQswMTA1MDk5NzQ2OQ0xMTgyNzg5NTI2NDY5GTIwMTQtMTItMTEg7Jik7ZuEIDQ6MTU6Mjkd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATEGMTcsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTYzODMwHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAiUPZBYEZg8VCSzrsJXsnKDsp4QgICAgICAgICAgICAgICAgICAgICAgICAo67CV7Jyg7KeEKQswMTA3MTE2OTM0Mw0xMTgyNzc5MzY2MzQzGTIwMTQtMTItMTEg7Jik7ZuEIDE6Mjk6NTMd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATEGMTcsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTYzODAyHwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAiYPZBYEZg8VCSzsobDssL3tnawgICAgICAgICAgICAgICAgICAgICAgICAo7KGw7LC97Z2sKQswMTAyMjY2NTk5MA0xMTgyNzU1NzIxOTkwGTIwMTQtMTItMTEg7Jik7ZuEIDE6MDk6NDAd66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCAwICDroa%2FrjbDsm5Trk5wgTmlnaHQgUGFydHkg7J6Q7Jyg7J207Jqp6raMKDHsnbgpATIGMzQsMDAwBuyYiOyVvWQCAQ8QDxYEHwQFBTYzNzk0HwVnFgIfBgUicmV0dXJuIGZuVmFsdWVDaGFuZ2VDb25maXJtKHRoaXMpOxAVBQbshKDtg50G7JiI7JW9BuywuOyEnQbrtojssLgG7ZmY67aIFQUAAU4BRgFYAVIUKwMFZ2dnZ2cWAWZkAicPZBYEZg8VCSzrj4Trnpjrr7ggICAgICAgICAgICAgICAgICAgICAgICAo64%2BE656Y66%2B4KQswMTAyNDA4MzU3NQ0xMTgyNzU0NTAzNTc1GjIwMTQtMTItMTEg7Jik7KCEIDEwOjExOjUxHeuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbsmIjslb1kAgEPEA8WBB8EBQU2Mzc1OB8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIFDxYCHwJoZGRRd5LX9vcJCbyDjkSRp1SpQVYJqw%3D%3D&__VIEWSTATEGENERATOR=FA3FC143&sortNM=&sortDirection=&ctl00%24ctl00%24body%24body%24txtUSERNAME=&ctl00%24ctl00%24body%24body%24txtTELNO=&ctl00%24ctl00%24body%24body%24ddlLine=&ctl00%24ctl00%24body%24body%24ddlOptionValue=&ctl00%24ctl00%24body%24body%24rptList%24ctl00%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl01%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl02%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl03%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl04%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl05%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl06%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl07%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl08%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl09%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl10%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl11%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl12%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl13%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl14%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl15%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl16%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl17%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl18%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl19%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl20%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl21%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl22%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl23%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl24%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl25%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl26%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl27%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl28%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl29%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl30%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl31%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl32%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl33%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl34%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl35%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl36%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl37%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl38%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl39%24ddlType=";


                            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, sendparameter, Cookie_, null, "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36", 180000);


                            if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr" || pResponse.CharacterSet == "EUC-KR" || pResponse.CharacterSet == "KSC5601")
                            {
                                FileStream fs = File.OpenWrite(downString);

                                string d = pResponse.CharacterSet;
                                Stream responsestream = pResponse.GetResponseStream();
                                byte[] buffer = new byte[2048];

                                long totalBytesRead = 0;
                                int bytesRead;

                                while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                                {
                                    totalBytesRead += bytesRead;
                                    fs.Write(buffer, 0, bytesRead);
                                }
                                fs.Close();
                                fs.Dispose();
                            }
                            else
                            {
                                TextReader textReader = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding(pResponse.CharacterSet));
                                string htmlBuffer = textReader.ReadToEnd();
                                HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                                textReader.Close();
                                textReader.Dispose();
                            }
                        }
                        catch (System.Exception ex)
                        {
                            NewLogManager2.Instance.Log(ex.Message);
                            continue;
                        }

                        GoodsDownInfo_.Add(pGoodInfo.Goods_Code_, downString);
                        ProcessStateManager.Instance.CurDownLoadCount_++;
                    }
                    else
                    {
                        ProcessStateManager.Instance.PassDownLoadCount_++;
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_DownLoadExcel() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        protected override bool Internal_Excel_Parsing(ChannelGoodInfo pChannelGoodInfo)
        {
            try
            {
                if (GoodsDownInfo_.ContainsKey(pChannelGoodInfo.Goods_Code_) == false)
                {
                    NewLogManager2.Instance.Log(string.Format("!! 상품 코드 엑셀이 없습니다. - {0}", pChannelGoodInfo.Goods_Code_));
                    return false;
                }
                string filepath = GoodsDownInfo_[pChannelGoodInfo.Goods_Code_];
                Microsoft.Office.Interop.Excel.Application ap = null;
                Workbook wb = null;
                Worksheet ws = null;
                HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                string tempString = "";
                string comparesitename = "";

                Int32 nCurrentRow = LQCrawlerInfo_.ExData_Start_;
                Int32 ExData_Option = LQCrawlerInfo_.ExData_Option_;
                Int32 ExData_Coupncode = LQCrawlerInfo_.ExData_Coupncode_;
                Int32 ExData_Buyer = LQCrawlerInfo_.ExData_Buyer_;
                Int32 ExData_Cancel = LQCrawlerInfo_.ExData_Cancel_;
                Int32 ExData_Use = LQCrawlerInfo_.ExData_Use_;
                Int32 ExData_Buyphone = LQCrawlerInfo_.ExData_Buyphone_;
                Int32 ExData_Price = LQCrawlerInfo_.ExData_Price_;
                Int32 ExData_BuyDate = LQCrawlerInfo_.ExData_Buydate_;
                Int32 ExData_BuyCount = LQCrawlerInfo_.ExData_Count_;
                Int32 ExData_GoodsName = LQCrawlerInfo_.ExData_GoodName_;

                if (nCurrentRow > 0)
                    ProcessStateManager.Instance.NeedParsingCount_ += (ws.UsedRange.Rows.Count - (nCurrentRow - 1));

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, 1];
                        comparesitename = Convert.ToString(tRange.Value2);

                        tRange = ws.Cells[nCurrentRow, ExData_Option];
                        if (tRange == null)
                            break;

                        tempString = tRange.Value2;
                        if (tempString == null)
                        {
                            break;
                        }

                        COrderData pExcelData = new COrderData();
                        pExcelData.channelSeq_ = LQCrawlerInfo_.Channel_Idx_;
                        pExcelData.goodsSeq_ = pChannelGoodInfo.Idx_;
                        pExcelData.ExData_Option_ = tempString;
                        pExcelData.ExData_OptionOriginal_ = tempString;
                        tRange = ws.Cells[nCurrentRow, ExData_GoodsName];
                        pExcelData.ExData_GoodsName_ = tRange.Value2;
                        pExcelData.goodsCode_ = pChannelGoodInfo.Goods_Code_;

                        tRange = ws.Cells[nCurrentRow, ExData_Coupncode];
                        if (tRange == null)
                            break;

                        pExcelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        if (pExcelData.channelOrderCode_ == null)
                            break;
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Replace("'", "");
                        pExcelData.channelOrderCode_ = pExcelData.channelOrderCode_.Trim();   // 공백 제거

                        tRange = ws.Cells[nCurrentRow, ExData_Buyer];
                        pExcelData.orderName_ = Convert.ToString(tRange.Value2);
                        pExcelData.orderName_ = Regex.Replace(pExcelData.orderName_, @"\((\S+)\)","");
                        pExcelData.orderName_ = pExcelData.orderName_.Trim();
                        if (pExcelData.orderName_ == null) pExcelData.orderName_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Cancel];
                        pExcelData.ExData_Cancel_ = tRange.Value2;
                        if (pExcelData.ExData_Cancel_ == null) pExcelData.ExData_Cancel_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Use];
                        pExcelData.ExData_Use_ = tRange.Value2;
                        if (pExcelData.ExData_Use_ == null) pExcelData.ExData_Use_ = "";

                        tRange = ws.Cells[nCurrentRow, ExData_Buyphone];
                        pExcelData.orderPhone_ = Convert.ToString(tRange.Value2);
                        pExcelData.orderPhone_ = "0"+pExcelData.orderPhone_;
                        if (pExcelData.orderPhone_ == null) pExcelData.orderPhone_ = "";
                        pExcelData.orderPhone_ = Regex.Replace(pExcelData.orderPhone_, @"^(01[016789]{1}|02|0[3-9]{1}[0-9]{1})-?([0-9]{3,4})-?([0-9]{4})$", @"$1-$2-$3");

                        if (ExData_Price != 0)
                        {
                            tRange = ws.Cells[nCurrentRow, ExData_Price];

                            if (tRange.Value2 != null)
                            {// 돈에 , 가 있으면 제거하자.
                                tempString = Convert.ToString(tRange.Value2);
                                tempString = tempString.Replace(",", "");
                                pExcelData.orderSettlePrice_ = Convert.ToInt32(tempString);
                            }
                        }

                        tRange = ws.Cells[nCurrentRow, ExData_BuyDate];
                        DateTime dta = Convert.ToDateTime(tRange.Value2);
                        pExcelData.BuyDate_ = dta.ToString("u");
                        pExcelData.BuyDate_ = pExcelData.BuyDate_.Replace("Z", "");
                        
                        if (ExData_BuyCount != 0)// 구매갯수를 따로 뽑아야 하는 채널에서만
                        {
                            tRange = ws.Cells[nCurrentRow, ExData_BuyCount];
                            pExcelData.BuyCount_ = Convert.ToInt32(tRange.Value2);
                        }

                        SplitDealAndInsertExcelData(pExcelData, comparesitename);

                    }
                    catch (System.Exception ex)
                    {
                        NewLogManager2.Instance.Log(string.Format("엑셀 파싱 에러 : {0}", ex.Message));
                        break;
                        //nCurrentRow++;
                        //continue;
                    }

                    ProcessStateManager.Instance.CurParsingCount_++;
                    nCurrentRow++;
                }

                wb.Close(false, Type.Missing, Type.Missing);
                ap.Quit();

                Marshal.FinalReleaseComObject(ws);
                Marshal.FinalReleaseComObject(wb);
                Marshal.FinalReleaseComObject(ap);
                ws = null;
                wb = null;
                ap = null;
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error protected override bool Internal_Excel_Parsing - {0}", ex.Message));
                return false;
            }

            return true;
        }

        protected override bool Internal_ExcelCancel_Parsing(string filepath)
        {
            try
            {
                Microsoft.Office.Interop.Excel.Application ap = null;
                Workbook wb = null;
                Worksheet ws = null;
                HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

                Range tRange = null;
                Int32 StateColumn = 9;
                Int32 nCurrentRow = 4;
                Int32 CouponColumn = 4;
                Int32 CancelCountColumn = 8;

                while (true)
                {
                    try
                    {
                        tRange = ws.Cells[nCurrentRow, StateColumn];
                        if (tRange == null)
                            break;
                        string StateData = tRange.Value2;
                        if (string.IsNullOrEmpty(StateData) == true)
                            break;
                        StateData = StateData.Trim();
                        if (StateData != "환불")
                        {
                            nCurrentRow++;
                            continue;   //취소된 날짜가 들어와있으면 취소 처리상태
                        }

                        tRange = ws.Cells[nCurrentRow, CouponColumn];
                        if (tRange == null)
                            break;

                        CCancelData pCCancelData = new CCancelData();
                        pCCancelData.channelOrderCode_ = Convert.ToString(tRange.Value2);
                        pCCancelData.State_ = Convert.ToString(tRange.Value2);

                        if (string.IsNullOrEmpty(pCCancelData.channelOrderCode_) == true)
                        {
                            break;
                        }

                        tRange = ws.Cells[nCurrentRow, CancelCountColumn];
                        pCCancelData.CancelCount_ = Convert.ToInt32(tRange.Value2);

                        for (int i = 1; i < pCCancelData.CancelCount_; i++)
                        {
                            CCancelData tempExcelData = new CCancelData();
                            tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pCCancelData.channelOrderCode_, i);
                            tempExcelData.CancelCount_ = 1;
                            Excel_Cancel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
                        }

                            
                    }
                    catch (System.Exception ex)
                    {
                        NewLogManager2.Instance.Log(string.Format("Internal_ExcelCancel_Parsing 엑셀 파싱 에러 : {0}/{1}", filepath, ex.Message));
                        nCurrentRow++;
                        continue;
                    }

                    nCurrentRow++;
                }


                wb.Close(false, Type.Missing, Type.Missing);
                ap.Quit();

                Marshal.FinalReleaseComObject(ws);
                Marshal.FinalReleaseComObject(wb);
                Marshal.FinalReleaseComObject(ap);
                ws = null;
                wb = null;
                ap = null;
                GC.Collect();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error override bool Internal_ExcelCancel_Parsing - {0}", ex.Message));
                return false;
            }

            return true;
        }
        // 하나의 딜을 여러개로 나눌 필요가 있는가? 있다면 나눠서 넣고 없다면 그냥 넣는다.        
        protected override Int32 SplitDealAndInsertExcelData(COrderData pExcelData, string comparesitename = "")
        {
            string optionstring = pExcelData.ExData_Option_;
            Int32 nBuycount = 0;
            Int32 nTotalcount = 0;
            string optionname = "";
            string regstring = @"(?<OptionName>\S+)";
            optionstring = optionstring.Replace(" ", "");
            pExcelData.ExData_GoodsNick_ = Regex.Replace(pExcelData.ExData_GoodsName_, @"[^a-zA-Z0-9가-힣]", "");
            Regex re = new Regex(regstring, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            MatchCollection oe = re.Matches(optionstring);

            foreach (Match mat in oe)
            {
                GroupCollection group = mat.Groups;
                optionname = Convert.ToString(group["OptionName"].Value);
                optionname = Regex.Replace(optionname, @"[^a-zA-Z0-9가-힣]", "");
                nBuycount = pExcelData.BuyCount_;

                for (Int32 i = 0; i < nBuycount; i++)
                {
                    nTotalcount++;
                    COrderData tempExcelData = new COrderData();
                    tempExcelData.CopyFrom(pExcelData);
                    tempExcelData.ExData_Option_ = optionname;
                    tempExcelData.ExData_GoodsName_ = pExcelData.ExData_GoodsName_;
                    tempExcelData.ExData_GoodsNick_ = pExcelData.ExData_GoodsNick_;
                    tempExcelData.ExData_Cancel_ = pExcelData.ExData_Cancel_;
                    tempExcelData.ExData_Use_ = pExcelData.ExData_Use_;
                    tempExcelData.channelOrderCode_ = string.Format("{0}_{1}", pExcelData.channelOrderCode_, nTotalcount);

                    if (Excel_List_.ContainsKey(tempExcelData.channelOrderCode_) == false)
                    {
                        Excel_List_.Add(tempExcelData.channelOrderCode_, tempExcelData);
                    }
                }
            }

            return nTotalcount;
        }

        // 웹에서 사용처리
        public override bool Web_Use()
        {
            try
            {
                ProcessStateManager.Instance.NeedWebProcessCount_ = WebProcess_List_.Count;
                foreach (var pData in WebProcess_List_)
                {
                    if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                    {
                        if (Use_Deal(pData.Value.channelOrderCode_, pData.Value.orderName_, pData.Value.orderPhone_) == true)
                        {
                            CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.USED];
                            DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                            ProcessStateManager.Instance.CurWebProcessCount_++;
                        }
                        else
                        {
                            ProcessStateManager.Instance.FailedWebProcessCount_++;
                        }
                    }
                    else if (pData.Value.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                    {
                        if (Use_Deal(pData.Value.channelOrderCode_, pData.Value.orderName_, pData.Value.orderPhone_) == true)
                        {
                            CrawlerManager.Instance.GetResultData().TotalUseDeal_++;
                            pData.Value.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.A];
                            DBProccess_List_.Add(pData.Value.channelOrderCode_, pData.Value);
                            ProcessStateManager.Instance.CurWebProcessCount_++;
                        }
                        else
                        {
                            ProcessStateManager.Instance.FailedWebProcessCount_++;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool Web_Use() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        public override bool OpenMarketChangeState()
        {
            return true;
        }

        bool GetUseTicketInfo(string couponcode, string userName, string userPhone, ref string ticketcode)
        {
            try
            {
                string strurl = LQCrawlerInfo_.UseGoodsUrl_;
                string strparam = "ctl00%24ctl00%24sm=ctl00%24ctl00%24sm%7Cctl00%24ctl00%24body%24body%24btnSearch&hdRefererUrl=https%3A%2F%2Finvite.cangoto.kr%2Fauth%2Fuserlogin.aspx%3FreUrl%3D%2Fdefault.aspx&sortNM=&sortDirection=&ctl00%24ctl00%24body%24body%24txtUSERNAME={UserName}&ctl00%24ctl00%24body%24body%24txtTELNO={UserPhone}&ctl00%24ctl00%24body%24body%24ddlLine=&ctl00%24ctl00%24body%24body%24ddlOptionValue=&ctl00%24ctl00%24body%24body%24rptList%24ctl00%24ddlType=&__EVENTTARGET=ctl00%24ctl00%24body%24body%24btnSearch&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=%2FwEPDwULLTE2Njg2MTc5NTAPFgIeDFBhZ2VWaWV3RGF0YTLVBwABAAAA%2F%2F%2F%2F%2FwEAAAAAAAAADAIAAABBRVNOZngzLldlYiwgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwFAQAAABhFU05meDMuV2ViLlBhZ2UuVmlld0RhdGEBAAAABV9kYXRhAxxTeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlAgAAAAkDAAAABAMAAAAcU3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0YWJsZQcAAAAKTG9hZEZhY3RvcgdWZXJzaW9uCENvbXBhcmVyEEhhc2hDb2RlUHJvdmlkZXIISGFzaFNpemUES2V5cwZWYWx1ZXMAAAMDAAUFCwgcU3lzdGVtLkNvbGxlY3Rpb25zLklDb21wYXJlciRTeXN0ZW0uQ29sbGVjdGlvbnMuSUhhc2hDb2RlUHJvdmlkZXII7FE4P1IAAAAKCi8AAAAJBAAAAAkFAAAAEAQAAAAQAAAABgYAAAAJQURNUGFzc05PBgcAAAAIUk9MRVR5cGUGCAAAAAdBRE1OYW1lBgkAAAAFRVZFSUQGCgAAAAtTTVNNU0dfVEFJTAYLAAAAB01lc3NhZ2UGDAAAAA5Jc0xvZ2luU3VjY2VzcwYNAAAABVRFTE5PBg4AAAAFRUNPREUGDwAAAAhVc2VyVHlwZQYQAAAABEVBSUQGEQAAAApBRE1Mb2dpbklEBhIAAAAKQURNQWRtaW5OTwYTAAAADE1FVEFfS0VZV09SRAYUAAAACU1FVEFfREVTQwYVAAAACFVzZXJOYW1lEAUAAAAQAAAACgYWAAAAAUQGFwAAADTroa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIExPVFRFIFdPUkxEIE5JR0hUIFBBUlRZCAgjBwAABhgAAAAABhkAAAAG7ZmV7J24CAEBCRgAAAAJGAAAAAkbAAAACAjpBwAABhwAAAAHaGFsbGkwMQYdAAAAB2NhbmdvdG8GHgAAACNCMkIg7KCE7J6Q6rKw7KCcIOydtOyDgeuEpO2KuOybjeyKpAYfAAAAQuuMgO2VnOuvvOq1rSDrjIDtkZxCMkIgTWFya2V0UGxhY2Ug7KCE7J6Q6rKw7KCcIOydtOyDgeuEpO2KuOybjeyKpAkXAAAADCEAAAA%2FQXBwX0NvZGUsIFZlcnNpb249MC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxsBRsAAAAIVXNlclR5cGUBAAAAB3ZhbHVlX18ACCEAAAACAAAACxYCZg9kFgJmD2QWAgIDD2QWAgIDD2QWBmYPFgIeCWlubmVyaHRtbAU%2B66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCA8YnIvPkxPVFRFIFdPUkxEIE5JR0hUIFBBUlRZPGJyLz5kAgEPFgIfAQUhMTIuMTko6riIKSB%2BIDEyLjE5KOq4iCkgLyAx7J286rCEZAICD2QWBgIFDxBkEBUCBuyEoO2DnS8g66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRUCAC9A66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRQrAwJnZ2RkAgcPEGQQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnZGQCDQ9kFgJmD2QWBgIBDxYCHgdWaXNpYmxlaBYCAgEPFgIeC18hSXRlbUNvdW50AgEWAmYPZBYCZg8VAwoyMDE1LTAxLTA1ATIGMzQsMDAwZAIDDxYCHwMCARYCZg9kFgRmDxUJLOyEnOycpOqyvSAgICAgICAgICAgICAgICAgICAgICAgICjshJzsnKTqsr0pCzAxMDg4NjY0MzMyDTExODI3MjU3NDgzMzIZMjAxNC0xMi0xOSDsmKTtm4QgMjoxNTo1Mx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7JiI7JW9ZAIBDxAPFgQeB1Rvb2xUaXAFBTY4MDQxHgtfIURhdGFCb3VuZGcWAh4Ib25jaGFuZ2UFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIFDxYCHwJoZGSdXqnQ7Upx7mt2v2BInSnRRnkSJQ%3D%3D&__VIEWSTATEGENERATOR=FA3FC143&__ASYNCPOST=true&";




                userPhone = userPhone.Replace("-", "");

                strparam = strparam.Replace("{UserName}", userName);
                strparam = strparam.Replace("{UserPhone}", userPhone);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", strurl, strparam, Cookie_, null,
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36");

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();
                htmlBuffer = htmlBuffer.Replace(" ", "");
                if (htmlBuffer.IndexOf(LQCrawlerInfo_.UseGoodsCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;
                }
                string[] cp_sub = couponcode.Split('_');
                couponcode = cp_sub[0];

                LQCrawlerInfo_.UseGoodsRule_ = LQCrawlerInfo_.UseGoodsRule_.Replace("{CouponCode}", couponcode);
                Regex re = new Regex(LQCrawlerInfo_.UseGoodsRule_, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                MatchCollection oe = re.Matches(htmlBuffer);

                ticketcode = oe[0].Groups["TicketCode"].ToString();
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool GetUseTicketInfo - {0}", ex.Message));
                return false;
            }

            return true;
        }

        bool Use_Deal(string cpcode, string userName, string userPhone)
        {
            try
            {
                string useurl = LQCrawlerInfo_.UseUserUrl_;
                string useparam = LQCrawlerInfo_.UseUserParam_;

                string ticketCode = "";

                GetUseTicketInfo(cpcode, userName, userPhone, ref ticketCode);

                useparam = useparam.Replace("{TicketCode}", ticketCode);

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("POST", useurl, useparam, Cookie_, "",
                    "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36", 60000, "json");

                if (pResponse == null)
                    return false;

                TextReader r = (TextReader)new StreamReader(pResponse.GetResponseStream());
                string htmlBuffer = r.ReadToEnd();

                if (htmlBuffer.IndexOf(LQCrawlerInfo_.UseUserCheck_) < 0)
                {
                    NewLogManager2.Instance.Log(htmlBuffer);
                    return false;

                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error bool Use_Deal( 사용처리 에러남 - {0}", ex.Message));
                return false;
            }

            return true;
        }

        // 웹에서 사용처리 해야 할게 있는지 체크
        public override bool CheckNeedUseWeb()
        {
            try
            {
                foreach (var pData in Excel_List_)
                {
                    ChannelGoodInfo pInfo = null;

                    if (LQCrawlerInfo_.ExData_GoodName_ == 0)
                        pInfo = GetGoodInfoByGoodCodeAndOptionName(pData.Value.goodsCode_, pData.Value.ExData_Option_);
                    else
                        pInfo = GetGoodInfoByGoodOptionName(pData.Value.ExData_GoodsName_, pData.Value.ExData_Option_);

                    if (pInfo == null)
                        continue;

                    pData.Value.ExData_GoodsName_ = pInfo.GoodsName_;
                    pData.Value.goodsSeq_ = pInfo.Idx_;
                    pData.Value.goodsCode_ = pInfo.Goods_Code_;

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        COrderData pDBData = DBSelected_List_[pData.Value.channelOrderCode_];
                        if (pData.Value.State_ == pDBData.State_)
                            continue;

                        // 레저큐에서 예약을 완료한 상태 웹에 사용 처리를 해야한다.
                        if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.FINISH_RESERVED])
                        {
                            pDBData.BuyDate_ = pData.Value.BuyDate_;
                            WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                        else if (pDBData.State_ == DealStateManager.Instance.StateString_[(Int32)DealStateEnum.AR])
                        {
                            pDBData.BuyDate_ = pData.Value.BuyDate_;
                            WebProcess_List_.Add(pDBData.channelOrderCode_, pDBData);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckNeedUseWeb() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        public override bool CheckIsCancel()
        {
            try
            {
                foreach (var pData in Excel_Cancel_List_)
                {
                    if (pData.Value.State_ != "취소완료")
                        continue;

                    if (DBSelected_List_.ContainsKey(pData.Key) == true)
                    {
                        if (DBProccess_List_.ContainsKey(pData.Key) == true)
                        {
                            NewLogManager2.Instance.Log(string.Format("CheckIsCancel DB 처리에 두가지가 다 들어가 있다.{0}", pData.Key));
                            continue;
                        }

                        COrderData pCOrderData = DBSelected_List_[pData.Key];

                        if (pCOrderData.State_ != DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL])
                        {
                            pCOrderData.State_ = DealStateManager.Instance.StateString_[(Int32)DealStateEnum.CANCEL];
                            DBCancel_List_.Add(pCOrderData.channelOrderCode_, pCOrderData);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error public override bool CheckIsCancel() - {0}", ex.Message));
                return false;
            }

            return true;
        }

        // 취소 엑셀 파싱해서 리스트에 담자.
        public override bool ExcelParsing_Cancel()
        {
            Dictionary<string, string> DoneList_ = new Dictionary<string, string>();

            foreach (var pData in CancelDownInfo_)
            {
                if (DoneList_.ContainsKey(pData.Key) == false)
                {
                    Internal_ExcelCancel_Parsing(pData.Value);

                    DoneList_.Add(pData.Key, pData.Key);
                }
            }

            return true;
        }

        public override bool Web_DownLoad_CancelList()
        {
            try
            {
                DateTime dtNow = DateTime.Now;
                DateTime beforeData = dtNow.AddDays(-31);
                string eDate = string.Format("{0:D4}-{1:D2}-{2:D2}", dtNow.Year, dtNow.Month, dtNow.Day);
                string sDate = string.Format("{0:D4}-{1:D2}-{2:D2}", beforeData.Year, beforeData.Month, beforeData.Day);
                
                string method = "GET";
                string url = @"https://invite.cangoto.kr/manager/ListSupply_THEATER.aspx";
                string param = @"hdRefererUrl=https%3A%2F%2Finvite.cangoto.kr%2Fauth%2Fuserlogin.aspx%3FreUrl%3D%2Fdefault.aspx&sortNM=&sortDirection=&ctl00%24ctl00%24body%24body%24txtUSERNAME=&ctl00%24ctl00%24body%24body%24txtTELNO=&ctl00%24ctl00%24body%24body%24ddlLine=&ctl00%24ctl00%24body%24body%24ddlOptionValue=R&ctl00%24ctl00%24body%24body%24rptList%24ctl00%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl01%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl02%24ddlType=&ctl00%24ctl00%24body%24body%24rptList%24ctl03%24ddlType=&__EVENTTARGET=ctl00%24ctl00%24body%24body%24btnexcel&__EVENTARGUMENT=&__LASTFOCUS=&__VIEWSTATE=%2FwEPDwULLTE2Njg2MTc5NTAPFgIeDFBhZ2VWaWV3RGF0YTLVBwABAAAA%2F%2F%2F%2F%2FwEAAAAAAAAADAIAAABBRVNOZngzLldlYiwgVmVyc2lvbj0xLjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPW51bGwFAQAAABhFU05meDMuV2ViLlBhZ2UuVmlld0RhdGEBAAAABV9kYXRhAxxTeXN0ZW0uQ29sbGVjdGlvbnMuSGFzaHRhYmxlAgAAAAkDAAAABAMAAAAcU3lzdGVtLkNvbGxlY3Rpb25zLkhhc2h0YWJsZQcAAAAKTG9hZEZhY3RvcgdWZXJzaW9uCENvbXBhcmVyEEhhc2hDb2RlUHJvdmlkZXIISGFzaFNpemUES2V5cwZWYWx1ZXMAAAMDAAUFCwgcU3lzdGVtLkNvbGxlY3Rpb25zLklDb21wYXJlciRTeXN0ZW0uQ29sbGVjdGlvbnMuSUhhc2hDb2RlUHJvdmlkZXII7FE4PyIAAAAKCi8AAAAJBAAAAAkFAAAAEAQAAAAQAAAABgYAAAAJQURNUGFzc05PBgcAAAAFVEVMTk8GCAAAAAhST0xFVHlwZQYJAAAABUVWRUlEBgoAAAAIVXNlck5hbWUGCwAAAA5Jc0xvZ2luU3VjY2VzcwYMAAAAB0FETU5hbWUGDQAAAAVFQ09ERQYOAAAAC1NNU01TR19UQUlMBg8AAAAIVXNlclR5cGUGEAAAAARFQUlEBhEAAAAMTUVUQV9LRVlXT1JEBhIAAAAKQURNTG9naW5JRAYTAAAACkFETUFkbWluTk8GFAAAAAdNZXNzYWdlBhUAAAAJTUVUQV9ERVNDEAUAAAAQAAAACgYWAAAAAAYXAAAAAUQICCMHAAAGGAAAADTroa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIExPVFRFIFdPUkxEIE5JR0hUIFBBUlRZCAEBCRgAAAAJFgAAAAkWAAAACRoAAAAICOkHAAAGGwAAACNCMkIg7KCE7J6Q6rKw7KCcIOydtOyDgeuEpO2KuOybjeyKpAYcAAAAB2hhbGxpMDEGHQAAAAdjYW5nb3RvBh4AAAAG7ZmV7J24Bh8AAABC64yA7ZWc66%2B86rWtIOuMgO2RnEIyQiBNYXJrZXRQbGFjZSDsoITsnpDqsrDsoJwg7J207IOB64Sk7Yq47JuN7IqkDCAAAAA%2FQXBwX0NvZGUsIFZlcnNpb249MC4wLjAuMCwgQ3VsdHVyZT1uZXV0cmFsLCBQdWJsaWNLZXlUb2tlbj1udWxsBRoAAAAIVXNlclR5cGUBAAAAB3ZhbHVlX18ACCAAAAACAAAACxYCZg9kFgJmD2QWAgIDD2QWAgIDD2QWBmYPFgIeCWlubmVyaHRtbAU%2B66Gv642w7JuU65OcIOuCmOydtO2KuO2MjO2LsCA8YnIvPkxPVFRFIFdPUkxEIE5JR0hUIFBBUlRZPGJyLz5kAgEPFgIfAQUhMTIuMTko6riIKSB%2BIDEyLjE5KOq4iCkgLyAx7J286rCEZAICD2QWBgIFDxBkEBUCBuyEoO2DnS8g66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRUCAC9A66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KRQrAwJnZ2RkAgcPEGQQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnZGQCDQ9kFgJmD2QWBgIBDxYCHgdWaXNpYmxlaBYCAgEPFgIeC18hSXRlbUNvdW50AgEWAmYPZBYCZg8VAwoyMDE1LTAxLTA1ATIGMzQsMDAwZAIDDxYCHwMCBBYIZg9kFgRmDxUJCyjsnbTsp4Tso7wpCzAxMDI5NjYyMzYzDTExODI3MTg4NDYzNjMZMjAxNC0xMi0xOCDsmKTtm4QgMzowMjozMx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7ZmY67aIZAIBDxAPFgQeB1Rvb2xUaXAFBTY3NzkyHgtfIURhdGFCb3VuZGcWAh4Ib25jaGFuZ2UFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIBD2QWBGYPFQks7ZmN66%2B47KCVICAgICAgICAgICAgICAgICAgICAgICAgKO2ZjeuvuOyglSkLMDEwNTU4MzU3MTYNMTE4Mjc2MjUwMTcxNhkyMDE0LTEyLTE3IOyYpOyghCAzOjQ3OjA3Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQEyBjM0LDAwMAbtmZjrtohkAgEPEA8WBB8EBQU2NzYwMh8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAICD2QWBGYPFQks6rmA7IiY6rK9ICAgICAgICAgICAgICAgICAgICAgICAgKOq5gOyImOqyvSkLMDEwOTA3NzM1NTkNMTE4Mjc2NjIyMDU1ORkyMDE0LTEyLTEyIOyYpO2bhCA5OjE4OjQ4Heuhr%2BuNsOyblOuTnCDrgpjsnbTtirjtjIzti7AgMCAg66Gv642w7JuU65OcIE5pZ2h0IFBhcnR5IOyekOycoOydtOyaqeq2jCgx7J24KQExBjE3LDAwMAbtmZjrtohkAgEPEA8WBB8EBQU2NDI5OR8FZxYCHwYFInJldHVybiBmblZhbHVlQ2hhbmdlQ29uZmlybSh0aGlzKTsQFQUG7ISg7YOdBuyYiOyVvQbssLjshJ0G67aI7LC4Bu2ZmOu2iBUFAAFOAUYBWAFSFCsDBWdnZ2dnFgFmZAIDD2QWBGYPFQks7LWc64Ko66%2B4ICAgICAgICAgICAgICAgICAgICAgICAgKOy1nOuCqOuvuCkLMDEwMjQwNjgxODkNMTE4MjczNzIyODE4ORoyMDE0LTEyLTEyIOyYpOyghCAxMDo1NDoyNx3roa%2FrjbDsm5Trk5wg64KY7J207Yq47YyM7YuwIDAgIOuhr%2BuNsOyblOuTnCBOaWdodCBQYXJ0eSDsnpDsnKDsnbTsmqnqtowoMeyduCkBMgYzNCwwMDAG7ZmY67aIZAIBDxAPFgQfBAUFNjQwNDEfBWcWAh8GBSJyZXR1cm4gZm5WYWx1ZUNoYW5nZUNvbmZpcm0odGhpcyk7EBUFBuyEoO2DnQbsmIjslb0G7LC47ISdBuu2iOywuAbtmZjrtogVBQABTgFGAVgBUhQrAwVnZ2dnZxYBZmQCBQ8WAh8CaGRkqSvjSxdjO6yENmFgL9vYohaxWrY%3D&__VIEWSTATEGENERATOR=FA3FC143";


                string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
                makefolder += "\\";
                makefolder += CINIManager.Instance.channelseq_;
                makefolder += "\\";
                makefolder += dtNow.ToShortDateString();
                HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);

                /*string sendparam = param.Replace("{sDate}", sDate);
                sendparam = sendparam.Replace("{eDate}", eDate);*/
                string downString = string.Format(@"{0}\Cancel_{1}.xls"
                    , makefolder, Convert.ToString(dtNow.Ticks));

                HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest(method, url, param, Cookie_, null, null, 180000);

                if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr" || pResponse.CharacterSet == "EUC-KR" || pResponse.CharacterSet == "KSC5601")
                {
                    FileStream fs = File.OpenWrite(downString);

                    string d = pResponse.CharacterSet;
                    Stream responsestream = pResponse.GetResponseStream();
                    byte[] buffer = new byte[2048];

                    long totalBytesRead = 0;
                    int bytesRead;

                    while ((bytesRead = responsestream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        totalBytesRead += bytesRead;
                        fs.Write(buffer, 0, bytesRead);
                    }
                    fs.Close();
                    fs.Dispose();
                }
                else
                {
                    TextReader textReader = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding(pResponse.CharacterSet));
                    string htmlBuffer = textReader.ReadToEnd();
                    HKLibrary.UTIL.HKFileHelper.SaveToFile(downString, htmlBuffer);
                    textReader.Close();
                    textReader.Dispose();
                }

                CancelDownInfo_.Add("CANCEL", downString);

            }
            catch (System.Exception ex)
            {
                NewLogManager2.Instance.Log(string.Format("Error Web_DownLoad_CancelList {0}", ex.Message));
                return false;
            }

            return true;
        
        }
    }
}

