namespace Iruniview
{
    partial class RunProcessForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunProcessForm));
            this.textBox_Processname = new System.Windows.Forms.TextBox();
            this.button_Killprocess_OK = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBox_Processname
            // 
            this.textBox_Processname.Location = new System.Drawing.Point(12, 78);
            this.textBox_Processname.Name = "textBox_Processname";
            this.textBox_Processname.Size = new System.Drawing.Size(268, 21);
            this.textBox_Processname.TabIndex = 0;
            this.textBox_Processname.TextChanged += new System.EventHandler(this.Processname_Text_Changed);
            this.textBox_Processname.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.RunProcess_EnterKeyPress);
            // 
            // button_Killprocess_OK
            // 
            this.button_Killprocess_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_Killprocess_OK.Location = new System.Drawing.Point(221, 114);
            this.button_Killprocess_OK.Name = "button_Killprocess_OK";
            this.button_Killprocess_OK.Size = new System.Drawing.Size(59, 23);
            this.button_Killprocess_OK.TabIndex = 1;
            this.button_Killprocess_OK.Text = "OK";
            this.button_Killprocess_OK.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(161, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "Input to command porompt.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(258, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "You can use comma, it\'s run multiple script.";
            // 
            // RunProcessForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(293, 149);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button_Killprocess_OK);
            this.Controls.Add(this.textBox_Processname);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RunProcessForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Run Command Script";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_Processname;
        private System.Windows.Forms.Button button_Killprocess_OK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}