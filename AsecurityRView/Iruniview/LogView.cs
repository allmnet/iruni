using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Iruniview
{
    delegate void SetTextBoxCallback(string text);
    public partial class LogView : Form
    {
        public LogView()
        {
            InitializeComponent();
        }

        public void PassText(string param)
        {
            if(this.InvokeRequired)
            {
                SetTextBoxCallback d = new SetTextBoxCallback(PassText);
                this.Invoke(d, new object[] { param });
            }
            else
            {
                textBox.Text += param;
            }
            
        }

        private void textbox_TextChanged(object sender, EventArgs e)
        {
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        private void MyForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
