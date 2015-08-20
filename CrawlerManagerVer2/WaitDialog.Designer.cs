namespace CrawlerManagerVer2
{
    partial class WaitDialog
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
            this.progressBar_Running = new System.Windows.Forms.ProgressBar();
            this.label_State = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // progressBar_Running
            // 
            this.progressBar_Running.Cursor = System.Windows.Forms.Cursors.AppStarting;
            this.progressBar_Running.Location = new System.Drawing.Point(16, 25);
            this.progressBar_Running.Name = "progressBar_Running";
            this.progressBar_Running.Size = new System.Drawing.Size(259, 23);
            this.progressBar_Running.TabIndex = 0;
            this.progressBar_Running.Value = 100;
            // 
            // label_State
            // 
            this.label_State.AutoSize = true;
            this.label_State.Location = new System.Drawing.Point(18, 10);
            this.label_State.Name = "label_State";
            this.label_State.Size = new System.Drawing.Size(93, 12);
            this.label_State.TabIndex = 1;
            this.label_State.Text = "작업을 넣으세요";
            // 
            // WaitDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(287, 56);
            this.Controls.Add(this.label_State);
            this.Controls.Add(this.progressBar_Running);
            this.Name = "WaitDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Waiting...";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar progressBar_Running;
        private System.Windows.Forms.Label label_State;
    }
}