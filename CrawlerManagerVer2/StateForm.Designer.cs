namespace CrawlerManagerVer2
{
    partial class StateForm
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
            this.components = new System.ComponentModel.Container();
            this.dataGridView_List = new System.Windows.Forms.DataGridView();
            this.Seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IP_Seq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CrawlerName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.권리사 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Channel = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AuthorityLogin = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.State = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip_List = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItem_Delete = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItem_Edit = new System.Windows.Forms.ToolStripMenuItem();
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Refresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_List)).BeginInit();
            this.contextMenuStrip_List.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView_List
            // 
            this.dataGridView_List.AllowUserToAddRows = false;
            this.dataGridView_List.AllowUserToDeleteRows = false;
            this.dataGridView_List.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView_List.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Seq,
            this.IP_Seq,
            this.CrawlerName,
            this.권리사,
            this.Channel,
            this.AuthorityLogin,
            this.State,
            this.UpdateTime});
            this.dataGridView_List.Location = new System.Drawing.Point(12, 66);
            this.dataGridView_List.Name = "dataGridView_List";
            this.dataGridView_List.ReadOnly = true;
            this.dataGridView_List.RowTemplate.Height = 23;
            this.dataGridView_List.Size = new System.Drawing.Size(660, 484);
            this.dataGridView_List.TabIndex = 0;
            // 
            // Seq
            // 
            this.Seq.FillWeight = 50F;
            this.Seq.HeaderText = "Seq";
            this.Seq.Name = "Seq";
            this.Seq.ReadOnly = true;
            this.Seq.Width = 50;
            // 
            // IP_Seq
            // 
            this.IP_Seq.HeaderText = "고유번호";
            this.IP_Seq.Name = "IP_Seq";
            this.IP_Seq.ReadOnly = true;
            this.IP_Seq.Width = 80;
            // 
            // CrawlerName
            // 
            this.CrawlerName.HeaderText = "크롤러 이름";
            this.CrawlerName.Name = "CrawlerName";
            this.CrawlerName.ReadOnly = true;
            // 
            // 권리사
            // 
            this.권리사.HeaderText = "권리사";
            this.권리사.Name = "권리사";
            this.권리사.ReadOnly = true;
            // 
            // Channel
            // 
            this.Channel.HeaderText = "채널";
            this.Channel.Name = "Channel";
            this.Channel.ReadOnly = true;
            this.Channel.Width = 80;
            // 
            // AuthorityLogin
            // 
            this.AuthorityLogin.HeaderText = "권리사로그인";
            this.AuthorityLogin.Name = "AuthorityLogin";
            this.AuthorityLogin.ReadOnly = true;
            // 
            // State
            // 
            this.State.HeaderText = "상태";
            this.State.Name = "State";
            this.State.ReadOnly = true;
            this.State.Width = 80;
            // 
            // UpdateTime
            // 
            this.UpdateTime.HeaderText = "최근변경시간";
            this.UpdateTime.Name = "UpdateTime";
            this.UpdateTime.ReadOnly = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "● 크롤러 상태 체크";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(279, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "(리스트를 클릭하시면 상세 정보를 볼수 있습니다.)";
            // 
            // contextMenuStrip_List
            // 
            this.contextMenuStrip_List.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItem_Delete,
            this.ToolStripMenuItem_Edit});
            this.contextMenuStrip_List.Name = "contextMenuStrip_List";
            this.contextMenuStrip_List.Size = new System.Drawing.Size(99, 48);
            // 
            // ToolStripMenuItem_Delete
            // 
            this.ToolStripMenuItem_Delete.Name = "ToolStripMenuItem_Delete";
            this.ToolStripMenuItem_Delete.Size = new System.Drawing.Size(98, 22);
            this.ToolStripMenuItem_Delete.Text = "삭제";
            this.ToolStripMenuItem_Delete.Click += new System.EventHandler(this.ToolStripMenuItem_Delete_Click);
            // 
            // ToolStripMenuItem_Edit
            // 
            this.ToolStripMenuItem_Edit.Name = "ToolStripMenuItem_Edit";
            this.ToolStripMenuItem_Edit.Size = new System.Drawing.Size(98, 22);
            this.ToolStripMenuItem_Edit.Text = "수정";
            this.ToolStripMenuItem_Edit.Click += new System.EventHandler(this.ToolStripMenuItem_Edit_Click);
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(580, 29);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(92, 29);
            this.button_Add.TabIndex = 3;
            this.button_Add.Text = "추가";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Refresh
            // 
            this.button_Refresh.Location = new System.Drawing.Point(463, 29);
            this.button_Refresh.Name = "button_Refresh";
            this.button_Refresh.Size = new System.Drawing.Size(92, 29);
            this.button_Refresh.TabIndex = 4;
            this.button_Refresh.Text = "갱신";
            this.button_Refresh.UseVisualStyleBackColor = true;
            this.button_Refresh.Click += new System.EventHandler(this.button_Refresh_Click);
            // 
            // StateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 562);
            this.Controls.Add(this.button_Refresh);
            this.Controls.Add(this.button_Add);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView_List);
            this.Name = "StateForm";
            this.Text = "CrawlerStateForm";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView_List)).EndInit();
            this.contextMenuStrip_List.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView_List;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip_List;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Delete;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItem_Edit;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn IP_Seq;
        private System.Windows.Forms.DataGridViewTextBoxColumn CrawlerName;
        private System.Windows.Forms.DataGridViewTextBoxColumn 권리사;
        private System.Windows.Forms.DataGridViewTextBoxColumn Channel;
        private System.Windows.Forms.DataGridViewTextBoxColumn AuthorityLogin;
        private System.Windows.Forms.DataGridViewTextBoxColumn State;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdateTime;
        private System.Windows.Forms.Button button_Refresh;

    }
}