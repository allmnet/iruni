namespace Iruniview
{
    partial class Addfirewallrule
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Addfirewallrule));
            this.button1 = new System.Windows.Forms.Button();
            this.ipaddressbox = new System.Windows.Forms.TextBox();
            this.protocolcomboBox = new System.Windows.Forms.ComboBox();
            this.localportbox = new System.Windows.Forms.TextBox();
            this.remoteportbox = new System.Windows.Forms.TextBox();
            this.allowradioButton = new System.Windows.Forms.RadioButton();
            this.blockradioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(12, 123);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Okay";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // ipaddressbox
            // 
            this.ipaddressbox.Location = new System.Drawing.Point(12, 96);
            this.ipaddressbox.Name = "ipaddressbox";
            this.ipaddressbox.Size = new System.Drawing.Size(333, 21);
            this.ipaddressbox.TabIndex = 1;
            this.ipaddressbox.Text = "any";
            this.ipaddressbox.TextChanged += new System.EventHandler(this.ipaddressboxtextchanged);
            // 
            // protocolcomboBox
            // 
            this.protocolcomboBox.FormattingEnabled = true;
            this.protocolcomboBox.Items.AddRange(new object[] {
            "ALL",
            "TCP",
            "UDP"});
            this.protocolcomboBox.Location = new System.Drawing.Point(12, 49);
            this.protocolcomboBox.Name = "protocolcomboBox";
            this.protocolcomboBox.Size = new System.Drawing.Size(70, 20);
            this.protocolcomboBox.TabIndex = 2;
            this.protocolcomboBox.Text = "ALL";
            this.protocolcomboBox.SelectedIndexChanged += new System.EventHandler(this.protocolcomboboxselectindexchanged);
            // 
            // localportbox
            // 
            this.localportbox.Location = new System.Drawing.Point(156, 49);
            this.localportbox.Name = "localportbox";
            this.localportbox.Size = new System.Drawing.Size(50, 21);
            this.localportbox.TabIndex = 3;
            this.localportbox.TextChanged += new System.EventHandler(this.localporttextchanged);
            // 
            // remoteportbox
            // 
            this.remoteportbox.Location = new System.Drawing.Point(292, 49);
            this.remoteportbox.Name = "remoteportbox";
            this.remoteportbox.Size = new System.Drawing.Size(50, 21);
            this.remoteportbox.TabIndex = 3;
            // 
            // allowradioButton
            // 
            this.allowradioButton.AutoSize = true;
            this.allowradioButton.Location = new System.Drawing.Point(12, 27);
            this.allowradioButton.Name = "allowradioButton";
            this.allowradioButton.Size = new System.Drawing.Size(54, 16);
            this.allowradioButton.TabIndex = 4;
            this.allowradioButton.TabStop = true;
            this.allowradioButton.Text = "Allow";
            this.allowradioButton.UseVisualStyleBackColor = true;
            this.allowradioButton.CheckedChanged += new System.EventHandler(this.fwallowradiocheckchanged);
            // 
            // blockradioButton
            // 
            this.blockradioButton.AutoSize = true;
            this.blockradioButton.Location = new System.Drawing.Point(72, 27);
            this.blockradioButton.Name = "blockradioButton";
            this.blockradioButton.Size = new System.Drawing.Size(54, 16);
            this.blockradioButton.TabIndex = 4;
            this.blockradioButton.TabStop = true;
            this.blockradioButton.Text = "Block";
            this.blockradioButton.UseVisualStyleBackColor = true;
            this.blockradioButton.CheckedChanged += new System.EventHandler(this.fwblockradiocheckchanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(212, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Remote Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(88, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "Local Port";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "IP address";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(335, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "You can use comma for multiple input port and ipaddress.";
            // 
            // Addfirewallrule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(358, 158);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.blockradioButton);
            this.Controls.Add(this.allowradioButton);
            this.Controls.Add(this.remoteportbox);
            this.Controls.Add(this.localportbox);
            this.Controls.Add(this.protocolcomboBox);
            this.Controls.Add(this.ipaddressbox);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Addfirewallrule";
            this.Text = "Add Rule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox ipaddressbox;
        private System.Windows.Forms.ComboBox protocolcomboBox;
        private System.Windows.Forms.TextBox localportbox;
        private System.Windows.Forms.TextBox remoteportbox;
        private System.Windows.Forms.RadioButton allowradioButton;
        private System.Windows.Forms.RadioButton blockradioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}