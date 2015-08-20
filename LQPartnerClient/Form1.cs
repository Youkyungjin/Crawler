using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LQPartnerClient
{
    public partial class Form1 : Form
    {
        LoginForm LoginForm_ = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            panel_Home.Hide();
        }

        public void SetLoginForm(LoginForm lf)
        {
            LoginForm_ = lf;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            MessageBox.Show("죵료합니다.");
            Environment.Exit(0);
            //Application.Exit();
//            LoginForm_.Show();
        }
    }
}
