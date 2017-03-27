using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using AForge.Video;
using System.IO;
using System.Threading;
using System.Net;

namespace VideoChat
{
    public partial class Form1 : Form
    {
        // const
        private const int lengthDgram = 65500;
        private const int startPortsUsers = 9010;
        private const int portRequestNewUser = 9002;
        // const

        private FilterInfoCollection videoCaptureDiveses;
        private VideoCaptureDevice finalVideo;
        private Udp_Server udp_Server;
        private Udp_Client getRequestsOfNewUser;
        private Udp_Server setRequestOfNewUser;
        private Thread threadGetRequests;        
        private int userChatId;
        private List<string> listUsersIp;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            videoCaptureDiveses = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoCaptureDevice in videoCaptureDiveses)
            {
                comboBox1.Items.Add(videoCaptureDevice.Name);
            }
            comboBox1.SelectedIndex = 0;
            udp_Server = new Udp_Server(9001);
            userChatId = GetUserChatId();
            listUsersIp = new List<string>();
        }
        private void SetRequest(byte cancelFlag)
        {
            setRequestOfNewUser = new Udp_Server(portRequestNewUser);
            string ipAddress = GetHostIP();
            byte[] ip = new byte[5];
            ip[0] = (byte)GetNumberOfIp(ipAddress, 1);
            ip[1] = (byte)GetNumberOfIp(ipAddress, 2);
            ip[2] = (byte)GetNumberOfIp(ipAddress, 3);
            ip[3] = (byte)GetNumberOfIp(ipAddress, 4);
            ip[4] = (byte)cancelFlag;
            setRequestOfNewUser.SendTo(ip);
            setRequestOfNewUser.Close();
            setRequestOfNewUser = null;
        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            if ((finalVideo == null) || (!finalVideo.IsRunning))
            {
                finalVideo = new VideoCaptureDevice(videoCaptureDiveses[comboBox1.SelectedIndex].MonikerString);
                finalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
                finalVideo.Start();
            }
            if ((threadGetRequests == null) || (!threadGetRequests.IsAlive))
            {
                threadGetRequests = new Thread(new ParameterizedThreadStart(GetRequest));
                threadGetRequests.Start();
            }
        }
        private void GetRequest(object sender)
        {
            try
            {
                using (getRequestsOfNewUser = new Udp_Client(portRequestNewUser))
                {
                    while (true)
                    {
                        byte[] ipBytes = getRequestsOfNewUser.ReceiveTo(5);
                        if (ipBytes[4] == 1)
                        {
                            SetRequest(0);
                            AddUserIp(ipBytes[0].ToString() + "." + ipBytes[1].ToString() + "." + ipBytes[2].ToString() + "." + ipBytes[3].ToString());
                        }
                    }
                }
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void AddUserIp(string ip)
        {
            if (!listUsersIp.Contains(ip))
            {
                listUsersIp.Add(ip);
            }
        }
        private void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap picture = (Bitmap)eventArgs.Frame.Clone();
            byte[] pictureInByte = ImageToByteArray(picture);
            SendBytesForUdp(udp_Server, pictureInByte);       
            pictureBox1.Image = picture;          
        }
        private void SendBytesForUdp(Udp_Server udp_Server, byte[] data)
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
                udp_Server.SendTo(sendData);
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
                    udp_Server.SendTo(sendData);
                }
            }
            catch (Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void btn_Stop_Click(object sender, EventArgs e)
        {
            StopExecuteThreads();
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopExecuteThreads();
        }
        private void StopExecuteThreads()
        {
            if ((finalVideo != null) && (finalVideo.IsRunning))
            {
                finalVideo.Stop();
            }
            if ((threadGetRequests != null) && (threadGetRequests.IsAlive))
            {
                getRequestsOfNewUser.Close();
                getRequestsOfNewUser = null;
                threadGetRequests.Abort();                
            }
        }
        private int GetUserChatId()
        {
            string ipAddress = GetHostIP();
            int chatId = GetNumberOfIp(ipAddress, 4);
            chatId += startPortsUsers;
            return chatId;
        }
        private int GetNumberOfIp(string ipAddress, int number)
        {
            string[] numbersOfIp = new string[4] {"", "", "", ""};
            for (int i = 0, j = 0; i < ipAddress.Length; i++)
            {
                if (ipAddress[i] != '.')
                {
                    numbersOfIp[j] += ipAddress[i];
                }
                else
                {
                    j++;
                }
            }
            return Convert.ToInt32(numbersOfIp[number - 1]);
        }
        private string GetHostIP()
        {
            string Host = Dns.GetHostName();
            return Dns.GetHostByName(Host).AddressList[0].ToString();
        }
    }
}
