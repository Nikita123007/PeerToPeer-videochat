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
    class Udp_Server : IDisposable
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

        public Udp_Server()
        {
            connected = false;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }
        public void Connect(string ip, int port)
        {
            try
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
            catch (Exception)
            {
                connected = false;
            }
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
        public void Dispose()
        {
            Close();
        }
    }
}
