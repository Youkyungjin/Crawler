using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Channels;
using CrawlerShare;
using HKLibrary;
using HKLibrary.UTIL;
using TMS;
using LQStructures;
using System.Runtime.InteropServices;

namespace CrawlerVer2
{
    public partial class CrawlerVer2 : Form
    {
        public CrawlerVer2()
        {
            InitializeComponent();
            StartApp();
        }

        bool bSucceedLoad_ = true;

        void StartApp()
        {
            InitLogFile();
            InitComboBox();            
            InitWorker();
            InitUITimer();            
            InitCrawlerTimer();
            string IniPath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + "/CrawlerVer2.INI";
            bSucceedLoad_ = CINIManager.Instance.LoadIni(IniPath);

            if (bSucceedLoad_ == false)
            {
                MessageBox.Show("초기화 과정중 에러가 발생했습니다.\n프로그램을 점검하고 다시 시작해 주세요.");
                return;
            }

            comboBox_Function.SelectedIndex = CINIManager.Instance.MODE_;
            comboBox_Function.Enabled = false;
            StartCrawlerTimer(1000);
            InitConnectionWorker(); // 체커에 연결
            InitConnectionTimer();
        }

        void InitLogFile()
        {
            string makefolder = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory();
            makefolder += "\\";
            makefolder += "Log";
            HKLibrary.UTIL.HKFileHelper.MakeFolder(makefolder);


            // 로그 파일 만들기
            DateTime dt = System.DateTime.Now;
            string LogFileName = string.Format(@"{0}\CrawlerLog_{1:D4}_{2:D2}_{3:D2}.txt", "Log", dt.Year, dt.Month, dt.Day);
            NewLogManager2.Instance.SetLogFile(LogFileName);
        }

        #region UI 관련        
        System.Windows.Forms.Timer UI_Timer_ = new System.Windows.Forms.Timer();
        void InitUITimer()
        {
            UI_Timer_.Tick += new EventHandler(UI_Timer_Function);
            UI_Timer_.Interval = 500;
            UI_Timer_.Enabled = true;
        }

        void UI_Timer_Function(object sender, EventArgs e)
        {
            UI_Timer_.Enabled = false;

            label_UID.InvokeIfNeeded(SetUID);
            label_CurrentState.InvokeIfNeeded(SetCurrentState, ProcessStateManager.Instance.GetCurStateString());
            label_ChannelName.InvokeIfNeeded(SetChannelName, ProcessStateManager.Instance.ChannelName_);
            this.InvokeIfNeeded(SetTitle);
            UI_Timer_.Enabled = true;
        }

        #endregion

        #region 크롤러 백그라운드 작업 관련
        System.Windows.Forms.Timer Crawler_Timer_ = new System.Windows.Forms.Timer();

        void StartCrawlerTimer(Int32 interval)
        {
            if (Crawler_Timer_.Enabled == false)
            {
                Crawler_Timer_.Interval = interval;
                Crawler_Timer_.Enabled = true;
            }
            else
            {
                Crawler_Timer_.Enabled = false;
            }
        }

        void InitCrawlerTimer()
        {
            Crawler_Timer_.Tick += new EventHandler(Crawler_Timer_Function);            
        }

        void Crawler_Timer_Function(object sender, EventArgs e)
        {
            Crawler_Timer_.Enabled = false;

            Crawler_Worker_.RunWorkerAsync();
        }

        BackgroundWorker Crawler_Worker_;
        void InitWorker()
        {
            Crawler_Worker_ = new BackgroundWorker();
            Crawler_Worker_.WorkerReportsProgress = true;
            Crawler_Worker_.WorkerSupportsCancellation = true;
            Crawler_Worker_.DoWork += new DoWorkEventHandler(Crawler_Function);
            Crawler_Worker_.ProgressChanged += new ProgressChangedEventHandler(Crawler_Ing);
            Crawler_Worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Crawler_Compleate);            
        }

        void Crawler_Function(object sender, DoWorkEventArgs e)
        {
            NewLogManager2.Instance.Log("<<<<< 크롤링 시작 >>>>>");
            ProcessStateManager.Instance.Init();            
            CManager.Instance.StartCrawling(Crawler_Worker_
                , (CRAWLER_ACTION)CINIManager.Instance.MODE_
                , CINIManager.Instance.channelseq_);
        }

        void Crawler_Ing(object sender, ProgressChangedEventArgs e)
        {
            label_CurrentState.InvokeIfNeeded(SetCurrentState, ProcessStateManager.Instance.GetCurStateString());
        }

        void Crawler_Compleate(object sender, RunWorkerCompletedEventArgs e)
        {
            
            string resultstring = ProcessStateManager.Instance.GetBeforeStateString();
            label_BeforeState.Text = resultstring;
            NewLogManager2.Instance.Log("<<<<<< 크롤링 끝 >>>>>>");
            NewLogManager2.Instance.Log(string.Format("결과 : {0}", resultstring));

            ProcessStateManager.Instance.CrawlingCount_++;

            // 재시작
            ProcessStateManager.Instance.NextCrawlingTikc_ = Environment.TickCount + CINIManager.Instance.crawlingtick_;
            ProcessStateManager.Instance.ChangeStateAndReport(CRAWLER_STATE.WAIT, null);
            Crawler_Timer_.Interval = CINIManager.Instance.crawlingtick_;
            Crawler_Timer_.Enabled = true;
        }

        #endregion

        #region 화면에 표시되는 컨트롤 수정 혹은 접근 함수.

        void SetChannelName(string str)
        {
            label_ChannelName.Text = str;
        }

