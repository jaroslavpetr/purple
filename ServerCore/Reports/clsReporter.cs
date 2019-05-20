using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using System.IO;

namespace ServerCore.Reports
{
    public class clsReporter
    {
        //data over all serves
        private List<clsDealInfo> CollectedData = new List<clsDealInfo>();
        private List<clsServerXGroup> AllGroups = new List<clsServerXGroup>();

        public List<clsServerConnectionInfo> AllServers = new List<clsServerConnectionInfo>();

        public DateTime StartDate = new DateTime(2000,1,1);
        public DateTime StopDate = new DateTime(2100, 1, 1);

        public string ExcludedUser = "";
        public string ExcludedGroup = "";

        public string ReportPath = @"..\..\Reports\mt_report" + DateTime.Now.ToString("yyyyMMdd_hhmmss") + ".json";

        //return excluded user as Ulong type
        private ulong uExcludedUser
        {
            get
            {
                try
                {
                    ulong.TryParse(this.ExcludedUser, out ulong j);
                    return j;
                }
                catch
                {
                    return 0;
                }
            }
        }


        //generate one report for all servers in given list
        public bool CreateReport() {
            try
            {
                foreach (var ServerInfo in this.AllServers)
                {
                    CollectDeals4OneServer(ServerInfo);
                }
                CalculateReportStat();

                return true;
            }
            catch
            {
                return false;
            }
            
        }


        //calculate all stats based on collected data and save it as file
        private void CalculateReportStat() {

            var myReport = new clsJsonReportMain() {ReportDate = DateTime.Now};

            foreach (var actServerInfo in this.AllServers)
            {
                var reportActSrv = new clsJsonReportServer();
                reportActSrv.ServerName = actServerInfo.Server;

                var AllGroupAtServer = this.AllGroups.Where(x => x.Server == actServerInfo.Server).Select(x => x.Group).Distinct().ToList();
                foreach (var actGroupName in AllGroupAtServer)
                {
                    var reportGroup = new clsJsonReportGroup() { GroupName = actGroupName };

                    //number of deals
                    reportGroup.NumOfDeals = this.CollectedData.Where(x => x.Server == actServerInfo.Server && x.Group == actGroupName).Count();

                    //Avg Volume
                    var VolumeList = this.CollectedData.Where(x => x.Server == actServerInfo.Server && x.Group == actGroupName).Select(x => x.Volume);
                    if (VolumeList != null && VolumeList.Count() > 0) reportGroup.AvgVolume = VolumeList.Average();

                    //Avg Profit
                    var AvgProfitList = this.CollectedData.Where(x => x.Server == actServerInfo.Server && x.Group == actGroupName && x.BuySell == 1).Select(x => x.Profit);
                    if (AvgProfitList != null && AvgProfitList.Count() > 0) reportGroup.AvgProfit = AvgProfitList.Average();

                    reportActSrv.Groups.Add(reportGroup);
                }

                myReport.Servers.Add(reportActSrv);
            }

            string ReportContent = Newtonsoft.Json.JsonConvert.SerializeObject(myReport);
            File.WriteAllText(this.ReportPath, ReportContent);
        }

        //generate report for given server
        private bool CollectDeals4OneServer(clsServerConnectionInfo ServerInfo)
        {
            using (clsMT5 mt5 = new clsMT5())
            {
                mt5.Initialize();

                try
                {
                    var log_result = mt5.m_manager.Connect(ServerInfo.Server, ServerInfo.uLogin, ServerInfo.Password, "", MetaQuotes.MT5ManagerAPI.CIMTManagerAPI.EnPumpModes.PUMP_MODE_FULL, 10000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return false;
                }
                

                //get all groups at server
                MTRetCode res = MTRetCode.MT_RET_ERROR;
                var GroupList = mt5.GetAllGroups(out res);
                if (!(res == MTRetCode.MT_RET_OK || res == MTRetCode.MT_RET_ERR_NOTFOUND)) return false;

                GroupList.RemoveAll(x => x.Group() == this.ExcludedGroup);

                foreach (var actGroup in GroupList)
                {
                    var newGroup2add = new clsServerXGroup() { Server = ServerInfo.Server, Group = actGroup.Group() };
                    this.AllGroups.Add(newGroup2add);
                }
                
                
                foreach (var actGroup in GroupList)
                {
                    //get all Users at server
                    var userList = mt5.GetAllUsersAtGroup(actGroup.Group(), out res);
                    if (!(res == MTRetCode.MT_RET_OK || res == MTRetCode.MT_RET_ERR_NOTFOUND)) return false;
                    userList.RemoveAll(x => x.Login() == this.uExcludedUser);

                    foreach (var actUser in userList)
                    {
                        //get all deals as server
                        var dealList = mt5.GetAllDeals(actUser.Login(), out res, this.StartDate, this.StopDate);
                        Console.WriteLine($"user {actUser.Login()} trades count={dealList.Count()}");
                        if (!(res == MTRetCode.MT_RET_OK || res == MTRetCode.MT_RET_ERR_NOTFOUND)) return false;

                        foreach (var actDeal in dealList)
                        {
                            var dataNode = new clsDealInfo();
                            dataNode.Server = ServerInfo.Server;
                            dataNode.Group = actGroup.Group();
                            dataNode.Profit = actDeal.Profit();
                            dataNode.Volume = actDeal.Volume();
                            dataNode.BuySell = (int)actDeal.Action();
                            dataNode.uOpenTime = actDeal.Time();
                            this.CollectedData.Add(dataNode);
                        }
                    }
                }
            }

            return true;
        }


    }
}
