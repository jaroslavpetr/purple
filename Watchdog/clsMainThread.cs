using ServerCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace Watchdog
{
    class clsMainThread
    {
        public delegate void StopFunction();
        private List<StopFunction> AllStopFunction = new List<StopFunction>();

        private List<clsDealInfo> LastDeals = new List<clsDealInfo>();
        private int LastDealID = 0;

        //start main process for Watchdog
        public void Start()
        {
            common.StartLogging();
            common.DisplayAndLog("Start Main Program");


            var mainT = new clsMainServerThread();
            var AllConns = ConfigHelp.getAllConnections();

            foreach (var actCon in AllConns)
            {
                var SrvMainThread = new clsMainServerThread();
                SrvMainThread.Connection = actCon;

                StopFunction StopThisWatcher;
                StopThisWatcher = new StopFunction(SrvMainThread.Stop);
                this.AllStopFunction.Add(StopThisWatcher);

                SrvMainThread.set_fullDealcallback_function(NewFullDealReceive);

                var bwthread = new Thread(SrvMainThread.StartWatch);
                bwthread.Start();
            }

            string KeybordCode = "";
            while (KeybordCode == "")
            {
                Thread.Sleep(10000);
                Console.WriteLine("Press andy key to quit");
                KeybordCode = Console.ReadKey().ToString();
            }

            // stop all threads
            foreach (var stopFunction in this.AllStopFunction)
            {
                stopFunction();
            }
            Thread.Sleep(2000);


            common.DisplayAndLog("Program end");
        }


        //Receive new deal info from all servers and compere them 
        public void NewFullDealReceive(clsDealInfo SmallDeal)
        {
            this.LastDealID++;
            SmallDeal.myID = this.LastDealID;
            this.LastDeals.Add(SmallDeal);
            FindCheating(SmallDeal);
        }


        //Try find and log any suspicious activity
        private void FindCheating(clsDealInfo ValidateDeal) {

            Console.WriteLine($"New Deal .. Symbol={ValidateDeal.Symbol} time={ValidateDeal.OpenTime} Volume/Balane={ValidateDeal.Volume / ValidateDeal.Balance}");
            if (this.LastDeals != null & this.LastDeals.Count > 0)
            {
                var similar = this.LastDeals.Where(x =>
                    x.myID != ValidateDeal.myID &&
                    x.Symbol == ValidateDeal.Symbol &&
                    Math.Abs(x.uOpenTime - ValidateDeal.uOpenTime) <= 1 &&
                    Math.Abs(x.Volume / x.Balance - ValidateDeal.Volume / ValidateDeal.Balance) <= 0.05
                    );

                if (similar != null && similar.Count() > 0)
                {
                    common.DisplayAndLog("Find Some similar deals.");
                    foreach (var actDeal in similar)
                    {
                        common.DisplayAndLog($"A) Deal Server= {ValidateDeal.Server} Symbol={ValidateDeal.Symbol} Account={ValidateDeal.UserLogin} PositionID={ValidateDeal.PositionID} OpenTime={ValidateDeal.OpenTime}");
                        common.DisplayAndLog($"B) Deal Server= {actDeal.Server} Symbol={ValidateDeal.Symbol} Account={actDeal.UserLogin} PositionID={actDeal.PositionID} OpenTime={actDeal.OpenTime}");
                    }

                }
            }

            //delete all old items
            DateTime dNow = DateTime.Now.AddHours(1);             //Server has differ dime, so we need to shift it
            this.LastDeals.RemoveAll(x => (dNow - x.OpenTime).TotalSeconds > 10);
        }
    }
}
