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
    public partial class UpdateMonitorForm : Form
    {
        CrawlerData CrawlerData_ = null;

        List<TempChannel> List_TempChannel_ = new List<TempChannel>();
        List<TempAutoLogin> List_TempAutoLogin_ = new List<TempAutoLogin>();

        public UpdateMonitorForm()
        {
            InitializeComponent();

            comboBox_Authority.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_Authority;
            comboBox_Channel.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_Channel;
            comboBox_AuthLogin.SelectedIndexChanged += comboDropDown_SelectedIndexChanged_AuthoLogin;
        }

        public void MakeCombo()
        {
            // 권리사
            comboBox_Authority.Sorted = false;
            comboBox_Authority.Items.Add("권리사를 선택하세요.");
            Dictionary<Int32, AuthorityInfoData> List_Auth = AuthorityManager.Instance.GetList();
            foreach (var pData in List_Auth)
                comboBox_Authority.Items.Add(string.Format("{0}_{1}", pData.Value.seq_, pData.Value.partnerName_));

            comboBox_Authority.SelectedIndex = 0;

            // 채널
            comboBox_Channel.Sorted = false;
            //comboBox_Channel.Items.Add("채널을 선택하세요.");
            //Dictionary<Int32, ChannelInfoData> List_Channel = ChannelManager.Instance.GetList();
            //foreach (var pData in List_Channel)
            //    comboBox_Channel.Items.Add(string.Format("{0}_{1}", pData.Value.seq_, pData.Value.ChannelName_));

            //comboBox_Channel.SelectedIndex = 0;

            // 권리사 Login
            comboBox_AuthLogin.Sorted = false;
            //comboBox_AuthLogin.Items.Add("권리사 로그인을 선택하세요.");
            // 모드
            comboBox_CrawlerMode.Sorted = false;
            comboBox_CrawlerMode.Items.Add("모드를 선택하세요");
            comboBox_CrawlerMode.Items.Add("취합");
            comboBox_CrawlerMode.Items.Add("처리");
            comboBox_CrawlerMode.Items.Add("반환");
            comboBox_CrawlerMode.SelectedIndex = 0;
        }

        public void SettingCurInfo(CrawlerData pCrawlerData)
        {
            CrawlerData_ = pCrawlerData;

            AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrity(pCrawlerData.AuthoritySeq_);
            if (pAuthorityInfoData != null)
                comboBox_Authority.SelectedIndex = pAuthorityInfoData.ComboIndex_;
            else
                comboBox_Authority.SelectedIndex = 0;


            Int32 nSeleIndex = 0;
            if (List_TempChannel_.Count > 0 && pCrawlerData.ChannelSeq_ > 0)
            {
                for (Int32 i = 0; i < List_TempChannel_.Count; i++ )
                {
                    if (List_TempChannel_[i].seq_ == pCrawlerData.ChannelSeq_)
                    {
                        nSeleIndex = i + 1;
                        break;
                    }
                }
            }            

            comboBox_Channel.SelectedIndex = nSeleIndex;

            nSeleIndex = 0;
            if (List_TempAutoLogin_.Count > 0 && pCrawlerData.AuthorityLoginSeq_ > 0)
            {
                for (Int32 i = 0; i < List_TempAutoLogin_.Count; i++)
                {
                    if (List_TempAutoLogin_[i].seq_ == pCrawlerData.AuthorityLoginSeq_)
                    {
                        nSeleIndex = i + 1;
                        break;
                    }
                }
            }

            comboBox_AuthLogin.SelectedIndex = nSeleIndex;


            textBox_CrawlerSeq.Text = pCrawlerData.CrawlerSeq_.ToString();

            label_MonitorSeq.Text = pCrawlerData.seq_.ToString();
            textBox_CrawlerName.Text = pCrawlerData.CrawlerName_;

            comboBox_CrawlerMode.SelectedIndex = pCrawlerData.Mode_;
            textBox_Memo.Text = pCrawlerData.Memo_;
        }

        private void button_Confirm_Click(object sender, EventArgs e)
        {
            bool bChanged = false;

            MainForm pMainForm = (MainForm)this.Owner;

            if (comboBox_Authority.SelectedIndex == 0)
            {
                MessageBox.Show("권리사를 선택하세요");
                return;
            }

            if (comboBox_Channel.SelectedIndex == 0 || List_TempChannel_.Count == 0)
            {
                MessageBox.Show("채널을 선택하세요");
                return;
            }

            if (comboBox_AuthLogin.SelectedIndex == 0 || List_TempAutoLogin_.Count == 0)
            {
                MessageBox.Show("권리사 로그인을 선택하세요");
                return;
            }

            if (comboBox_CrawlerMode.SelectedIndex == 0)
            {
                MessageBox.Show("모드를 선택하세요");
                return;
            }

            Int32 xMonitorSeq = Convert.ToInt32(label_MonitorSeq.Text);

            string xCrawlerName = textBox_CrawlerName.Text;

            TempChannel pTempChannel = List_TempChannel_[comboBox_Channel.SelectedIndex-1];

            //ChannelInfoData pChannelInfoData = ChannelManager.Instance.GetChannelByComboBoxIndex(comboBox_Channel.SelectedIndex);
            Int32 xChannelSeq = pTempChannel.seq_;
            string xChannelName = pTempChannel.ChannelName_;

            TempAutoLogin pTempAutoLogin = List_TempAutoLogin_[comboBox_AuthLogin.SelectedIndex - 1];
            Int32 xAuthLoginSeq = pTempAutoLogin.seq_;
            string xAuthLoginName = pTempAutoLogin.Name_;

            AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(comboBox_Authority.SelectedIndex);
            Int32 xAuthoritySeq = pAuthorityInfoData.seq_;
            string xAuthorityName = pAuthorityInfoData.partnerName_;

            Int32 xCrawlerSeq = 0;

            try
            {
                xCrawlerSeq = Convert.ToInt32(textBox_CrawlerSeq.Text);
            }
            catch (System.Exception ex)
            {
                xCrawlerSeq = 0;
            }
            
            if (xCrawlerSeq == 0)
            {
                MessageBox.Show("크롤러 시퀀스 값이 이상합니다.");
                return;
            }
            Int32 xMode = comboBox_CrawlerMode.SelectedIndex;

            string xCrawler_OnOff = "";
            string xCrawler_location = "";
            Int32 xCrawlerCheckTime = 0;
            Int32 xDBUpdateTime = 0;
            string xMemo = textBox_Memo.Text;

            if (xCrawlerName != CrawlerData_.CrawlerName_ || xCrawlerSeq != CrawlerData_.CrawlerSeq_
                || xChannelSeq != CrawlerData_.ChannelSeq_ || xAuthoritySeq != CrawlerData_.AuthoritySeq_
                || xMode != CrawlerData_.Mode_ || xMemo != CrawlerData_.Memo_
                || xAuthLoginSeq != CrawlerData_.AuthorityLoginSeq_)
            {
                bChanged = CMDBInterFace.UpdateCrawlerMonitorInfo(CInfoManager.Instance.DB(), xMonitorSeq, xCrawlerName, xChannelSeq, xChannelName
                , xAuthoritySeq, xAuthorityName, xCrawlerSeq, xMode, xCrawler_OnOff, xCrawler_location, xCrawlerCheckTime, xDBUpdateTime
                , xMemo, xAuthLoginSeq, xAuthLoginName);

                if (bChanged == true)
                {
                    CrawlerData_.CrawlerName_ = xCrawlerName;
                    CrawlerData_.CrawlerSeq_ = xCrawlerSeq;
                    CrawlerData_.ChannelSeq_ = xChannelSeq;
                    CrawlerData_.ChannelName_ = xChannelName;
                    CrawlerData_.AuthoritySeq_ = xAuthoritySeq;
                    CrawlerData_.AuthorityLoginSeq_ = xAuthLoginSeq;
                    CrawlerData_.AuthorityLoginName_ = xAuthLoginName;
                    CrawlerData_.Mode_ = xMode;
                    CrawlerData_.Memo_ = xMemo;

                    pMainForm.ChangeCrawlerMonitorInfo(CrawlerData_);
                }
            }

            CrawlerData_ = null;

            Close();

            if (bChanged == true)
                MessageBox.Show("변경 완료 되었습니다.");
        }

        private void comboDropDown_SelectedIndexChanged_Authority(object sender, EventArgs e)
        {
            comboBox_Channel.Items.Clear();
            comboBox_Channel.Items.Add("채널을 선택하세요.");

            if (comboBox_Authority.SelectedIndex == 0)
            {
                comboBox_Channel.Enabled = false;                
            }
            else
            {
                comboBox_Channel.Enabled = true;

                AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(comboBox_Authority.SelectedIndex);

                List_TempChannel_.Clear();
                AuthorityLoginManager.Instance.GetChannelListByAuthoritySeq(pAuthorityInfoData.seq_, ref List_TempChannel_);

                foreach (var pData in List_TempChannel_)
                {
                    comboBox_Channel.Items.Add(pData.ChannelName_);
                }
            }

            comboBox_Channel.SelectedIndex = 0;
        }

        private void comboDropDown_SelectedIndexChanged_Channel(object sender, EventArgs e)
        {
            comboBox_AuthLogin.Items.Clear();
            comboBox_AuthLogin.Items.Add("권리사 로그인을 선택하세요.");

            if (comboBox_Channel.SelectedIndex == 0)
            {
                comboBox_AuthLogin.Enabled = false;
            }
            else
            {
                comboBox_AuthLogin.Enabled = true;
                AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrityByComboBoxIndex(comboBox_Authority.SelectedIndex);
                TempChannel pTempChannel = List_TempChannel_[comboBox_Channel.SelectedIndex - 1];
                List_TempAutoLogin_.Clear();
                AuthorityLoginManager.Instance.GetAuthorityLoginListByAuthoSeqAndChannelSeq(pAuthorityInfoData.seq_, pTempChannel.seq_, ref List_TempAutoLogin_);
                foreach (var pData in List_TempAutoLogin_)
                {
                    comboBox_AuthLogin.Items.Add(pData.Name_);
                }
            }

            comboBox_AuthLogin.SelectedIndex = 0;
        }

        private void comboDropDown_SelectedIndexChanged_AuthoLogin(object sender, EventArgs e)
        {

        }
    }
}
