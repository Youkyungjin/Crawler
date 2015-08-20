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

using LQCrawlerManager.Data;
using LQStructures;
using TMS;

namespace LQCrawlerManager
{
    public partial class LQCrawlerManger : Form
    {
        Int32 nSelectedPartnerIdx_ = -1;
        public LQCrawlerManger()
        {
            InitializeComponent();
            StartApp();
        }

        void StartApp()
        {
            AppManager.Instance.SetForm(this);

            bool bSuccees = LoadIni();
            bSuccees = GetChannelInfo();
            
            InitGridView();
            AppManager.Instance.GetUITimer().Tick += new EventHandler(UI_Timer_Function);
            AppManager.Instance.GetUITimer().Interval = InfoManager.Instance.checktick_;            
            AppManager.Instance.GetUITimer().Enabled = true;

            // 채널 정보를 받아온다.
            AppManager.Instance.SelectChannelInfos();
            
            // 콤보 박스에 파트너 정보를 추가한다.
            comboBox_Partner.Items.Add("모든 파트너");
            Dictionary<string, string> tempDic = new Dictionary<string, string>();
            
            foreach (var pData in AppManager.Instance.ChannelInfo_List_)
            {
                if(tempDic.ContainsKey(pData.Value.PartnerName_) == true)
                    continue;

                tempDic.Add(pData.Value.PartnerName_, pData.Value.PartnerName_);
                comboBox_Partner.Items.Add(pData.Value.PartnerName_);
            }

            comboBox_Partner.SelectedIndex = 0;
            nSelectedPartnerIdx_ = 0;
            comboBox_Partner.SelectedIndexChanged += comboDropDown_SelectedIndexChanged;
            MakeDataGridView(comboBox_Partner.SelectedItem as string);

            // 소켓 서버를 시작한다.
            AppManager.Instance.ServerStart();
        }

        private void comboDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (nSelectedPartnerIdx_ == comboBox_Partner.SelectedIndex)
                return;

            nSelectedPartnerIdx_ = comboBox_Partner.SelectedIndex;
            MakeDataGridView(comboBox_Partner.SelectedItem as string);
        }


        bool GetChannelInfo()
        {
            return true;
        }

        bool LoadIni()
        {
            string IniPath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + "/LQCrawlerManager.INI";
            bool bSucceess = InfoManager.Instance.LoadIni(IniPath);
            if (bSucceess == false)
            {
                MessageBox.Show("INI 로드중에 에러가 발생했습니다.");
                return false;
            }

            return true;
        }

        // 리스트뷰 만들기
        void InitGridView()
        {   
            dataGridView_Crawler.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;   // 셀사이즈 자동 맞춤.
            dataGridView_Crawler.CellClick += new DataGridViewCellEventHandler(dataGridView_Crawler_CellClick);
        }

        // Row 추가
        void AddListView(string Idx, string PartnerIdx, string PartnerName, string ChannelIdx
            , string ChannelName, string Ip, string curState, string checkerstate )
        {
            // 0: 인덱스 1: 파트너 인덱스 2: 파트너 이름 3: 채널 인덱스 4: 채널 이름 5: 채널 아이피 6: 동작 상태 7: 스타터 연결 8: 버튼
            DataGridViewRow r1 = new DataGridViewRow();

            // Unique Index
            DataGridViewTextBoxCell cell_idx = new DataGridViewTextBoxCell();
            cell_idx.Value = Idx;
            r1.Cells.Add(cell_idx);

            // 파트너 인덱스
            DataGridViewTextBoxCell cell_partner_idx = new DataGridViewTextBoxCell();
            cell_partner_idx.Value = PartnerIdx;
            r1.Cells.Add(cell_partner_idx);

            // 파트너 이름
            DataGridViewTextBoxCell cell_partner_name = new DataGridViewTextBoxCell();
            cell_partner_name.Value = PartnerName;
            r1.Cells.Add(cell_partner_name);

            // 채널 인덱스
            DataGridViewTextBoxCell cell_channel_idx = new DataGridViewTextBoxCell();
            cell_channel_idx.Value = ChannelIdx;
            r1.Cells.Add(cell_channel_idx);

            // 채널 이름
            DataGridViewTextBoxCell cell_channel_name = new DataGridViewTextBoxCell();
            cell_channel_name.Value = ChannelName;
            r1.Cells.Add(cell_channel_name);

            // 채널 아이피
            DataGridViewTextBoxCell cell_channel_ip = new DataGridViewTextBoxCell();
            cell_channel_ip.Value = Ip;
            r1.Cells.Add(cell_channel_ip);

            // 동작 상태
            DataGridViewTextBoxCell cell_crawler_state = new DataGridViewTextBoxCell();
            cell_crawler_state.Value = curState;
            r1.Cells.Add(cell_crawler_state);

            // 스타터 연결
            DataGridViewTextBoxCell cell_cheker_state = new DataGridViewTextBoxCell();
            cell_cheker_state.Value = checkerstate;
            r1.Cells.Add(cell_cheker_state);

            // Button
            DataGridViewButtonCell b1 = new DataGridViewButtonCell();
            Button pButton = new Button();
            b1.Value = "ReStart";
            r1.Cells.Add(b1);

            dataGridView_Crawler.Rows.Add(r1);
        }

