using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;

using TMS.Common;
using HKLibrary;
using TMS;
using LQStructures;
using CrawlerShare;

namespace LQCrawlerChecker
{
    public partial class CrawlerChecker : Form
    {
        // INI 파일 읽어온곳.
        private bool bLoaded_ = false;      // INI 파일 제대로 로딩 했는지 여부
        private string ExePath_ = "";       // 프로그램 실행 경로
        private string ProcessName_ = "";   // 프로세스 이름
        private Int32 CheckTime_ = -1;      // 체크할 Term 
        private Int32 crawleridx_ = -1;     // 크롤러 인덱스
        private Int32 nchannelidx_ = -1;    // 채널 인덱스
        private string manager_ip_ = "";    // 매니저 아이피
        private Int32 manager_port_ = 0;    // 매니저 포트
        private Int32 ListenCrawlerPort_ = 0;   // 크롤러의 접속을 기다리는 포트
        private Int32 WaitCrawlerTime_ = 0;     // 크롤러의 크롤링이 걸리는 최대 시간 이시간동안 끝났다고 통지가 오지 않으면 재시작한다.

        // 내부 변수
        private Int32 nLastCheckTime_ = 0;  // 마지막으로 체크한 시간.
        private Int32 CheckProcessID_ = -1; // 크롤러의 프로세스 아이디, 종료할때 필요하다.
        private Int32 HeartBeatTime_ = -1;  // 크롤러가 heartbeat 패킷을 보내온 시간
        private Int32 CrawlingCount_ = 0;   // 크롤러가 보내온 크롤링 횟수
        private Int32 CrawlingCountChangeTime_ = 0; // 크롤러가 보내온 크롤링 회수가 변한 시간.

        System.Windows.Forms.Timer Check_Timer_ = new System.Windows.Forms.Timer();

        
        public CrawlerChecker()
        {
            InitializeComponent();
            LogManager.Instance.SetLogFile("CrawlerChecker.txt");
            LogManager.Instance.Log("체커실행~");
            InitIni();
            InitConnectionWork();
            ServerStart();
            InitTimer();
            RefreshLabels();
        }

        private bool InitIni()
        {
            try
            {
                string inifilepath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + "/LQCrawlerChecker.INI";
                INIControlor ic = new INIControlor(inifilepath);
                ProcessName_ = ic.GetIniValue2("Checker", "processname");
                ExePath_ = ic.GetIniValue2("Checker", "exefullpath");
                string tempstr = ic.GetIniValue2("Checker", "CheckTime");
                CheckTime_ = Convert.ToInt32(tempstr);
                tempstr = ic.GetIniValue2("Checker", "crawleridx");
                crawleridx_ = Convert.ToInt32(tempstr);
                tempstr = ic.GetIniValue2("Checker", "channelidx");
                nchannelidx_ = Convert.ToInt32(tempstr);

                tempstr = ic.GetIniValue2("Checker", "ListenCrawlerPort");
                ListenCrawlerPort_ = Convert.ToInt32(tempstr);

                tempstr = ic.GetIniValue2("Checker", "WaitCrawlerTime");
                WaitCrawlerTime_ = Convert.ToInt32(tempstr); 

                // 매니저쪽 정보 로드
                manager_ip_ = ic.GetIniValue2("Manager", "ip");
                tempstr = ic.GetIniValue2("Manager", "port");
                manager_port_ = Convert.ToInt32(tempstr);

                bLoaded_ = true;
                return true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message);
                bLoaded_ = false;
                return false;
            }
        }

        private void InitTimer()
        {
            Check_Timer_.Tick += new EventHandler(CheckTimer_Function);
            Check_Timer_.Interval = 1000;
            Check_Timer_.Enabled = true;
            nLastCheckTime_ = Environment.TickCount;
        }

        private void RefreshLabels()
        {
            if (bLoaded_ == false)
            {
                label_NextCheck.Text = "재시작 불가 다시 INI 체크후 다시 실행 바람.";
            }
            else
            {
                label_FilePath.Text = ExePath_;
                label_NextCheck.Text = "대기중..";
            }
        }

        private void button_StartStop_Click(object sender, EventArgs e)
        {
            if (bLoaded_ == false)
            {
                MessageBox.Show("ini 파일 로딩이 실패 했습니다. 확인후 프로그램을 다시 시작해 주세요.");
                return;
            }
            else
            {
                CheckAndRunProgram();
            }
        }

