using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

// 웹관련
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

// 사용자 정의 클래스
using HK.Database;
using MySql.Data.MySqlClient;
using HK.Util;
using TMS.Common;
using LQCrawler.Data;
using HKLibrary.Excel;

using Renci.SshNet;
using Renci.SshNet.Common;
using CrawlerShare;
using LQStructures;

namespace LQCrawler
{
    public partial class LQCrawler : Form
    {
        public LQCrawler()
        {
            InitializeComponent();
            StartApp();
        }

        // 앱 초기화 작업
        public void StartApp()
        {
            ChangeCurrentStatus(CrawlerState.INITIATING);
            // INI 로드( DB 접속 정보, 매니저 접속 정보 )
            string IniPath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + "/LQCrawler.INI";
            bool bSucceess = CrawlerInfoManager.Instance.LoadIni(IniPath);
            if (bSucceess == false)
            {
                MessageBox.Show("INI 로드중에 에러가 발생했습니다.");
                return;
            }

            // 크롤링 타이머 초기화
            AppManager.Instance.GetCrawlerTimer().Tick += new EventHandler(Crawler_Timer_Function);
            AppManager.Instance.GetCrawlerTimer().Interval = CrawlerInfoManager.Instance.crawlingtick_;

            // UI 타이머 초기화
            AppManager.Instance.GetUIItmer().Tick += new EventHandler(UI_Timer_Function);
            AppManager.Instance.GetUIItmer().Interval = 1000;
            AppManager.Instance.GetUIItmer().Enabled = true;

            // Connection 타이머 초기화
            AppManager.Instance.GetConnectionItmer().Tick += new EventHandler(Connection_Timer_Function);
            AppManager.Instance.GetConnectionItmer().Interval = 5000;
            AppManager.Instance.GetConnectionItmer().Enabled = true;


            // 크롤링 쓰레드 초기화
            InitWorker();
            InitConnectionWork();
            LogManager.Instance.SetLogFile("Crawler.txt");
            ChangeCurrentStatus(CrawlerState.STOP);

            // 크롤링 시작
            StartCrawling();
        }

        // 채널 정보 로드 관련
        private void test_channelInfo()
        {
            LQStructures.LQChannelInfo struaa = new LQStructures.LQChannelInfo();

            LQStructures.LQChannelInfo stru = new LQStructures.LQChannelInfo();
            //stru.strCEOName_ = "Adasdjklhakdljhflaksdasdfasasdasdasddfasdfsadfhrfklajhelrkjsdhlfkjsdhf";
            
            
            //byte[] pByte = Packet_Convert.Serialize<LQStructures.LQChannelInfo>(stru);
            //MessageBox.Show(string.Format("{0}", pByte.Length));
            //LQStructures.LQChannelInfo stru1 = new LQStructures.LQChannelInfo();
            //Packet_Convert.Deserialize<LQStructures.LQChannelInfo>(ref stru1, ref pByte);
            //MessageBox.Show(string.Format("{0}", stru1.strCEOName_));


            MemoryStream stream = new MemoryStream();
            Packet_Convert.Serialize_Stream<LQStructures.LQChannelInfo>(ref stru, ref stream);
            MessageBox.Show(string.Format("{0}", stream.Length));

            
            stream.Position = 0;
            LQStructures.LQChannelInfo stru1 = Packet_Convert.Deserialize_Stream<LQStructures.LQChannelInfo>(ref stream) as LQStructures.LQChannelInfo;
            if (stru1 == null)
                MessageBox.Show("실패함");

            byte[] pByte = new byte[(int)stream.Length];
            stream.Write(pByte, 0, (int)stream.Length);
            MessageBox.Show(string.Format("{0}", pByte.Length));
            GC.Collect();

         //   MessageBox.Show(string.Format("{0} 개가 생성이 되었습니다.", LQStructures.LQChannelInfo.count));
        }

