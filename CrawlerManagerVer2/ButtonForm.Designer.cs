namespace CrawlerManagerVer2
{
    partial class ButtonForm
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
            this.button_C_State = new System.Windows.Forms.Button();
            this.button_LogOut = new System.Windows.Forms.Button();
            this.button_Account = new System.Windows.Forms.Button();
            this.button_Good = new System.Windows.Forms.Button();
            this.label_Name = new System.Windows.Forms.Label();
            this.label_Mobile = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_C_State
            // 
            this.button_C_State.Location = new System.Drawing.Point(7, 115);
            this.button_C_State.Name = "button_C_State";
            this.button_C_State.Size = new System.Drawing.Size(160, 32);
            this.button_C_State.TabIndex = 0;
            this.button_C_State.Text = "크롤러 상태체크";
            this.button_C_State.UseVisualStyleBackColor = true;
            this.button_C_State.Click += new System.EventHandler(this.button_C_State_Click);
            // 
            // button_LogOut
            // 
            this.button_LogOut.Location = new System.Drawing.Point(93, 68);
            this.button_LogOut.Name = "button_LogOut";
            this.button_LogOut.Size = new System.Drawing.Size(74, 32);
            this.button_LogOut.TabIndex = 1;
            this.button_LogOut.Text = "로그 아웃";
            this.button_LogOut.UseVisualStyleBackColor = true;
            this.button_LogOut.Click += new System.EventHandler(this.button_LogOut_Click);
            // 
            // button_Account
            // 
            this.button_Account.Location = new System.Drawing.Point(7, 68);
            this.button_Account.Name = "button_Account";
            this.button_Account.Size = new System.Drawing.Size(80, 32);
            this.button_Account.TabIndex = 2;
            this.button_Account.Text = "계정 설정";
            this.button_Account.UseVisualStyleBackColor = true;
            this.button_Account.Click += new System.EventHandler(this.button_Account_Click);
            // 
            // button_Good
            // 
            this.button_Good.Location = new System.Drawing.Point(7, 153);
            this.button_Good.Name = "button_Good";
            this.button_Good.Size = new System.Drawing.Size(160, 32);
            this.button_Good.TabIndex = 3;
            this.button_Good.Text = "상품 관리";
            this.button_Good.UseVisualStyleBackColor = true;
            this.button_Good.Click += new System.EventHandler(this.button_Good_Click);
            // 
            // label_Name
            // 
            this.label_Name.AutoSize = true;
            this.label_Name.Location = new System.Drawing.Point(13, 14);
            this.label_Name.Name = "label_Name";
            this.label_Name.Size = new System.Drawing.Size(29, 12);
            this.label_Name.TabIndex = 4;
            this.label_Name.Text = "이름";
            // 
            // label_Mobile
            // 
            this.label_Mobile.AutoSize = true;
            this.label_Mobile.Location = new System.Drawing.Point(13, 41);
            this.label_Mobile.Name = "label_Mobile";
            this.label_Mobile.Size = new System.Drawing.Size(57, 12);
            this.label_Mobile.TabIndex = 5;
            this.label_Mobile.Text = "전화 번호";
            // 
            // ButtonForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(174, 562);
            this.Controls.Add(this.label_Mobile);
            this.Controls.Add(this.label_Name);
            this.Controls.Add(this.button_Good);
            this.Controls.Add(this.button_Account);
            this.Controls.Add(this.button_LogOut);
            this.Controls.Add(this.button_C_State);
            this.Name = "ButtonForm";
            this.Text = "ButtonForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_C_State;
        private System.Windows.Forms.Button button_LogOut;
        private System.Windows.Forms.Button button_Account;
        private System.Windows.Forms.Button button_Good;
        private System.Windows.Forms.Label label_Name;
        private System.Windows.Forms.Label label_Mobile;
    }
}