        // 그리드 뷰 초기에 만들기
        public void MakeDataGridView(string selectedName)
        {
            dataGridView_Crawler.Rows.Clear();
            Dictionary<Int32, LQChannelInfo> pChannelInfo = AppManager.Instance.ChannelInfo_List_;

            foreach (var pData in pChannelInfo)
            {
                if (pData.Value.PartnerName_ == selectedName && selectedName != "모든 파트너")
                {
                    AddListView(pData.Value.nIdx_.ToString(), pData.Value.PartnerSeq_.ToString(), pData.Value.PartnerName_
                    , pData.Value.Channel_Idx_.ToString(), pData.Value.Channel_Name_, pData.Value.connected_ip_
                    , pData.Value.crawler_status_, pData.Value.checker_status_);
                }
                else
                {
                    AddListView(pData.Value.nIdx_.ToString(), pData.Value.PartnerSeq_.ToString(), pData.Value.PartnerName_
                    , pData.Value.Channel_Idx_.ToString(), pData.Value.Channel_Name_, pData.Value.connected_ip_
                    , pData.Value.crawler_status_, pData.Value.checker_status_);
                }
            }
        }

        // 그리드 뷰 갱신하가ㅣ
        public void UpdateDataGridView()
        {
            foreach(DataGridViewRow pData in dataGridView_Crawler.Rows)
            {
                Int32 nIndex = Convert.ToInt32(pData.Cells[0].Value);
                if(pData.Cells[0].Value == null)
                    continue;

                LQChannelInfo pInfo = AppManager.Instance.GetChannelInfo(nIndex);

                // 0: 인덱스 1: 파트너 인덱스 2: 파트너 이름 3: 채널 인덱스 4: 채널 이름 5: 채널 아이피 6: 동작 상태 7: 스타터 연결 8: 버튼

                if (pInfo != null)
                {
                    pData.Cells[0].Value = pInfo.nIdx_.ToString();
                    pData.Cells[1].Value = pInfo.PartnerSeq_.ToString();
                    pData.Cells[2].Value = pInfo.PartnerName_;
                    pData.Cells[3].Value = pInfo.Channel_Idx_.ToString();
                    pData.Cells[4].Value = pInfo.Channel_Name_;
                    pData.Cells[5].Value = pInfo.connected_ip_;
                    pData.Cells[6].Value = pInfo.crawler_status_;
                    pData.Cells[7].Value = pInfo.checker_status_;
                }
                else
                {
                    pData.Cells[2].Value = "not in operation";
                    pData.Cells[3].Value = "not in operation";
                }
            }
        }

        // UI 관련 타이머 함수
        void UI_Timer_Function(object sender, EventArgs e)
        {
            AppManager.Instance.GetUITimer().Enabled = false;

            UpdateDataGridView();

            AppManager.Instance.GetUITimer().Enabled = true;
        }

        public void AddLog(string strlog)
        {
            listBox_Log.InvokeIfNeeded(Log, strlog);
        }

        public void Log(string strlog)
        {
            listBox_Log.Items.Add(strlog);
            listBox_Log.SelectedIndex = listBox_Log.Items.Count - 1;
            if (listBox_Log.Items.Count > 200)
            {
                listBox_Log.Items.Clear();
            }
        }

        private void dataGridView_Crawler_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 8)
            {
                string str = dataGridView_Crawler.Rows[e.RowIndex].Cells[0].Value as string;
                Int32 nChIdx = Convert.ToInt32(str);
                if (AppManager.Instance.Checker_Connection_List_.ContainsKey(nChIdx) == true)
                {
                    AsyncSocketClient psocket = AppManager.Instance.Checker_Connection_List_[nChIdx];

                    M_TO_K_RESTART p = new M_TO_K_RESTART();
                    p.num = (byte)PACKET_IDX.MK_RESTART;
                    p.len = (Int16)Marshal.SizeOf(p);
                    byte[] sendbuffer = new byte[p.len];
                    PacketProcess.Serialize(p, sendbuffer);

                    psocket.Send(sendbuffer);
                }
                else
                {
                    MessageBox.Show("체커와 연결이되지 않았습니다. 체커를 실행 시켜 주세요.");
                }
            }
        }

        private void button_Test1_Click(object sender, EventArgs e)
        {
            AppManager.Instance.SelectChannelInfos();
            MakeDataGridView(comboBox_Partner.SelectedItem as string);
        }

        private void button_Test2_Click(object sender, EventArgs e)
        {
            AppManager.Instance.ServerStart();
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
