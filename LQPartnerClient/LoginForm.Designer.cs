namespace LQPartnerClient
{
    partial class LoginForm
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
            this.button_Login = new System.Windows.Forms.Button();
            this.checkBox_RememberPW = new System.Windows.Forms.CheckBox();
            this.textBox_ID = new System.Windows.Forms.TextBox();
            this.textBox_PW = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // button_Login
            // 
            this.button_Login.Location = new System.Drawing.Point(346, 134);
            this.button_Login.Name = "button_Login";
            this.button_Login.Size = new System.Drawing.Size(69, 49);
            this.button_Login.TabIndex = 0;
            this.button_Login.Text = "로그인";
            this.button_Login.UseVisualStyleBackColor = true;
            this.button_Login.Click += new System.EventHandler(this.button_Login_Click);
            // 
            // checkBox_RememberPW
            // 
            this.checkBox_RememberPW.AutoSize = true;
            this.checkBox_RememberPW.Location = new System.Drawing.Point(207, 201);
            this.checkBox_RememberPW.Name = "checkBox_RememberPW";
            this.checkBox_RememberPW.Size = new System.Drawing.Size(100, 16);
            this.checkBox_RememberPW.TabIndex = 1;
            this.checkBox_RememberPW.Text = "암호 기억하기";
            this.checkBox_RememberPW.UseVisualStyleBackColor = true;
            // 
            // textBox_ID
            // 
            this.textBox_ID.Location = new System.Drawing.Point(146, 134);
            this.textBox_ID.Name = "textBox_ID";
            this.textBox_ID.Size = new System.Drawing.Size(181, 21);
            this.textBox_ID.TabIndex = 2;
            this.textBox_ID.Text = "아이디 입력";
            // 
            // textBox_PW
            // 
            this.textBox_PW.Location = new System.Drawing.Point(146, 162);
            this.textBox_PW.Name = "textBox_PW";
            this.textBox_PW.Size = new System.Drawing.Size(181, 21);
            this.textBox_PW.TabIndex = 3;
            this.textBox_PW.Text = "암호 입력";
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 395);
            this.Controls.Add(this.textBox_PW);
            this.Controls.Add(this.textBox_ID);
            this.Controls.Add(this.checkBox_RememberPW);
            this.Controls.Add(this.button_Login);
            this.Name = "LoginForm";
            this.Text = "LoginForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_Login;
        private System.Windows.Forms.CheckBox checkBox_RememberPW;
        private System.Windows.Forms.TextBox textBox_ID;
        private System.Windows.Forms.TextBox textBox_PW;
    }
}