using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChat
{
    public class InfoSendUser
    {
        public string ip = "";
        public string name = "";

        public InfoSendUser(string ip, string name)
        {
            this.ip = ip;
            this.name = name;
        }
    }
}
