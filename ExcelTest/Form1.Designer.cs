namespace ExcelTest
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_MemoryUsage = new System.Windows.Forms.Label();
            this.label_TestCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label_increase_Memory = new System.Windows.Forms.Label();
            this.label_totalIncrease = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(196, 161);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 42);
            this.button1.TabIndex = 0;
            this.button1.Text = "test memory";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(45, 161);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(97, 42);
            this.button2.TabIndex = 1;
            this.button2.Text = "test column";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(24, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "메모리 사용량 :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "테스트 횟수 :";
            // 
            // label_MemoryUsage
            // 
            this.label_MemoryUsage.AutoSize = true;
            this.label_MemoryUsage.Location = new System.Drawing.Point(151, 27);
            this.label_MemoryUsage.Name = "label_MemoryUsage";
            this.label_MemoryUsage.Size = new System.Drawing.Size(11, 12);
            this.label_MemoryUsage.TabIndex = 4;
            this.label_MemoryUsage.Text = "0";
            // 
            // label_TestCount
            // 
            this.label_TestCount.AutoSize = true;
            this.label_TestCount.Location = new System.Drawing.Point(151, 49);
            this.label_TestCount.Name = "label_TestCount";
            this.label_TestCount.Size = new System.Drawing.Size(11, 12);
            this.label_TestCount.TabIndex = 5;
            this.label_TestCount.Text = "0";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(24, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "늘어난 메모리 :";
            // 
            // label_increase_Memory
            // 
            this.label_increase_Memory.AutoSize = true;
            this.label_increase_Memory.Location = new System.Drawing.Point(151, 72);
            this.label_increase_Memory.Name = "label_increase_Memory";
            this.label_increase_Memory.Size = new System.Drawing.Size(11, 12);
            this.label_increase_Memory.TabIndex = 7;
            this.label_increase_Memory.Text = "0";
            // 
            // label_totalIncrease
            // 
            this.label_totalIncrease.AutoSize = true;
            this.label_totalIncrease.Location = new System.Drawing.Point(261, 72);
            this.label_totalIncrease.Name = "label_totalIncrease";
            this.label_totalIncrease.Size = new System.Drawing.Size(11, 12);
            this.label_totalIncrease.TabIndex = 8;
            this.label_totalIncrease.Text = "0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(344, 247);
            this.Controls.Add(this.label_totalIncrease);
            this.Controls.Add(this.label_increase_Memory);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label_TestCount);
            this.Controls.Add(this.label_MemoryUsage);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_MemoryUsage;
        private System.Windows.Forms.Label label_TestCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_increase_Memory;
        private System.Windows.Forms.Label label_totalIncrease;
    }
}

