namespace CrawlerVer2
{
    partial class CrawlerVer2
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
            this.button_Start = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label_ChannelName = new System.Windows.Forms.Label();
            this.comboBox_Function = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_CurrentState = new System.Windows.Forms.Label();
            this.label_BeforeState = new System.Windows.Forms.Label();
            this.label_UID = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_Start
            // 
            this.button_Start.Enabled = false;
            this.button_Start.Location = new System.Drawing.Point(9, 10);
            this.button_Start.Name = "button_Start";
            this.button_Start.Size = new System.Drawing.Size(95, 28);
            this.button_Start.TabIndex = 0;
            this.button_Start.Text = "시작";
            this.button_Start.UseVisualStyleBackColor = true;
            this.button_Start.Click += new System.EventHandler(this.button_Start_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(176, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "채널 :";
            // 
            // label_ChannelName
            // 
            this.label_ChannelName.AutoSize = true;
            this.label_ChannelName.Location = new System.Drawing.Point(218, 18);
            this.label_ChannelName.Name = "label_ChannelName";
            this.label_ChannelName.Size = new System.Drawing.Size(35, 12);
            this.label_ChannelName.TabIndex = 2;
            this.label_ChannelName.Text = "어디?";
            // 
            // comboBox_Function
            // 
            this.comboBox_Function.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Function.FormattingEnabled = true;
            this.comboBox_Function.Location = new System.Drawing.Point(337, 12);
            this.comboBox_Function.Name = "comboBox_Function";
            this.comboBox_Function.Size = new System.Drawing.Size(60, 20);
            this.comboBox_Function.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label2.Location = new System.Drawing.Point(12, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "현재 상태 -";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label3.Location = new System.Drawing.Point(12, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(105, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "이전 동작 정보 -";
            // 
            // label_CurrentState
            // 
            this.label_CurrentState.Location = new System.Drawing.Point(12, 69);
            this.label_CurrentState.Name = "label_CurrentState";
            this.label_CurrentState.Size = new System.Drawing.Size(385, 45);
            this.label_CurrentState.TabIndex = 6;
            this.label_CurrentState.Text = "데이타 파싱( 254/7000 )";
            // 
            // label_BeforeState
            // 
            this.label_BeforeState.Location = new System.Drawing.Point(12, 136);
            this.label_BeforeState.Name = "label_BeforeState";
            this.label_BeforeState.Size = new System.Drawing.Size(375, 46);
            this.label_BeforeState.TabIndex = 7;
            this.label_BeforeState.Text = "-";
            // 
            // label_UID
            // 
            this.label_UID.AutoSize = true;
            this.label_UID.Font = new System.Drawing.Font("굴림", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.label_UID.Location = new System.Drawing.Point(126, 18);
            this.label_UID.Name = "label_UID";
            this.label_UID.Size = new System.Drawing.Size(19, 12);
            this.label_UID.TabIndex = 8;
            this.label_UID.Text = "-1";
            // 
            // CrawlerVer2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 193);
            this.Controls.Add(this.label_UID);
            this.Controls.Add(this.label_BeforeState);
            this.Controls.Add(this.label_CurrentState);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboBox_Function);
            this.Controls.Add(this.label_ChannelName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Start);
            this.Name = "CrawlerVer2";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Start;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label_ChannelName;
        private System.Windows.Forms.ComboBox comboBox_Function;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_CurrentState;
        private System.Windows.Forms.Label label_BeforeState;
        private System.Windows.Forms.Label label_UID;
    }
}

