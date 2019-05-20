using ServerCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public static class ConfigHelp
    {
        //find all keys with right name and return list of all values
        private static List<string> SearchKeys(string searchTerm)
        {
            var keys = ConfigurationManager.AppSettings.Keys;
            return keys.Cast<object>()
                       .Where(key => key.ToString().ToLower()
                       .Contains(searchTerm.ToLower()))
                       .Select(key => ConfigurationManager.AppSettings.Get(key.ToString())).ToList();
        }

        //return all connections to servers where we have to do our check
        public static List<clsServerConnectionInfo> getAllConnections()
        {

            var result = new List<clsServerConnectionInfo>();
            var mylist = SearchKeys("tradeserver");
            foreach (var confifConn in mylist)
            {
                try
                {
                    var confparama = confifConn.Split(';');
                    var ServerA = new clsServerConnectionInfo() { Server = confparama[0], Login = confparama[1], Password = confparama[2] };
                    result.Add(ServerA);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }

            return result;
        }


        //return configured startdate for reporting interval
        public static DateTime getStartDate()
        {
            DateTime result = new DateTime(2019, 1, 1);
            try
            {
                string cfgDate = SearchKeys("StartDate")[0];
                result = DateTime.Parse(cfgDate);
            }
            catch { }
            return result;
        }

        //return configured stopdate for reporting interval
        public static DateTime getStopDate()
        {
            DateTime result = new DateTime(2040, 1, 1);
            try
            {
                string cfgDate = SearchKeys("EndDate")[0];
                result = DateTime.Parse(cfgDate);
            }
            catch { }
            return result;
        }

        //return configured Excluded group
        public static string getExcludedGroup()
        {
            try
            {
                return SearchKeys("ExcludedGroup")[0];
            }
            catch { }
            return "";
        }

        //return configured Excluded group
        public static string getExcludedUser()
        {
            try
            {
                return SearchKeys("ExcludedUser")[0];
            }
            catch { }
            return "";
        }
    }
}
