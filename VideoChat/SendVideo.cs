using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AForge.Video.DirectShow;
using AForge.Video;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace VideoChat
{
    class SendVideo
    {      
        private VideoCaptureDevice finalVideo;
        private List<InfoSendUser> listUsers;
        private byte[] ip;
        private Udp_Sender udp_Sender;
        private string monikerStringVideo;
        private AutoResetEvent nextEventThread;
        private AutoResetEvent thisEventThread;

        public string MonikerStringVideo
        {
            set
            {
                monikerStringVideo = value;
                finalVideo = new VideoCaptureDevice(value);
                finalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
            }
        }

        public SendVideo(string monikerStringVideo, string ip, PictureBox pb_Video, AutoResetEvent nextEventThread, AutoResetEvent thisEventThread)
        {
            udp_Sender = new Udp_Sender();
            this.ip = GetIpInBytes(ip);
            listUsers = new List<InfoSendUser>();
            MonikerStringVideo = monikerStringVideo;
            this.nextEventThread = nextEventThread;
            this.thisEventThread = thisEventThread;
        }
        public void StartSendVideo()
        {
            if ((finalVideo == null) || (!finalVideo.IsRunning))
            {               
                finalVideo.Start();
            }
        }
        private void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            thisEventThread.WaitOne();
            lock (listUsers)
            {
                nextEventThread.Set();
                if (listUsers.Count != 0)
                {
                    Bitmap picture = new Bitmap(eventArgs.Frame, eventArgs.Frame.Width / Defines.reducingQuality, eventArgs.Frame.Height * (eventArgs.Frame.Width / Defines.reducingQuality) / eventArgs.Frame.Width);
                    byte[] pictureInByte = ImageToByteArray(picture);
                    foreach (InfoSendUser user in listUsers)
                    {
                        SendBytesForUdp(udp_Sender, pictureInByte, user.ip);
                    }
                }
            }
        }
        private void SendBytesForUdp(Udp_Sender udp_Server, byte[] data, string ip)
        {
            byte[] sendData = new byte[Defines.lengthDgram];
            int pointer = 0;
            udp_Server.Connect(ip, Defines.startPortsUsers);
            if (udp_Server.Connected)
            {
                try
                {
                    sendData[Defines.lengthDgram - 5] = this.ip[0];
                    sendData[Defines.lengthDgram - 4] = this.ip[1];
                    sendData[Defines.lengthDgram - 3] = this.ip[2];
                    sendData[Defines.lengthDgram - 2] = this.ip[3];
                    if (data.Length <= (Defines.lengthDgram - 5))
                    {
                        data.CopyTo(sendData, 0);
                        sendData[Defines.lengthDgram - 1] = 1;
                        thisEventThread.WaitOne();
                        udp_Server.SendTo(sendData);
                        nextEventThread.Set();
                    }
                    else
                    {
                        while (pointer < data.Length)
                        {
                            if (pointer == 0)
                                sendData[Defines.lengthDgram - 1] = 1;
                            else
                                sendData[Defines.lengthDgram - 1] = 0;
                            for (int index = 0; index < Defines.lengthDgram - 5; index++)
                            {
                                if (pointer < data.Length)
                                {
                                    sendData[index] = data[pointer];
                                    pointer++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            thisEventThread.WaitOne();
                            udp_Server.SendTo(sendData);
                            nextEventThread.Set();
                        }
                    }
                }
                catch (Exception error)
                {
                    
                }
            }
        }
        public void StopSendVideo()
        {
            if ((finalVideo != null) && (finalVideo.IsRunning))
            {
                finalVideo.Stop();
            }
        }
        public byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        public void RemoveUser(string ip)
        {
            lock (listUsers)
            {
                InfoSendUser user = GetUser(ip);
                if (user != null)
                {
                    listUsers.Remove(user);
                }
            }
        }
        public void AddUser(string ip)
        {
            lock (listUsers)
            {
                InfoSendUser user = GetUser(ip);
                if (user == null)
                {
                    listUsers.Add(user);
                }
            }
        }
        public bool ContainUser(string ip)
        {
            lock (listUsers)
            {
                return listUsers.Contains(GetUser(ip));
            }
        }
        public void ClearListUsers()
        {
            lock (listUsers)
            {
                listUsers.Clear();
            }
        }
        public byte[] GetIpInBytes(string ipAddress)
        {
            string[] numbersOfIp = ipAddress.Split('.');
            return new byte[4] { Convert.ToByte(numbersOfIp[0]), Convert.ToByte(numbersOfIp[1]), Convert.ToByte(numbersOfIp[2]), Convert.ToByte(numbersOfIp[3]) };
        }
        private InfoSendUser GetUser(string ip)
        {
            InfoSendUser user = null;
            foreach (InfoSendUser valueUser in listUsers)
            {
                if (valueUser.ip == ip)
                {
                    user = valueUser;
                }
            }
            return user;
        }
    }
}
