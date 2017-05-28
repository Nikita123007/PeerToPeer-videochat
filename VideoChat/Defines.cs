using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoChat
{
    static class Defines
    {
        public static int lengthDgram = 27000;
        public static int startPortsUsers = 9010;
        public static int portGetRequests = 9002;
        public static string broadcast = "255.255.255.255";
        public static int MaxPackagesOnOneImage = 10;
        public static int ReceiveTimeout = 300;
        public static int RequestSize = 25;
        public static int minLenghtName = 5;
        public static int maxLenghtName = 20;
        public static byte reducingQuality = 5;
        public static int startPositionNameInRequest = 5;
        public static string defaultName = "User";
        public static string pathSettingFile = "setting.xml";
        public static string pathCallImage = "call.bmp";
        public static string pathAnswerImage = "answer.bmp";
        public static int emSize = 20;
        public static double reletionSizeInCallImage = 18 / 22.5;
        public static double reletionSizeHeightInAnswerImage = 13 / 14.3;
        public static double reletionSizeWidthDownInAnswerImage = 10.4 / 25.4;
        public static double reletionSizeWidthUpInAnswerImage = 15.8 / 25.4;
        public static string familyName = "Segoe Marker";
        public static SettingVideoChat defaultSetting = new SettingVideoChat() {Name = Defines.defaultName, Quality = Qualityes.Small};
    }
}
