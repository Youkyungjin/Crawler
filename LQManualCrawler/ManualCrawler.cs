using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
using System.Runtime.InteropServices;


using HKLibrary.Excel;
using CrawlerShare;
using LQStructures;
using HK.Database;
using MySql.Data.MySqlClient;

namespace LQManualCrawler
{
    public partial class ManualCrawler : Form
    {
        string ExcelTypeName_ = "레저큐 양식";       // 레저큐에 사용되는 고정 엑셀 양식 이름
        string SelectedFilePath_ = "";
        BackgroundWorker crawler_worker_ = new BackgroundWorker();
        Dictionary<Int32, LQCrawlerInfo> CrawlerInfoList_ = new Dictionary<Int32, LQCrawlerInfo>();

        public ManualCrawler()
        {
            InitializeComponent();
            LogManager.Instance.SetLogFile("ManualCrawler.txt");

            string IniPath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + "/LQManualCrawler.INI";
            bool bSucceess = CrawlerInfoManager.Instance.LoadIni(IniPath);

            comboBox_SelectAuthor.SelectedIndexChanged += comboBox_SelectChannel_SelectedIndexChanged;
            comboBox_ExcelFrom.SelectedIndexChanged += comboBox_ExcelFrom_SelectedIndexChanged;
            comboBox_SelectChannel.SelectedIndexChanged += comboBox_SelectAuthor_SelectedIndexChanged;
            dataGridView_DataView.ReadOnly = true;

            InitBackWorker();
            MakeGrid();

            // 모든 채널 리스트를 얻어 온다.
            if (SelectAllChannelInfos() == true)
            {
                InsertChannelInfoComboBox();
                InsertExcelTypeComboBox();
                InsertAuthorInfoComboBox();
            }
            else
            {
                MessageBox.Show("채널정보 로딩에 실패했습니다.");
                return;
            }
        }

