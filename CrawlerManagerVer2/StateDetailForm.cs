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
    public partial class StateDetailForm : Form
    {
        public StateDetailForm()
        {
            InitializeComponent();
        }

        private void button_ToList_Click(object sender, EventArgs e)
        {
            ManagerNavigation.Instance.Selected_Monitor_ = 0;
            MainForm pForm = (MainForm)this.ParentForm;
            pForm.ShowCState();
        }

        public void SetInfo()
        {
            CrawlerData pCrawlerData = CInfoManager.Instance.GetCrawlerData(ManagerNavigation.Instance.Selected_Monitor_);
            List<string> listinfo = new List<string>();
            listinfo.Add(string.Format("크롤러 IP : {0} {1}번", pCrawlerData.IP_, pCrawlerData.CrawlerSeq_));
            listinfo.Add(string.Format("크롤러 동작방식 : {0}번 모드", pCrawlerData.Mode_));
            listinfo.Add(string.Format("담당 채널 : {0}", pCrawlerData.ChannelName_));
            listinfo.Add(string.Format("권리사 : {0}_{1}", pCrawlerData.AuthoritySeq_, pCrawlerData.AuthorityName_));
            listinfo.Add(string.Format("관리자 메모 : {0}", pCrawlerData.Memo_));

            richTextBox_Info.Lines = listinfo.ToArray();
        }

        public void RefreshList()
        {
            CrawlerData pCrawlerData = CInfoManager.Instance.GetCrawlerData(ManagerNavigation.Instance.Selected_Monitor_);

            dataGridView_GoodsList.Rows.Clear();

            Dictionary<Int32, CGoodsData> OutList = new Dictionary<Int32, CGoodsData>();
            Int32 nCount = GoodsManager.Instance.GetListByCrawerSeq(pCrawlerData.CrawlerSeq_, ref OutList);

            foreach (var pData in OutList)
            {
                AddToList(pData.Value);
            }
        }

        void AddToList(CGoodsData pCGoodsData)
        {
            DataGridViewRow r1 = new DataGridViewRow();

            // 상품 시퀀스
            DataGridViewTextBoxCell cell_seq = new DataGridViewTextBoxCell();
            cell_seq.Value = pCGoodsData.Seq_;
            r1.Cells.Add(cell_seq);

            // 상품명
            DataGridViewTextBoxCell cell_GoodsName = new DataGridViewTextBoxCell();
            cell_GoodsName.Value = pCGoodsData.GoodsName_;
            r1.Cells.Add(cell_GoodsName);

            // 옵션명
            DataGridViewTextBoxCell cell_OptionName = new DataGridViewTextBoxCell();
            cell_OptionName.Value = pCGoodsData.OptionName_;
            r1.Cells.Add(cell_OptionName);

            // 채널
            DataGridViewTextBoxCell cell_Channel = new DataGridViewTextBoxCell();
            cell_Channel.Value = pCGoodsData.ChannelName_;
            r1.Cells.Add(cell_Channel);

            dataGridView_GoodsList.Rows.Add(r1);
        }
    }
}
