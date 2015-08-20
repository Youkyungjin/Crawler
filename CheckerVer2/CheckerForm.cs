using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CheckerVer2.Data;
using TMS;
using HKLibrary.UTIL;
using LQStructures;

namespace CheckerVer2
{
    public partial class CheckerForm : Form
    {
        string CheckerINIPath_ = "";
        string CrawlerINIPath_ = "";
        public CheckerForm()
        {
            InitializeComponent();
            CheckerINIPath_ = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + @"\CheckerVer2.INI";
            CrawlerINIPath_ = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + @"\CrawlerVer2.INI";

            LoadINI();

            InitUITimer();

            ConnectDBAndGetCrawlerInfo();
        }
        #region 로그파일 생성
        void InitLogFile()
        {
            string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
            makefolder += "\\";
            makefolder += "Log";
            HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);


            // 로그 파일 만들기
            DateTime dt = System.DateTime.Now;
            string LogFileName = string.Format(@"{0}\CheckerLog_{1:D4}_{2:D2}_{3:D2}.txt", "Log", dt.Year, dt.Month, dt.Day);
            NewLogManager2.Instance.SetLogFile(LogFileName);
        }
        #endregion

        void LoadINI()
        {
            CheckerINIManager.Instance.LoadIni(CheckerINIPath_);
        }

        bool ConnectDB()
        {
            return CheckerAppManager.Instance.ConnectDB();
        }

        void ConnectDBAndGetCrawlerInfo()
        {
            bool bResult = ConnectDB();

            if(bResult == true)
            {
                if(CheckerINIManager.Instance.MonitorSeq_ > 0)             
                {
                    Int32 Result = 0;
                    Int32 ChannelSeq = 0;
                    Int32 AuthoritySeq = 0;
                    Int32 Mode = 0;
                    Int32 CrawlerSeq = 0;

                    CheckerDBInterface.UpdateCrawlerMonitorInfo(CheckerAppManager.Instance.DB(), CheckerINIManager.Instance.MonitorSeq_
                    , UTIL.CLIENTIP, CheckerINIManager.Instance.ListenCrawlerPort_, "OK", ref Result, ref ChannelSeq, ref AuthoritySeq
                    , ref Mode, ref CrawlerSeq);

                    if (Result > 0 && ChannelSeq > 0 && AuthoritySeq > 0 && Mode > 0 && CrawlerSeq > 0)
                    {
                        StartChecker(AuthoritySeq, ChannelSeq, CrawlerSeq, Mode);
                    }
                    else
                    {
                        SetInfo();
                        CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.WAIT_MONITOR_TABLE_SET;
                        InitMonitorCheckTimer();// 이제 매니저에서 제대로 등록되기만을 기다리면 된다.
                    }
                }
                else
                {
                    Int32 MonitorSeq = 0;
                    CheckerDBInterface.InsertCrawlerMonitorInfo(CheckerAppManager.Instance.DB(), UTIL.CLIENTIP
                        , CheckerINIManager.Instance.ListenCrawlerPort_, ref MonitorSeq);

                    if (MonitorSeq > 0)
                    {
                        CheckerINIManager.Instance.UpdateMonitorSeq(CheckerINIPath_, MonitorSeq);
                        CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.WAIT_MONITOR_TABLE_SET;
                        InitMonitorCheckTimer();// 이제 매니저에서 제대로 등록되기만을 기다리면 된다.
                    }
                    else
                    {
                        // 에러남 표시하고 아무것도 하지말자
                        CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.ERROR;
                        CheckerAppManager.Instance.ErrorString_ = "DB에 모니터링 Insert 중에 문제가 발생함";
                    }
                }
            }
        }

#region UI 타이머
        void SetInfo()
        {
            List<string> Info_list = new List<string>();
            Info_list.Add(string.Format("모니터 Seq : {0}", CheckerINIManager.Instance.MonitorSeq_));
            Info_list.Add(string.Format("크롤러 Seq : {0}", CheckerAppManager.Instance.CrawlerSeq_));
            Info_list.Add(string.Format("채널 : {0}, 권리사 : {1}", CheckerAppManager.Instance.ChannelSeq_, CheckerAppManager.Instance.AuthoritySeq_));

            richTextBox_Info.Lines = Info_list.ToArray();
        }

