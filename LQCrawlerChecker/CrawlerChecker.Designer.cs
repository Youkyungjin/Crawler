namespace LQCrawlerChecker
{
    partial class CrawlerChecker
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
            this.button_StartStop = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_NextCheck = new System.Windows.Forms.Label();
            this.label_FilePath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_StartStop
            // 
            this.button_StartStop.Location = new System.Drawing.Point(12, 55);
            this.button_StartStop.Name = "button_StartStop";
            this.button_StartStop.Size = new System.Drawing.Size(82, 25);
            this.button_StartStop.TabIndex = 0;
            this.button_StartStop.Text = "시작/중단";
            this.button_StartStop.UseVisualStyleBackColor = true;
            this.button_StartStop.Click += new System.EventHandler(this.button_StartStop_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "실행 경로 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "체크 시간 :";
            // 
            // label_NextCheck
            // 
            this.label_NextCheck.AutoSize = true;
            this.label_NextCheck.Location = new System.Drawing.Point(97, 15);
            this.label_NextCheck.Name = "label_NextCheck";
            this.label_NextCheck.Size = new System.Drawing.Size(113, 12);
            this.label_NextCheck.TabIndex = 3;
            this.label_NextCheck.Text = "다음 체크 남은 시간";
            // 
            // label_FilePath
            // 
            this.label_FilePath.AutoSize = true;
            this.label_FilePath.Location = new System.Drawing.Point(97, 36);
            this.label_FilePath.Name = "label_FilePath";
            this.label_FilePath.Size = new System.Drawing.Size(29, 12);
            this.label_FilePath.TabIndex = 4;
            this.label_FilePath.Text = "경로";
            // 
            // CrawlerChecker
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 93);
            this.Controls.Add(this.label_FilePath);
            this.Controls.Add(this.label_NextCheck);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_StartStop);
            this.Name = "CrawlerChecker";
            this.Text = "크롤러 체커";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button_StartStop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_NextCheck;
        private System.Windows.Forms.Label label_FilePath;
    }
}

