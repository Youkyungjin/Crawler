using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using System.Windows.Forms;

using Microsoft.Office.Interop.Excel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;

// 사용자 정의 클래스
using LQCrawler.Data;
using HKLibrary.Excel;
using HK.Database;
using HKLibrary.WEB;
using LQStructures;
using TMS;
using CrawlerShare;


namespace LQCrawler
{
    partial class LQCrawler
    {
        #region #크롤링 백그라운드 작업 
        // 백그라운드 작업
        BackgroundWorker crawler_worker_;
        //private object lockObject_ = new object();
        private Int32 LastCrawlingTick_ = 0;

        // 작업 초기화
        void InitWorker()
        {
            crawler_worker_ = new BackgroundWorker();
            crawler_worker_.WorkerReportsProgress = true;
            crawler_worker_.WorkerSupportsCancellation = true;
            crawler_worker_.DoWork += new DoWorkEventHandler(Crawler_Function);
            crawler_worker_.ProgressChanged += new ProgressChangedEventHandler(change_func);
            crawler_worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void Crawler_Function(object sender, DoWorkEventArgs e)
        {
            label_CrawlingState.InvokeIfNeeded(SetCrawlingState, "크롤링 시작");
            LastCrawlingTick_ = 0;

            CrawlerManager.Instance.MakeCrawler(CrawlerInfoManager.Instance.channelidx_);
            LQCrawlerBase pCrawler =  CrawlerManager.Instance.GetCrawler();
            Int32 nStartTick = Environment.TickCount;
            pCrawler.Crawling(crawler_worker_);
            deletedownloadfile();
            label_CrawlingTime.InvokeIfNeeded(CheckCrawlingTime, nStartTick);
        }

        private void deletedownloadfile()
        {

            if (CrawlerInfoManager.Instance.deletedownfile_ == true)
            {
                Dictionary<string, string> GoodsDownInfo = OrderManager.Instance.GetGoodsList();

                foreach (var pData in GoodsDownInfo)
                {
                    if (System.IO.File.Exists(pData.Value) == true)
                    {
                        try
                        {
                            System.IO.File.Delete(pData.Value);
                        }
                        catch (System.IO.IOException ex)
                        {
                            LogManager.Instance.Log("System.IO.File.Exists(downString) " + ex.Message);
                            continue;
                        }
                    }
                }
            }
        }

        // 크롤링 중간에 바뀌는것
        void change_func(object sender, ProgressChangedEventArgs e)
        {
            string[] strstatus = new string[]{"", "크롤링 시작", "채널 정보 로드", "상품 정보 로드", "판매 상품 Select", "채널 로그인"
                , "엑셀 다운로드", "판매/취소 처리중", "DB 갱신중", "DB 갱신 완료"};
            
            if(e.ProgressPercentage >= strstatus.Length)
                return;
            label_CrawlingState.Text = strstatus[e.ProgressPercentage];
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
                return;
            }

            GC.Collect();

            label_CrawlingFailedCount.InvokeIfNeeded(SetFailedCount);
            label_CrawlingCount.InvokeIfNeeded(SetTotalCrawlingCount);

            CrawlerState pState = CrawlerManager.Instance.GetState();

            if (pState == CrawlerState.WORKING)
            {
                AppManager.Instance.GetCrawlerTimer().Interval = CrawlerInfoManager.Instance.crawlingtick_;
                AppManager.Instance.GetCrawlerTimer().Enabled = true;
                LastCrawlingTick_ = Environment.TickCount;
            }
            else if (pState == CrawlerState.STOPPING)
            {
                ChangeCurrentStatus(CrawlerState.STOP);
                LastCrawlingTick_ = 0;
            }
            else
            {
                LastCrawlingTick_ = 0;
            }

            label_NextCrawling.InvokeIfNeeded(SetNextCrawlingLeftTime);
        }

        #endregion

        #region #접속 관련 백그라운드 작업
        BackgroundWorker connection_worker_;
        AsyncSocketClient manager_socket_;

        AsyncSocketClient checker_socket_;
        

        void InitConnectionWork()
        {
            connection_worker_ = new BackgroundWorker();
            connection_worker_.WorkerReportsProgress = false;
            connection_worker_.WorkerSupportsCancellation = true;
            connection_worker_.DoWork += new DoWorkEventHandler(Connection_Function);
            connection_worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConnectionWorker_Completed);
        }