        // 모든 크롤러 정보 로드 하기
        public bool SelectAllChannelInfos()
        {
            try
            {
                SqlHelper pMySqlDB = new SqlHelper();

                pMySqlDB.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                    , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_, CrawlerInfoManager.Instance.sshuser_
                    , CrawlerInfoManager.Instance.sshpw_);

                MySqlDataReader datareader = pMySqlDB.call_proc("sp_select_All_Crawler_Info", null);

                while (datareader.Read())
                {
                    LQCrawlerInfo pInfo = new LQCrawlerInfo();
                    pInfo.nIdx_ = Convert.ToInt32(datareader["idx"]);
                    pInfo.Channel_Idx_ = Convert.ToInt32(datareader["Channel_Idx"]);
                    pInfo.ChannelName_ = Convert.ToString(datareader["Channel_Name"]);
                    pInfo.PartnerSeq_ = Convert.ToInt32(datareader["PartnerSeq"]);
                    pInfo.PartnerName_ = Convert.ToString(datareader["PartnerName"]);

                    // 권리사 (2014-07-30) . 추가
                    pInfo.AuthoritySeq_ = Convert.ToInt32(datareader["AuthoritySeq"]);
                    pInfo.AuthoriryName_ = Convert.ToString(datareader["AuthorityName"]);

                    pInfo.MainUrl_ = Convert.ToString(datareader["MainUrl"]);         // 메인 URL
                    pInfo.LoginIDTAG_ = Convert.ToString(datareader["LoginIDTAG"]);
                    pInfo.LoginPWTAG_ = Convert.ToString(datareader["LoginPWTAG"]);
                    pInfo.LoginUrl_ = Convert.ToString(datareader["LoginUrl"]);
                    pInfo.LoginParam_ = Convert.ToString(datareader["LoginParam"]);      // 로그인 셋팅값
                    pInfo.LoginID_ = Convert.ToString(datareader["LoginID"]);         // 로그인 아이디
                    pInfo.LoginPW_ = Convert.ToString(datareader["LoginPW"]);         // 로그인 암호
                    pInfo.LoginMethod_ = Convert.ToString(datareader["LoginMethod"]);     // 로그인 방식
                    pInfo.LoginEvent_ = Convert.ToString(datareader["LoginEvent"]);      // 로그인 버튼 이벤트
                    pInfo.LoginCheck_ = Convert.ToString(datareader["LoginCheck"]);
                    pInfo.LoginType_ = Convert.ToChar(datareader["LoginType"]);
                    pInfo.ExcelDownUrl_ = Convert.ToString(datareader["ExcelDownUrl"]);    // 엑셀 다운로드 URL
                    pInfo.ExcelDownParameter_ = Convert.ToString(datareader["ExcelDownParameter"]);
                    pInfo.ExcelDownMethod_ = Convert.ToString(datareader["ExcelDownMethod"]);    // 엑셀 다운로드 방식                    
                    pInfo.ExcelDownRule_ = Convert.ToString(datareader["ExcelDownRule"]);

                    pInfo.UseGoodsUrl_ = Convert.ToString(datareader["UseGoodsUrl"]);
                    pInfo.UseGoodsParam_ = Convert.ToString(datareader["UseGoodsParam"]);
                    pInfo.UseGoodsCheck_ = Convert.ToString(datareader["UseGoodsCheck"]);
                    pInfo.UseGoodsRule_ = Convert.ToString(datareader["UseGoodsRule"]);

                    pInfo.UseUserUrl_ = Convert.ToString(datareader["UseUserUrl"]);
                    pInfo.UseUserParam_ = Convert.ToString(datareader["UseUserParam"]);
                    pInfo.UseUserCheck_ = Convert.ToString(datareader["UseUserCheck"]);

                    pInfo.NUseGoodsUrl_ = Convert.ToString(datareader["NUseGoodsUrl"]);
                    pInfo.NUseGoodsParam_ = Convert.ToString(datareader["NUseGoodsParam"]);
                    pInfo.NUseGoodsCheck_ = Convert.ToString(datareader["NUseGoodsCheck"]);
                    pInfo.NUseGoodsRule_ = Convert.ToString(datareader["NUseGoodsRule"]);

                    pInfo.NUseUserUrl_ = Convert.ToString(datareader["NUseUserUrl"]);
                    pInfo.NUseUserParam_ = Convert.ToString(datareader["NUseUserParam"]);
                    pInfo.NUseUserCheck_ = Convert.ToString(datareader["NUseUserCheck"]);

                    pInfo.RUseUserUrl_ = Convert.ToString(datareader["RUseUserUrl"]);
                    pInfo.RUseUserParam_ = Convert.ToString(datareader["RUseUserParam"]);
                    pInfo.RUseUserCheck_ = Convert.ToString(datareader["RUseUserCheck"]);

                    pInfo.ExData_Start_ = Convert.ToInt32(datareader["ExData_Start"]);
                    pInfo.ExData_Coupncode_ = Convert.ToInt32(datareader["ExData_Coupncode"]);
                    pInfo.ExData_Buydate_ = Convert.ToInt32(datareader["ExData_Buydate"]);
                    pInfo.ExData_Option_ = Convert.ToInt32(datareader["ExData_Option"]);
                    pInfo.ExData_Cancel_ = Convert.ToInt32(datareader["ExData_Cancel"]);
                    pInfo.ExData_Use_ = Convert.ToInt32(datareader["ExData_Use"]);
                    pInfo.ExData_Buyer_ = Convert.ToInt32(datareader["ExData_Buyer"]);
                    pInfo.ExData_Count_ = Convert.ToInt32(datareader["ExData_Count"]);
                    pInfo.ExData_Buyphone_ = Convert.ToInt32(datareader["ExData_Buyphone"]);
                    pInfo.ExData_Price_ = Convert.ToInt32(datareader["ExData_Price"]);
                    pInfo.ExData_UseCheck_ = Convert.ToString(datareader["ExData_UseCheck"]);
                    pInfo.ExData_CancelCheck_ = Convert.ToString(datareader["ExData_CancelCheck"]);


                    CrawlerInfoList_.Add(pInfo.AuthoritySeq_, pInfo);
                }

                datareader.Close();
                datareader.Dispose();
                datareader = null;
            }
            catch (System.Exception ex)
            {
                LogManager.Instance.Log(string.Format("채널정보 로드 실패 {0}",ex.Message));
                return false;
            }


