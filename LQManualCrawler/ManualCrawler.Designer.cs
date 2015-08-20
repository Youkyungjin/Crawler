namespace LQManualCrawler
{
    partial class ManualCrawler
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
            this.comboBox_SelectChannel = new System.Windows.Forms.ComboBox();
            this.comboBox_ExcelFrom = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_FileName = new System.Windows.Forms.TextBox();
            this.button_FileSelect = new System.Windows.Forms.Button();
            this.button_DBInsert = new System.Windows.Forms.Button();
            this.dataGridView_DataView = new System.Windows.Forms.DataGridView();
            this.button_FileShow = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.comboBox_SelectAuthor = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DataView)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBox_SelectChannel
            // 
            this.comboBox_SelectChannel.FormattingEnabled = true;
            this.comboBox_SelectChannel.Location = new System.Drawing.Point(84, 10);
            this.comboBox_SelectChannel.Name = "comboBox_SelectChannel";
            this.comboBox_SelectChannel.Size = new System.Drawing.Size(220, 20);
            this.comboBox_SelectChannel.TabIndex = 0;
            // 
            // comboBox_ExcelFrom
            // 
            this.comboBox_ExcelFrom.FormattingEnabled = true;
            this.comboBox_ExcelFrom.Location = new System.Drawing.Point(374, 10);
            this.comboBox_ExcelFrom.Name = "comboBox_ExcelFrom";
            this.comboBox_ExcelFrom.Size = new System.Drawing.Size(224, 20);
            this.comboBox_ExcelFrom.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "채널선택";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(321, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "엑셀양식";
            // 
            // textBox_FileName
            // 
            this.textBox_FileName.Location = new System.Drawing.Point(107, 42);
            this.textBox_FileName.Name = "textBox_FileName";
            this.textBox_FileName.Size = new System.Drawing.Size(471, 21);
            this.textBox_FileName.TabIndex = 4;
            // 
            // button_FileSelect
            // 
            this.button_FileSelect.Location = new System.Drawing.Point(12, 37);
            this.button_FileSelect.Name = "button_FileSelect";
            this.button_FileSelect.Size = new System.Drawing.Size(81, 29);
            this.button_FileSelect.TabIndex = 5;
            this.button_FileSelect.Text = "파일선택";
            this.button_FileSelect.UseVisualStyleBackColor = true;
            this.button_FileSelect.Click += new System.EventHandler(this.button_FileSelect_Click);
            // 
            // button_DBInsert
            // 
            this.button_DBInsert.Location = new System.Drawing.Point(815, 42);
            this.button_DBInsert.Name = "button_DBInsert";
            this.button_DBInsert.Size = new System.Drawing.Size(81, 29);
            this.button_DBInsert.TabIndex = 6;
            this.button_DBInsert.Text = "DB Insert";
            this.button_DBInsert.UseVisualStyleBackColor = true;
            this.button_DBInsert.Click += new System.EventHandler(this.button_DBInsert_Click);
            // 
            // dataGridView_DataView
            // 
            this.dataGridView_DataView.AllowUserToAddRows = false;
            this.dataGridView_DataView.AllowUserToDeleteRows = false;
            this.dataGridView_DataView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_DataView.Location = new System.Drawing.Point(23, 72);
            this.dataGridView_DataView.Name = "dataGridView_DataView";
            this.dataGridView_DataView.RowTemplate.Height = 23;
            this.dataGridView_DataView.Size = new System.Drawing.Size(873, 410);
            this.dataGridView_DataView.TabIndex = 7;
            // 
            // button_FileShow
            // 
            this.button_FileShow.Location = new System.Drawing.Point(728, 42);
            this.button_FileShow.Name = "button_FileShow";
            this.button_FileShow.Size = new System.Drawing.Size(81, 29);
            this.button_FileShow.TabIndex = 8;
            this.button_FileShow.Text = "파일확인";
            this.button_FileShow.UseVisualStyleBackColor = true;
            this.button_FileShow.Click += new System.EventHandler(this.button_FileShow_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(617, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "권리사선택";
            // 
            // comboBox_SelectAuthor
            // 
            this.comboBox_SelectAuthor.FormattingEnabled = true;
            this.comboBox_SelectAuthor.Location = new System.Drawing.Point(698, 10);
            this.comboBox_SelectAuthor.Name = "comboBox_SelectAuthor";
            this.comboBox_SelectAuthor.Size = new System.Drawing.Size(209, 20);
            this.comboBox_SelectAuthor.TabIndex = 9;
            // 
            // ManualCrawler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(932, 516);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox_SelectAuthor);
            this.Controls.Add(this.button_FileShow);
            this.Controls.Add(this.dataGridView_DataView);
            this.Controls.Add(this.button_DBInsert);
            this.Controls.Add(this.button_FileSelect);
            this.Controls.Add(this.textBox_FileName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox_ExcelFrom);
            this.Controls.Add(this.comboBox_SelectChannel);
            this.Name = "ManualCrawler";
            this.Text = "수동 크롤러";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_DataView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_SelectChannel;
        private System.Windows.Forms.ComboBox comboBox_ExcelFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_FileName;
        private System.Windows.Forms.Button button_FileSelect;
        private System.Windows.Forms.Button button_DBInsert;
        private System.Windows.Forms.DataGridView dataGridView_DataView;
        private System.Windows.Forms.Button button_FileShow;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboBox_SelectAuthor;
    }
}

