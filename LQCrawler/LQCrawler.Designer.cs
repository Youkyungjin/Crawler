namespace LQCrawler
{
    partial class LQCrawler
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
            this.TestButton1 = new System.Windows.Forms.Button();
            this.label_MemoryUsage = new System.Windows.Forms.Label();
            this.label_CrawlingTime = new System.Windows.Forms.Label();
            this.label_CurrentState = new System.Windows.Forms.Label();
            this.label_TestValue = new System.Windows.Forms.Label();
            this.button_onoff = new System.Windows.Forms.Button();
            this.groupBox_Status = new System.Windows.Forms.GroupBox();
            this.label_CrawlingState = new System.Windows.Forms.Label();
            this.label_NextCrawling = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label_CrawlingFailedCount = new System.Windows.Forms.Label();
            this.label_CrawlingCount = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_ChannelName = new System.Windows.Forms.Label();
            this.buttonTest2 = new System.Windows.Forms.Button();
            this.groupBox_Status.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TestButton1
            // 
            this.TestButton1.Location = new System.Drawing.Point(18, 365);
            this.TestButton1.Name = "TestButton1";
            this.TestButton1.Size = new System.Drawing.Size(113, 56);
            this.TestButton1.TabIndex = 0;
            this.TestButton1.Text = "TEST 1";
            this.TestButton1.UseVisualStyleBackColor = true;
            this.TestButton1.Click += new System.EventHandler(this.TestButton1_Click);
            // 
            // label_MemoryUsage
            // 
            this.label_MemoryUsage.AutoSize = true;
            this.label_MemoryUsage.Location = new System.Drawing.Point(127, 89);
            this.label_MemoryUsage.Name = "label_MemoryUsage";
            this.label_MemoryUsage.Size = new System.Drawing.Size(11, 12);
            this.label_MemoryUsage.TabIndex = 7;
            this.label_MemoryUsage.Text = "-";
            // 
            // label_CrawlingTime
            // 
            this.label_CrawlingTime.AutoSize = true;
            this.label_CrawlingTime.Location = new System.Drawing.Point(125, 101);
            this.label_CrawlingTime.Name = "label_CrawlingTime";
            this.label_CrawlingTime.Size = new System.Drawing.Size(11, 12);
            this.label_CrawlingTime.TabIndex = 8;
            this.label_CrawlingTime.Text = "-";
            // 
            // label_CurrentState
            // 
            this.label_CurrentState.AutoSize = true;
            this.label_CurrentState.Location = new System.Drawing.Point(84, 17);
            this.label_CurrentState.Name = "label_CurrentState";
            this.label_CurrentState.Size = new System.Drawing.Size(11, 12);
            this.label_CurrentState.TabIndex = 10;
            this.label_CurrentState.Text = "-";
            // 
            // label_TestValue
            // 
            this.label_TestValue.AutoSize = true;
            this.label_TestValue.Location = new System.Drawing.Point(17, 21);
            this.label_TestValue.Name = "label_TestValue";
            this.label_TestValue.Size = new System.Drawing.Size(109, 12);
            this.label_TestValue.TabIndex = 11;
            this.label_TestValue.Text = "최근 디버그 메시지";
            // 
            // button_onoff
            // 
            this.button_onoff.Location = new System.Drawing.Point(21, 14);
            this.button_onoff.Name = "button_onoff";
            this.button_onoff.Size = new System.Drawing.Size(105, 27);
            this.button_onoff.TabIndex = 12;
            this.button_onoff.Text = "시작";
            this.button_onoff.UseVisualStyleBackColor = true;
            this.button_onoff.Click += new System.EventHandler(this.button_onoff_Click);
            // 
            // groupBox_Status
            // 
            this.groupBox_Status.Controls.Add(this.label_CrawlingState);
            this.groupBox_Status.Controls.Add(this.label_NextCrawling);
            this.groupBox_Status.Controls.Add(this.label7);
            this.groupBox_Status.Controls.Add(this.label6);
            this.groupBox_Status.Controls.Add(this.label3);
            this.groupBox_Status.Controls.Add(this.label1);
            this.groupBox_Status.Controls.Add(this.label2);
            this.groupBox_Status.Controls.Add(this.label_MemoryUsage);
            this.groupBox_Status.Controls.Add(this.label_CurrentState);
            this.groupBox_Status.Controls.Add(this.label_CrawlingTime);
            this.groupBox_Status.Location = new System.Drawing.Point(21, 65);
            this.groupBox_Status.Name = "groupBox_Status";
            this.groupBox_Status.Size = new System.Drawing.Size(386, 134);
            this.groupBox_Status.TabIndex = 13;
            this.groupBox_Status.TabStop = false;
            this.groupBox_Status.Text = "Monitor";
            // 
            // label_CrawlingState
            // 
            this.label_CrawlingState.AutoSize = true;
            this.label_CrawlingState.Location = new System.Drawing.Point(99, 52);
            this.label_CrawlingState.Name = "label_CrawlingState";
            this.label_CrawlingState.Size = new System.Drawing.Size(11, 12);
            this.label_CrawlingState.TabIndex = 18;
            this.label_CrawlingState.Text = "-";
            // 
            // label_NextCrawling
            // 
            this.label_NextCrawling.AutoSize = true;
            this.label_NextCrawling.Location = new System.Drawing.Point(179, 34);
            this.label_NextCrawling.Name = "label_NextCrawling";
            this.label_NextCrawling.Size = new System.Drawing.Size(11, 12);
            this.label_NextCrawling.TabIndex = 17;
            this.label_NextCrawling.Text = "-";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 52);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 12);
            this.label7.TabIndex = 16;
            this.label7.Text = "크롤링 상태 :";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(16, 34);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 12);
            this.label6.TabIndex = 15;
            this.label6.Text = "다음 크롤링까지 남은 시간 :";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 12);
            this.label3.TabIndex = 10;
            this.label3.Text = "최근 크롤링 시간 : ";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(69, 12);
            this.label1.TabIndex = 14;
            this.label1.Text = "동작 상태 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "메모리 사용량 : ";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_CrawlingFailedCount);
            this.groupBox1.Controls.Add(this.label_CrawlingCount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label_TestValue);
            this.groupBox1.Location = new System.Drawing.Point(21, 205);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(300, 154);
            this.groupBox1.TabIndex = 15;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Debug";
            // 
            // label_CrawlingFailedCount
            // 
            this.label_CrawlingFailedCount.AutoSize = true;
            this.label_CrawlingFailedCount.Location = new System.Drawing.Point(127, 99);
            this.label_CrawlingFailedCount.Name = "label_CrawlingFailedCount";
            this.label_CrawlingFailedCount.Size = new System.Drawing.Size(11, 12);
            this.label_CrawlingFailedCount.TabIndex = 15;
            this.label_CrawlingFailedCount.Text = "0";
            // 
            // label_CrawlingCount
            // 
            this.label_CrawlingCount.AutoSize = true;
            this.label_CrawlingCount.Location = new System.Drawing.Point(127, 76);
            this.label_CrawlingCount.Name = "label_CrawlingCount";
            this.label_CrawlingCount.Size = new System.Drawing.Size(11, 12);
            this.label_CrawlingCount.TabIndex = 14;
            this.label_CrawlingCount.Text = "0";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 12);
            this.label5.TabIndex = 13;
            this.label5.Text = "실패 횟수";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(16, 76);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 12);
            this.label4.TabIndex = 12;
            this.label4.Text = "크롤링 횟수";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(146, 14);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(37, 12);
            this.label8.TabIndex = 16;
            this.label8.Text = "채널 :";
            // 
            // label_ChannelName
            // 
            this.label_ChannelName.AutoSize = true;
            this.label_ChannelName.Location = new System.Drawing.Point(189, 14);
            this.label_ChannelName.Name = "label_ChannelName";
            this.label_ChannelName.Size = new System.Drawing.Size(11, 12);
            this.label_ChannelName.TabIndex = 17;
            this.label_ChannelName.Text = "-";
            // 
            // buttonTest2
            // 
            this.buttonTest2.Location = new System.Drawing.Point(148, 368);
            this.buttonTest2.Name = "buttonTest2";
            this.buttonTest2.Size = new System.Drawing.Size(98, 52);
            this.buttonTest2.TabIndex = 18;
            this.buttonTest2.Text = "TEST 2";
            this.buttonTest2.UseVisualStyleBackColor = true;
            this.buttonTest2.Click += new System.EventHandler(this.buttonTest2_Click);
            // 
            // LQCrawler
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(413, 362);
            this.Controls.Add(this.buttonTest2);
            this.Controls.Add(this.label_ChannelName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox_Status);
            this.Controls.Add(this.button_onoff);
            this.Controls.Add(this.TestButton1);
            this.Name = "LQCrawler";
            this.Text = "크롤러";
            this.groupBox_Status.ResumeLayout(false);
            this.groupBox_Status.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button TestButton1;
        private System.Windows.Forms.Label label_MemoryUsage;
        private System.Windows.Forms.Label label_CrawlingTime;
        private System.Windows.Forms.Label label_CurrentState;
        private System.Windows.Forms.Label label_TestValue;
        private System.Windows.Forms.Button button_onoff;
        private System.Windows.Forms.GroupBox groupBox_Status;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_CrawlingFailedCount;
        private System.Windows.Forms.Label label_CrawlingCount;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_CrawlingState;
        private System.Windows.Forms.Label label_NextCrawling;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_ChannelName;
        private System.Windows.Forms.Button buttonTest2;
    }
}

