using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class clsDealSink : CIMTDealSink
    {
        public delegate void ReportFunction(clsDealInfo SmallDeal);
        ReportFunction ExternalReport;

        //get signal when new Deal is added to server
        public override void OnDealAdd(CIMTDeal ServerDeal)
        {
            var actDeal = new clsDealInfo();
            actDeal.Profit = ServerDeal.Profit();
            actDeal.Volume = ServerDeal.Volume();
            actDeal.Symbol = ServerDeal.Symbol();
            actDeal.UserLogin = ServerDeal.Login();
            actDeal.PositionID = ServerDeal.PositionID();
            actDeal.uOpenTime = ServerDeal.Time();
            this.ExternalReport(actDeal);
        }

        //set new report function to report outside
        public void set_report_function(ReportFunction myFunction)
        {
            this.ExternalReport = new ReportFunction(myFunction);
        }
    }
}
