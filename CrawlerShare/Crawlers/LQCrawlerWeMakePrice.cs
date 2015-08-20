using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;

using HKLibrary.WEB;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;

using HKLibrary.Excel;
using HK.Database;
using LQStructures;
using System.Text.RegularExpressions;

namespace CrawlerShare
{
    public class LQCrawlerWeMakePrice : LQCrawlerBase
    {
        bool DownRefundList(string downfile, string goodscode)
        {
            // 웹 호출을 통해서 사용처리한다.
            LQCrawlerInfo pCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();
            string useurl = @"http://biz.wemakeprice.com/dealer/deal_list/do_xsl_download/{GoodsCode}/1";
            string useparam = "";

            useparam = useparam.Replace("{GoodsCode}", goodscode);

            HttpWebResponse pResponse = HKHttpWebRequest.ReqHttpRequest("GET", useurl, useparam, cookie_);

            if (pResponse == null)
                return false;

            if (pResponse.CharacterSet == "" || pResponse.CharacterSet == "euc-kr")
            {
                FileStream fs = File.OpenWrite(downfile);

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
            }
            else
            {
                TextReader textReader = (TextReader)new StreamReader(pResponse.GetResponseStream(), Encoding.GetEncoding(pResponse.CharacterSet));
                string htmlBuffer = textReader.ReadToEnd();
                HKLibrary.UTIL.HKFileHelper.SaveToFile(downfile, htmlBuffer);
            }

            return true;
        }
    }
}
