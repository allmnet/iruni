using System;
using System.Collections.Generic;
using System.Text;

namespace Iruniview
{
    interface INotifier
    {
        void Initialize(Dictionary<string, string> settings);
        void Execute(string title, string message);
    }
}