        void CheckTimer_Function(object sender, EventArgs e)
        {
            Check_Timer_.Enabled = false;
            
            CheckAndRunProgram();

            Check_Timer_.Enabled = true;
        }

        bool RestartCrawler(Int32 nCurrentTick)
        {
            if (CheckProcessID_ > 0)
            {
                if (HK.Util.HKProgramExecuter.StopProgramByProcessID(CheckProcessID_) == false)
                {
                    LogManager.Instance.Log(string.Format("RestartCrawler 에서 CheckProcessID_ 가 있으나 실제 프로세스는 없었습니다.{0}", CheckProcessID_));
                }
            }

            HK.Util.ExecuteResult pResult = HK.Util.HKProgramExecuter.StartProgram(ExePath_);
            if (pResult.ID < 0) // 실패
            {
                label_NextCheck.Text = "재시작을 실패 했습니다.";
                return false;
            }

            CheckProcessID_ = pResult.ID;
            HeartBeatTime_ = nCurrentTick;
            CrawlingCount_ = 0;
            CrawlingCountChangeTime_ = nCurrentTick;
            return true;
        }

        void CheckAndRunProgram()
        {
            if (bLoaded_ == true)
            {
                Int32 nCurrentTick = Environment.TickCount;
                Int32 nPassTime = nCurrentTick - nLastCheckTime_;

                // 마지막으로 크롤링이 됬던 시간을 지금으로 변경해준다.
                if (CrawlingCount_ == 0 && CrawlingCountChangeTime_ == 0)
                {
                    CrawlingCountChangeTime_ = nCurrentTick;
                }

                
                if (nPassTime >= CheckTime_)
                {
                    label_NextCheck.Text = "체크중";

                    if (CheckProcessID_ < 0)
                    {// 자신이 실행한 프로세스 아이디가 없는경우.

                        if (RestartCrawler(nCurrentTick) == false)
                            return;
                    }
                    else
                    {// 자신이 실행한 프로세스 아이디가 있는경우. 1분이상 heartbeat 이 오지 않았다면, 끊겼거나 문제가 생겼을것이다. kill 하고 재시작 하자.

                        // HeartBeat 패킷이 너무 늦고 있다. 재시작 하자.
                        if (nCurrentTick - HeartBeatTime_ > 1000 * 30)
                        {
                            LogManager.Instance.Log("HeartBeat 패킷이 오지않음 다시 시작한다.");
                            if (RestartCrawler(nCurrentTick) == false)
                                return;                            
                        }
                        else
                        {// 마지막으로 크롤링 횟수가 올라간 시간이 너무 오래 되었다면(10 분으로 하자) 재시작 하는게 맞다. 

                            if (nCurrentTick - CrawlingCountChangeTime_ > WaitCrawlerTime_)
                            {
                                LogManager.Instance.Log("크롤링 횟수가 너무 오랫동안 그대로임 다시 시작한다.");
                                if (RestartCrawler(nCurrentTick) == false)
                                    return;                            
                            }
                        }
                    }

                    nLastCheckTime_ = Environment.TickCount;
                }
                else
                {
                    Int32 nLeft = CheckTime_ - nPassTime;
                    label_NextCheck.Text = string.Format("{0:F1} 초 후 체크", nLeft / 1000);
                }
            }
            else
            {
                label_NextCheck.Text = "재시작 불가 다시 INI 체크후 다시 실행 바람.";
            }
        }

        BackgroundWorker connection_worker_;
        AsyncSocketClient manager_socket_;


        AsyncSocketServer CrawlerServer_;   // 크롤러의 접속을 대기하는 서버
        Int32 SocketID_ = 0;

