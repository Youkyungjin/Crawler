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
    public partial class LoginForm : Form
    {
        bool bLoginSucceed_ = false;
        string IniPath_ = HKLibrary.UTIL.HKFileHelper.GetCurrentDirectory() + @"\CrawlerManagerVer2.INI";

        public LoginForm()
        {
            InitializeComponent();
        }

        public void SetLoginForm()
        {
            if (CMIniManager.Instance.checkbox_ == 1)
            {
                textBox_ID.Text = CMIniManager.Instance.loginid_;
                textBox_PW.Text = "";
                checkBox_Save.Checked = true;
            }
            else
            {
                textBox_ID.Text = "";
                textBox_PW.Text = "";
                checkBox_Save.Checked = false;
            }
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            if (CMIniManager.Instance.bLoad_ == false)
            {
                MessageBox.Show("INI 파일 로드에 실패하여 로그인이 불가 합니다. 확인부탁합니다.");
                return;
            }

            if (string.IsNullOrEmpty(textBox_ID.Text) == true
                || string.IsNullOrEmpty(textBox_PW.Text) == true)
            {
                MessageBox.Show("아이디 혹은 암호를 확인해주세요.");
                return;
            }

            BackgroundWorker Worker_Connection = new BackgroundWorker();
            Worker_Connection = new BackgroundWorker();
            Worker_Connection.WorkerReportsProgress = false;
            Worker_Connection.WorkerSupportsCancellation = true;
            Worker_Connection.DoWork += new DoWorkEventHandler(DBConnection_Worker);
            Worker_Connection.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DBConnection_Worker_Completed);

            Worker_Connection.RunWorkerAsync();

            MainForm pForm = (MainForm)this.ParentForm;
            pForm.ShowWaitDialog();
        }

        void DBConnection_Worker(object sender, DoWorkEventArgs e)
        {
            MainForm pMainForm = (MainForm)this.ParentForm;

            bool bResult = CInfoManager.Instance.ConnectDB();

            if (bResult == true)
                bResult = CMDBInterFace.LoginManager(CInfoManager.Instance.DB(), textBox_ID.Text, textBox_PW.Text);

            if (bResult == true)
                bResult = CMDBInterFace.GetAllCrawlerMonitor(CInfoManager.Instance.DB());

            if (bResult == true)
                bResult = CMDBInterFace.GetAuthorityList(CInfoManager.Instance.DB());

            if (bResult == true)
                bResult = CMDBInterFace.GetAuthorityLoginList(CInfoManager.Instance.DB());

            //if (bResult == true)
            //    bResult = CMDBInterFace.GetChannelList(CInfoManager.Instance.DB());

            if (bResult == true)
                bResult = CMDBInterFace.GetGoodsTable(CInfoManager.Instance.DB());

            if (bResult == true)
            {
                pMainForm.RefreshAdmInfo();
                pMainForm.RemakeComboBox();
                pMainForm.RefreshGoodsTable();
                pMainForm.RefreshCrawerInfo();
            }

            bLoginSucceed_ = bResult;
        }

        void DBConnection_Worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MainForm pForm = (MainForm)this.ParentForm;                
                pForm.HideWaitDialog();
                MessageBox.Show(e.Error.Message, "DB 연결중 에러 발생 다시 실행해 주세요.");
                return;
            }

            if (bLoginSucceed_ == true)
            {
                if (checkBox_Save.Checked == true)
                    CMIniManager.Instance.SaveCheckBoxAndID(IniPath_, "1", textBox_ID.Text);
                else
                    CMIniManager.Instance.SaveCheckBoxAndID(IniPath_, "0", "");

                CInfoManager.Instance.AdminInfoData_.ID_ = textBox_ID.Text;
                CInfoManager.Instance.AdminInfoData_.PW_ = textBox_PW.Text;

                MainForm pForm = (MainForm)this.ParentForm;
                pForm.ShowCState();
                pForm.HideWaitDialog();
            }
            else
            {
                MainForm pForm = (MainForm)this.ParentForm;
                pForm.HideWaitDialog();
                MessageBox.Show("로그인중 문제가 발생했습니다. 확인후 다시 시도해 주세요.");
            }
        }
    }
}
