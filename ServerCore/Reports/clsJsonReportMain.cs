using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Reports
{
    class clsJsonReportMain
    {
        public DateTime ReportDate { get; set; }
        public List<clsJsonReportServer> Servers { get; set; } = new List<clsJsonReportServer>();
    }
}
