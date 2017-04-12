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
        private const int addHeightForm = 100;
        private const int addWidthForm = 20;
        private const string broadcast = "255.255.255.255";
        // const

        private FilterInfoCollection videoCaptureDiveses;
        private VideoCaptureDevice finalVideo;
        private Udp_Server udp_Server; //9001
        private Udp_Client RequestsFromNewUser;
        private Udp_Server RequestAboutNewUser;
        private Thread threadGetRequests;        
        private int myChatNumber;
        private List<string> listUsersIp;
        private List<int> usersChatNumbers;
        private bool setSettingVideoOnForm;
        private Point imageSize;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            BeginInitializeParams();
            SettingCamera();
            myChatNumber = 1;
            StartGetRequest();
            SetMyChatNumber();
        }
        private void SettingCamera()
        {
            videoCaptureDiveses = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoCaptureDevice in videoCaptureDiveses)
            {
                comboBox1.Items.Add(videoCaptureDevice.Name);
            }
            comboBox1.SelectedIndex = 0;
        }
        private void BeginInitializeParams()
        {
            udp_Server = new Udp_Server();
            listUsersIp = new List<string>();
            RequestAboutNewUser = new Udp_Server();
            RequestsFromNewUser = new Udp_Client(portRequestNewUser);
            setSettingVideoOnForm = false;
            imageSize = new Point(0, 0);
            usersChatNumbers = new List<int>();
        }
        private void SetRequestAboutNewUser(byte cancelFlag, string ip)
        {
            string ipAddress = GetHostIP();
            byte[] request = new byte[6];
            request[0] = (byte)GetNumberOfIp(ipAddress, 1);
            request[1] = (byte)GetNumberOfIp(ipAddress, 2);
            request[2] = (byte)GetNumberOfIp(ipAddress, 3);
            request[3] = (byte)GetNumberOfIp(ipAddress, 4);
            request[4] = (byte)myChatNumber;
            request[request.Length - 1] = (byte)cancelFlag;
            RequestAboutNewUser.Connect(ip, portRequestNewUser);
            if (RequestAboutNewUser.Connected)
            {
                RequestAboutNewUser.SendTo(request);
            }
        }
        private void btn_Start_Click(object sender, EventArgs e)
        {
            GetTestImageWithCamera();
            SetSettingVideoOnForm();
            StartVideo();       
        }
        private void GetTestImageWithCamera()
        {
            if ((finalVideo == null) || (!finalVideo.IsRunning))
            {
                finalVideo = new VideoCaptureDevice(videoCaptureDiveses[comboBox1.SelectedIndex].MonikerString);
                finalVideo.NewFrame += new NewFrameEventHandler(GetTestImage);
                imageSize.X = 0;
                finalVideo.Start();
                while (imageSize.X == 0) { Thread.Sleep(100); }
                finalVideo.Stop();
            }
        }
        private void GetTestImage(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap image = (Bitmap)eventArgs.Frame.Clone();
            imageSize = new Point(image.Width, image.Height);
        }
        private void StartGetRequest()
        {
            if ((threadGetRequests == null) || (!threadGetRequests.IsAlive))
            {
                threadGetRequests = new Thread(new ParameterizedThreadStart(GetRequest));
                threadGetRequests.Start();
            }
        }
        private void StartVideo()
        {
            if ((finalVideo == null) || (!finalVideo.IsRunning))
            {
                finalVideo = new VideoCaptureDevice(videoCaptureDiveses[comboBox1.SelectedIndex].MonikerString);
                finalVideo.NewFrame += new NewFrameEventHandler(FinalVideo_NewFrame);
                finalVideo.Start();
            }
        }
        private void SetSettingVideoOnForm()
        {
            int widthImage = imageSize.X;
            int heightImage = imageSize.Y;
            this.Height = addHeightForm + heightImage;
            this.Width = addWidthForm * 2 + widthImage;
            pb_Video.Height = heightImage;
            pb_Video.Width = widthImage;
            pb_Video.Location = new Point(addWidthForm / 2, addWidthForm / 2);
            gb_standartButtons.Location = new Point(addWidthForm / 2, pb_Video.Location.Y + pb_Video.Height);
            setSettingVideoOnForm = true;
        }
        private void GetRequest(object sender)
        {
            try
            {
                while (true)
                {
                    byte[] ipBytes = RequestsFromNewUser.ReceiveTo(6);
                    if (ipBytes[ipBytes.Length - 1] == 0)
                    {
                        AddUserIpAndPort(ipBytes);
                    }
                    if (ipBytes[ipBytes.Length - 1] == 1)
                    {
                        SetRequestAboutNewUser(0, ipBytes[0].ToString() + "." + ipBytes[1].ToString() + "." + ipBytes[2].ToString() + "." + ipBytes[3].ToString());
                        AddUserIpAndPort(ipBytes);
                    }
                    UpdateUsers();
                }
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        public void UpdateUsers()
        {
            cb_Users.Items.Clear();
            for (int i = 0; i < listUsersIp.Count; i++)
            {
                cb_Users.Items.Add(listUsersIp[i] + "   " + usersChatNumbers[i].ToString());
            }
        }
        private void AddUserIpAndPort(byte[] ipBytes)
        {
            string ip = ipBytes[0].ToString() + "." + ipBytes[1].ToString() + "." + ipBytes[2].ToString() + "." + ipBytes[3].ToString();
            if (!listUsersIp.Contains(ip))
            {
                listUsersIp.Add(ip);
                usersChatNumbers.Add(ipBytes[4]);
            }
            else
            {
                int indexElement = listUsersIp.IndexOf(ip);
                usersChatNumbers[indexElement] = ipBytes[4];
            }
        }
        private void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap picture = (Bitmap)eventArgs.Frame.Clone();
            byte[] pictureInByte = ImageToByteArray(picture);
            for (int i = 0; i < listUsersIp.Count; i++)
            {
                SendBytesForUdp(udp_Server, pictureInByte, listUsersIp[i]);
            }       
            pb_Video.Image = picture;          
        }
        private void SendBytesForUdp(Udp_Server udp_Server, byte[] data, string ip)
        {
            byte[] sendData = new byte[lengthDgram];
            int pointer = 0;
            udp_Server.Connect(ip, startPortsUsers + myChatNumber);
            if (udp_Server.Connected)
            {
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
                RequestsFromNewUser.Close();
                RequestsFromNewUser = null;
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
        private void btnGetListUsers_Click(object sender, EventArgs e)
        {
            UpdateListUsers();           
        }
        private void UpdateListUsers()
        {
            listUsersIp.Clear();
            usersChatNumbers.Clear();
            SetRequestAboutNewUser(1, broadcast);
        }
        private void SetMyChatNumber()
        {
            UpdateListUsers();
            int myNumber = 1;
            while (usersChatNumbers.Contains(myNumber))
            {
                myNumber++;
            }
            myChatNumber = myNumber;
            for (int i = 0; i < listUsersIp.Count; i++)
            {
                SetRequestAboutNewUser(0, listUsersIp[i]);
            }
        }
    }
}
