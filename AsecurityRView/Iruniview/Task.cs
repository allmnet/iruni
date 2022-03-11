using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Diagnostics;

namespace Iruniview
{
    class Task
    {
        public event EventHandler Changed;

        private IMonitor _monitor;
        private Timer _timer = new Timer();
        private BackgroundWorker _worker = new BackgroundWorker();
        private DateTime? _lastRunTime;
        private DateTime _nextRunTime;
        private TaskStatus _status;
        private string _description;

        public Task(IMonitor monitor)
        {
            _monitor = monitor;
            if (!String.IsNullOrEmpty(_monitor.Port))
            {
                _worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Worker_RunWorkerCompleted);
                _worker.DoWork += new DoWorkEventHandler(Worker_DoWork);
                _status = TaskStatus.Sleep;

                _timer.Interval = (int)_monitor.RunFrequency.Value.TotalMilliseconds;
                _timer.Tick += new EventHandler(Timer_Tick);
                _timer.Start();

                SetNextRunTime();
            }
        }

        public TaskStatus Status
        {
            get { return _status; }
        }

        public DateTime? LastRunTime
        {
            get { return _lastRunTime; }
        }

        public DateTime NextRunTime
        {
            get { return _nextRunTime; }
        }

        private void SetNextRunTime()
        {
            _nextRunTime = DateTime.Now + _monitor.RunFrequency.Value;
        }

        public void Run()
        {                       
            _lastRunTime = DateTime.Now;
            _timer.Stop();
            _status = TaskStatus.Checking;
            OnChanged();
            try
            {
                _worker.RunWorkerAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                return;
            }
        }

        public IMonitor Monitor
        {
            get { return _monitor; }
        }

        void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _timer.Start();
            SetNextRunTime();
            SetState();
            SetDescription();
            OnChanged();
        }

        private void SetState()
        {
            _status = _monitor.TaskStatus;
        }

        private void SetDescription()
        {
            _description = _monitor.Description;
        }

        void Timer_Tick(object sender, EventArgs e)
        {
            Run();
        }

        void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            _monitor.Execute();
        }

        private void OnChanged()
        {
            if (Changed != null)
            {
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