            return true;
        }

        // 크롤링 정보를 얻어 온다.
        LQCrawlerInfo GetCrawlingInfo(string strname)
        {
            foreach(var pData in CrawlerInfoList_)
            {
                if (string.Compare(pData.Value.ChannelName_, strname) == 0)
                {
                    return pData.Value;
                }
            }

            return null;
        }

        // 권리사 정보를 얻어 온다.
        void InsertAuthorInfoComboBox()
        {
            comboBox_SelectAuthor.Items.Add("권리사 계정정보를 선택해 주세요.");
            comboBox_SelectAuthor.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var p in CrawlerInfoList_)
            {
                comboBox_SelectAuthor.Items.Add(p.Value.AuthoriryName_);
            }
            comboBox_SelectAuthor.SelectedIndex = 0;
        }

        private void comboBox_SelectAuthor_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (CrawlerManager.Instance.GetCrawlerInfo() != null)
            {
                dataGridView_DataView.Rows.Clear();
                CrawlerManager.Instance.InitCrawler();
                textBox_FileName.Text = "";
            }
        }

        // 채널 정보를 로딩한다.
        void InsertChannelInfoComboBox()
        {
            comboBox_SelectChannel.Items.Add("채널을 선택해 주세요.");
            comboBox_SelectChannel.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var p in CrawlerInfoList_)
            {
                 comboBox_SelectChannel.Items.Add(p.Value.ChannelName_);
                //comboBox_SelectChannel.Items.Add(p.Value.ChannelName_ + " - " + p.Value.AuthoriryName_);
            }
            comboBox_SelectChannel.SelectedIndex = 0;
        }

        private void comboBox_SelectChannel_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (CrawlerManager.Instance.GetCrawlerInfo() != null)
            {
                dataGridView_DataView.Rows.Clear();
                CrawlerManager.Instance.InitCrawler();
                textBox_FileName.Text = "";
            }
        }

        // 엑셀 타입을 고른다.
        void InsertExcelTypeComboBox()
        {
            comboBox_ExcelFrom.Items.Add("엑셀 타입을 선택해 주세요");
            comboBox_ExcelFrom.DropDownStyle = ComboBoxStyle.DropDownList;
            foreach (var p in CrawlerInfoList_)
            {
                comboBox_ExcelFrom.Items.Add(p.Value.AuthoriryName_);
                //comboBox_ExcelFrom.Items.Add(p.Value.ChannelName_ + " - " + p.Value.AuthoriryName_);
            }

            comboBox_ExcelFrom.Items.Add(ExcelTypeName_);

            comboBox_ExcelFrom.SelectedIndex = 0;
        }

        private void comboBox_ExcelFrom_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if (CrawlerManager.Instance.GetCrawlerInfo() != null)
            {
                dataGridView_DataView.Rows.Clear();
                CrawlerManager.Instance.InitCrawler();
                textBox_FileName.Text = "";
            }
        }

        // 그리드 기본으로 만들기
        void MakeGrid()
        {
            dataGridView_DataView.ColumnCount = 7;
            dataGridView_DataView.Columns[0].Name = "주문자";
            dataGridView_DataView.Columns[1].Name = "전화번호";
            dataGridView_DataView.Columns[2].Name = "쿠폰번호";
            dataGridView_DataView.Columns[3].Name = "옵션명";
            dataGridView_DataView.Columns[4].Name = "상품 가격";
            dataGridView_DataView.Columns[5].Name = "상태값";
            dataGridView_DataView.Columns[6].Name = "구매일시";
        }

        #region  백그라운드 작업
        void InitBackWorker()
        {
            crawler_worker_.WorkerReportsProgress = true;
            crawler_worker_.WorkerSupportsCancellation = true;
            crawler_worker_.DoWork += new DoWorkEventHandler(LoadExcel_Function);
            crawler_worker_.ProgressChanged += new ProgressChangedEventHandler(Progress_LoadExcel_Function);
            crawler_worker_.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Finish_LoadExcel_Function);
        }

        void LoadExcel_Function(object sender, DoWorkEventArgs e)
        {

        }

        void Progress_LoadExcel_Function(object sender, ProgressChangedEventArgs e)
        {

        }
        
        void Finish_LoadExcel_Function(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "Error");
                return;
            }

            GC.Collect();
        }
        #endregion

        private void button_FileSelect_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Filter = "Text Files (.xlsx,.xls)|*.xlsx;*.xls|All Files (*.*)|*.*";
            openFileDialog1.FilterIndex = 1;

            openFileDialog1.Multiselect = false;

            DialogResult result = openFileDialog1.ShowDialog();

            if (result == DialogResult.OK)
            {
                SelectedFilePath_= openFileDialog1.FileName;
                textBox_FileName.Text = openFileDialog1.FileName;
            }
            else
            {
                SelectedFilePath_ = "";
                textBox_FileName.Text = "";
            }

            CrawlerManager.Instance.InitCrawler();
            ResetGridView();
        }

        private void button_DBInsert_Click(object sender, EventArgs e)
        {

            if (comboBox_SelectAuthor.SelectedIndex <= 0)
            {
                MessageBox.Show("권리사를 선택 해주세요.");
                return;
            } 
            
            if (comboBox_SelectChannel.SelectedIndex <= 0)
            {
                MessageBox.Show("채널을 선택 해주세요.");
                return;
            }

            if (comboBox_ExcelFrom.SelectedIndex <= 0)
            {
                MessageBox.Show("엑셀 폼을 선택해 주세요.");
                return;
            }

            LQCrawlerBase pLQCrawlerBase = CrawlerManager.Instance.GetCrawler();
            LQCrawlerInfo pLQCrawlerInfo = CrawlerManager.Instance.GetCrawlerInfo();

            if (pLQCrawlerBase == null || pLQCrawlerInfo == null)
            {
                MessageBox.Show("크롤러가 생성되지 않았습니다.");
                return;
            }

            // 상품 정보 읽어오기
            SqlHelper pMySqlDB = new SqlHelper();
            pMySqlDB.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_
                , CrawlerInfoManager.Instance.sshuser_, CrawlerInfoManager.Instance.sshpw_);

            DBFunctions.SelectStateTable(pMySqlDB);
            DBFunctions.GetGoodsTable(pMySqlDB, pLQCrawlerInfo.Channel_Idx_, pLQCrawlerInfo.AuthoritySeq_);
            Dictionary<Int32, ChannelGoodInfo> pGoodInfoList = CrawlerManager.Instance.GetGoodsInfo();

            DBFunctions.Select_tblOrder(pMySqlDB, pLQCrawlerInfo.Channel_Idx_);
            DBFunctions.Select_tblOrderWr(pMySqlDB, pLQCrawlerInfo.Channel_Idx_);

            pLQCrawlerBase.Login(); 
            pLQCrawlerBase.Combine_DB_And_Excel(false);
            pLQCrawlerBase.Process_RefundData(pMySqlDB);
            pLQCrawlerBase.Process_ExpiredData();

            // 채널에 상품처리// 채널에 취소처리  // 채널에 반품처리
            pLQCrawlerBase.Process_Use_Cancel_Refund();

            // DB 에 처리
            SqlHelper pMySqlDB2 = new SqlHelper();
            pMySqlDB2.Connect(CrawlerInfoManager.Instance.method_, CrawlerInfoManager.Instance.dbip_, CrawlerInfoManager.Instance.dbport_, CrawlerInfoManager.Instance.dbname_
                , CrawlerInfoManager.Instance.dbaccount_, CrawlerInfoManager.Instance.dbpw_, CrawlerInfoManager.Instance.sshhostname_
                , CrawlerInfoManager.Instance.sshuser_, CrawlerInfoManager.Instance.sshpw_);
            pLQCrawlerBase.Process_DB(pMySqlDB2);

            if (pMySqlDB2.Close() == false)
            {
                LogManager.Instance.Log("<<< 삭제 실패 2 >>>");
            }

            pMySqlDB2 = null;

            MessageBox.Show("크롤링이 끝났습니다.");
        }

        // 데이터를 그리드뷰에 넣자
        void DataInsertToGrid()
        {
            LogManager.Instance.Log("void DataInsert()");

            Dictionary<string, tblOrderData> pOrderList = OrderManager.Instance.GetExcelOrderList();
            
            foreach (var pData in pOrderList)
            {
                DataGridViewRow r1 = new DataGridViewRow();

                DataGridViewTextBoxCell cell_name = new DataGridViewTextBoxCell();
                cell_name.Value = pData.Value.orderName_;
                r1.Cells.Add(cell_name);

                DataGridViewTextBoxCell cell_phone = new DataGridViewTextBoxCell();
                cell_phone.Value = pData.Value.orderPhone_;
                r1.Cells.Add(cell_phone);

                DataGridViewTextBoxCell cell_coupon = new DataGridViewTextBoxCell();
                cell_coupon.Value = pData.Value.channelOrderCode_;
                r1.Cells.Add(cell_coupon);

                DataGridViewTextBoxCell cell_optionname = new DataGridViewTextBoxCell();
                cell_optionname.Value = pData.Value.ExData_Option_;
                r1.Cells.Add(cell_optionname);

                DataGridViewTextBoxCell cell_price = new DataGridViewTextBoxCell();
                cell_price.Value = Convert.ToString(pData.Value.orderSettlePrice_);
                r1.Cells.Add(cell_price);

                DataGridViewTextBoxCell cell_state  = new DataGridViewTextBoxCell();
                cell_state.Value = pData.Value.ExData_Use_;
                r1.Cells.Add(cell_state);

                DataGridViewTextBoxCell cell_buydate = new DataGridViewTextBoxCell();
                cell_buydate.Value = pData.Value.BuyDate_;
                r1.Cells.Add(cell_buydate);


                dataGridView_DataView.Rows.Add(r1);
            }
            
        }

        // 데이터 그리드뷰를 초기화 하자
        void ResetGridView()
        {
            dataGridView_DataView.Rows.Clear();
        }

        private void button_FileShow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(SelectedFilePath_) == true)
            {
                MessageBox.Show("파일을 먼저 선택하세요.");
                return;
            }

            string channel_name = Convert.ToString(comboBox_SelectChannel.SelectedItem);
            LQCrawlerInfo pInfo = GetCrawlingInfo(channel_name);
            CrawlerManager.Instance.SetCrawlerInfo(pInfo);

            if (pInfo == null)
            {
                MessageBox.Show("크롤러 정보가 잘못되었습니다.");
                return;
            }

            // 크롤러 정보 만들기
            // 크롤러 생성
            CrawlerManager.Instance.MakeCrawler(pInfo.Channel_Idx_);
            LQCrawlerBase pLQCrawlerBase =  CrawlerManager.Instance.GetCrawler();
            if (pLQCrawlerBase == null)
            {
                MessageBox.Show("크롤러 매니저 생성 실패");
                return;
            }

            // 엑셀 데이터 읽어오기
            bool bFixed = false;
            string exceltype = Convert.ToString(comboBox_ExcelFrom.SelectedItem);
            if (string.Compare(ExcelTypeName_, exceltype) == 0)
            {
                bFixed = true;
            }

            OrderManager.Instance.Init();
            pLQCrawlerBase.LoadExcelAndInsertList(SelectedFilePath_, 0, bFixed, "");

            DataInsertToGrid();
        }
    }
}
