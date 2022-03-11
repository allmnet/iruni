using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Iruniview
{
    interface IMonitor
    {
        List<INotifier> Notifiers { get; }
        TimeSpan? RunFrequency { get; }
        string Service { get; }
        string IPAddress { get; }
        string Agent { get; }
        string Version { get; }
        string Firewall { get; }
        string Port { get; }
        string Type { get; }
        string Health { get; }

        string Asecuritystate { get; }

        MonitorType MonitorType { get; }
        void Execute();
        string Description { get; }
        TaskStatus TaskStatus { get; }
        void Initialize(TimeSpan? runFrequency,
                                   Dictionary<string, string> settings);
        Icon Icon { get; }
    }
}