using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChat
{
    static class Defines
    {
        public static int lengthDgram = 15000;//65500;
        public static int startPortsUsers = 9010;
        public static int portRequestNewUser = 9002;
        public static string broadcast = "255.255.255.255";
        public static int MaxPackagesOnOneImage = 5;
        public static int ReceiveTimeout = 300;
    }
}
