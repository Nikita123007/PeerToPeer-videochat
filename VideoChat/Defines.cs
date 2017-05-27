using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChat
{
    static class Defines
    {
        public static int lengthDgram = 15000;
        public static int startPortsUsers = 9010;
        public static int portRequestNewUser = 9002;
        public static string broadcast = "255.255.255.255";
        public static int MaxPackagesOnOneImage = 10;
        public static int ReceiveTimeout = 300;
        public static int RequestSize = 26;
        public static int minLenghtName = 5;
        public static int maxLenghtName = 20;
        public static byte reducingQuality = 5;
        public static int startPositionNameInRequest = 6;
        public static string defaultName = "User";
        public static string pathSettingFile = "setting.xml";
        public static byte DefaultChatNumber = 1;
        public static SettingVideoChat defaultSetting = new SettingVideoChat() {Name = Defines.defaultName, Quality = Qualityes.Small};
    }
}