        System.Windows.Forms.Timer UI_Timer_ = new System.Windows.Forms.Timer();
        private void InitUITimer()
        {
            UI_Timer_.Tick += new EventHandler(SUITimer_Function);
            UI_Timer_.Interval = 1000;
            UI_Timer_.Enabled = true;
        }

        void SUITimer_Function(object sender, EventArgs e)
        {
            List<string> State_list = new List<string>();

            switch (CheckerAppManager.Instance.CHECKER_STATE_)
            {

                case CHECKER_STATE.ERROR:
                    {
                        State_list.Add(CheckerAppManager.Instance.ErrorString_);
                    }
                    break;
                case CHECKER_STATE.INIT:
                    {
                        State_list.Add("초기화중");
                    }
                    break;
                case CHECKER_STATE.WAIT_MONITOR_TABLE_SET:
                    {
                        Int32 nLeft = (CheckerINIManager.Instance.CheckTime_ + LastSelectCheckTime_) - Environment.TickCount;
                        nLeft = (Int32)(nLeft * 0.001f);
                        State_list.Add("DB 에 아직 크롤링 정보가 셋팅되지 않았습니다.");
                        State_list.Add(string.Format("{0} 초 뒤에 다시 DB를 확인 합니다.", nLeft));
                    }
                    break;
                case CHECKER_STATE.REPORT_DB:
                    {
                        Int32 nDBLeft = (CheckerINIManager.Instance.CheckTime_ + LastCheckUpdateTime_) - Environment.TickCount;
                        nDBLeft = (Int32)(nDBLeft * 0.001f);
                        Int32 nCheckLeft = (CheckerINIManager.Instance.CheckTime_ + LastCheckTime_) - Environment.TickCount;
                        nCheckLeft = (Int32)(nCheckLeft * 0.001f);
                        State_list.Add("작동중-");
                        State_list.Add(string.Format("{0} 초 뒤에 크롤러를 체크 합니다.", nCheckLeft));
                        State_list.Add(string.Format("{0} 초 뒤에 상태값을 DB에 Update 합니다.", nDBLeft));
                    }
                    break;
            }

            richTextBox_Status.Lines = State_list.ToArray();
        }
#endregion

        #region 주기적으로 DB 에 상태를 Update 하는 타이머
        System.Windows.Forms.Timer State_Update_Timer_ = new System.Windows.Forms.Timer();
        Int32 LastCheckUpdateTime_ = 0;
        private void InitStateUpdateTimer()
        {
            LastCheckUpdateTime_ = Environment.TickCount;
            State_Update_Timer_.Tick += new EventHandler(StateUpdateTimer_Function);
            State_Update_Timer_.Interval = CheckerINIManager.Instance.CheckTime_;
            State_Update_Timer_.Enabled = true;
        }

        void StateUpdateTimer_Function(object sender, EventArgs e)
        {
            State_Update_Timer_.Enabled = false;

            Int32 Result = 0;
            Int32 ChannelSeq = 0;
            Int32 AuthoritySeq = 0;
            Int32 Mode = 0;
            Int32 CrawlerSeq = 0;

            CheckerDBInterface.UpdateCrawlerMonitorInfo(CheckerAppManager.Instance.DB()
                , CheckerINIManager.Instance.MonitorSeq_, UTIL.CLIENTIP, CheckerINIManager.Instance.ListenCrawlerPort_
                , "OK", ref Result, ref ChannelSeq, ref AuthoritySeq, ref Mode, ref CrawlerSeq);

            LastCheckUpdateTime_ = Environment.TickCount;

            State_Update_Timer_.Enabled = true;
        }

        #endregion

        #region 주기적으로 DB 에서 모니터링 정보를 Select 하는 타이머
        Int32 LastSelectCheckTime_ = 0;

        System.Windows.Forms.Timer Monitoring_Select_Timer_ = new System.Windows.Forms.Timer();
        private void InitMonitorCheckTimer()
        {
            LastSelectCheckTime_ = Environment.TickCount;

            Monitoring_Select_Timer_.Tick += new EventHandler(MonitorSelectTimer_Function);
            Monitoring_Select_Timer_.Interval = CheckerINIManager.Instance.CheckTime_;
            Monitoring_Select_Timer_.Enabled = true;            
        }

