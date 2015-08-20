using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CrawlerManagerVer2
{
    public partial class StateForm : Form
    {
        //Int32 EditButtonIndex = 0;      // datagridview 리스트에서 수정 버튼의 인덱스
        Int32 SeqIndex_ = 0;            // datagridview 리스트에서 시퀀스의 인덱스

        Int32 SelectedRowIndex_ = -1;   // 선택된 RowIndex

        public StateForm()
        {
            InitializeComponent();

            dataGridView_List.CellMouseDown += DataGridView1_CellMouseDown;
        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex != -1 && e.ColumnIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    DataGridViewCell clickedCell = (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex];

                    // Here you can do whatever you want with the cell
                    this.dataGridView_List.CurrentCell = clickedCell;  // Select the clicked cell, for instance
                    this.dataGridView_List.Rows[e.RowIndex].Selected = true;
                    // Get mouse position relative to the vehicles grid
                    var relativeMousePosition = dataGridView_List.PointToClient(Cursor.Position);
                    SelectedRowIndex_ = e.RowIndex;
                    // Show the context menu
                    this.contextMenuStrip_List.Show(dataGridView_List, relativeMousePosition);
                    //this.ContextMenuStrip1.Show(dataGridView_List, relativeMousePosition);
                }
                else if(e.Button == MouseButtons.Left)
                {
                    if (e.RowIndex >= 0)
                    {
                        this.dataGridView_List.Rows[e.RowIndex].Selected = true;
                        MainForm pForm = (MainForm)this.ParentForm;
                        string strvalue = dataGridView_List.Rows[e.RowIndex].Cells[SeqIndex_].Value.ToString();
                        ManagerNavigation.Instance.Selected_Monitor_ = Convert.ToInt32(strvalue);
                        pForm.ShowDetailCState();
                    }
                }
            }
        }

        public void ClearList()
        {
            dataGridView_List.Rows.Clear();
        }

        public void Add_List(CrawlerData pCrawlerData)
        {         
            DataGridViewRow r1 = new DataGridViewRow();

            // 모니터 시퀀스
            DataGridViewTextBoxCell cell_seq = new DataGridViewTextBoxCell();
            cell_seq.Value = pCrawlerData.seq_;
            r1.Cells.Add(cell_seq);

            // 고유 번호( 아이피 와 크롤러 시퀀스 )
            DataGridViewTextBoxCell cell_IP = new DataGridViewTextBoxCell();
            cell_IP.Value = string.Format("{0}:{1}", pCrawlerData.IP_, pCrawlerData.CrawlerSeq_);
            r1.Cells.Add(cell_IP);

            // 크롤러 이름
            DataGridViewTextBoxCell cell_CName = new DataGridViewTextBoxCell();
            cell_CName.Value = pCrawlerData.CrawlerName_;
            r1.Cells.Add(cell_CName);

            // 권리사
            string id = "";
            AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrity(pCrawlerData.AuthoritySeq_);
            if (pAuthorityInfoData != null)
                id = pAuthorityInfoData.partnerName_;
            
            DataGridViewTextBoxCell cell_auth = new DataGridViewTextBoxCell();
            cell_auth.Value = id;
            r1.Cells.Add(cell_auth);

            // 채널
            DataGridViewTextBoxCell cell_channel = new DataGridViewTextBoxCell();
            cell_channel.Value = pCrawlerData.ChannelName_;
            r1.Cells.Add(cell_channel);
            
            // 권리사 로그인
            DataGridViewTextBoxCell cell_authlogin = new DataGridViewTextBoxCell();
            cell_authlogin.Value = string.Format("{0}", pCrawlerData.AuthorityLoginName_);
            r1.Cells.Add(cell_authlogin);

            // 상태
            DataGridViewTextBoxCell cell_state = new DataGridViewTextBoxCell();
            cell_state.Value = pCrawlerData.CrawlerState_;
            r1.Cells.Add(cell_state);

            // 최근 변경 시간
            DataGridViewTextBoxCell cell_update = new DataGridViewTextBoxCell();
            cell_update.Value = pCrawlerData.UpdateDate_;
            r1.Cells.Add(cell_update);

            dataGridView_List.Rows.Add(r1);
        }

        public void ChangeCrawlerMonitorInfo(CrawlerData pCrawlerData)
        {
            for (Int32 i = 0; i < dataGridView_List.Rows.Count; i++)
            {
                Int32 monitorseq = Convert.ToInt32(dataGridView_List.Rows[i].Cells[SeqIndex_].Value);

                if (monitorseq == pCrawlerData.seq_)
                {
                    string id = "";
                    AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrity(pCrawlerData.AuthoritySeq_);
                    if (pAuthorityInfoData != null)
                        id = pAuthorityInfoData.partnerName_;

                    dataGridView_List.Rows[i].Cells[1].Value = string.Format("{0}:{1}", pCrawlerData.IP_, pCrawlerData.CrawlerSeq_);
                    dataGridView_List.Rows[i].Cells[2].Value = pCrawlerData.CrawlerName_;
                    dataGridView_List.Rows[i].Cells[3].Value = id;
                    dataGridView_List.Rows[i].Cells[4].Value = pCrawlerData.ChannelName_;
                    dataGridView_List.Rows[i].Cells[5].Value = string.Format("{0}", pCrawlerData.AuthorityLoginName_);
                    dataGridView_List.Rows[i].Cells[6].Value = pCrawlerData.CrawlerState_;
                    break;
                }
            }
        }

        bool RemoveRowByMonitorSeq(Int32 MonitorSeq)
        {
            for(Int32 i = 0; i < dataGridView_List.Rows.Count; i++)
            {
                string str = dataGridView_List.Rows[i].Cells[SeqIndex_].Value.ToString();
                if (Convert.ToInt32(str) == MonitorSeq)
                {
                    dataGridView_List.Rows.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        private void ToolStripMenuItem_Delete_Click(object sender, EventArgs e)
        {
            try
            {
                string strvalue = dataGridView_List.Rows[SelectedRowIndex_].Cells[SeqIndex_].Value.ToString();                            
                CrawlerData pCrawlerData = CInfoManager.Instance.GetCrawlerData(Convert.ToInt32(strvalue));

                string message = string.Format("선택된({0}) 크롤러 정보를 삭제 할까요?\n신중히 생각해 주세요.\n현재 동작중인 크롤러를 삭제 할경우 정상적인 동작이 보장되지 않습니다."
                    , pCrawlerData.CrawlerName_);
                DialogResult dialogResult = MessageBox.Show(message, "확인", MessageBoxButtons.YesNo);
                
                if (dialogResult == DialogResult.Yes)
                {
                    // 삭제 할하고 빼자
                    if (CMDBInterFace.DeleteCrawlerMonitorInfo(CInfoManager.Instance.DB(), pCrawlerData.seq_) == true)
                    {
                        Int32 MonitorSeq = pCrawlerData.seq_;
                        CInfoManager.Instance.DelCrawlerData(MonitorSeq);
                        if (RemoveRowByMonitorSeq(MonitorSeq) == true)
                        {
                            MessageBox.Show(string.Format("{0} 번 크롤러 모니터가 삭제 되었습니다.", MonitorSeq));
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                string message = string.Format("삭제중 문제가 생겼습니다.\n{0}", ex.Message);
                MessageBox.Show(message);
            }
        }

        private void ToolStripMenuItem_Edit_Click(object sender, EventArgs e)
        {
            string strvalue = dataGridView_List.Rows[SelectedRowIndex_].Cells[SeqIndex_].Value.ToString();
            ManagerNavigation.Instance.Selected_Monitor_ = Convert.ToInt32(strvalue);
            CrawlerData pCrawlerData = CInfoManager.Instance.GetCrawlerData(ManagerNavigation.Instance.Selected_Monitor_);

            UpdateMonitorForm pUpdateMonitorForm = new UpdateMonitorForm();
            pUpdateMonitorForm.MakeCombo();
            pUpdateMonitorForm.SettingCurInfo(pCrawlerData);
            pUpdateMonitorForm.ShowDialog(this);
        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            MessageBox.Show("기능정의가 되지 않았습니다.");
            
            //// DB에 Add를 한후 성공이면, 
            //Int32 xMonitorSeq = 0;

            //CMDBInterFace.InsertCrawlerMonitorInfo(CInfoManager.Instance.DB(), "Monitor", -1000, ref xMonitorSeq);

            //if (xMonitorSeq > 0)
            //{
            //    CrawlerData pCrawlerData = new CrawlerData();
            //}
        }

        private void button_Refresh_Click(object sender, EventArgs e)
        {
            if (CMDBInterFace.GetAllCrawlerMonitor(CInfoManager.Instance.DB()) == true)
            {
                MainForm pMainForm = (MainForm)this.ParentForm;
                pMainForm.RefreshCrawerInfo();
            }
        }
    }
}
