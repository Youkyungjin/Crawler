using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HK.Database;
using MySql.Data.MySqlClient;
using LQStructures;
using TMS.Common;
using TMS;

namespace LQCrawlerManager.Data
{
    class InfoManager : BaseSingleton<InfoManager>
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

        // 기타 관련 
        public Int32 checktick_ = -1;


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

                string str = ic.GetIniValue2("ETC", "checktick");
                checktick_ = Convert.ToInt32(str);
            }
            catch
            {
                bResult = false;
            }

            return bResult;
        }

    }

    class AppManager : BaseSingleton<AppManager>
    {
        public Dictionary<Int32, LQChannelInfo> ChannelInfo_List_ = new Dictionary<Int32, LQChannelInfo>();
        System.Windows.Forms.Timer UI_Timer_ = new System.Windows.Forms.Timer();

        LQCrawlerManger pForm_ = null;

        public Dictionary<Int32, AsyncSocketClient> Crawler_Connection_List_ = new Dictionary<Int32, AsyncSocketClient>();
        public Dictionary<Int32, AsyncSocketClient> Checker_Connection_List_ = new Dictionary<Int32, AsyncSocketClient>();

        AsyncSocketServer CrawlerServer_;
        AsyncSocketServer CheckerServer_;

        //Int32 PartnerIdx_ = -1;
        Int32 SocketID_ = 0;


        public void SetForm(LQCrawlerManger pForm)
        {
            pForm_ = pForm;
        }

        public void AddLog(string str)
        {
            pForm_.AddLog(str);
        }

        public void SelectChannelInfos()
        {
            ChannelInfo_List_.Clear();
            SqlHelper pMySqlDB = new SqlHelper();

            pMySqlDB.Connect(InfoManager.Instance.method_, InfoManager.Instance.dbip_, InfoManager.Instance.dbport_, InfoManager.Instance.dbname_
                , InfoManager.Instance.dbaccount_, InfoManager.Instance.dbpw_, InfoManager.Instance.sshhostname_, InfoManager.Instance.sshuser_
                , InfoManager.Instance.sshpw_);

            MySqlDataReader datareader = pMySqlDB.call_proc("sp_select_All_Crawler_Info", null);

            while (datareader.Read())
            {
                LQChannelInfo pChannelInfo = new LQChannelInfo();
                pChannelInfo.nIdx_ = Convert.ToInt32(datareader["idx"]);
                pChannelInfo.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);
                pChannelInfo.PartnerName_ = Convert.ToString(datareader["PartnerName"]);
                pChannelInfo.Channel_Idx_ = Convert.ToInt32(datareader["Channel_Idx"]);
                pChannelInfo.Channel_Name_ = Convert.ToString(datareader["Channel_Name"]);
                ChannelInfo_List_.Add(pChannelInfo.nIdx_, pChannelInfo);
            }
        }

        public LQChannelInfo GetChannelInfo(Int32 idx)
        {
            if (ChannelInfo_List_.ContainsKey(idx) == false)
                return null;

            return ChannelInfo_List_[idx];
        }

        public System.Windows.Forms.Timer GetUITimer()
        {
            return UI_Timer_;
        }

        public void ServerStart()
        {
            CrawlerServer_ = new AsyncSocketServer(20001);
            CrawlerServer_.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptCrawler);
            CrawlerServer_.Listen();

            CheckerServer_ = new AsyncSocketServer(20002);
            CheckerServer_.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptChecker);
            CheckerServer_.Listen();
        }

        private void OnAcceptCrawler(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient worker = new AsyncSocketClient(SocketID_++, e.Worker);

            // 데이터 수신을 대기한다.
            worker.Receive();
            worker.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            worker.OnClose += new AsyncSocketCloseEventHandler(OnCloseCrawler);
            worker.OnError += new AsyncSocketErrorEventHandler(OnErrorCrawler);
            worker.OnSend += new AsyncSocketSendEventHandler(OnSend);
            worker.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
        }

        private void OnAcceptChecker(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient worker = new AsyncSocketClient(SocketID_++, e.Worker);

            // 데이터 수신을 대기한다.
            worker.Receive();
            worker.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            worker.OnClose += new AsyncSocketCloseEventHandler(OnCloseChecker);
            worker.OnError += new AsyncSocketErrorEventHandler(OnErrorChecker);
            worker.OnSend += new AsyncSocketSendEventHandler(OnSend);
            worker.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
        }

        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            //AppManager.Instance.AddLog(Convert.ToString(e.ID));
        }

        private void OnCloseCrawler(object sender, AsyncSocketConnectionEventArgs e)
        {
            AppManager.Instance.AddLog(string.Format("크롤러 접속 끊김 {0}", e.ID));
            foreach (var pData in Crawler_Connection_List_)
            {
                if (pData.Value == sender)
                {
                    ChannelInfo_List_[pData.Key].connected_ip_ = "";
                    ChannelInfo_List_[pData.Key].crawler_status_ = "접속 끊김";
                    Crawler_Connection_List_.Remove(pData.Key);
                    AppManager.Instance.AddLog(string.Format("Crawler Close {0}", pData.Key));
                    break;
                }
            }
        }

        private void OnErrorCrawler(object sender, AsyncSocketErrorEventArgs e)
        {
            AppManager.Instance.AddLog(string.Format("크롤러 에러남 {0}", e.ID));
            foreach (var pData in Crawler_Connection_List_)
            {
                if (pData.Value == sender)
                {
                    ChannelInfo_List_[pData.Key].connected_ip_ = "";
                    ChannelInfo_List_[pData.Key].crawler_status_ = "접속 끊김";
                    Crawler_Connection_List_.Remove(pData.Key);
                    AppManager.Instance.AddLog(string.Format("Crawler Error Close {0}", pData.Key));
                    break;
                }
            }
        }

        private void OnCloseChecker(object sender, AsyncSocketConnectionEventArgs e)
        {
            AppManager.Instance.AddLog(string.Format("체커 접속 끊김 {0}", e.ID));
            foreach (var pData in Checker_Connection_List_)
            {
                if (pData.Value == sender)
                {
                    //ChannelInfo_List_[pData.Key].connected_ip_ = "";
                    ChannelInfo_List_[pData.Key].checker_status_ = "접속 끊김";
                    Checker_Connection_List_.Remove(pData.Key);
                    AppManager.Instance.AddLog(string.Format("Checker Close {0}", pData.Key));
                    break;
                }
            }
        }

        private void OnErrorChecker(object sender, AsyncSocketErrorEventArgs e)
        {
            AppManager.Instance.AddLog(string.Format("체커 에러남 {0}", e.ID));
            foreach (var pData in Checker_Connection_List_)
            {
                if (pData.Value == sender)
                {
                    //ChannelInfo_List_[pData.Key].connected_ip_ = "";
                    ChannelInfo_List_[pData.Key].checker_status_ = "접속 끊김";
                    Checker_Connection_List_.Remove(pData.Key);
                    AppManager.Instance.AddLog(string.Format("Checker Error Close {0}", pData.Key));
                    break;
                }
            }
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            AsyncSocketClient psocket = (AsyncSocketClient)sender;
            //AppManager.Instance.AddLog(Convert.ToString(e.ReceiveBytes));
            Int16 len = System.BitConverter.ToInt16(e.ReceiveData,0);
            if (len != e.ReceiveBytes)
            {
                AppManager.Instance.AddLog("패킷의 길이가 달라요");
                return;
            }
            
            PACKET_IDX pidx = (PACKET_IDX)System.BitConverter.ToChar(e.ReceiveData, 2);

            switch (pidx)
            {
                case PACKET_IDX.CM_CHANNEL_IDX:
                    {
                        C_TO_M_CHANNEL_IDX p = new C_TO_M_CHANNEL_IDX();
                        PacketProcess.Deserialize(p, e.ReceiveData);

                        if (Crawler_Connection_List_.ContainsKey(p.nIdx) == false)
                        {
                            Crawler_Connection_List_.Add(p.nIdx, psocket);
                        }

                        ChannelInfo_List_[p.nIdx].connected_ip_ = psocket.Connection.RemoteEndPoint.ToString();
                        ChannelInfo_List_[p.nIdx].crawler_status_ = "연결됨";
                    }
                    break;
                case PACKET_IDX.KM_CHANNEL_IDX:
                    {
                        K_TO_M_CHANNEL_IDX p = new K_TO_M_CHANNEL_IDX();
                        PacketProcess.Deserialize(p, e.ReceiveData);

                        if (Checker_Connection_List_.ContainsKey(p.nIdx) == false)
                        {
                            Checker_Connection_List_.Add(p.nIdx, psocket);
                        }

                        ChannelInfo_List_[p.nIdx].checker_status_ = "연결됨";
                    }
                    break;
                default:
                    {
                        AppManager.Instance.AddLog("잘못된 패킷이 도착했습니다.");
                    }
                    break;
            }
        }
    }
}
