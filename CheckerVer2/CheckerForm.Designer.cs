namespace CheckerVer2
{
    partial class CheckerForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.richTextBox_Info = new System.Windows.Forms.RichTextBox();
            this.richTextBox_Status = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 81);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "상태 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "정보 : ";
            // 
            // richTextBox_Info
            // 
            this.richTextBox_Info.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_Info.Location = new System.Drawing.Point(49, 4);
            this.richTextBox_Info.Name = "richTextBox_Info";
            this.richTextBox_Info.ReadOnly = true;
            this.richTextBox_Info.Size = new System.Drawing.Size(226, 66);
            this.richTextBox_Info.TabIndex = 4;
            this.richTextBox_Info.Text = "";
            // 
            // richTextBox_Status
            // 
            this.richTextBox_Status.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.richTextBox_Status.Location = new System.Drawing.Point(49, 76);
            this.richTextBox_Status.Name = "richTextBox_Status";
            this.richTextBox_Status.ReadOnly = true;
            this.richTextBox_Status.Size = new System.Drawing.Size(226, 83);
            this.richTextBox_Status.TabIndex = 6;
            this.richTextBox_Status.Text = "";
            // 
            // CheckerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 165);
            this.Controls.Add(this.richTextBox_Status);
            this.Controls.Add(this.richTextBox_Info);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "CheckerForm";
            this.Text = "체커";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RichTextBox richTextBox_Info;
        private System.Windows.Forms.RichTextBox richTextBox_Status;
    }
}

