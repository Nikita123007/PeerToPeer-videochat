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
        private List<string> lisеUsersIp;
        private int myChatNumber;
        private PictureBox pb_Video;
        private Udp_Sender udp_Sender;
        private string monikerStringVideo;
        private AutoResetEvent nextEventThread;
        private AutoResetEvent thisEventThread;


        public int MyChatNumber
        {
            set
            {
                myChatNumber = value;
            }
        }
        public string MonikerStringVideo
        {
            set
            {
                monikerStringVideo = value;
                finalVideo = new VideoCaptureDevice(value);
                finalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
            }
        }
        public PictureBox Pb_Video
        {
            set
            {
                pb_Video = value;
            }
        }

        public SendVideo(string monikerStringVideo, int myChatNumber, PictureBox pb_Video, AutoResetEvent nextEventThread, AutoResetEvent thisEventThread)
        {
            udp_Sender = new Udp_Sender();
            Pb_Video = pb_Video;
            MyChatNumber = myChatNumber;
            lisеUsersIp = new List<string>();
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
            lock (lisеUsersIp)
            {
                nextEventThread.Set();
                if (lisеUsersIp.Count != 0)
                {
                    Bitmap picture = new Bitmap(eventArgs.Frame, pb_Video.Width / Defines.reducingQuality, eventArgs.Frame.Height * (pb_Video.Width / Defines.reducingQuality) / eventArgs.Frame.Width);
                    byte[] pictureInByte = ImageToByteArray(picture);
                    for (int i = 0; i < lisеUsersIp.Count; i++)
                    {
                        SendBytesForUdp(udp_Sender, pictureInByte, lisеUsersIp[i]);
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
                    sendData[Defines.lengthDgram - 2] = (byte)myChatNumber;
                    if (data.Length <= (Defines.lengthDgram - 2))
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
                            for (int index = 0; index < Defines.lengthDgram - 2; index++)
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
        public bool RemoveUser(string ip)
        {
            lock (lisеUsersIp)
            {
                if (lisеUsersIp.Contains(ip))
                {
                    lisеUsersIp.Remove(ip);
                    ClearVideo();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool AddUser(string ip)
        {
            lock (lisеUsersIp)
            {
                if (!lisеUsersIp.Contains(ip))
                {
                    lisеUsersIp.Add(ip);
                    ClearVideo();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool ContainUser(string ip)
        {
            lock (lisеUsersIp)
            {
                return lisеUsersIp.Contains(ip);
            }
        }
        public void ClearListUsers()
        {
            lock (lisеUsersIp)
            {
                lisеUsersIp.Clear();
            }
        }
        private void ClearVideo()
        {
            Graphics g = pb_Video.CreateGraphics();
            g.Clear(Color.White);
        }
    }
}
