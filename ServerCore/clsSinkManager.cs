using MetaQuotes.MT5ManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class clsSinkManager : CIMTManagerSink
    {
        public override void OnConnect()
        {
            Console.WriteLine($"*********************");
        }
    }
}
