namespace LQCrawlerManager
{
    partial class LQCrawlerManger
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다.
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_Test1 = new System.Windows.Forms.Button();
            this.button_Test2 = new System.Windows.Forms.Button();
            this.dataGridView_Crawler = new System.Windows.Forms.DataGridView();
            this.listBox_Log = new System.Windows.Forms.ListBox();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.comboBox_Partner = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Crawler)).BeginInit();
            this.SuspendLayout();
            // 
            // button_Test1
            // 
            this.button_Test1.Location = new System.Drawing.Point(88, 409);
            this.button_Test1.Name = "button_Test1";
            this.button_Test1.Size = new System.Drawing.Size(118, 39);
            this.button_Test1.TabIndex = 0;
            this.button_Test1.Text = "테스트1";
            this.button_Test1.UseVisualStyleBackColor = true;
            this.button_Test1.Click += new System.EventHandler(this.button_Test1_Click);
            // 
            // button_Test2
            // 
            this.button_Test2.Location = new System.Drawing.Point(242, 409);
            this.button_Test2.Name = "button_Test2";
            this.button_Test2.Size = new System.Drawing.Size(118, 39);
            this.button_Test2.TabIndex = 1;
            this.button_Test2.Text = " 테스트2";
            this.button_Test2.UseVisualStyleBackColor = true;
            this.button_Test2.Click += new System.EventHandler(this.button_Test2_Click);
            // 
            // dataGridView_Crawler
            // 
            this.dataGridView_Crawler.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_Crawler.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1,
            this.Column9,
            this.Column7,
            this.Column8,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column6,
            this.Column5});
            this.dataGridView_Crawler.Location = new System.Drawing.Point(12, 47);
            this.dataGridView_Crawler.Name = "dataGridView_Crawler";
            this.dataGridView_Crawler.RowTemplate.Height = 23;
            this.dataGridView_Crawler.Size = new System.Drawing.Size(744, 332);
            this.dataGridView_Crawler.TabIndex = 2;
            // 
            // listBox_Log
            // 
            this.listBox_Log.FormattingEnabled = true;
            this.listBox_Log.ItemHeight = 12;
            this.listBox_Log.Location = new System.Drawing.Point(776, 12);
            this.listBox_Log.Name = "listBox_Log";
            this.listBox_Log.Size = new System.Drawing.Size(171, 364);
            this.listBox_Log.TabIndex = 3;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "인덱스";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 80;
            // 
            // Column9
            // 
            this.Column9.HeaderText = "파트너 인덱스";
            this.Column9.Name = "Column9";
            // 
            // Column7
            // 
            this.Column7.HeaderText = "파트너 이름";
            this.Column7.Name = "Column7";
            // 
            // Column8
            // 
            this.Column8.HeaderText = "채널 인덱스";
            this.Column8.Name = "Column8";
            // 
            // Column2
            // 
            this.Column2.HeaderText = "채널 이름";
            this.Column2.MinimumWidth = 150;
            this.Column2.Name = "Column2";
            this.Column2.ReadOnly = true;
            this.Column2.Width = 150;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "아이피";
            this.Column3.MinimumWidth = 100;
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            // 
            // Column4
            // 
            this.Column4.FillWeight = 120F;
            this.Column4.HeaderText = "크롤러 연결";
            this.Column4.MinimumWidth = 120;
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            this.Column4.Width = 120;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "체커 연결";
            this.Column6.MinimumWidth = 100;
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "ReStart";
            this.Column5.Name = "Column5";
            // 
            // comboBox_Partner
            // 
            this.comboBox_Partner.FormattingEnabled = true;
            this.comboBox_Partner.Location = new System.Drawing.Point(88, 7);
            this.comboBox_Partner.Name = "comboBox_Partner";
            this.comboBox_Partner.Size = new System.Drawing.Size(148, 20);
            this.comboBox_Partner.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "파트너";
            // 
            // LQCrawlerManger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(964, 394);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_Partner);
            this.Controls.Add(this.listBox_Log);
            this.Controls.Add(this.dataGridView_Crawler);
            this.Controls.Add(this.button_Test2);
            this.Controls.Add(this.button_Test1);
            this.Name = "LQCrawlerManger";
            this.Text = "크롤러 매니저";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_Crawler)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Test1;
        private System.Windows.Forms.Button button_Test2;
        private System.Windows.Forms.DataGridView dataGridView_Crawler;
        private System.Windows.Forms.ListBox listBox_Log;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column7;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column8;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.ComboBox comboBox_Partner;
        private System.Windows.Forms.Label label1;
    }
}

