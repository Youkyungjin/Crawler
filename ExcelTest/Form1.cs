using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HKLibrary.Excel;
using System.Threading;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
using HK.Database;
using MySql.Data.MySqlClient;
using CrawlerShare;

namespace ExcelTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            
//            Environment.TickCount;
            InitializeComponent();
            InitWorker();
            DataManager.Instance.MakeList();
            LogManager.Instance.SetLogFile("ExcelTest.txt");
        }
    
        

        void InitWorker()
        {
            Test_worker_ = new BackgroundWorker();
            Test_worker_.WorkerReportsProgress = true;
            Test_worker_.WorkerSupportsCancellation = true;
            Test_worker_.DoWork += new DoWorkEventHandler(BackGround_Function);
            Test_worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);
        }

        void BackGround_Function(object sender, DoWorkEventArgs e)
        {
            TestCount_++;

            try
            {
                Callki();
            }
            catch (System.Exception ex)
            {
                
            }

            label_MemoryUsage.InvokeIfNeeded(SetMemory);
            label_TestCount.InvokeIfNeeded(SetCount);
        }


        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
                return;
            }


            ListSellData_.Clear();
            ListSellData2_.Clear();
            
            DataManager.Instance.InitAllList();

            GC.Collect();

            if (bStart_ == true)
            {
                Test_worker_.RunWorkerAsync();
            }
        }

        float fBeforeUsage_ = 0;
        float fFirstMemory_ = 0;
        void SetMemory()
        {
            float nUsage = (float)HKLibrary.UTIL.ProcessChecker.GetUsageMemory() / 1000000;
            label_MemoryUsage.Text = string.Format("{0:f2} MB", nUsage);

            label_increase_Memory.Text = string.Format("{0:f2} MB", nUsage - fBeforeUsage_);

            if (fBeforeUsage_ == 0)
            {
                fFirstMemory_ = nUsage;
            }

            label_totalIncrease.Text = string.Format("{0:f2} MB", nUsage - fFirstMemory_);

            fBeforeUsage_ = nUsage;
        }

        void SetCount()
        {
            label_TestCount.Text = string.Format("{0}", TestCount_);
        }

        BackgroundWorker Test_worker_;

        Dictionary<string, SellData> ListSellData_ = new Dictionary<string, SellData>();

        Dictionary<string, SellData> ListSellData2_ = new Dictionary<string, SellData>();

        //Dictionary<Int32, SellData> ListSellData2_ = new Dictionary<Int32, SellData>();
        bool bStart_ = false;
        Int32 TestCount_ = 0;

        private void button1_Click(object sender, EventArgs e)
        {
            if (bStart_ == true)
            {
                bStart_ = false;                
            }
            else
            {
                bStart_ = true;
                Test_worker_.RunWorkerAsync();
            }

            

//            Int32 tempCount = 0;

            //while (TestCount_ < 1000)
            //{
            //    LoadAndDelete();

            //    float nUsage = (float)HKLibrary.UTIL.ProcessChecker.GetUsageMemory() / 1000000;
            //    label_MemoryUsage.Text = string.Format("{0:f2} MB", nUsage);

            //    TestCount_++;
            //    label_TestCount.Text = string.Format("{0}", TestCount_);

            //    Thread.Sleep(100);
            //}
        }

        Int32 TimeFor_LoadExcel_ = 0;
        Int32 TimeFor_SelectDB_ = 0;
        Int32 TimeFor_InsertDB_ = 0;
        Int32 TimeFor_Cacurate_ = 0;
        Int32 TimeFor_ConnectDB_ = 0;

        Int32 TimeFor_All_ = 0;

        Int32 TempTime_ = 0;
        Int32 TempAllTime_ = 0;


        void Callki()
        {
            bool bResult = true;
            TempAllTime_ = Environment.TickCount;
            TempTime_ = Environment.TickCount;
            bResult = ConnectDB();
            TimeFor_ConnectDB_ = Environment.TickCount - TempTime_;

            //if (bResult == true)
            {
                TempTime_ = Environment.TickCount;
                bResult = SelectDB();
                TimeFor_SelectDB_ = Environment.TickCount - TempTime_;
            }

            float fone = 0;
            //if (bResult == true)
            {
                TempTime_ = Environment.TickCount;
                //bResult = LoadAndDelete(@"d:\69025564_635482434167599047.xls");
                bResult = LoadAndDelete(@"H:\010_LeisureQ\00_SVN\00_Source\Crawler\LQCrawler\bin\Debug\6\2014-10-13\69025564_635488107921292599.xls");
                
                TimeFor_LoadExcel_ = Environment.TickCount - TempTime_;
                fone = (float)TimeFor_LoadExcel_ / (float)proc_count_;
            }

            
            //if (bResult == true)
            {
                TempTime_ = Environment.TickCount;
                bResult = Calcu();
                TimeFor_Cacurate_ = Environment.TickCount - TempTime_;
                
            }

            //if (bResult == true)
            {
                TempTime_ = Environment.TickCount;
                bResult = InsertDB();
                TimeFor_InsertDB_ = Environment.TickCount - TempTime_;
            }


            CloseEveryThing();

            TimeFor_All_ = Environment.TickCount - TempAllTime_;

            string resultstr = string.Format("[{0}] Total:{6}, Connect:{1}, Select:{2}, LoadExcel:{3}, Calcu:{4}, Insert:{5}, One:{7}"
            , TestCount_, TimeFor_ConnectDB_, TimeFor_SelectDB_, TimeFor_LoadExcel_, TimeFor_Cacurate_, TimeFor_InsertDB_
            , TimeFor_All_, fone);

            LogManager.Instance.Log(resultstr);
        }

        bool CloseEveryThing()
        {
            //try
            //{
                pMySqlDB_.Close();
                pMySqlDB_ = null;
            //}
            //catch (System.Exception ex)
            //{
            //    return false;
            //}

            return true;
        }

        SqlHelper pMySqlDB_ = null;

        bool ConnectDB()
        {
            //try
            //{

                pMySqlDB_ = new SqlHelper();
                pMySqlDB_.Connect("ssh", "ledev.leisureq.co.kr", "3306", "testdb_1"
                    , "lsquser", "fpvpQb", "leisuredb01"
                    , "lsquser", "fpwu55!#");
            //}
            //catch (System.Exception ex)
            //{
            //    return false;
            //}

            return true;
        }

        bool SelectDB()
        {
            //try
            //{
                MySqlDataReader datareader = pMySqlDB_.call_proc("sp_select_data");

                while (datareader.Read())
                {
                    //SellData pSellData = new SellData();
                    SellData pSellData = DataManager.Instance.GetData();

                    pSellData.UserName_ = Convert.ToString(datareader["UserName"]);
                    pSellData.CouponNumber_ = Convert.ToString(datareader["CouponNumber"]);
                    pSellData.Cost_ = (float)Convert.ToDouble(datareader["Cost"]);

                    ListSellData2_.Add(pSellData.CouponNumber_, pSellData);
                }

                datareader.Close();
                datareader.Dispose();
                datareader = null;
            //}
            //catch (System.Exception ex)
            //{
            //    return false;
            //}



            return true;
        }

        bool InsertDB()
        {
            //try
            //{
                SqlHelper pMySqlDB = new SqlHelper();
                pMySqlDB.Connect("ssh", "ledev.leisureq.co.kr", "3306", "testdb_1"
                    , "lsquser", "fpvpQb", "leisuredb01"
                    , "lsquser", "fpwu55!#");

                foreach (var pData in ListSellData_)
                {
                    if (pData.Value.bNeedInsert_ == false)
                        continue;

                    Dictionary<string, object> argdic = new Dictionary<string, object>();
                    argdic.Add("xCouponNumber", pData.Value.CouponNumber_);
                    argdic.Add("xUserName", pData.Value.UserName_);
                    argdic.Add("xCost", pData.Value.Cost_);

                    MySqlDataReader datareader = pMySqlDB.call_proc("sp_insert_data", argdic);

                    datareader.Close();
                    datareader.Dispose();
                    datareader = null;
                }

                pMySqlDB.Close();
                pMySqlDB = null;
            //}
            //catch (System.Exception ex)
            //{
            //    return false;
            //}

            return true;
        }

        bool Calcu()
        {
            //try
            //{
                foreach (var pData in ListSellData_)
                {
                    if (ListSellData2_.ContainsKey(pData.Key) == false)
                        pData.Value.bNeedInsert_ = true;
                }
            //}
            //catch (System.Exception ex)
            //{
            //    return false;
            //}

            return true;
        }

        Int32 proc_count_ = 0;
        bool LoadAndDelete(string filepath)
        {
            proc_count_ = 0;
            //string filepath = @"d:\gbs_order_20141006.xls";
            Microsoft.Office.Interop.Excel.Application ap = null;
            Workbook wb = null;
            Worksheet ws = null;
            HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

            Int32 Columns = ws.UsedRange.Columns.Count;
            Int32 RowCount = ws.UsedRange.Rows.Count;

            Range tRange = null;
            
            for (Int32 i = 2; i < RowCount; i++)
            {
                //if (i > 2000)
                //    break;

                proc_count_++;
                //SellData pSellData = new SellData();
                SellData pSellData = DataManager.Instance.GetData();
                tRange = ws.Cells[i, 5];
                pSellData.CouponNumber_ = Convert.ToString(tRange.Value2);
                pSellData.CouponNumber_ = pSellData.CouponNumber_.Trim();

                tRange = ws.Cells[i, 12];
                pSellData.UserName_ = Convert.ToString(tRange.Value2);
                pSellData.UserName_ = pSellData.UserName_.Trim();

                tRange = ws.Cells[i, 13];
                pSellData.Cost_ = (float)Convert.ToDouble(tRange.Value2);

                ListSellData_.Add(pSellData.CouponNumber_, pSellData);
            }


            // 초기화
            wb.Close(false, Type.Missing, Type.Missing);
            ap.Quit();

            Marshal.FinalReleaseComObject(ws);
            Marshal.FinalReleaseComObject(wb);
            Marshal.FinalReleaseComObject(ap);
            ws = null;
            wb = null;
            ap = null;

            return true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label_increase_Memory.ForeColor = Color.Blue;
            label_increase_Memory.BackColor = Color.Red;
            label_increase_Memory.Font = new System.Drawing.Font(label_increase_Memory.Font, FontStyle.Bold);
            //string filepath = @"d:\gbs_order_20141006.xls";
            //Microsoft.Office.Interop.Excel.Application ap = null;
            //Workbook wb = null;
            //Worksheet ws = null;
            //HKExcelHelper.GetWorkSheet(filepath, ref ap, ref wb, ref ws);

            //Int32 Columns = ws.UsedRange.Columns.Count;
            //Int32 RowCount = ws.UsedRange.Rows.Count;

            //Range tRange = ws.Cells[3, 3];
            //string tempstring = Convert.ToString(tRange.Value2);
            //MessageBox.Show(tempstring);
            

            //tRange = ws.Cells[4, 5];
            //tempstring = Convert.ToString(tRange.Value2);
            //MessageBox.Show(tempstring);


            //// 초기화
            //wb.Close(false, Type.Missing, Type.Missing);
            //ap.Quit();

            //Marshal.FinalReleaseComObject(ws);
            //Marshal.FinalReleaseComObject(wb);
            //Marshal.FinalReleaseComObject(ap);
            //ws = null;
            //wb = null;
            //ap = null;
            //GC.Collect();
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
