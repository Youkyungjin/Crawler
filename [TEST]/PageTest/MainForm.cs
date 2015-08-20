using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PageTest
{
    public partial class MainForm : Form
    {
        Panel Panel_Login_ = null;
        Panel Panel_Button_ = null;

        public MainForm()
        {
            InitializeComponent();
            MakePanels();

        }

        void MakePanels()
        {
            Panel_Login_ = new Panel();
            Panel_Login_.Location = new Point(60, 5 * 20);
            Panel_Login_.Size = new Size(100, 100);
            Panel_Login_.BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(Panel_Login_);


        }
        //private void buttonTest_1_Click(object sender, EventArgs e)
        //{
        //    panel_Test1.Hide();
        //    panel_Test2.Show();
        //    //panel_Test1.Enabled = false;
        //    //panel_Test2.Enabled = true;
        //}

        //private void button_Test2_Click(object sender, EventArgs e)
        //{
        //    panel_Test1.Show();
        //    panel_Test2.Hide();
        //}

        //private void button_Page1_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("button_Page1_Click");
        //}

        //private void button_Page2_Click(object sender, EventArgs e)
        //{
        //    MessageBox.Show("button_Page2_Click");
        //    TextBox pTest  = new TextBox();
        //    pTest.Location = new Point(60, 5 * 20);
        //    pTest.Size = new Size(100, 10);
        //    pTest.Name = "호랑말코";
        //    panel_Test1.Controls.Add(pTest);
        //}

        //private void button_Test3_Click(object sender, EventArgs e)
        //{
        //    Form f1 = new StateForm();
        //    f1.TopLevel = false;
        //    f1.Dock = System.Windows.Forms.DockStyle.Fill;
        //    panel_Test3.Controls.Add(f1);
        //    f1.Show();
        //    f1.FormBorderStyle = FormBorderStyle.None;
        //}
    }
}