        // 현재 상태 라벨 변경
        void SetStateString(string str)
        {
            label_CurrentState.Text = str;
        }
        // 크롤링 실패 횟수 갱신
        void SetFailedCount()
        {
            label_CrawlingFailedCount.Text = CrawlerManager.Instance.GetResultData().ErrorCount_.ToString();
        }
        // 크롤링 횟수 갱신
        void SetTotalCrawlingCount()
        {
            //label_CrawlingCount.Text = CrawlerManager.Instance.GetResultData().ProcessTime_DB_.ToString();
        }
        // 크롤링 타이머 관련
        void Crawler_Timer_Function(object sender, EventArgs e)
        {
            AppManager.Instance.GetCrawlerTimer().Enabled = false;

            if (CrawlerManager.Instance.GetState() != CrawlerState.WORKING)
                return;

            crawler_worker_.RunWorkerAsync();
        }
        void SetChannelName()
        {
            LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();
            if (pCrawler == null)
            {
                label_ChannelName.Text = "-";
            }
            else
            {
                label_ChannelName.Text = pCrawler.ChannelName_;
            }
        }
        // 크롤링까지 남은 시간 표시
        void SetNextCrawlingLeftTime()
        {
            if (LastCrawlingTick_ == 0)
            {
                label_NextCrawling.Text = "-";
            }
            else
            {
                LQStructures.LQCrawlerInfo pCrawler = CrawlerManager.Instance.GetCrawlerInfo();
                Int32 nNextTick = LastCrawlingTick_ + CrawlerInfoManager.Instance.crawlingtick_;
                Int32 nCurrent = Environment.TickCount;

                Int32 nLeftTime = nNextTick - nCurrent;
                nLeftTime /= 1000;
                if (nLeftTime > 0)
                {
                    label_NextCrawling.Text = string.Format("{0} 초 후", nLeftTime);
                }
            }
        }
        // 현재 크롤링 상태
        void SetCrawlingState(string CurrentState)
        {
            if (CrawlerManager.Instance.GetState() != CrawlerState.WORKING)
            {
                label_CrawlingState.Text = "-";
            }
            else
            {
                label_CrawlingState.Text = CurrentState;                
            }
        }

        // UI 관련 타이머 함수
        void UI_Timer_Function(object sender, EventArgs e)
        {
            AppManager.Instance.GetUIItmer().Enabled = false;

            ChangeCurrentStatus(CrawlerManager.Instance.GetState());
            CheckMemoryUsing();
            // 크롤링까지 남은시간 표시
            SetNextCrawlingLeftTime();
            label_ChannelName.InvokeIfNeeded(SetChannelName);   // 채널이름 변경
            label_TestValue.InvokeIfNeeded(SetCrawlerResult);   // 디버깅 메시지
            AppManager.Instance.GetUIItmer().Enabled = true;
        }

        void Connection_Timer_Function(object sender, EventArgs e)
        {
            AppManager.Instance.GetConnectionItmer().Enabled = false;

            if (connection_worker_.IsBusy == false)
                connection_worker_.RunWorkerAsync();

            AppManager.Instance.GetConnectionItmer().Enabled = true;
        }

        // 현재 상태변경
        void ChangeCurrentStatus(CrawlerState state)
        {
            // 상태 변경
            CrawlerManager.Instance.SetState(state);
            
            // 상태 문자열 변경
            string message = StringData.strCrawLerState[(Int32)state];
            label_CurrentState.InvokeIfNeeded(SetCurrentStatus, message);

            // 버튼 변경.
            if (state == CrawlerState.STOP)
            {
                button_onoff.Text = "시작";
                button_onoff.Enabled = true;
                LastCrawlingTick_ = 0;
            }
            else if (state == CrawlerState.STOPPING)
            {
                button_onoff.Text = "중지중";
                button_onoff.Enabled = false;
            }
            else if (state == CrawlerState.WORKING)
            {
                button_onoff.Text = "중지";
                button_onoff.Enabled = true;
            }
        }
        // 현재 상태 출력 변경
        void SetCurrentStatus(string strState)
        {
            label_CurrentState.Text = strState;
        }
        // 메모리 사용량 측정
        void CheckMemoryUsing()
        {
            float nUsage = (float)HKLibrary.UTIL.ProcessChecker.GetUsageMemory() / 1000000;            
            label_MemoryUsage.Text = string.Format("{0:f2} MB", nUsage);
        }
        // 크롤링 시간 측정
        void CheckCrawlingTime(Int32 nStartTick)
        {
            Int32 nEndTick = Environment.TickCount;
            Int32 nGap = nEndTick - nStartTick;
            label_CrawlingTime.Text = string.Format("{0} sec", nGap/1000);            
        }
        // 크롤링 결과값 출력
        void SetCrawlerResult()
        {
            ResultData p = CrawlerManager.Instance.GetResultData();
            label_TestValue.Text = string.Format("Deal:{0}, Cancel:{1}, Refund:{2}, Error:{3}\n\rInsert:{4}, Update:{4}"
                , p.TotalUseDeal_, p.TotalCancelDeal_, p.TotalRefundDeal_, p.TotalErrorCount_, p.Inserted_, p.Updated_);
        }

