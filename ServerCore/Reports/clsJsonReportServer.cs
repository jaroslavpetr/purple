using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore.Reports
{
    public class clsJsonReportServer
    {
        public string ServerName { get; set; }
        public List<clsJsonReportGroup> Groups { get; set; } = new List<clsJsonReportGroup>();

    }
}
