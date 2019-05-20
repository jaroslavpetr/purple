using MetaQuotes.MT5CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class clsDealInfo
    {
        public int myID { get; set; }
        public string Server { get; set; }
        public string Group { get; set; }
        public double Volume { get; set; }
        public double Profit { get; set; }
        public ulong PositionID { get; set; }
        public string Symbol { get; set; }
        public ulong UserLogin { get; set; }
        public double Balance { get; set; }
        public int BuySell { get; set; }
        public long uOpenTime { get; set; }

        public DateTime OpenTime {
            get {
                DateTime ZeroTime = new DateTime(1970, 1, 1);
                return ZeroTime.AddSeconds(this.uOpenTime);
            }
        }
    }
}
