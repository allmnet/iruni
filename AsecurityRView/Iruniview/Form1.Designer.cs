namespace Iruniview
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.tasksListView = new System.Windows.Forms.ListView();
            this.Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.ServerIP = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Service = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Version = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Health = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Agent = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Firewall = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TaskState = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.LastCheck = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.NextCheck = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Asecurity = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.taskMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.CheckStatusMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.CheckServerInfoMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startServiceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopServiceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.AsecurityStartMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStartallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStarteventMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStartfileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStartnetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStartperfMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStartprocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopeventMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopfileMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopnetworkMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopperfMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityStopprocMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityUploadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityDownloadMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AsecurityPatchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.AsecurityInstallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.IISInstallMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RemoteDesktopMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RunProcessMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.firewallToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FirewallenableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FirewalldisableMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FirewalladdRuleMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.agentUpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.agentConfigViewMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearItemMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.notifyIconMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.showMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel5 = new System.Windows.Forms.ToolStripLabel();
            this.textBox_ServerState = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel4 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel7 = new System.Windows.Forms.ToolStripLabel();
            this.textBox_Port = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel6 = new System.Windows.Forms.ToolStripLabel();
            this.button_Serverlist = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel11 = new System.Windows.Forms.ToolStripLabel();
            this.button_logview = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel3 = new System.Windows.Forms.ToolStripLabel();
            this.SearchButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripLabel8 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel10 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel9 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.taskMenuStrip.SuspendLayout();
            this.notifyIconMenuStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tasksListView
            // 
            this.tasksListView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tasksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Type,
            this.ServerIP,
            this.Service,
            this.Version,
            this.Health,
            this.Agent,
            this.Firewall,
            this.Description,
            this.TaskState,
            this.LastCheck,
            this.NextCheck,
            this.Asecurity});
            this.tasksListView.ContextMenuStrip = this.taskMenuStrip;
            this.tasksListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tasksListView.Font = new System.Drawing.Font("굴림", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.tasksListView.FullRowSelect = true;
            this.tasksListView.GridLines = true;
            this.tasksListView.Location = new System.Drawing.Point(0, 0);
            this.tasksListView.Name = "tasksListView";
            this.tasksListView.Size = new System.Drawing.Size(1222, 643);
            this.tasksListView.SmallImageList = this.imageList;
            this.tasksListView.TabIndex = 3;
            this.tasksListView.UseCompatibleStateImageBehavior = false;
            this.tasksListView.View = System.Windows.Forms.View.Details;
            // 
            // Type
            // 
            this.Type.Text = "Type";
            this.Type.Width = 120;
            // 
            // ServerIP
            // 
            this.ServerIP.Text = "Server IP";
            this.ServerIP.Width = 100;
            // 
            // Service
            // 
            this.Service.Text = "Service";
            this.Service.Width = 120;
            // 
            // Version
            // 
            this.Version.Text = "Version";
            this.Version.Width = 160;
            // 
            // Health
            // 
            this.Health.Text = "Health";
            // 
            // Agent
            // 
            this.Agent.Text = "Agent";
            // 
            // Firewall
            // 
            this.Firewall.Text = "Firewall";
            // 
            // Description
            // 
            this.Description.Text = "Description";
            this.Description.Width = 250;
            // 
            // TaskState
            // 
            this.TaskState.Text = "Task State";
            this.TaskState.Width = 75;
            // 
            // LastCheck
            // 
            this.LastCheck.Text = "LastCheck";
            this.LastCheck.Width = 100;
            // 
            // NextCheck
            // 
            this.NextCheck.Text = "Next Check";
            this.NextCheck.Width = 100;
            // 
            // Asecurity
            // 
            this.Asecurity.Text = "Asecuritystate";
            this.Asecurity.Width = 0;
            // 
            // taskMenuStrip
            // 
            this.taskMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CheckStatusMenuItem,
            this.toolStripSeparator1,
            this.CheckServerInfoMenuItem,
            this.startServiceMenuItem,
            this.stopServiceMenuItem,
            this.PatchMenuItem,
            this.toolStripSeparator2,
            this.AsecurityStartMenuItem,
            this.AsecurityStopMenuItem,
            this.AsecurityUploadMenuItem,
            this.AsecurityDownloadMenuItem,
            this.AsecurityPatchMenuItem,
            this.toolStripSeparator3,
            this.AsecurityInstallMenuItem,
            this.IISInstallMenuItem,
            this.RemoteDesktopMenuItem,
            this.RunProcessMenuItem,
            this.firewallToolStripMenuItem,
            this.toolStripSeparator4,
            this.agentUpdateMenuItem,
            this.agentConfigViewMenuItem,
            this.clearItemMenuItem});
            this.taskMenuStrip.Name = "taskMenuStrip";
            this.taskMenuStrip.Size = new System.Drawing.Size(233, 424);
            // 
            // CheckStatusMenuItem
            // 
            this.CheckStatusMenuItem.Enabled = false;
            this.CheckStatusMenuItem.Image = global::Iruniview.Properties.Resources.information;
            this.CheckStatusMenuItem.Name = "CheckStatusMenuItem";
            this.CheckStatusMenuItem.Size = new System.Drawing.Size(232, 22);
            this.CheckStatusMenuItem.Text = "Direct check to port of server";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(229, 6);
            // 
            // CheckServerInfoMenuItem
            // 
            this.CheckServerInfoMenuItem.Enabled = false;
            this.CheckServerInfoMenuItem.Image = global::Iruniview.Properties.Resources.undo;
            this.CheckServerInfoMenuItem.Name = "CheckServerInfoMenuItem";
            this.CheckServerInfoMenuItem.Size = new System.Drawing.Size(232, 22);
            this.CheckServerInfoMenuItem.Text = "Reload server state";
            // 
            // startServiceMenuItem
            // 
            this.startServiceMenuItem.Enabled = false;
            this.startServiceMenuItem.Image = global::Iruniview.Properties.Resources.start;
            this.startServiceMenuItem.Name = "startServiceMenuItem";
            this.startServiceMenuItem.Size = new System.Drawing.Size(232, 22);
            this.startServiceMenuItem.Text = "Start service";
            // 
            // stopServiceMenuItem
            // 
            this.stopServiceMenuItem.Enabled = false;
            this.stopServiceMenuItem.Image = global::Iruniview.Properties.Resources.stop;
            this.stopServiceMenuItem.Name = "stopServiceMenuItem";
            this.stopServiceMenuItem.Size = new System.Drawing.Size(232, 22);
            this.stopServiceMenuItem.Text = "Stop service";
            // 
            // PatchMenuItem
            // 
            this.PatchMenuItem.Enabled = false;
            this.PatchMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("PatchMenuItem.Image")));
            this.PatchMenuItem.Name = "PatchMenuItem";
            this.PatchMenuItem.Size = new System.Drawing.Size(232, 22);
            this.PatchMenuItem.Text = "Patch service";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(229, 6);
            // 
            // AsecurityStartMenuItem
            // 
            this.AsecurityStartMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AsecurityStartallMenuItem,
            this.AsecurityStarteventMenuItem,
            this.AsecurityStartfileMenuItem,
            this.AsecurityStartnetworkMenuItem,
            this.AsecurityStartperfMenuItem,
            this.AsecurityStartprocMenuItem});
            this.AsecurityStartMenuItem.Enabled = false;
            this.AsecurityStartMenuItem.Image = global::Iruniview.Properties.Resources.asecuritystart;
            this.AsecurityStartMenuItem.Name = "AsecurityStartMenuItem";
            this.AsecurityStartMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityStartMenuItem.Text = "Start asecurity";
            // 
            // AsecurityStartallMenuItem
            // 
            this.AsecurityStartallMenuItem.Name = "AsecurityStartallMenuItem";
            this.AsecurityStartallMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStartallMenuItem.Text = "All";
            // 
            // AsecurityStarteventMenuItem
            // 
            this.AsecurityStarteventMenuItem.Name = "AsecurityStarteventMenuItem";
            this.AsecurityStarteventMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStarteventMenuItem.Text = "Eventlog";
            // 
            // AsecurityStartfileMenuItem
            // 
            this.AsecurityStartfileMenuItem.Name = "AsecurityStartfileMenuItem";
            this.AsecurityStartfileMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStartfileMenuItem.Text = "File";
            // 
            // AsecurityStartnetworkMenuItem
            // 
            this.AsecurityStartnetworkMenuItem.Name = "AsecurityStartnetworkMenuItem";
            this.AsecurityStartnetworkMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStartnetworkMenuItem.Text = "Network";
            // 
            // AsecurityStartperfMenuItem
            // 
            this.AsecurityStartperfMenuItem.Name = "AsecurityStartperfMenuItem";
            this.AsecurityStartperfMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStartperfMenuItem.Text = "Performance";
            // 
            // AsecurityStartprocMenuItem
            // 
            this.AsecurityStartprocMenuItem.Name = "AsecurityStartprocMenuItem";
            this.AsecurityStartprocMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStartprocMenuItem.Text = "Process";
            // 
            // AsecurityStopMenuItem
            // 
            this.AsecurityStopMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AsecurityStopallMenuItem,
            this.AsecurityStopeventMenuItem,
            this.AsecurityStopfileMenuItem,
            this.AsecurityStopnetworkMenuItem,
            this.AsecurityStopperfMenuItem,
            this.AsecurityStopprocMenuItem});
            this.AsecurityStopMenuItem.Enabled = false;
            this.AsecurityStopMenuItem.Image = global::Iruniview.Properties.Resources.asecuritystop;
            this.AsecurityStopMenuItem.Name = "AsecurityStopMenuItem";
            this.AsecurityStopMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityStopMenuItem.Text = "Stop asecurity";
            // 
            // AsecurityStopallMenuItem
            // 
            this.AsecurityStopallMenuItem.Name = "AsecurityStopallMenuItem";
            this.AsecurityStopallMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopallMenuItem.Text = "All";
            // 
            // AsecurityStopeventMenuItem
            // 
            this.AsecurityStopeventMenuItem.Name = "AsecurityStopeventMenuItem";
            this.AsecurityStopeventMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopeventMenuItem.Text = "Eventlog";
            // 
            // AsecurityStopfileMenuItem
            // 
            this.AsecurityStopfileMenuItem.Name = "AsecurityStopfileMenuItem";
            this.AsecurityStopfileMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopfileMenuItem.Text = "File";
            // 
            // AsecurityStopnetworkMenuItem
            // 
            this.AsecurityStopnetworkMenuItem.Name = "AsecurityStopnetworkMenuItem";
            this.AsecurityStopnetworkMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopnetworkMenuItem.Text = "Network";
            // 
            // AsecurityStopperfMenuItem
            // 
            this.AsecurityStopperfMenuItem.Name = "AsecurityStopperfMenuItem";
            this.AsecurityStopperfMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopperfMenuItem.Text = "Performance";
            // 
            // AsecurityStopprocMenuItem
            // 
            this.AsecurityStopprocMenuItem.Name = "AsecurityStopprocMenuItem";
            this.AsecurityStopprocMenuItem.Size = new System.Drawing.Size(142, 22);
            this.AsecurityStopprocMenuItem.Text = "Process";
            // 
            // AsecurityUploadMenuItem
            // 
            this.AsecurityUploadMenuItem.Enabled = false;
            this.AsecurityUploadMenuItem.Image = global::Iruniview.Properties.Resources.asecurityconfig;
            this.AsecurityUploadMenuItem.Name = "AsecurityUploadMenuItem";
            this.AsecurityUploadMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityUploadMenuItem.Text = "Config upload";
            // 
            // AsecurityDownloadMenuItem
            // 
            this.AsecurityDownloadMenuItem.Enabled = false;
            this.AsecurityDownloadMenuItem.Image = global::Iruniview.Properties.Resources.asecurityview;
            this.AsecurityDownloadMenuItem.Name = "AsecurityDownloadMenuItem";
            this.AsecurityDownloadMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityDownloadMenuItem.Text = "Config view";
            // 
            // AsecurityPatchMenuItem
            // 
            this.AsecurityPatchMenuItem.Enabled = false;
            this.AsecurityPatchMenuItem.Image = global::Iruniview.Properties.Resources.asecuritypatch;
            this.AsecurityPatchMenuItem.Name = "AsecurityPatchMenuItem";
            this.AsecurityPatchMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityPatchMenuItem.Text = "Patch";
            this.AsecurityPatchMenuItem.Visible = false;
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(229, 6);
            // 
            // AsecurityInstallMenuItem
            // 
            this.AsecurityInstallMenuItem.Enabled = false;
            this.AsecurityInstallMenuItem.Image = global::Iruniview.Properties.Resources.asecuritypatch;
            this.AsecurityInstallMenuItem.Name = "AsecurityInstallMenuItem";
            this.AsecurityInstallMenuItem.Size = new System.Drawing.Size(232, 22);
            this.AsecurityInstallMenuItem.Text = "Asecurity Install/Patch";
            // 
            // IISInstallMenuItem
            // 
            this.IISInstallMenuItem.Enabled = false;
            this.IISInstallMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("IISInstallMenuItem.Image")));
            this.IISInstallMenuItem.Name = "IISInstallMenuItem";
            this.IISInstallMenuItem.Size = new System.Drawing.Size(232, 22);
            this.IISInstallMenuItem.Text = "IIS Install";
            // 
            // RemoteDesktopMenuItem
            // 
            this.RemoteDesktopMenuItem.Enabled = false;
            this.RemoteDesktopMenuItem.Image = global::Iruniview.Properties.Resources.google_chrome_remote_desktop_icon;
            this.RemoteDesktopMenuItem.Name = "RemoteDesktopMenuItem";
            this.RemoteDesktopMenuItem.Size = new System.Drawing.Size(232, 22);
            this.RemoteDesktopMenuItem.Text = "RemoteDesktop";
            // 
            // RunProcessMenuItem
            // 
            this.RunProcessMenuItem.Enabled = false;
            this.RunProcessMenuItem.Image = global::Iruniview.Properties.Resources.kill;
            this.RunProcessMenuItem.Name = "RunProcessMenuItem";
            this.RunProcessMenuItem.Size = new System.Drawing.Size(232, 22);
            this.RunProcessMenuItem.Text = "Run CLI Script";
            // 
            // firewallToolStripMenuItem
            // 
            this.firewallToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FirewallenableMenuItem,
            this.FirewalldisableMenuItem,
            this.FirewalladdRuleMenuItem});
            this.firewallToolStripMenuItem.Enabled = false;
            this.firewallToolStripMenuItem.Image = global::Iruniview.Properties.Resources.firewall;
            this.firewallToolStripMenuItem.Name = "firewallToolStripMenuItem";
            this.firewallToolStripMenuItem.Size = new System.Drawing.Size(232, 22);
            this.firewallToolStripMenuItem.Text = "Firewall";
            // 
            // FirewallenableMenuItem
            // 
            this.FirewallenableMenuItem.Name = "FirewallenableMenuItem";
            this.FirewallenableMenuItem.Size = new System.Drawing.Size(120, 22);
            this.FirewallenableMenuItem.Text = "Enable";
            // 
            // FirewalldisableMenuItem
            // 
            this.FirewalldisableMenuItem.Name = "FirewalldisableMenuItem";
            this.FirewalldisableMenuItem.Size = new System.Drawing.Size(120, 22);
            this.FirewalldisableMenuItem.Text = "Disable";
            // 
            // FirewalladdRuleMenuItem
            // 
            this.FirewalladdRuleMenuItem.Name = "FirewalladdRuleMenuItem";
            this.FirewalladdRuleMenuItem.Size = new System.Drawing.Size(120, 22);
            this.FirewalladdRuleMenuItem.Text = "Add rule";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(229, 6);
            // 
            // agentUpdateMenuItem
            // 
            this.agentUpdateMenuItem.Enabled = false;
            this.agentUpdateMenuItem.Image = global::Iruniview.Properties.Resources.iruni;
            this.agentUpdateMenuItem.Name = "agentUpdateMenuItem";
            this.agentUpdateMenuItem.Size = new System.Drawing.Size(232, 22);
            this.agentUpdateMenuItem.Text = "IRUNI agent update";
            // 
            // agentConfigViewMenuItem
            // 
            this.agentConfigViewMenuItem.Enabled = false;
            this.agentConfigViewMenuItem.Image = global::Iruniview.Properties.Resources.iruni;
            this.agentConfigViewMenuItem.Name = "agentConfigViewMenuItem";
            this.agentConfigViewMenuItem.Size = new System.Drawing.Size(232, 22);
            this.agentConfigViewMenuItem.Text = "IRUNI agent config view";
            // 
            // clearItemMenuItem
            // 
            this.clearItemMenuItem.Enabled = false;
            this.clearItemMenuItem.Image = global::Iruniview.Properties.Resources.remove;
            this.clearItemMenuItem.Name = "clearItemMenuItem";
            this.clearItemMenuItem.Size = new System.Drawing.Size(232, 22);
            this.clearItemMenuItem.Text = "IRUNI agent Clear ";
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Black;
            // 
            // notifyIcon
            // 
            this.notifyIcon.ContextMenuStrip = this.notifyIconMenuStrip;
            this.notifyIcon.Text = "notifyIcon1";
            this.notifyIcon.Visible = true;
            // 
            // notifyIconMenuStrip
            // 
            this.notifyIconMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showMenuItem,
            this.exitMenuItem});
            this.notifyIconMenuStrip.Name = "notifyIconMenuStrip";
            this.notifyIconMenuStrip.Size = new System.Drawing.Size(105, 48);
            // 
            // showMenuItem
            // 
            this.showMenuItem.Name = "showMenuItem";
            this.showMenuItem.Size = new System.Drawing.Size(104, 22);
            this.showMenuItem.Text = "Show";
            // 
            // exitMenuItem
            // 
            this.exitMenuItem.Name = "exitMenuItem";
            this.exitMenuItem.Size = new System.Drawing.Size(104, 22);
            this.exitMenuItem.Text = "Exit";
            // 
            // statusStrip1
            // 
            this.statusStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.statusStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripProgressBar,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 0);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1222, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripProgressBar.Size = new System.Drawing.Size(200, 16);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStatusLabel1.Image = global::Iruniview.Properties.Resources.asecurity;
            this.toolStripStatusLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.toolStripStatusLabel1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(1005, 17);
            this.toolStripStatusLabel1.Spring = true;
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.Click_Aboutbutton);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
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
            this.toolStripContainer1.ContentPanel.Controls.Add(this.tasksListView);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1222, 643);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.LeftToolStripPanelVisible = false;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.RightToolStripPanelVisible = false;
            this.toolStripContainer1.Size = new System.Drawing.Size(1222, 706);
            this.toolStripContainer1.TabIndex = 7;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // toolStripContainer1.TopToolStripPanel
            // 
            this.toolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripContainer1.TopToolStripPanel.Controls.Add(this.toolStrip1);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripLabel5,
            this.textBox_ServerState,
            this.toolStripLabel4,
            this.toolStripLabel2,
            this.toolStripLabel7,
            this.textBox_Port,
            this.toolStripLabel6,
            this.button_Serverlist,
            this.toolStripLabel11,
            this.button_logview,
            this.toolStripLabel3,
            this.SearchButton,
            this.toolStripSeparator5,
            this.toolStripLabel8,
            this.toolStripLabel10,
            this.toolStripButton1,
            this.toolStripLabel9,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(3, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.Size = new System.Drawing.Size(1087, 41);
            this.toolStrip1.TabIndex = 0;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(107, 38);
            this.toolStripLabel1.Text = "Agent Server State";
            // 
            // toolStripLabel5
            // 
            this.toolStripLabel5.Name = "toolStripLabel5";
            this.toolStripLabel5.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel5.Text = " ";
            // 
            // textBox_ServerState
            // 
            this.textBox_ServerState.Name = "textBox_ServerState";
            this.textBox_ServerState.ReadOnly = true;
            this.textBox_ServerState.Size = new System.Drawing.Size(100, 41);
            // 
            // toolStripLabel4
            // 
            this.toolStripLabel4.Name = "toolStripLabel4";
            this.toolStripLabel4.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel4.Text = " ";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(29, 38);
            this.toolStripLabel2.Text = "Port";
            // 
            // toolStripLabel7
            // 
            this.toolStripLabel7.Name = "toolStripLabel7";
            this.toolStripLabel7.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel7.Text = " ";
            // 
            // textBox_Port
            // 
            this.textBox_Port.Name = "textBox_Port";
            this.textBox_Port.Size = new System.Drawing.Size(100, 41);
            // 
            // toolStripLabel6
            // 
            this.toolStripLabel6.Name = "toolStripLabel6";
            this.toolStripLabel6.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel6.Text = " ";
            // 
            // button_Serverlist
            // 
            this.button_Serverlist.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_Serverlist.Image = global::Iruniview.Properties.Resources.serverstart;
            this.button_Serverlist.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_Serverlist.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_Serverlist.Name = "button_Serverlist";
            this.button_Serverlist.Size = new System.Drawing.Size(104, 38);
            this.button_Serverlist.Text = "Agent Server Start/Stop";
            this.button_Serverlist.Click += new System.EventHandler(this.Click_Server_button);
            // 
            // toolStripLabel11
            // 
            this.toolStripLabel11.Name = "toolStripLabel11";
            this.toolStripLabel11.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel11.Text = " ";
            // 
            // button_logview
            // 
            this.button_logview.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.button_logview.Image = global::Iruniview.Properties.Resources.log;
            this.button_logview.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.button_logview.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.button_logview.Name = "button_logview";
            this.button_logview.Size = new System.Drawing.Size(104, 38);
            this.button_logview.Text = "LogView";
            this.button_logview.Click += new System.EventHandler(this.Click_Logviewbutton);
            // 
            // toolStripLabel3
            // 
            this.toolStripLabel3.Name = "toolStripLabel3";
            this.toolStripLabel3.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel3.Text = " ";
            // 
            // SearchButton
            // 
            this.SearchButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SearchButton.Image = ((System.Drawing.Image)(resources.GetObject("SearchButton.Image")));
            this.SearchButton.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.SearchButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(99, 38);
            this.SearchButton.Text = "SearchButton";
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 41);
            // 
            // toolStripLabel8
            // 
            this.toolStripLabel8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel8.Image = global::Iruniview.Properties.Resources.siem;
            this.toolStripLabel8.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripLabel8.Name = "toolStripLabel8";
            this.toolStripLabel8.Size = new System.Drawing.Size(127, 38);
            this.toolStripLabel8.Text = "SIEM";
            // 
            // toolStripLabel10
            // 
            this.toolStripLabel10.Name = "toolStripLabel10";
            this.toolStripLabel10.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel10.Text = " ";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::Iruniview.Properties.Resources.rulelist;
            this.toolStripButton1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(108, 38);
            this.toolStripButton1.Text = "Show Rule List";
            this.toolStripButton1.Click += new System.EventHandler(this.Click_Ruleviewbutton);
            // 
            // toolStripLabel9
            // 
            this.toolStripLabel9.Name = "toolStripLabel9";
            this.toolStripLabel9.Size = new System.Drawing.Size(11, 38);
            this.toolStripLabel9.Text = " ";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::Iruniview.Properties.Resources.siemlog;
            this.toolStripButton2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(108, 38);
            this.toolStripButton2.Text = "Show SIEM Log";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1222, 706);
            this.Controls.Add(this.toolStripContainer1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "IRUNI Server Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1Closing);
            this.Resize += new System.EventHandler(this.ImportStatusForm_Resize);
            this.taskMenuStrip.ResumeLayout(false);
            this.notifyIconMenuStrip.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.BottomToolStripPanel.PerformLayout();
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.TopToolStripPanel.PerformLayout();
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView tasksListView;

        private System.Windows.Forms.ColumnHeader ServerIP;
        private System.Windows.Forms.ColumnHeader Description;
        private System.Windows.Forms.ColumnHeader TaskState;
        private System.Windows.Forms.ContextMenuStrip taskMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem CheckStatusMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CheckServerInfoMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem startServiceMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopServiceMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.ContextMenuStrip notifyIconMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem showMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitMenuItem;
        private System.Windows.Forms.ColumnHeader LastCheck;
        private System.Windows.Forms.ColumnHeader NextCheck;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStripMenuItem agentUpdateMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearItemMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ColumnHeader Agent;
        private System.Windows.Forms.ColumnHeader Service;
        private System.Windows.Forms.ColumnHeader Type;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ColumnHeader Health;
        private System.Windows.Forms.ToolStripMenuItem PatchMenuItem;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripTextBox textBox_ServerState;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripTextBox textBox_Port;
        private System.Windows.Forms.ToolStripButton button_Serverlist;
        private System.Windows.Forms.ToolStripLabel toolStripLabel3;
        private System.Windows.Forms.ToolStripLabel toolStripLabel5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel4;
        private System.Windows.Forms.ToolStripButton button_logview;
        private System.Windows.Forms.ToolStripLabel toolStripLabel6;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityUploadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityPatchMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem IISInstallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RunProcessMenuItem;
        private System.Windows.Forms.ColumnHeader Version;
        private System.Windows.Forms.ToolStripLabel toolStripLabel7;
        private System.Windows.Forms.ToolStripMenuItem AsecurityDownloadMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityInstallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem agentConfigViewMenuItem;
        private System.Windows.Forms.ColumnHeader Firewall;
        private System.Windows.Forms.ToolStripMenuItem firewallToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewallenableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewalldisableMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FirewalladdRuleMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStarteventMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartfileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartnetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartperfMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStartprocMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopallMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopeventMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopfileMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopnetworkMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopperfMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AsecurityStopprocMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripLabel toolStripLabel8;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel10;
        private System.Windows.Forms.ToolStripLabel toolStripLabel9;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
        private System.Windows.Forms.ToolStripMenuItem RemoteDesktopMenuItem;
        private System.Windows.Forms.ColumnHeader Asecurity;
        private System.Windows.Forms.ToolStripLabel toolStripLabel11;
        private System.Windows.Forms.ToolStripButton SearchButton;
    }
}

