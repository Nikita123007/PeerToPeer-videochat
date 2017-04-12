using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace VideoChat
{
    class Udp_Client : IDisposable
    {
        private Socket socket;
        private IPEndPoint endPoint;
        private string ipAddress = "127.0.0.1";

        public Udp_Client(int port)
        {
            CreateNew(port);
        }
        public void CreateNew(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            //Host = Dns.GetHostName();
            //ipAddress = Dns.GetHostByName(Host).AddressList[0].ToString();
            endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            socket.Bind(endPoint);
        }
        public byte[] ReceiveTo(int lengthDgram)
        {
            byte[] data = new byte[lengthDgram];
            try
            {
                socket.Receive(data);
            }
            catch (Exception er)
            {
            }
            return data;
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
