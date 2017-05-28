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
    class Udp_Sender
    {
        private Socket socket;
        private IPEndPoint endPoint;
        private bool connected;
        public bool Connected
        {
            get
            {
                return connected;
            }
        }

        public Udp_Sender()
        {
            connected = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public void Connect(string ip, int port)
        {
            if (ip != "255.255.255.255")
            {
                endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket.EnableBroadcast = false;
            }
            else
            {
                endPoint = new IPEndPoint(IPAddress.Broadcast, port);
                socket.EnableBroadcast = true;
            }
            connected = true;
        }
        public void SendTo(byte[] data)
        {
            try
            {
                socket.SendTo(data, endPoint);
            }
            catch (Exception)
            {                
            }
        }
        public void Close()
        {
            socket.Shutdown(SocketShutdown.Send);
            socket.Close();
        }
    }
}
