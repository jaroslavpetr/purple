using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class clsServerConnectionInfo
    {
        public string Server { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public ulong uLogin {
            get {
                try
                {
                    ulong.TryParse(this.Login, out ulong j);
                    return j;
                }
                catch
                {
                    return 0;
                }
            }
        }

    }
}
