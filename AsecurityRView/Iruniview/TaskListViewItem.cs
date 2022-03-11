using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Iruniview
{

    class TaskListViewItem : ListViewItem
    {

        private IMonitor _monitor;
        private Task _task;

        public TaskListViewItem(Task task)
            : base(new string[] { "", "", "", "", "", "", "", "", "", "", "", "" })
        {
            _task = task;
            _monitor = task.Monitor;
            _task.Changed += delegate
            {
                UpdateSubItems();
            };
            UpdateSubItems();
        }

        public IMonitor Monitor
        {
            get { return _monitor; }
        }

        public Task Task
        {
            get { return _task; }
        }

        private void UpdateSubItems()
        {
            if (SubItems[1].Text == "")
            {
                SubItems[0].Text = _monitor.Type;
                SubItems[1].Text = _monitor.IPAddress;
                SubItems[2].Text = _monitor.Service;
                SubItems[3].Text = _monitor.Version;
                SubItems[4].Text = _monitor.Health;
                SubItems[5].Text = _monitor.Agent;
                SubItems[6].Text = _monitor.Firewall;
                SubItems[11].Text = _monitor.Asecuritystate;
            }
            else
            {
                SubItems[7].Text = _monitor.Description;
                SubItems[8].Text = _task.Status.ToString();
            }
            SubItems[9].Text = _task.LastRunTime.HasValue ? _task.LastRunTime.Value.ToShortTimeString() : "(none)";
            SubItems[10].Text = _task.NextRunTime.ToShortTimeString();
            
        }
    }
}
