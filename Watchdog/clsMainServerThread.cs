using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Watchdog
{
    public class clsMainServerThread
    {
        public clsServerConnectionInfo Connection { get; set; }

        public delegate void StopFunction();

        public delegate double LocalGetBalance(ulong UserLogin);
        LocalGetBalance GetBalanceNow;

        private bool RunMe = true;

        public delegate void ReportFunction(clsDealInfo SmallDeal);
        ReportFunction ExternalNewDeallFullReport;

        StopFunction StopBalanceWatcher;
        StopFunction StopDealWatcher;

        //start main process for Watchdog
        public void StartWatch() {
            common.DisplayAndLog($"Start watching Server = {this.Connection.Server}");

            //Create new treat for Balancewatcher and connect it with delegates
            var BalanceWatcher = new clsBalanceWatcher();
            BalanceWatcher.Connection = this.Connection;
            StopBalanceWatcher = new StopFunction(BalanceWatcher.Stop);
            GetBalanceNow = new LocalGetBalance(BalanceWatcher.GetUserBalance);

            var bwthread = new Thread(BalanceWatcher.StartWatch);
            bwthread.Start();

            //Create new treat for Dealwatcher and connect it with delegates
            var DealWatcher = new clsDealWatcher();
            DealWatcher.Connection = this.Connection;
            DealWatcher.set_report_function(NewDealReceive);
            StopDealWatcher = new StopFunction(DealWatcher.Stop);

            var dwthread = new Thread(DealWatcher.StartWatch);
            dwthread.Start();

            while (this.RunMe)
            {
                Thread.Sleep(1000);
            }

            common.DisplayAndLog($"Quitting watching Server = {this.Connection.Server}");
       }

        //Receive new deal info from server, find balance and tell it back to main thread
        public void NewDealReceive(clsDealInfo SmallDeal)
        {
            
            SmallDeal.Balance = this.GetBalanceNow(SmallDeal.UserLogin);
            this.TellOutsideNewDealFull(SmallDeal);
        }

        //Stops this thread
        public void Stop()
        {
            StopBalanceWatcher();
            StopDealWatcher();
            this.RunMe = false;
        }

        //call outside function to proced new Deal
        public void TellOutsideNewDealFull(clsDealInfo SmallDeal)
        {
            SmallDeal.Server = this.Connection.Server;
            this.ExternalNewDeallFullReport(SmallDeal);
        }

        //set function to report new FullDeal external
        public void set_fullDealcallback_function(ReportFunction myFunction)
        {
            this.ExternalNewDeallFullReport = new ReportFunction(myFunction);
        }
    }

   
}

