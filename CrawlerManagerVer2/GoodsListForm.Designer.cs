namespace CrawlerManagerVer2
{
    partial class GoodsListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView_GoodList = new System.Windows.Forms.DataGridView();
            this.textBox_GoodName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_Auth = new System.Windows.Forms.ComboBox();
            this.comboBox_Channel = new System.Windows.Forms.ComboBox();
            this.button_Search = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox_AuthLogin = new System.Windows.Forms.ComboBox();
            this.Seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GoodsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GoodsNameNICK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionNameNICK = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Auth = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CrawlerSeq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AuthorityLogin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GoodList)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView_GoodList
            // 
            this.dataGridView_GoodList.AllowUserToAddRows = false;
            this.dataGridView_GoodList.AllowUserToDeleteRows = false;
            this.dataGridView_GoodList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_GoodList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Seq,
            this.GoodsName,
            this.OptionName,
            this.Channel,
            this.GoodsNameNICK,
            this.OptionNameNICK,
            this.Auth,
            this.CrawlerSeq,
            this.State,
            this.AuthorityLogin});
            this.dataGridView_GoodList.Location = new System.Drawing.Point(12, 130);
            this.dataGridView_GoodList.Name = "dataGridView_GoodList";
            this.dataGridView_GoodList.ReadOnly = true;
            this.dataGridView_GoodList.RowTemplate.Height = 23;
            this.dataGridView_GoodList.Size = new System.Drawing.Size(660, 420);
            this.dataGridView_GoodList.TabIndex = 0;
            // 
            // textBox_GoodName
            // 
            this.textBox_GoodName.Location = new System.Drawing.Point(87, 85);
            this.textBox_GoodName.Name = "textBox_GoodName";
            this.textBox_GoodName.Size = new System.Drawing.Size(195, 21);
            this.textBox_GoodName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(29, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "상품명";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "분류";
            // 
            // comboBox_Auth
            // 
            this.comboBox_Auth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Auth.FormattingEnabled = true;
            this.comboBox_Auth.Location = new System.Drawing.Point(87, 55);
            this.comboBox_Auth.Name = "comboBox_Auth";
            this.comboBox_Auth.Size = new System.Drawing.Size(170, 20);
            this.comboBox_Auth.TabIndex = 4;
            // 
            // comboBox_Channel
            // 
            this.comboBox_Channel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Channel.FormattingEnabled = true;
            this.comboBox_Channel.Location = new System.Drawing.Point(276, 55);
            this.comboBox_Channel.Name = "comboBox_Channel";
            this.comboBox_Channel.Size = new System.Drawing.Size(141, 20);
            this.comboBox_Channel.TabIndex = 5;
            // 
            // button_Search
            // 
            this.button_Search.Location = new System.Drawing.Point(316, 85);
            this.button_Search.Name = "button_Search";
            this.button_Search.Size = new System.Drawing.Size(114, 24);
            this.button_Search.TabIndex = 6;
            this.button_Search.Text = "검색";
            this.button_Search.UseVisualStyleBackColor = true;
            this.button_Search.Click += new System.EventHandler(this.button_Search_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "● 상품 관리";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(29, 32);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(199, 12);
            this.label4.TabIndex = 8;
            this.label4.Text = "(상품을 클릭하면 수정 가능합니다.)";
            // 
            // comboBox_AuthLogin
            // 
            this.comboBox_AuthLogin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_AuthLogin.FormattingEnabled = true;
            this.comboBox_AuthLogin.Location = new System.Drawing.Point(453, 55);
            this.comboBox_AuthLogin.Name = "comboBox_AuthLogin";
            this.comboBox_AuthLogin.Size = new System.Drawing.Size(141, 20);
            this.comboBox_AuthLogin.TabIndex = 9;
            // 
            // Seq
            // 
            this.Seq.FillWeight = 50F;
            this.Seq.HeaderText = "Seq";
            this.Seq.Name = "Seq";
            this.Seq.ReadOnly = true;
            this.Seq.Width = 50;
            // 
            // GoodsName
            // 
            this.GoodsName.HeaderText = "상품명";
            this.GoodsName.Name = "GoodsName";
            this.GoodsName.ReadOnly = true;
            // 
            // OptionName
            // 
            this.OptionName.HeaderText = "옵션명";
            this.OptionName.Name = "OptionName";
            this.OptionName.ReadOnly = true;
            // 
            // Channel
            // 
            this.Channel.HeaderText = "채널";
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            // 
            // GoodsNameNICK
            // 
            this.GoodsNameNICK.HeaderText = "상품명(매칭)";
            this.GoodsNameNICK.Name = "GoodsNameNICK";
            this.GoodsNameNICK.ReadOnly = true;
            // 
            // OptionNameNICK
            // 
            this.OptionNameNICK.HeaderText = "옵션명(매칭)";
            this.OptionNameNICK.Name = "OptionNameNICK";
            this.OptionNameNICK.ReadOnly = true;
            // 
            // Auth
            // 
            this.Auth.HeaderText = "권리사";
            this.Auth.Name = "Auth";
            this.Auth.ReadOnly = true;
            // 
            // CrawlerSeq
            // 
            this.CrawlerSeq.HeaderText = "크롤러시퀀스";
            this.CrawlerSeq.Name = "CrawlerSeq";
            this.CrawlerSeq.ReadOnly = true;
            // 
            // State
            // 
            this.State.HeaderText = "상태";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            // 
            // AuthorityLogin
            // 
            this.AuthorityLogin.HeaderText = "권리사로그인";
            this.AuthorityLogin.Name = "AuthorityLogin";
            this.AuthorityLogin.ReadOnly = true;
            // 
            // GoodsListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.comboBox_AuthLogin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button_Search);
            this.Controls.Add(this.comboBox_Channel);
            this.Controls.Add(this.comboBox_Auth);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox_GoodName);
            this.Controls.Add(this.dataGridView_GoodList);
            this.Name = "GoodsListForm";
            this.Text = "GoodsListForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GoodList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_GoodList;
        private System.Windows.Forms.TextBox textBox_GoodName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_Auth;
        private System.Windows.Forms.ComboBox comboBox_Channel;
        private System.Windows.Forms.Button button_Search;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_AuthLogin;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn GoodsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn GoodsNameNICK;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionNameNICK;
        private System.Windows.Forms.DataGridViewTextBoxColumn Auth;
        private System.Windows.Forms.DataGridViewTextBoxColumn CrawlerSeq;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn AuthorityLogin;
    }
}