        void MonitorSelectTimer_Function(object sender, EventArgs e)
        {
            Monitoring_Select_Timer_.Enabled = false;

            if (CheckerINIManager.Instance.MonitorSeq_ > 0)
            {
                Int32 Result = 0;
                Int32 ChannelSeq = 0;
                Int32 AuthoritySeq = 0;
                Int32 Mode = 0;
                Int32 CrawlerSeq = 0;

                CheckerDBInterface.UpdateCrawlerMonitorInfo(CheckerAppManager.Instance.DB(), CheckerINIManager.Instance.MonitorSeq_
                    , UTIL.CLIENTIP, CheckerINIManager.Instance.ListenCrawlerPort_, "OK", ref Result, ref ChannelSeq, ref AuthoritySeq
                    , ref Mode, ref CrawlerSeq);

                if (Result > 0 && ChannelSeq > 0 && AuthoritySeq > 0 && Mode > 0 && CrawlerSeq > 0)
                {
                    SetInfo();
                    StartChecker(AuthoritySeq, ChannelSeq, CrawlerSeq, Mode);
                    return;
                }
                else
                {
                    SetInfo();
                    CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.WAIT_MONITOR_TABLE_SET;                    
                }
            }

            LastSelectCheckTime_ = Environment.TickCount;
            Monitoring_Select_Timer_.Enabled = true;
        }

        void StartChecker(Int32 AuthoritySeq, Int32 ChannelSeq, Int32 CrawlerSeq, Int32 Mode)
        {
            CheckerINIManager.Instance.UpdateCrawlerINI(CrawlerINIPath_, AuthoritySeq, ChannelSeq, CrawlerSeq, Mode);
            CheckerAppManager.Instance.AuthoritySeq_ = AuthoritySeq;
            CheckerAppManager.Instance.ChannelSeq_ = ChannelSeq;
            CheckerAppManager.Instance.CrawlerSeq_ = CrawlerSeq;
            CheckerAppManager.Instance.channelidx_ = CrawlerSeq;
            CheckerAppManager.Instance.Mode_ = Mode;

            if (ServerStart() == true)          // 리슨 시작
            {
                if (RestartCrawler() == true)
                {
                    CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.REPORT_DB;
                    InitCheckTimer();       // 크롤러 체크 타이머 동작
                    InitStateUpdateTimer(); // Update 타이머 동작 시작
                    SetInfo();
                }
                else
                {
                    CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.ERROR;
                    CheckerAppManager.Instance.ErrorString_ = string.Format("크롤러 시작시 에러 발생. 크롤러 경로 확인 바람. {0}", CheckerINIManager.Instance.ExePath_);
                }
            }
            else
            {
                CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.ERROR;
                CheckerAppManager.Instance.ErrorString_ = "서버 리슨중에 에러가 발생했습니다. 동일한 포트를 사용중인 프로그램이 있을수 있습니다.";
                MessageBox.Show(CheckerAppManager.Instance.ErrorString_);
            }
        }
        #endregion

        #region 주기적으로 크롤러의 상태를 체크해서 재실행 하는 타이머
        System.Windows.Forms.Timer Crawler_Check_Timer_ = new System.Windows.Forms.Timer();
        private Int32 LastCheckTime_ = 0;           // 마지막으로 체크한 시간.
        private Int32 CheckProcessID_ = -1;         // 크롤러의 프로세스 아이디, 종료할때 필요하다.
        private Int32 HeartBeatTime_ = -1;          // 크롤러가 heartbeat 패킷을 보내온 시간
        private Int32 CrawlingCount_ = 0;           // 크롤러가 보내온 크롤링 횟수
        private Int32 CrawlingCountChangeTime_ = 0; // 크롤러가 보내온 크롤링 회수가 변한 시간.

        private void InitCheckTimer()
        {
            Crawler_Check_Timer_.Tick += new EventHandler(CheckTimer_Function);
            Crawler_Check_Timer_.Interval = CheckerINIManager.Instance.CheckTime_;
            Crawler_Check_Timer_.Enabled = true;
            LastCheckTime_ = Environment.TickCount;
        }

