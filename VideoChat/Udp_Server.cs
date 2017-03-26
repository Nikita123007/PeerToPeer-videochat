using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace VideoChat
{
    class Udp_Server
    {
        private Socket socket;
        private IPEndPoint endPoint;
        private const int lengthDgram = 65500;

        public Udp_Server(int port)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            //endPoint = new IPEndPoint(IPAddress.Broadcast, port);
            //socket.EnableBroadcast = true;
        }

        public string SendTo(byte[] data)
        {
            byte[] sendData = new byte[lengthDgram];
            int pointer = 0;
            try
            {
                sendData[lengthDgram - 3] = 1;
                sendData[lengthDgram - 2] = 1;
                sendData[lengthDgram - 1] = 1;
                sendData[0] = (byte)(data.Length / sendData.Length);
                sendData[0] += (data.Length % sendData.Length != 0) ? (byte)1 : (byte)0;
                socket.SendTo(sendData, endPoint);
                for (int i = sendData[0]; i > 0; i--)
                {
                    for (int index = 0; index < lengthDgram; index++)
                    {
                        if (pointer < data.Length)
                        {
                            sendData[index] = data[pointer];
                            pointer++;
                        }
                        else
                        {
                            sendData[index] = 0;
                        }
                    }
                    socket.SendTo(sendData, endPoint);
                }
                return "Файл успешно отправлен.";
            }
            catch (Exception error)
            {
                return error.ToString();
            }
        }

        public void Close()
        {
            socket.Close();
        }
    }
}
