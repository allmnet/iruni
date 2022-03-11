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
    public partial class Addfirewallrule : Form
    {
        public string addfwlocalport { get; set; }
        public string addfwremoteport { get; set; }
        public string addfwprotocol { get; set; }
        public string addfwstat { get; set; }
        public string addfwaddress { get; set; }
        public Addfirewallrule()
        {
            InitializeComponent();
        }

        private void fwallowradiocheckchanged(object sender, EventArgs e)
        {
            if(allowradioButton.Checked == true)
            {
                blockradioButton.Checked = false;
                addfwstat = "allow";
            }
            else
            {
                blockradioButton.Checked = true;
                addfwstat = "block";
            }
        }

        private void fwblockradiocheckchanged(object sender, EventArgs e)
        {
            if (blockradioButton.Checked == true)
            {
                allowradioButton.Checked = false;
                addfwstat = "block";
            }
            else
            {
                allowradioButton.Checked = true;
                addfwstat = "allow";
            }
        }

        private void localporttextchanged(object sender, EventArgs e)
        {
            addfwlocalport = localportbox.Text.Trim();
        }

        private void remoteportboxtextchanged(object sender, EventArgs e)
        {
            addfwremoteport = remoteportbox.Text.Trim();
        }

        private void protocolcomboboxselectindexchanged(object sender, EventArgs e)
        {
            addfwprotocol = protocolcomboBox.SelectedItem.ToString().Trim();
        }

        private void ipaddressboxtextchanged(object sender, EventArgs e)
        {
            addfwaddress = ipaddressbox.Text.Trim();
        }
    }
}
