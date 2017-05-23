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
        private const int lengthDgram = 65500;
        private const int startPortsUsers = 9010;
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
            Bitmap picture = (Bitmap)eventArgs.Frame.Clone();
            lock (listCurrentUsersIp)
            {
                if (listCurrentUsersIp.Count != 0)
                {
                    picture = ReSizeBitmap(picture, pb_Video.Width / listCurrentUsersIp.Count, pb_Video.Height);
                    byte[] pictureInByte = ImageToByteArray(picture);
                    for (int i = 0; i < listCurrentUsersIp.Count; i++)
                    {
                        SendBytesForUdp(udp_Sender, pictureInByte, listCurrentUsersIp[i]);
                    }
                }
            }
            //pb_Video.Image = ReSizeBitmap(picture, pb_Video.Width, pb_Video.Height);
        }
        private void SendBytesForUdp(Udp_Sender udp_Server, byte[] data, string ip)
        {
            byte[] sendData = new byte[lengthDgram];
            int pointer = 0;
            udp_Server.Connect(ip, startPortsUsers);
            if (udp_Server.Connected)
            {
                try
                {
                    while (pointer < data.Length)
                    {
                        sendData[lengthDgram - 2] = (byte)myChatNumber;
                        if(pointer == 0)
                            sendData[lengthDgram - 1] = 1;
                        for (int index = 0; index < lengthDgram - 2; index++)
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
            double relationSource = (double)source.Width / (double)source.Height;
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
            }
        }
        public bool RemoveUser(string ip)
        {
            lock (listCurrentUsersIp)
            {
                if (listCurrentUsersIp.Contains(ip))
                {
                    //StopSendVideo();
                    listCurrentUsersIp.Remove(ip);
                    //StartSendVideo();
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
                    //StopSendVideo();
                    listCurrentUsersIp.Add(ip);
                    //StartSendVideo();
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
