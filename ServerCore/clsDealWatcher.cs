using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MetaQuotes.MT5CommonAPI;

namespace ServerCore
{
    public class clsDealWatcher
    {
        public delegate void ReportFunction(clsDealInfo SmallDeal);
        ReportFunction ExternalReport;
        private bool RunMe = true;

        public clsServerConnectionInfo Connection { get; set; }

        //connect to server and start monitoring new deals
        public void StartWatch() {

            using (clsMT5 mt5 = new clsMT5())
            {
                mt5.Initialize();

                var log_result = mt5.m_manager.Connect(this.Connection.Server, this.Connection.uLogin, this.Connection.Password, "", MetaQuotes.MT5ManagerAPI.CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, 10000);
                if (log_result != MTRetCode.MT_RET_OK) return;

                clsDealSink myDealSink = new clsDealSink();
                myDealSink.set_report_function(this.TellOutside);
                mt5.SubscribeDealSink(ref myDealSink);
                while (this.RunMe)
                {
                    Thread.Sleep(1000);
                }

                Console.WriteLine("Quting watching");
            }

        }

        //Stops this thread
        public void Stop()
        {
            this.RunMe = false;
            Console.WriteLine("STOP balance");
        }

        //call outside function to proced new Deal
        public void TellOutside(clsDealInfo SmallDeal) {
            this.ExternalReport(SmallDeal);
        }
        //set new report function to report outside
        public void set_report_function(ReportFunction myFunction) {
            this.ExternalReport = new ReportFunction(myFunction);
        }
    }
}
