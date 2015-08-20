namespace LQPartnerClient
{
    partial class Form1
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
            this.panel_Home = new System.Windows.Forms.Panel();
            this.button_Home = new System.Windows.Forms.Button();
            this.button_Manage_Reservation = new System.Windows.Forms.Button();
            this.button_Calculate = new System.Windows.Forms.Button();
            this.textBox_ResCode = new System.Windows.Forms.TextBox();
            this.button_Search = new System.Windows.Forms.Button();
            this.panel_Home.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel_Home
            // 
            this.panel_Home.Controls.Add(this.button_Search);
            this.panel_Home.Controls.Add(this.textBox_ResCode);
            this.panel_Home.Location = new System.Drawing.Point(12, 62);
            this.panel_Home.Name = "panel_Home";
            this.panel_Home.Size = new System.Drawing.Size(815, 440);
            this.panel_Home.TabIndex = 0;
            // 
            // button_Home
            // 
            this.button_Home.Location = new System.Drawing.Point(12, 12);
            this.button_Home.Name = "button_Home";
            this.button_Home.Size = new System.Drawing.Size(74, 33);
            this.button_Home.TabIndex = 0;
            this.button_Home.Text = "홈버튼";
            this.button_Home.UseVisualStyleBackColor = true;
            this.button_Home.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_Manage_Reservation
            // 
            this.button_Manage_Reservation.Location = new System.Drawing.Point(111, 12);
            this.button_Manage_Reservation.Name = "button_Manage_Reservation";
            this.button_Manage_Reservation.Size = new System.Drawing.Size(74, 33);
            this.button_Manage_Reservation.TabIndex = 0;
            this.button_Manage_Reservation.Text = "예약관리";
            this.button_Manage_Reservation.UseVisualStyleBackColor = true;
            // 
            // button_Calculate
            // 
            this.button_Calculate.Location = new System.Drawing.Point(214, 12);
            this.button_Calculate.Name = "button_Calculate";
            this.button_Calculate.Size = new System.Drawing.Size(74, 33);
            this.button_Calculate.TabIndex = 1;
            this.button_Calculate.Text = "정산";
            this.button_Calculate.UseVisualStyleBackColor = true;
            // 
            // textBox_ResCode
            // 
            this.textBox_ResCode.Location = new System.Drawing.Point(23, 19);
            this.textBox_ResCode.Name = "textBox_ResCode";
            this.textBox_ResCode.Size = new System.Drawing.Size(311, 21);
            this.textBox_ResCode.TabIndex = 0;
            // 
            // button_Search
            // 
            this.button_Search.Location = new System.Drawing.Point(348, 19);
            this.button_Search.Name = "button_Search";
            this.button_Search.Size = new System.Drawing.Size(66, 20);
            this.button_Search.TabIndex = 1;
            this.button_Search.Text = "검색";
            this.button_Search.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 533);
            this.Controls.Add(this.button_Calculate);
            this.Controls.Add(this.button_Manage_Reservation);
            this.Controls.Add(this.button_Home);
            this.Controls.Add(this.panel_Home);
            this.Name = "Form1";
            this.Text = "파트너 프로그램";
            this.panel_Home.ResumeLayout(false);
            this.panel_Home.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel_Home;
        private System.Windows.Forms.Button button_Home;
        private System.Windows.Forms.Button button_Manage_Reservation;
        private System.Windows.Forms.Button button_Calculate;
        private System.Windows.Forms.Button button_Search;
        private System.Windows.Forms.TextBox textBox_ResCode;
    }
}

