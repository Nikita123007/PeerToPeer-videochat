﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace VideoChat
{
    class Udp_Receiver
    {
        private Socket socket;
        private IPEndPoint endPoint;
        private string ipAddress;
        
        public int Timeout
        {
            set
            {
                socket.ReceiveTimeout = value;
            }
        }

        public void resetTimeout()
        {
            socket.ReceiveTimeout = 0;
        }
        public Udp_Receiver(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            ipAddress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0].ToString();
            endPoint = new IPEndPoint(IPAddress.Parse(ipAddress), port);
            socket.ReceiveBufferSize = Defines.lengthDgram * 10;
            socket.Bind(endPoint);
        }
        public byte[] ReceiveTo(int lengthDgram)
        {
            byte[] data = new byte[lengthDgram];
            try
            {
                socket.Receive(data);
            }
            catch (Exception)
            {
            }
            return data;
        }
        public int AvailableData()
        {
            return socket.Available;
        }
        public void Close()
        {
            socket.Shutdown(SocketShutdown.Receive);
            socket.Close();
        }
    }
}
