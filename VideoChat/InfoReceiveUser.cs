using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace VideoChat
{
    public class InfoReceiveUser
    {
        public string ip = "";
        public string name = "";
        public int chatNumber = 0;
        public bool callUser = false;
        public bool answerUser = false;
        public Point callButton = new Point();
        public Point answerDownButton = new Point();
        public Point answerUpButton = new Point();

        public InfoReceiveUser(string ip, string name, int chatNumber)
        {
            this.ip = ip;
            this.name = name;
            this.chatNumber = chatNumber;
        }
    }
}
