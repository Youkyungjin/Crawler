namespace CrawlerManagerVer2
{
    partial class StateDetailForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.button_ToList = new System.Windows.Forms.Button();
            this.dataGridView_GoodsList = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GoodsName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OptionName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.richTextBox_Info = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GoodsList)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "● 크롤러 정보";
            // 
            // button_ToList
            // 
            this.button_ToList.Location = new System.Drawing.Point(288, 484);
            this.button_ToList.Name = "button_ToList";
            this.button_ToList.Size = new System.Drawing.Size(108, 32);
            this.button_ToList.TabIndex = 1;
            this.button_ToList.Text = "목록으로";
            this.button_ToList.UseVisualStyleBackColor = true;
            this.button_ToList.Click += new System.EventHandler(this.button_ToList_Click);
            // 
            // dataGridView_GoodsList
            // 
            this.dataGridView_GoodsList.AllowUserToAddRows = false;
            this.dataGridView_GoodsList.AllowUserToDeleteRows = false;
            this.dataGridView_GoodsList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_GoodsList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.No,
            this.GoodsName,
            this.OptionName,
            this.Channel});
            this.dataGridView_GoodsList.Location = new System.Drawing.Point(12, 307);
            this.dataGridView_GoodsList.Name = "dataGridView_GoodsList";
            this.dataGridView_GoodsList.ReadOnly = true;
            this.dataGridView_GoodsList.RowTemplate.Height = 23;
            this.dataGridView_GoodsList.Size = new System.Drawing.Size(660, 171);
            this.dataGridView_GoodsList.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 282);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(141, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "● 크롤러 상품 분배 현황";
            // 
            // No
            // 
            this.No.HeaderText = "번호";
            this.No.Name = "No";
            this.No.ReadOnly = true;
            // 
            // GoodsName
            // 
            this.GoodsName.FillWeight = 200F;
            this.GoodsName.HeaderText = "상품명";
            this.GoodsName.Name = "GoodsName";
            this.GoodsName.ReadOnly = true;
            this.GoodsName.Width = 200;
            // 
            // OptionName
            // 
            this.OptionName.FillWeight = 200F;
            this.OptionName.HeaderText = "옵션명";
            this.OptionName.Name = "OptionName";
            this.OptionName.ReadOnly = true;
            this.OptionName.Width = 200;
            // 
            // Channel
            // 
            this.Channel.HeaderText = "채널";
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            // 
            // richTextBox_Info
            // 
            this.richTextBox_Info.Location = new System.Drawing.Point(17, 42);
            this.richTextBox_Info.Name = "richTextBox_Info";
            this.richTextBox_Info.ReadOnly = true;
            this.richTextBox_Info.Size = new System.Drawing.Size(655, 207);
            this.richTextBox_Info.TabIndex = 5;
            this.richTextBox_Info.Text = "";
            // 
            // StateDetailForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.richTextBox_Info);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView_GoodsList);
            this.Controls.Add(this.button_ToList);
            this.Controls.Add(this.label1);
            this.Name = "StateDetailForm";
            this.Text = "StateDetailForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_GoodsList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_ToList;
        private System.Windows.Forms.DataGridView dataGridView_GoodsList;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn No;
        private System.Windows.Forms.DataGridViewTextBoxColumn GoodsName;
        private System.Windows.Forms.DataGridViewTextBoxColumn OptionName;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.RichTextBox richTextBox_Info;
    }
}