        void InitConnectionWork()
        {
            LogManager.Instance.Log("void InitConnectionWork()");
            connection_worker_ = new BackgroundWorker();
            connection_worker_.WorkerReportsProgress = false;
            connection_worker_.WorkerSupportsCancellation = true;
            connection_worker_.DoWork += new DoWorkEventHandler(Connection_Function);
            connection_worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConnectionWorker_Completed);
        }       

        public void ServerStart()
        {
            try
            {
                CrawlerServer_ = new AsyncSocketServer(ListenCrawlerPort_);
                CrawlerServer_.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptCrawler);
                CrawlerServer_.Listen();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("크롤러 리슨 서버 시작 실패 {0}", ex.Message));
            }
        }

        // 크롤러가 접속 됐을때 호출되는 callback
        private void OnAcceptCrawler(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient worker = new AsyncSocketClient(SocketID_++, e.Worker);

            // 데이터 수신을 대기한다.
            worker.Receive();
            worker.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            worker.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            worker.OnError += new AsyncSocketErrorEventHandler(OnError);
            worker.OnSend += new AsyncSocketSendEventHandler(OnSend);
            worker.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
        }

        private void ServerConnect()
        {
            manager_socket_ = new AsyncSocketClient(0);

            // 이벤트 핸들러 재정의
            manager_socket_.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            manager_socket_.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            manager_socket_.OnSend += new AsyncSocketSendEventHandler(OnSend);
            manager_socket_.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            manager_socket_.OnError += new AsyncSocketErrorEventHandler(OnError);
            manager_socket_.Connect(manager_ip_, manager_port_);
        }

        void Connection_Function(object sender, DoWorkEventArgs e)
        {
            bool bNeedConnection = false;
            if (manager_socket_ == null)
            {
                bNeedConnection = true;
            }
            else
            {
                if (manager_socket_.Connection == null)
                {
                    bNeedConnection = true;
                }
                else
                {
                    if (manager_socket_.Connection.Connected == false)
                    {
                        bNeedConnection = true;
                    }
                    else
                    {
                        K_TO_M_CHANNEL_IDX p = new K_TO_M_CHANNEL_IDX();
                        p.num = (byte)PACKET_IDX.KM_CHANNEL_IDX;
                        p.len = (Int16)Marshal.SizeOf(p);
                        p.nIdx = crawleridx_;
                        //p.nChannelIdx = InfoManager.Instance.channelidx_;
                        byte[] sendbuffer = new byte[p.len];
                        PacketProcess.Serialize(p, sendbuffer);
                        ((AsyncSocketClient)sender).Send(sendbuffer);
                    }
                }
            }

            if (bNeedConnection == true)
            {
                ServerConnect();
            }
        }

        void ConnectionWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                return;
            }
        }

        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            K_TO_M_CHANNEL_IDX p = new K_TO_M_CHANNEL_IDX();
            p.num = (byte)PACKET_IDX.KM_CHANNEL_IDX;
            p.len = (Int16)Marshal.SizeOf(p);
            p.nIdx = crawleridx_;
            byte[] sendbuffer = new byte[p.len];
            PacketProcess.Serialize(p, sendbuffer);
            ((AsyncSocketClient)sender).Send(sendbuffer);
        }

        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            LogManager.Instance.Log(string.Format("private void OnClose {0}/{1}", sender.ToString(), e.ToString()));
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            LogManager.Instance.Log(string.Format("private void OnError {0}/{1}", sender.ToString(), e.ToString()));            
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
            LogManager.Instance.Log(string.Format("private void OnSend {0}/{1}", sender.ToString(), e.ToString()));            
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            try
            {
                LogManager.Instance.Log(string.Format("private void OnReceive {0}/{1}", sender.ToString(), e.ToString()));
                AsyncSocketClient psocket = (AsyncSocketClient)sender;
                Int16 len = System.BitConverter.ToInt16(e.ReceiveData, 0);
                if (len != e.ReceiveBytes)
                {
                    return;
                }

                PACKET_IDX pidx = (PACKET_IDX)System.BitConverter.ToChar(e.ReceiveData, 2);

                switch (pidx)
                {
                    case PACKET_IDX.MK_RESTART:
                        {

                        }
                        break;
                    case PACKET_IDX.CK_HEARTBEAT:
                        {
                            C_TO_K_HEARTBEAT p = new C_TO_K_HEARTBEAT();
                            PacketProcess.Deserialize(p, e.ReceiveData);

                            if (p.CrawlingCount > CrawlingCount_)
                            {
                                CrawlingCountChangeTime_ = Environment.TickCount;
                                LogManager.Instance.Log(string.Format("크롤링이 끝났다고 날라 왔구나. 고고싱~{0}", p.CrawlingCount));
                                CrawlingCount_ = p.CrawlingCount;
                            }
                            
                            HeartBeatTime_ = Environment.TickCount;
                            label_NextCheck.Text = string.Format("PACKET_IDX.CK_HEARTBEAT 패킷 도착{0}", HeartBeatTime_);
                        }
                        break;

                    default:
                        {
                        }
                        break;
                }
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(string.Format("private void Error OnReceive {0}", ex.Message));
            }
        }
    }
}