        void CheckTimer_Function(object sender, EventArgs e)
        {
            Crawler_Check_Timer_.Enabled = false;

            if (CheckAndRunProgram() == false)
                return;

            Crawler_Check_Timer_.Enabled = true;
        }

        // 재시작 함수
        bool RestartCrawler()
        {
            if (CheckProcessID_ > 0)
            {
                if (HK.Util.HKProgramExecuter.StopProgramByProcessID(CheckProcessID_) == false)
                {
                    NewLogManager2.Instance.Log(string.Format("RestartCrawler 에서 CheckProcessID_ 가 있으나 실제 프로세스는 없었습니다.{0}", CheckProcessID_));
                }
            }

            HK.Util.ExecuteResult pResult = HK.Util.HKProgramExecuter.StartProgram(CheckerINIManager.Instance.ExePath_);
            if (pResult.ID < 0) // 실패
            {
                CheckerAppManager.Instance.CHECKER_STATE_ = CHECKER_STATE.ERROR;
                CheckerAppManager.Instance.ErrorString_ = "크롤러 재시작을 실패 했습니다.";
                return false;
            }

            CheckProcessID_ = pResult.ID;
            return true;
        }

        bool CheckAndRunProgram()
        {
            LastCheckTime_ = Environment.TickCount;

            if (CheckProcessID_ < 0)
            {// 자신이 실행한 프로세스 아이디가 없는경우.
                return RestartCrawler();
            }
            else
            {// 자신이 실행한 프로세스 아이디가 있는경우. 1분이상 heartbeat 이 오지 않았다면, 끊겼거나 문제가 생겼을것이다. kill 하고 재시작 하자.

                // HeartBeat 패킷이 너무 늦고 있다. 재시작 하자.
                if (LastCheckTime_ - HeartBeatTime_ > 1000 * 60) 
                {
                    NewLogManager2.Instance.Log("HeartBeat 패킷이 오지않음 다시 시작한다.");
                    CheckerDBInterface.InsertCrawlerRestartLog(CheckerAppManager.Instance.DB(), "HeartBeat 패킷이 오지않음 다시 시작한다"
                        , CheckerAppManager.Instance.CrawlerSeq_, CheckerINIManager.Instance.MonitorSeq_, CheckerAppManager.Instance.ChannelSeq_
                        , CheckerAppManager.Instance.AuthoritySeq_, "--");
                    return RestartCrawler();
                }
            }

            return true;
        }


        #endregion

        #region 크롤러의 접속을 기다리는 서버
        AsyncSocketServer CrawlerServer_ = null;   // 크롤러의 접속을 대기하는 서버
        Int32 SocketID_ = 0;

        public bool ServerStart()
        {
            try
            {
                CrawlerServer_ = new AsyncSocketServer(CheckerINIManager.Instance.ListenCrawlerPort_);
                CrawlerServer_.OnAccept += new AsyncSocketAcceptEventHandler(OnAcceptCrawler);
                CrawlerServer_.Listen();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(string.Format("크롤러 리슨 서버 시작 실패 {0}", ex.Message));
                return false;
            }
            return true;
        }

        // 크롤러가 접속 됐을때 호출되는 callback
        private void OnAcceptCrawler(object sender, AsyncSocketAcceptEventArgs e)
        {
            AsyncSocketClient socketclient = new AsyncSocketClient(SocketID_++, e.Worker);

            // 데이터 수신을 대기한다.
            socketclient.Receive();
            socketclient.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            socketclient.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            socketclient.OnError += new AsyncSocketErrorEventHandler(OnError);
            socketclient.OnSend += new AsyncSocketSendEventHandler(OnSend);
            socketclient.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
        }

        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            NewLogManager2.Instance.Log("연결 되었어요");
        }

        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            try
            {
                NewLogManager2.Instance.Log(string.Format("private void OnReceive {0}/{1}", sender.ToString(), e.ToString()));
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
                                NewLogManager2.Instance.Log(string.Format("크롤링이 끝났다고 날라 왔구나. 고고싱~{0}", p.CrawlingCount));
                                CrawlingCount_ = p.CrawlingCount;
                            }

                            HeartBeatTime_ = Environment.TickCount;
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
                NewLogManager2.Instance.Log(string.Format("private void Error OnReceive {0}", ex.Message));
            }
        }

        #endregion
    }
}
