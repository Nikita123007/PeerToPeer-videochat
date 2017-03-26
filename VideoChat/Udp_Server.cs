using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace VideoChat
{
    class Udp_Server
    {
        private Socket socket;
        private IPEndPoint endPoint;

        public Udp_Server(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            //endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            //socket.EnableBroadcast = true;
        }
        public void SendTo(byte[] data)
        {
            try
            {
                socket.SendTo(data, endPoint);
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
        public void Close()
        {
            socket.Close();
        }
    }
}