        private void ManagerConnect()
        {
            manager_socket_ = new AsyncSocketClient(0);

            // 이벤트 핸들러 재정의
            manager_socket_.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            manager_socket_.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            manager_socket_.OnSend += new AsyncSocketSendEventHandler(OnSend);
            manager_socket_.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            manager_socket_.OnError += new AsyncSocketErrorEventHandler(OnError);
            manager_socket_.Connect(CrawlerInfoManager.Instance.managerip_, CrawlerInfoManager.Instance.managerport_);            
        }

        private void CheckerConnect()
        {
            LogManager.Instance.Log("private void CheckerConnect()");
            checker_socket_ = new AsyncSocketClient(1);

            // 이벤트 핸들러 재정의
            checker_socket_.OnConnet += new AsyncSocketConnectEventHandler(OnConnet);
            checker_socket_.OnClose += new AsyncSocketCloseEventHandler(OnClose);
            checker_socket_.OnSend += new AsyncSocketSendEventHandler(OnSend);
            checker_socket_.OnReceive += new AsyncSocketReceiveEventHandler(OnReceive);
            checker_socket_.OnError += new AsyncSocketErrorEventHandler(OnError);
            checker_socket_.Connect("127.0.0.1", CrawlerInfoManager.Instance.checkerport_);
        }

        void Connection_Function(object sender, DoWorkEventArgs e)
        {
            CheckChecker();
            CheckManager();
        }

        // 체커로의 연결을 확인
        void CheckChecker()
        {
            //LogManager.Instance.Log("체커 연결 확인");

            bool bNeedConnection = false;
            if (checker_socket_ == null)
            {
                bNeedConnection = true;
            }
            else
            {
                if (checker_socket_.Connection == null)
                {
                    bNeedConnection = true;
                }
                else
                {
                    if (checker_socket_.Connection.Connected == false)
                    {
                        bNeedConnection = true;
                    }
                    else
                    {
                        LQCrawlerInfo pInfo = CrawlerManager.Instance.GetCrawlerInfo();
                        C_TO_K_HEARTBEAT p = new C_TO_K_HEARTBEAT();
                        p.num = (byte)PACKET_IDX.CK_HEARTBEAT;
                        p.len = (Int16)Marshal.SizeOf(p);
                        p.CrawlingCount = CrawlerManager.Instance.CrawlingCount();
                        byte[] sendbuffer = new byte[p.len];
                        PacketProcess.Serialize(p, sendbuffer);
                        checker_socket_.Send(sendbuffer);                        
                    }
                }
            }

            if (bNeedConnection == true)
            {
                CheckerConnect();
            }
        }

        // 매니저의 연결을 확인
        void CheckManager()
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
                        LQCrawlerInfo pInfo = CrawlerManager.Instance.GetCrawlerInfo();
                        C_TO_M_CHANNEL_IDX p = new C_TO_M_CHANNEL_IDX();
                        p.num = (byte)PACKET_IDX.CM_CHANNEL_IDX;
                        p.len = (Int16)Marshal.SizeOf(p);
                        p.nIdx = pInfo.nIdx_;
                        byte[] sendbuffer = new byte[p.len];
                        PacketProcess.Serialize(p, sendbuffer);
                        manager_socket_.Send(sendbuffer);
                    }
                }
            }

            if (bNeedConnection == true)
            {
                ManagerConnect();
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
            LogManager.Instance.Log("private void OnConnet(object sender, AsyncSocketConnectionEventArgs e)");
            LQCrawlerInfo pInfo = CrawlerManager.Instance.GetCrawlerInfo();
            C_TO_M_CHANNEL_IDX p = new C_TO_M_CHANNEL_IDX();
            p.num = (byte)PACKET_IDX.CM_CHANNEL_IDX;
            p.len = (Int16)Marshal.SizeOf(p);
            //p.nIdx = CrawlerInfoManager.Instance.channelidx_;
            p.nIdx = pInfo.nIdx_;
            
            byte[] sendbuffer = new byte[p.len];
            PacketProcess.Serialize(p, sendbuffer);
            ((AsyncSocketClient)sender).Send(sendbuffer);
        }

        private void OnClose(object sender, AsyncSocketConnectionEventArgs e)
        {
            LogManager.Instance.Log("private void OnClose(object sender, AsyncSocketConnectionEventArgs e)");
        }

        private void OnError(object sender, AsyncSocketErrorEventArgs e)
        {
            //LogManager.Instance.Log("private void OnError(object sender, AsyncSocketErrorEventArgs e)");
        }

        private void OnSend(object sender, AsyncSocketSendEventArgs e)
        {
            //LogManager.Instance.Log("private void OnSend(object sender, AsyncSocketSendEventArgs e)");
        }

        private void OnReceive(object sender, AsyncSocketReceiveEventArgs e)
        {
            
        }

        #endregion
    }
}