        void StartCrawling()
        {
            CrawlerState currentstate = CrawlerManager.Instance.GetState();

            if (currentstate == CrawlerState.INITIATING
                || currentstate == CrawlerState.BEFORE_INIT)
            {
                MessageBox.Show("초기화 중입니다. 잠시후 다시 요청하세요.");
            }
            else if (currentstate == CrawlerState.WORKING)
            {
                if (crawler_worker_.IsBusy == true)
                {
                    ChangeCurrentStatus(CrawlerState.STOPPING);
                    crawler_worker_.CancelAsync();
                }
                else
                {
                    ChangeCurrentStatus(CrawlerState.STOP);
                }
            }
            else if (currentstate == CrawlerState.ERROR
                || currentstate == CrawlerState.STOP)
            {
                ChangeCurrentStatus(CrawlerState.WORKING);
                AppManager.Instance.GetCrawlerTimer().Interval = 10;
                AppManager.Instance.GetCrawlerTimer().Enabled = true;
            }
            else if (currentstate == CrawlerState.STOPPING)
            {
                MessageBox.Show("크롤링 중지 중입니다. 잠시후 다시 요청하세요.");
            }
        }

        // 수동 시작, 버튼
        private void button_onoff_Click(object sender, EventArgs e)
        {
            StartCrawling();
            
        }

        // 테스트 함수들 -------------------------------------------
        private void TestButton1_Click(object sender, EventArgs e)
        {
            //test_Use();
            
            string strDate = "2013-05-01";
            DateTime dt = Convert.ToDateTime(strDate);

        }

        private void buttonTest2_Click(object sender, EventArgs e)
        {
            test_Rex();
        }

        void test_Use()
        {
            SqlHelper pMySqlDB = new SqlHelper();
            pMySqlDB.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_
                , CrawlerInfoManager.Instance.sshuser_, CrawlerInfoManager.Instance.sshpw_);

            //MySqlDataReader data = pMySqlDB.execute_sql("select * from CRAWLER.Crawler_Info2");
            
           // ////- connection method  : standard TCP/IP over SSH 
           //////- SSH Hostname       : ledev.leisureq.co.kr:10001
           //////- SSH Username       : lsquser
           //////- SSH Password       : fpvpQb
           //////- MySQL Hostname   : leisuredb01
           //////- MySQL Server Port : 3306
           //////- UserName              : lsquser
           //////- Password               : fpvpQb
           //// // DB 접속

           // return;

            //SqlHelper pMySqlDB = new SqlHelper();

            //SshClient pSshClient = new SshClient("ledev.leisureq.co.kr", "lsquser", "fpvpQb");
            //pSshClient.Connect();
            //var port = new ForwardedPortLocal("127.0.0.1", "leisuredb01", 3306);
            //pSshClient.RemoveForwardedPort(port);
            //pSshClient.AddForwardedPort(port);

            //if (port.IsStarted == true)
            //{
            //    port.Stop();
            //}

            //port.Start();

            ////string connstr = "server=14.63.169.203;user=" + "lsquser" + ";database=crawler;port=" + "10001" + ";password=" + "fpvpQb" + ";";
            //string connstr = "server=127.0.0.1;user=" + "lsquser" + ";database=CRAWLER;port=" + Convert.ToString(port.BoundPort) + ";password=" + "fpvpQb" + ";";

            //pMySqlDB.MySqlConnection_ = new MySqlConnection(connstr);
            ////MySqlConnection psqlconnection = new MySqlConnection(connstr);
            //pMySqlDB.MySqlConnection_.Open();


            //// //// DB에서 채널 정보 로드하기
            //// //string querystring = "select * from crawler.Crawler_Info2;";
            //MySqlDataReader data = pMySqlDB.execute_sql("select * from CRAWLER.Crawler_Info2");
            //while (data.Read())
            //{
            //    string a = Convert.ToString(data["PartnerName"]);
            //}

            //data.Close();

            DBFunctions.GetGoodsTable(pMySqlDB, 3, 3);
        }
        
        // 취소 처리 테스트
        void Test_Cancel()
        {

        }
        // 정규식 테스트
        void test_Rex()
        {
          
        }
        
        
    }

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
}

