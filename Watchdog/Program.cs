using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCore;
using NLog;

namespace Watchdog
{
    class Program
    {
        public delegate void ReportFunction(string s);
        static void Main(string[] args)
        {
            var mainT = new clsMainThread();
            mainT.Start();
        }

        
        
    }
}
