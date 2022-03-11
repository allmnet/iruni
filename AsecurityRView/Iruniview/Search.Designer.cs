namespace Iruniview
{
    partial class Search
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Search));
            this.SearchlistView = new System.Windows.Forms.ListView();
            this.ServerIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Result = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.HostCount = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.HostDone = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.Type1_ComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.Type2 = new System.Windows.Forms.ToolStripLabel();
            this.Type2ComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SearchlistView
            // 
            this.SearchlistView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.SearchlistView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ServerIP,
            this.Type,
            this.Result});
            this.SearchlistView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SearchlistView.FullRowSelect = true;
            this.SearchlistView.GridLines = true;
            this.SearchlistView.Location = new System.Drawing.Point(0, 0);
            this.SearchlistView.Name = "SearchlistView";
            this.SearchlistView.Size = new System.Drawing.Size(1327, 488);
            this.SearchlistView.TabIndex = 0;
            this.SearchlistView.UseCompatibleStateImageBehavior = false;
            this.SearchlistView.View = System.Windows.Forms.View.Details;
            // 
            // ServerIP
            // 
            this.ServerIP.Text = "ServerIP";
            this.ServerIP.Width = 120;
            // 
            // Type
            // 
            this.Type.Text = "Type";
            this.Type.Width = 120;
            // 
            // Result
            // 
            this.Result.Text = "Result";
            this.Result.Width = 1024;
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.statusStrip1);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.SearchlistView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1327, 425);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1327, 488);
            this.toolStripContainer1.TabIndex = 3;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1327, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(100, 16);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.HostCount,
            this.toolStripLabel2,
            this.HostDone,
            this.toolStripSeparator1,
            this.toolStripLabel3,
            this.Type1_ComboBox,
            this.Type2,
            this.Type2ComboBox,
            this.toolStripLabel4,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(662, 41);
            this.toolStrip1.TabIndex = 2;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(75, 38);
            this.toolStripLabel1.Text = "Search Host:";
            // 
            // HostCount
            // 
            this.HostCount.Name = "HostCount";
            this.HostCount.Size = new System.Drawing.Size(14, 38);
            this.HostCount.Text = "0";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(12, 38);
            this.toolStripLabel2.Text = "/";
            // 
            // HostDone
            // 
            this.HostDone.Name = "HostDone";
            this.HostDone.Size = new System.Drawing.Size(36, 38);
            this.HostDone.Text = "Done";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 41);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(72, 38);
            this.toolStripLabel3.Text = "Search Type";
            // 
            // Type1_ComboBox
            // 
            this.Type1_ComboBox.Items.AddRange(new object[] {
            "Process",
            "Registry",
            "File",
            "Network",
            "======",
            "User",
            "Handle",
            "TaskSh",
            "Software",
            "Autorun"});
            this.Type1_ComboBox.Name = "Type1_ComboBox";
            this.Type1_ComboBox.Size = new System.Drawing.Size(121, 41);
            // 
            // Type2
            // 
            this.Type2.Name = "Type2";
            this.Type2.Size = new System.Drawing.Size(67, 38);
            this.Type2.Text = "Detail Type";
            // 
            // Type2ComboBox
            // 
            this.Type2ComboBox.Name = "Type2ComboBox";
            this.Type2ComboBox.Size = new System.Drawing.Size(121, 41);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(23, 38);
            this.toolStripLabel4.Text = "    ";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(99, 38);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // Search
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1327, 488);
            this.Controls.Add(this.toolStripContainer1);
            this.Name = "Search";
            this.Text = "Search";
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ListView SearchlistView;
        private System.Windows.Forms.ColumnHeader ServerIP;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.ColumnHeader Result;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        public System.Windows.Forms.ToolStripLabel HostCount;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        public System.Windows.Forms.ToolStripLabel HostDone;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripComboBox Type1_ComboBox;
        private System.Windows.Forms.ToolStripLabel Type2;
        private System.Windows.Forms.ToolStripComboBox Type2ComboBox;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}