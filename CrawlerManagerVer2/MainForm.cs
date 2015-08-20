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
    public partial class MainForm : Form
    {
        #region UI 에 필요한 패널과 WinForm 모음
        Panel Panel_Login_ = null;
        Panel Panel_Button_ = null;
        Panel Panel_Crawler_State_ = null;
        Panel Panel_Goods_Manage_ = null;
        Panel Panel_Crawler_Detail_State_ = null;

        LoginForm LoginForm_ = null;
        StateForm CrawlerStateForm_ = null;
        GoodsListForm GoodsListForm_ = null;
        WaitDialog WaitDialog_ = null;
        StateDetailForm StateDetailForm_ = null;
        ButtonForm buttonForm_ = null;
        #endregion

        public MainForm()
        {
            InitializeComponent();
            INILoad();
            MakeUI();            
        }

        #region 초기 INI 파일 로드
        void INILoad()
        {
            string IniPath = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + @"\CrawlerManagerVer2.INI";
            if (CMIniManager.Instance.LoadIni(IniPath) == false)
            {
                MessageBox.Show("INI 파일 로딩에 실패 했습니다. 셋팅을 확인하세요.");
            }
        }
        #endregion

        #region 패널과 WinForm 만들기
        void MakeUI()
        {
            Int32 RightPanelX = 200;
            Int32 RightPanelY = 20;
            Int32 RightPanelWidth = 690;
            Point center = new Point(this.Size.Width / 2, this.Size.Height / 2);
            #region 로그인 패널
            Panel_Login_ = new Panel();
            Panel_Login_.Location = new Point(center.X - 400/2, 50);
            Panel_Login_.Size = new Size(400, 300);
            Panel_Login_.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(Panel_Login_);

            LoginForm_ = new LoginForm();
            LoginForm_.TopLevel = false;
            LoginForm_.Dock = System.Windows.Forms.DockStyle.Fill;
            Panel_Login_.Controls.Add(LoginForm_);
            LoginForm_.Show();
            LoginForm_.FormBorderStyle = FormBorderStyle.None;            
            #endregion

            #region 버튼 패널
            Panel_Button_ = new Panel();
            Panel_Button_.Location = new Point(20, 20);
            Panel_Button_.Size = new Size(175, 550);
            Panel_Button_.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(Panel_Button_);

            buttonForm_ = new ButtonForm();
            buttonForm_.TopLevel = false;
            buttonForm_.Dock = System.Windows.Forms.DockStyle.Fill;
            Panel_Button_.Controls.Add(buttonForm_);
            buttonForm_.Show();
            buttonForm_.FormBorderStyle = FormBorderStyle.None;
            #endregion

            #region 크롤러 상태 패널
            Panel_Crawler_State_ = new Panel();
            Panel_Crawler_State_.Location = new Point(RightPanelX, RightPanelY);
            Panel_Crawler_State_.Size = new Size(RightPanelWidth, 550);
            Panel_Crawler_State_.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(Panel_Crawler_State_);

            CrawlerStateForm_ = new StateForm();
            CrawlerStateForm_.TopLevel = false;
            CrawlerStateForm_.Dock = System.Windows.Forms.DockStyle.Fill;
            Panel_Crawler_State_.Controls.Add(CrawlerStateForm_);
            CrawlerStateForm_.Show();
            CrawlerStateForm_.FormBorderStyle = FormBorderStyle.None;
            #endregion

            #region 크롤러 상태 상세 패널
            Panel_Crawler_Detail_State_ = new Panel();
            Panel_Crawler_Detail_State_.Location = new Point(RightPanelX, RightPanelY);
            Panel_Crawler_Detail_State_.Size = new Size(RightPanelWidth, 550);
            Panel_Crawler_Detail_State_.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(Panel_Crawler_Detail_State_);

            StateDetailForm_ = new StateDetailForm();
            StateDetailForm_.TopLevel = false;
            StateDetailForm_.Dock = System.Windows.Forms.DockStyle.Fill;
            Panel_Crawler_Detail_State_.Controls.Add(StateDetailForm_);
            StateDetailForm_.Show();
            StateDetailForm_.FormBorderStyle = FormBorderStyle.None;
            #endregion

            #region 상품 상태 패널
            Panel_Goods_Manage_ = new Panel();
            Panel_Goods_Manage_.Location = new Point(RightPanelX, RightPanelY);
            Panel_Goods_Manage_.Size = new Size(RightPanelWidth, 550);
            Panel_Goods_Manage_.BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(Panel_Goods_Manage_);

            GoodsListForm_ = new GoodsListForm();
            GoodsListForm_.TopLevel = false;
            GoodsListForm_.Dock = System.Windows.Forms.DockStyle.Fill;
            Panel_Goods_Manage_.Controls.Add(GoodsListForm_);
            GoodsListForm_.Show();
            GoodsListForm_.FormBorderStyle = FormBorderStyle.None;
            #endregion

            WaitDialog_ = new WaitDialog();
            WaitDialog_.Hide();

            #region 초기값으로 변경 하기
            ShowLogin();
            #endregion
        }
        #endregion

        #region 크롤러 상태 보기
        public void ShowCState()
        {
            Panel_Login_.Hide();
            Panel_Goods_Manage_.Hide();
            Panel_Crawler_Detail_State_.Hide();            
            Panel_Button_.Show();
            Panel_Crawler_State_.Show();
        }
        #endregion

        #region 크롤러 세부 정보 보기
        public void ShowDetailCState()
        {
            Panel_Goods_Manage_.Hide();            
            Panel_Crawler_State_.Hide();
            Panel_Login_.Hide();

            Panel_Button_.Show();
            StateDetailForm_.SetInfo();
            StateDetailForm_.RefreshList();
            Panel_Crawler_Detail_State_.Show();
        }
        #endregion

        

        #region 로그인 패널 보기
        public void ShowLogin()
        {
            Panel_Crawler_Detail_State_.Hide();
            Panel_Goods_Manage_.Hide();
            Panel_Button_.Hide();
            Panel_Crawler_State_.Hide();
            LoginForm_.SetLoginForm();
            Panel_Login_.Show();
        }
        #endregion

        #region 상품 리스트 보기
        public void ShowGoodsList()
        {
            Panel_Crawler_Detail_State_.Hide();
            Panel_Login_.Hide();
            Panel_Crawler_State_.Hide();
            Panel_Button_.Show();
            GoodsListForm_.InitFirst();
            Panel_Goods_Manage_.Show();
            WaitDialog_.Hide();
        }

        #endregion

        // 크롤러 모니터 정보 refresh
        public void RefreshCrawerInfo()
        {
            CrawlerStateForm_.ClearList();
            Dictionary<Int32, CrawlerData> plist = CInfoManager.Instance.GetList();
            foreach (var pData in plist)
            {
                CrawlerStateForm_.Add_List(pData.Value);
            }
        }
        // 크롤러 모니터 정보 refresh
        public void RefreshGoodsTable()
        {
            GoodsListForm_.ClearList();
            Dictionary<Int32, CGoodsData> plist = GoodsManager.Instance.GetList();
            foreach (var pData in plist)
            {
                AuthorityInfoData pAuthorityInfoData = AuthorityManager.Instance.GetAuthrity(pData.Value.AuthoritySeq_);
                if (pAuthorityInfoData == null)
                    pData.Value.AuthrityName_ = "권리사 찾을수 없음";
                else
                    pData.Value.AuthrityName_ = pAuthorityInfoData.partnerName_;

                ChannelInfoData pChannelInfoData = ChannelManager.Instance.GetChannel(pData.Value.ChannelSeq_);
                if (pChannelInfoData == null)
                    pData.Value.ChannelName_ = "채널 찾을수 없음";
                else
                    pData.Value.ChannelName_ = pChannelInfoData.ChannelName_;

                GoodsListForm_.Add_List(pData.Value);

                //GoodsListForm_.Add_List(pData.Value.Seq_.ToString(), pData.Value.GoodsName_, pData.Value.OptionName_, pData.Value.ChannelName_
                //    , pData.Value.AuthrityName_, "none");
            }
        }
        // 관리자 정보 Refresh
        public void RefreshAdmInfo()
        {
            buttonForm_.InvokeIfNeeded(buttonForm_.SetNameAndMobile);
        }

        // 상품 테이블 콤보 박스 만들기
        public void RemakeComboBox()
        {
            GoodsListForm_.InvokeIfNeeded(GoodsListForm_.MakeCombo);
        }

        public void ChangeCrawlerMonitorInfo(CrawlerData pCrawlerData)
        {
            CrawlerStateForm_.ChangeCrawlerMonitorInfo(pCrawlerData);
        }

        public void ChangeCrawlerSeq(CGoodsData pCGoodsData_)
        {
            GoodsListForm_.ChangeCrawlerSeq(pCGoodsData_);
        }

        public void ChangeGoodsNickAndOptionNick(CGoodsData pCGoodsData_)
        {
            GoodsListForm_.ChangeGoodsNickAndOptionNick(pCGoodsData_);
        }

        #region Wait Panel 변경 함수
        public void ShowWaitDialog(string str = "작업중...")
        {
            WaitDialog_.SetLabelState(str);
            WaitDialog_.ShowDialog();
        }

        public void HideWaitDialog()
        {
            WaitDialog_.Hide();
        }

        #endregion
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
