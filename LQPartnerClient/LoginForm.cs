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
    public partial class LoginForm : Form
    {
        Form1 MainForm_ = null;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void button_Login_Click(object sender, EventArgs e)
        {
            MainForm_ = new Form1();
            MainForm_.SetLoginForm(this);
            MainForm_.Show();
            
            
            Hide();
            //f2.MdiParent = this;
        }
    }
}
