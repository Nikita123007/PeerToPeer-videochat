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

namespace VideoChat
{
    class SendVideo
    {      
        private VideoCaptureDevice finalVideo;
        private List<string> listCurrentUsersIp;
        private int myChatNumber;
        private PictureBox pb_Video;
        private Udp_Sender udp_Sender;
        private string monikerStringVideo;


        public int MyChatNumber
        {
            set
            {
                myChatNumber = value;
            }
        }
        public List<string> ListCurrentUsersIp
        {
            set
            {
                listCurrentUsersIp = value;
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

        public SendVideo(string monikerStringVideo, int myChatNumber, PictureBox pb_Video)
        {
            StartInitialise(monikerStringVideo, myChatNumber, pb_Video);
        }
        public void StartInitialise(string monikerStringVideo, int myChatNumber, PictureBox pb_Video)
        {
            udp_Sender = new Udp_Sender();
            Pb_Video = pb_Video;
            MyChatNumber = myChatNumber;
            ListCurrentUsersIp = new List<string>();
            MonikerStringVideo = monikerStringVideo;
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
            lock (listCurrentUsersIp)
            {
                if (listCurrentUsersIp.Count != 0)
                {
                    /*Bitmap picture = (Bitmap)eventArgs.Frame.Clone();
                    picture = ReSizeBitmap(picture, pb_Video.Width / listCurrentUsersIp.Count / 8, pb_Video.Height);*/
                    Bitmap picture = new Bitmap(eventArgs.Frame, pb_Video.Width / listCurrentUsersIp.Count / 6, pb_Video.Height);
                    byte[] pictureInByte = ImageToByteArray(picture);
                    for (int i = 0; i < listCurrentUsersIp.Count; i++)
                    {
                        SendBytesForUdp(udp_Sender, pictureInByte, listCurrentUsersIp[i]);
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
                        udp_Server.SendTo(sendData);
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
                            udp_Server.SendTo(sendData);
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
        private Bitmap ReSizeBitmap(Bitmap source, int width, int height)
        {
            /*double relationSource = (double)source.Width / (double)source.Height;
            double relationDest = (double)width / (double)height;
            if (relationSource > relationDest)
            {
                return new Bitmap(source, width, (int)(width / relationSource));
            }
            else if(relationSource < relationDest)
            {
                return new Bitmap(source, height, (int)(relationSource * height));
            }
            else
            {
                return new Bitmap(source, width, height);
            }*/
            return new Bitmap(source, width, source.Height * width / source.Width);
        }
        public bool RemoveUser(string ip)
        {
            lock (listCurrentUsersIp)
            {
                if (listCurrentUsersIp.Contains(ip))
                {
                    listCurrentUsersIp.Remove(ip);
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
            lock (listCurrentUsersIp)
            {
                if (!listCurrentUsersIp.Contains(ip))
                {
                    listCurrentUsersIp.Add(ip);
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
