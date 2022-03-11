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
    public partial class RunProcessForm : Form
    {
        public string[] RunProcess { get; set; }

        public RunProcessForm()
        {
            InitializeComponent();
        }

        private void Processname_Text_Changed(object sender, EventArgs e)
        {
            RunProcess = textBox_Processname.Text.Split(',');
        }

        private void RunProcess_EnterKeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r') button_Killprocess_OK.Focus();
        }
    }
}