        void SetCurrentState(string str)
        {
            label_CurrentState.Text = str;
        }

        void SetBeforeState(string str)
        {
            label_BeforeState.Text = str;
        }

        void SetUID()
        {
            string str = string.Format("{0}", CINIManager.Instance.UID_);
            label_UID.Text = str;
        }

        void SetTitle()
        {
            string Funcstr = ProcessStateManager.Instance.ActionMode_[comboBox_Function.SelectedIndex];
            this.Text = string.Format("크롤러 SEQ : {0}, 채널명 : {1}, 기능 : {2}"
                , CINIManager.Instance.UID_, ProcessStateManager.Instance.ChannelName_, Funcstr);
        }

        #endregion
        
        //System.Windows.Forms.Timer Connection_Timer_ = new System.Windows.Forms.Timer();

        // 콤보박스
        void InitComboBox()
        {
            foreach (string pd in ProcessStateManager.Instance.ActionMode_)
            {
                comboBox_Function.Items.Add(pd);
            }

            comboBox_Function.SelectedIndex = 0;
            comboBox_Function.DropDownStyle = ComboBoxStyle.DropDownList;        
        }

        private void button_Start_Click(object sender, EventArgs e)
        {
            if (bSucceedLoad_ == true)
            {
                if (CINIManager.Instance.MODE_ == 0)
                    CINIManager.Instance.MODE_ = comboBox_Function.SelectedIndex;
                StartCrawlerTimer(1000);
            }
            else
            {
                MessageBox.Show("크롤링을 시작 할수 없습니다.\n재시작 해주세요.");
            }
        }

        #region #접속 관련 백그라운드 작업
        BackgroundWorker Connection_Worker_;
        AsyncSocketClient Checker_Socket_;
        Timer Connection_Timer_ = new Timer();

        void InitConnectionTimer()
        {
            Connection_Timer_.Tick += new EventHandler(Connection_Timer_Function);
            Connection_Timer_.Interval = 5000;
            Connection_Timer_.Enabled = true;
        }

        void Connection_Timer_Function(object sender, EventArgs e)
        {
            Connection_Timer_.Enabled = false;

            if (Connection_Worker_.IsBusy == false)
                Connection_Worker_.RunWorkerAsync();

            Connection_Timer_.Enabled = true;
        }

        void InitConnectionWorker()
        {
            Connection_Worker_ = new BackgroundWorker();
            Connection_Worker_.WorkerReportsProgress = false;
            Connection_Worker_.WorkerSupportsCancellation = true;
            Connection_Worker_.DoWork += new DoWorkEventHandler(Connection_Function);
            Connection_Worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConnectionWorker_Completed);
        }

        void Connection_Function(object sender, DoWorkEventArgs e)
        {
            CheckChecker();
        }

        void ConnectionWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                NewLogManager2.Instance.Log(string.Format("Error ConnectionWorker_Completed {0}", e.Error.ToString()));
                return;
            }
        }

        // 체커 연결
        private void CheckerConnect()
        {
            //NewLogManager2.Instance.Log("private void CheckerConnect()");
            Checker_Socket_ = new AsyncSocketClient(1);

            Checker_Socket_.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            Checker_Socket_.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            Checker_Socket_.OnSend += new AsyncSocketSendEventHandler(OnSend);
            Checker_Socket_.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            Checker_Socket_.OnError += new AsyncSocketErrorEventHandler(OnError);
            Checker_Socket_.Connect("127.0.0.1", CINIManager.Instance.checkerport_);
        }

        private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)
        {
            NewLogManager2.Instance.Log("연결 되었습니다.");
            C_TO_M_CHANNEL_IDX p = new C_TO_M_CHANNEL_IDX();
            p.num = (byte)PACKET_IDX.CM_CHANNEL_IDX;
            p.len = (Int16)Marshal.SizeOf(p);
            p.nIdx = CINIManager.Instance.channelseq_;

            byte[] sendbuffer = new byte[p.len];
            PacketProcess.Serialize(p, sendbuffer);
            ((AsyncSocketClient)sender).Send(sendbuffer);
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
        }

        void CheckChecker()
        {
            //NewLogManager2.Instance.Log("체커 연결 확인");

            bool bNeedConnection = false;
            if (Checker_Socket_ == null)
            {
                bNeedConnection = true;
            }
            else
            {
                if (Checker_Socket_.Connection == null)
                {
                    bNeedConnection = true;
                }
                else
                {
                    if (Checker_Socket_.Connection.Connected == false)
                    {
                        bNeedConnection = true;
                    }
                    else
                    {
                        C_TO_K_HEARTBEAT p = new C_TO_K_HEARTBEAT();
                        p.num = (byte)PACKET_IDX.CK_HEARTBEAT;
                        p.len = (Int16)Marshal.SizeOf(p);
                        p.CrawlingCount = ProcessStateManager.Instance.CrawlingCount_;
                        byte[] sendbuffer = new byte[p.len];
                        PacketProcess.Serialize(p, sendbuffer);
                        Checker_Socket_.Send(sendbuffer);
                    }
                }
            }

            if (bNeedConnection == true)
            {
                CheckerConnect();
            }
        }

        #endregion
    }


    #region 쓰레드 상황에서의 컨트롤 접근 관련
    public static class ControlExtensions
    {
        public static void InvokeIfNeeded(this Control control, System.Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();

        }

        public static void InvokeIfNeeded<T>(this Control control, Action<T> action, T arg)
        {
            if (control.InvokeRequired)
                control.Invoke(action, arg);
            else
                action(arg);

        }
    }
#endregion
}
