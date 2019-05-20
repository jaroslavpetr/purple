using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Reports
{
    public class clsJsonReportGroup
    {
        public string GroupName { get; set; }
        public int NumOfDeals { get; set; }
        public double AvgVolume { get; set; }
        public double AvgProfit { get; set; }
    }
}
