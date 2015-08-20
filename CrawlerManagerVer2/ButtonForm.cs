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
    public partial class ButtonForm : Form
    {
        public ButtonForm()
        {
            InitializeComponent();
        }

        private void button_C_State_Click(object sender, EventArgs e)
        {
            MainForm pForm = (MainForm)this.ParentForm;
            pForm.ShowCState();
        }

        private void button_LogOut_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("로그아웃 할까요?", "확인", MessageBoxButtons.YesNo);

            if (dialogResult == DialogResult.Yes)
            {
                CInfoManager.Instance.InitDB();
                MainForm pForm = (MainForm)this.ParentForm;
                pForm.ShowLogin();
            }
        }

        //protected override void OnShown(EventArgs e)
        //{
        //    MessageBox.Show("OnShow");
        //}

        //protected override void OnActivated(EventArgs e)
        //{
        //    MessageBox.Show("OnActivated");
        //}

        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    MessageBox.Show("OnFormClosed");
        //}
        //protected override void OnFormClosing(FormClosingEventArgs e)
        //{
        //    MessageBox.Show("OnFormClosing");
        //}

        //protected override void OnClosed(EventArgs e)
        //{
        //    MessageBox.Show("OnClosed");
        //}

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    MessageBox.Show("OnClosing");
        //}

        private void button_Good_Click(object sender, EventArgs e)
        {
            MainForm pForm = (MainForm)this.ParentForm;
            pForm.ShowGoodsList();
        }

        private void button_Account_Click(object sender, EventArgs e)
        {
            MessageBox.Show("기능 정의가 되지 않았습니다.");
        }

        public void SetNameAndMobile()
        {
            label_Name.Text = CInfoManager.Instance.AdminInfoData_.adminName_;
            label_Mobile.Text = CInfoManager.Instance.AdminInfoData_.Mobile_;
        }

        //protected override void OnFormClosed(FormClosedEventArgs e)
        //{
        //    MessageBox.Show("OnFormClosed");
        //}
    }
}
