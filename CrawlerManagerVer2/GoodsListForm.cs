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
    public partial class GoodsListForm : Form
    {
        Int32 SelectIndex_Authority_ = -1;
        Int32 SelectIndex_Channel_ = -1;
        Int32 SelectIndex_AuthorityLogin_ = -1;

        List<TempChannel> List_TempChannel_ = new List<TempChannel>();
        List<TempAutoLogin> List_TempAutoLogin_ = new List<TempAutoLogin>();

        public GoodsListForm()
        {
            InitializeComponent();

            dataGridView_GoodList.CellClick += dataGridView1_CellClick;
        }

        void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0)
            {
                this.dataGridView_GoodList.Rows[e.RowIndex].Selected = true;
                MainForm pForm = (MainForm)this.ParentForm;

                string value = dataGridView_GoodList.Rows[e.RowIndex].Cells[0].Value.ToString();
                ManagerNavigation.Instance.Selected_Goods_Seq_ = Convert.ToInt32(value);

                CGoodsData pCGoodsData = GoodsManager.Instance.GetCGoodsData(ManagerNavigation.Instance.Selected_Goods_Seq_);

                UpdateGoodsForm pUpdateGoodsForm = new UpdateGoodsForm();
                pUpdateGoodsForm.SettingCurInfo(pCGoodsData);
                pUpdateGoodsForm.ShowDialog(this);
            }
        }

        public void MakeCombo()
        {
            // 권리사
            comboBox_Auth.Sorted = false;
            comboBox_Auth.Items.Add("모든 권리사");
            Dictionary<Int32, AuthorityInfoData> List_Auth = AuthorityManager.Instance.GetList();
            foreach (var pData in List_Auth)
                comboBox_Auth.Items.Add(string.Format("{0}_{1}", pData.Value.seq_, pData.Value.partnerName_));
            
            comboBox_Auth.SelectedIndex = SelectIndex_Authority_ = 0;
            comboBox_Auth.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_Auth;

            // 채널
            comboBox_Channel.Sorted = false;
            comboBox_Channel.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_Channel;

            // Authologin
            comboBox_AuthLogin.Sorted = false;
            comboBox_AuthLogin.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_AuthoLogin;
        }

        public void InitFirst()
        {
            SelectIndex_Authority_ = -1;
            SelectIndex_Channel_ = -1;
            SelectIndex_AuthorityLogin_ = -1;
            comboBox_Auth.SelectedIndex = 0;
            comboDropDown_SelectedIndexChanged_Auth(null, null);
        }

        private void comboDropDown_SelectedIndexChanged_Auth(object sender, EventArgs e)
        {
            if (SelectIndex_Authority_ == comboBox_Auth.SelectedIndex)
                return;

            SelectIndex_Authority_ = comboBox_Auth.SelectedIndex;

            comboBox_Channel.Items.Clear();
            comboBox_Channel.Items.Add("모든 채널");

            if (SelectIndex_Authority_ > 0)
            {
                AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(SelectIndex_Authority_);
                List_TempChannel_.Clear();
                AuthorityLoginManager.Instance.GetChannelListByAuthoritySeq(pAuthorityInfoData.seq_, ref List_TempChannel_);

                foreach (var pData in List_TempChannel_)
                {
                    comboBox_Channel.Items.Add(pData.ChannelName_);
                }
            }

            if (SelectIndex_Authority_ == 0)
                comboBox_Channel.Enabled = false;
            else
                comboBox_Channel.Enabled = true;

            SelectIndex_Channel_ = -1;
            comboBox_Channel.SelectedIndex = 0;
        }

        private void comboDropDown_SelectedIndexChanged_Channel(object sender, EventArgs e)
        {
//            MessageBox.Show("comboDropDown_SelectedIndexChanged_Channel");
            if (SelectIndex_Channel_ == comboBox_Channel.SelectedIndex)
                return;

            SelectIndex_Channel_ = comboBox_Channel.SelectedIndex;

            AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(SelectIndex_Authority_);
            comboBox_AuthLogin.Items.Clear();
            comboBox_AuthLogin.Items.Add("모든 권리사 로그인");

            List_TempAutoLogin_.Clear();

            if (SelectIndex_Channel_ > 0)
            {
                
                //ChannelInfoTempData pChannelInfoTempData = ChannelTempManager.Instance.GetChannelByComboBoxIndex(SelectIndex_Channel_);
                AuthorityLoginManager.Instance.GetAuthorityLoginListByAuthoSeqAndChannelSeq(pAuthorityInfoData.seq_
                    , List_TempChannel_[SelectIndex_Channel_-1].seq_, ref List_TempAutoLogin_);

                foreach (var pData in List_TempAutoLogin_)
                {
                    comboBox_AuthLogin.Items.Add(pData.Name_);
                }
            }

            if (SelectIndex_Channel_ == 0)
                comboBox_AuthLogin.Enabled = false;
            else
                comboBox_AuthLogin.Enabled = true;

            SelectIndex_AuthorityLogin_ = -1;
            comboBox_AuthLogin.SelectedIndex = 0;
        }

        private void comboDropDown_SelectedIndexChanged_AuthoLogin(object sender, EventArgs e)
        {
//            MessageBox.Show("comboDropDown_SelectedIndexChanged_AuthoLogin");
            if (SelectIndex_AuthorityLogin_ == comboBox_AuthLogin.SelectedIndex)
                return;

            SelectIndex_AuthorityLogin_ = comboBox_AuthLogin.SelectedIndex;

            RefreshList();
        }

        void RefreshList()
        {
            const Int32 AuthoIndex = 6;
            const Int32 ChannelIndex = 3;
            const Int32 AuthoLoginIndex = 9;

            // 일단 다 켜고
            for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
                dataGridView_GoodList.Rows[i].Visible = true;

            if (SelectIndex_Authority_ > 0)
            {
                AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(SelectIndex_Authority_);
                for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
                {
                    if (dataGridView_GoodList.Rows[i].Visible == true)
                    {
                        string str_autho = dataGridView_GoodList.Rows[i].Cells[AuthoIndex].Value.ToString();
                        if (string.IsNullOrEmpty(str_autho) == true)
                        {
                            dataGridView_GoodList.Rows[i].Visible = false;
                        }
                        else
                        {
                            Int32 authoSeq = Convert.ToInt32(str_autho);
                            if (authoSeq != pAuthorityInfoData.seq_)
                                dataGridView_GoodList.Rows[i].Visible = false;
                        }
                    }
                }
            }

            if (SelectIndex_Channel_ > 0 && SelectIndex_Channel_ <= List_TempChannel_.Count)
            {
                TempChannel pTempChannel = List_TempChannel_[SelectIndex_Channel_-1];
                for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
                {
                    if (dataGridView_GoodList.Rows[i].Visible == true)
                    {
                        string str_channel = dataGridView_GoodList.Rows[i].Cells[ChannelIndex].Value.ToString();

                        if (string.IsNullOrEmpty(str_channel) == true)
                        {
                            dataGridView_GoodList.Rows[i].Visible = false;
                        }
                        else
                        {
                            Int32 channelseq = Convert.ToInt32(str_channel);
                            if (channelseq != pTempChannel.seq_)
                                dataGridView_GoodList.Rows[i].Visible = false;
                        }
                    }
                }
            }

            if (SelectIndex_AuthorityLogin_ > 0 && SelectIndex_AuthorityLogin_ <= List_TempAutoLogin_.Count)
            {
                TempAutoLogin pTempAutoLogin = List_TempAutoLogin_[SelectIndex_AuthorityLogin_-1];
                for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
                {
                    if (dataGridView_GoodList.Rows[i].Visible == true)
                    {
                        string str_authologin = dataGridView_GoodList.Rows[i].Cells[AuthoLoginIndex].Value.ToString();

                        if (string.IsNullOrEmpty(str_authologin) == true)
                        {
                            dataGridView_GoodList.Rows[i].Visible = false;
                        }
                        else
                        {
                            Int32 autologinseq = Convert.ToInt32(str_authologin);
                            if (autologinseq != pTempAutoLogin.seq_)
                                dataGridView_GoodList.Rows[i].Visible = false;
                        }
                    }
                }
            }
        }

        //void RefreshList()
        //{
        //    const Int32 AuthoIndex = 6;
        //    const Int32 ChannelIndex = 3;
        //    const Int32 AuthoLoginIndex = 9;

        //    if (SelectIndex_Authority_ == 0)
        //    {
        //        for(Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //            dataGridView_GoodList.Rows[i].Visible = true;
        //    }
        //    else if (SelectIndex_Authority_ > 0)
        //    {
        //        AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(SelectIndex_Authority_);

        //        if (SelectIndex_Channel_ == 0)
        //        {
        //            for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //            {
        //                string str_autho = dataGridView_GoodList.Rows[i].Cells[AuthoIndex].Value.ToString();
        //                if (string.IsNullOrEmpty(str_autho) == true)
        //                {
        //                    dataGridView_GoodList.Rows[i].Visible = false;
        //                }
        //                else
        //                {
        //                    Int32 authoSeq = Convert.ToInt32(str_autho);
        //                    if (authoSeq == pAuthorityInfoData.seq_)
        //                        dataGridView_GoodList.Rows[i].Visible = true;
        //                    else
        //                        dataGridView_GoodList.Rows[i].Visible = false;
        //                }
        //            }
        //        }
        //        else
        //        {
        //            TempChannel pTempChannel = List_TempChannel_[SelectIndex_Channel_];
        //            for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //            {
        //                string str_autho = dataGridView_GoodList.Rows[i].Cells[AuthoIndex].Value.ToString();
        //                string str_channel = dataGridView_GoodList.Rows[i].Cells[ChannelIndex].Value.ToString();
                        
        //                if (string.IsNullOrEmpty(str_autho) == true || string.IsNullOrEmpty(str_channel) == true)
        //                {
        //                    Int32 authoSeq = Convert.ToInt32(str_autho);
        //                    Int32 channelseq = Convert.ToInt32(str_channel);

        //                    if (authoSeq == pAuthorityInfoData.seq_ && channelseq == pTempChannel.seq_)
        //                        dataGridView_GoodList.Rows[i].Visible = true;
        //                    else
        //                        dataGridView_GoodList.Rows[i].Visible = false;
        //                }
        //                else
        //                {
        //                    dataGridView_GoodList.Rows[i].Visible = false;
        //                }
        //            }
        //        }
        //    }

        //    //string str_selected_auth = "";
        //    //string str_selected_channel = "";

        //    //AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(SelectIndex_Authority_);
        //    //if (pAuthorityInfoData != null)
        //    //    str_selected_auth = pAuthorityInfoData.partnerName_;

        //    ////List_TempChannel_[SelectIndex_Channel_].seq_;

        //    //ChannelInfoData pChannelInfoData = ChannelManager.Instance.GetChannelByComboBoxIndex(SelectIndex_Channel_);
        //    //if (pChannelInfoData != null)
        //    //    str_selected_channel = pChannelInfoData.ChannelName_;

        //    //if (comboBox_Auth.SelectedIndex == 0 && comboBox_Channel.SelectedIndex == 0)
        //    //{
        //    //    for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //    //            dataGridView_GoodList.Rows[i].Visible = true;
        //    //}
        //    //else if (comboBox_Auth.SelectedIndex > 0 && comboBox_Channel.SelectedIndex > 0)
        //    //{
        //    //    for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //    //    {
        //    //        if (str_selected_auth == dataGridView_GoodList.Rows[i].Cells[AuthoIndex].Value.ToString()
        //    //            && str_selected_channel == dataGridView_GoodList.Rows[i].Cells[ChannelIndex].Value.ToString())
        //    //        {
        //    //            dataGridView_GoodList.Rows[i].Visible = true;
        //    //        }
        //    //        else
        //    //        {
        //    //            dataGridView_GoodList.Rows[i].Visible = false;
        //    //        }
        //    //    }
        //    //}
        //    //else if (comboBox_Auth.SelectedIndex > 0 && comboBox_Channel.SelectedIndex == 0)
        //    //{
        //    //    for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //    //    {
        //    //        if (str_selected_auth == dataGridView_GoodList.Rows[i].Cells[AuthoIndex].Value.ToString())                        
        //    //            dataGridView_GoodList.Rows[i].Visible = true;
        //    //        else
        //    //            dataGridView_GoodList.Rows[i].Visible = false;
        //    //    }
        //    //}
        //    //else if (comboBox_Auth.SelectedIndex == 0 && comboBox_Channel.SelectedIndex > 0)
        //    //{
        //    //    for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
        //    //    {
        //    //        if (str_selected_channel == dataGridView_GoodList.Rows[i].Cells[ChannelIndex].Value.ToString())
        //    //            dataGridView_GoodList.Rows[i].Visible = true;
        //    //        else
        //    //            dataGridView_GoodList.Rows[i].Visible = false;
        //    //    }
        //    //}
        //}

        private void button_Search_Click(object sender, EventArgs e)
        {
            
        }

        public void ClearList()
        {
            dataGridView_GoodList.Rows.Clear();
        }

        public void Add_List(CGoodsData pCGoodsData)
        {
            DataGridViewRow r1 = new DataGridViewRow();

            // 시퀀스
            DataGridViewTextBoxCell cell_seq = new DataGridViewTextBoxCell();
            cell_seq.Value = pCGoodsData.Seq_;
            r1.Cells.Add(cell_seq);

            // 상품명
            DataGridViewTextBoxCell cell_Name = new DataGridViewTextBoxCell();
            cell_Name.Value = pCGoodsData.GoodsName_;
            r1.Cells.Add(cell_Name);

            // 옵션명
            DataGridViewTextBoxCell cell_Option = new DataGridViewTextBoxCell();
            cell_Option.Value = pCGoodsData.OptionName_;
            r1.Cells.Add(cell_Option);

            // 채널
            DataGridViewTextBoxCell cell_channel = new DataGridViewTextBoxCell();
            //cell_channel.Value = pCGoodsData.ChannelName_;
            cell_channel.Value = pCGoodsData.ChannelSeq_.ToString();
            r1.Cells.Add(cell_channel);

            // 상품명(닉
            DataGridViewTextBoxCell cell_goods_nick = new DataGridViewTextBoxCell();
            cell_goods_nick.Value = pCGoodsData.GoodsNickName_;
            r1.Cells.Add(cell_goods_nick);

            // 옵션명(닉
            DataGridViewTextBoxCell cell_option_nick = new DataGridViewTextBoxCell();
            cell_option_nick.Value = pCGoodsData.OptionNickName_;
            r1.Cells.Add(cell_option_nick);

            // 권리사
            DataGridViewTextBoxCell cell_Auth = new DataGridViewTextBoxCell();
            cell_Auth.Value = pCGoodsData.AuthoritySeq_;
            r1.Cells.Add(cell_Auth);

            // 크롤러 시퀀스
            DataGridViewTextBoxCell cell_crawler_seq = new DataGridViewTextBoxCell();
            cell_crawler_seq.Value = pCGoodsData.CrawlerSeq_;
            r1.Cells.Add(cell_crawler_seq);

            // 상태
            DataGridViewTextBoxCell cell_State = new DataGridViewTextBoxCell();
            cell_State.Value = pCGoodsData.State_;
            r1.Cells.Add(cell_State);

            // 권리사 로그인 시퀀스
            DataGridViewTextBoxCell cell_AuLogin = new DataGridViewTextBoxCell();
            cell_AuLogin.Value = pCGoodsData.AuthorityLoginSeq_;
            r1.Cells.Add(cell_AuLogin);

            dataGridView_GoodList.Rows.Add(r1);
        }

        public void ChangeCrawlerSeq(CGoodsData pCGoodsData)
        {
            for(Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
            {
                Int32 goodsseq = Convert.ToInt32(dataGridView_GoodList.Rows[i].Cells[0].Value);

                if (goodsseq == pCGoodsData.Seq_)
                {
                    dataGridView_GoodList.Rows[i].Cells[7].Value = pCGoodsData.CrawlerSeq_;
                    break;
                }
            }
        }

        public void ChangeGoodsNickAndOptionNick(CGoodsData pCGoodsData)
        {
            for (Int32 i = 0; i < dataGridView_GoodList.Rows.Count; i++)
            {
                Int32 goodsseq = Convert.ToInt32(dataGridView_GoodList.Rows[i].Cells[0].Value);

                if (goodsseq == pCGoodsData.Seq_)
                {
                    dataGridView_GoodList.Rows[i].Cells[4].Value = pCGoodsData.GoodsNickName_;
                    dataGridView_GoodList.Rows[i].Cells[5].Value = pCGoodsData.OptionNickName_;
                    break;
                }
            }
        }

    }
}
