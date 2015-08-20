namespace CrawlerManagerVer2
{
    partial class UpdateGoodsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_GoodsSeq = new System.Windows.Forms.Label();
            this.textBox_GoodNick = new System.Windows.Forms.TextBox();
            this.textBox_OptionNick = new System.Windows.Forms.TextBox();
            this.textBox_Crawler_Seq = new System.Windows.Forms.TextBox();
            this.button_Confirm = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox_GoodName = new System.Windows.Forms.TextBox();
            this.textBox_Option = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "상품 시퀀스 : ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "상품명(매칭): ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 136);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(83, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "옵션명(매칭): ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "크롤러 시퀀스 : ";
            // 
            // label_GoodsSeq
            // 
            this.label_GoodsSeq.AutoSize = true;
            this.label_GoodsSeq.Location = new System.Drawing.Point(102, 24);
            this.label_GoodsSeq.Name = "label_GoodsSeq";
            this.label_GoodsSeq.Size = new System.Drawing.Size(29, 12);
            this.label_GoodsSeq.TabIndex = 4;
            this.label_GoodsSeq.Text = "7745";
            // 
            // textBox_GoodNick
            // 
            this.textBox_GoodNick.Location = new System.Drawing.Point(106, 104);
            this.textBox_GoodNick.Name = "textBox_GoodNick";
            this.textBox_GoodNick.Size = new System.Drawing.Size(370, 21);
            this.textBox_GoodNick.TabIndex = 5;
            // 
            // textBox_OptionNick
            // 
            this.textBox_OptionNick.Location = new System.Drawing.Point(106, 133);
            this.textBox_OptionNick.Name = "textBox_OptionNick";
            this.textBox_OptionNick.Size = new System.Drawing.Size(370, 21);
            this.textBox_OptionNick.TabIndex = 6;
            // 
            // textBox_Crawler_Seq
            // 
            this.textBox_Crawler_Seq.Location = new System.Drawing.Point(106, 162);
            this.textBox_Crawler_Seq.Name = "textBox_Crawler_Seq";
            this.textBox_Crawler_Seq.Size = new System.Drawing.Size(71, 21);
            this.textBox_Crawler_Seq.TabIndex = 7;
            // 
            // button_Confirm
            // 
            this.button_Confirm.Location = new System.Drawing.Point(184, 204);
            this.button_Confirm.Name = "button_Confirm";
            this.button_Confirm.Size = new System.Drawing.Size(118, 29);
            this.button_Confirm.TabIndex = 8;
            this.button_Confirm.Text = "변경";
            this.button_Confirm.UseVisualStyleBackColor = true;
            this.button_Confirm.Click += new System.EventHandler(this.button_Confirm_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(10, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 9;
            this.label5.Text = "상품명 : ";
            // 
            // textBox_GoodName
            // 
            this.textBox_GoodName.Location = new System.Drawing.Point(104, 54);
            this.textBox_GoodName.Name = "textBox_GoodName";
            this.textBox_GoodName.ReadOnly = true;
            this.textBox_GoodName.Size = new System.Drawing.Size(370, 21);
            this.textBox_GoodName.TabIndex = 10;
            // 
            // textBox_Option
            // 
            this.textBox_Option.Location = new System.Drawing.Point(105, 79);
            this.textBox_Option.Name = "textBox_Option";
            this.textBox_Option.ReadOnly = true;
            this.textBox_Option.Size = new System.Drawing.Size(370, 21);
            this.textBox_Option.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 82);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "옵션명: ";
            // 
            // UpdateGoodsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(496, 240);
            this.Controls.Add(this.textBox_Option);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.textBox_GoodName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.button_Confirm);
            this.Controls.Add(this.textBox_Crawler_Seq);
            this.Controls.Add(this.textBox_OptionNick);
            this.Controls.Add(this.textBox_GoodNick);
            this.Controls.Add(this.label_GoodsSeq);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "UpdateGoodsForm";
            this.Text = "상품 변경";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_GoodsSeq;
        private System.Windows.Forms.TextBox textBox_GoodNick;
        private System.Windows.Forms.TextBox textBox_OptionNick;
        private System.Windows.Forms.TextBox textBox_Crawler_Seq;
        private System.Windows.Forms.Button button_Confirm;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox_GoodName;
        private System.Windows.Forms.TextBox textBox_Option;
        private System.Windows.Forms.Label label6;
    }
}