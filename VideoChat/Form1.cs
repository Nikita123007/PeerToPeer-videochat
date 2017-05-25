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
        private SendVideo sendVideo;
        private FilterInfoCollection videoCaptureDiveses;              
        private Udp_Receiver RequestsFromNewUser;
        private Udp_Sender RequestAboutNewUser;
        private Thread threadGetRequests;        
        private int myChatNumber;
        private List<string> listUsersIp;
        private List<int> listUsersChatNumbers;
        private Point imageSize;
        private ReceiveVideo receiveVideo;
        private bool flagGetRequests;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SettingCamera();
            BeginInitializeParams();
            StartGetRequest();
            SetMyChatNumber();
            StartReceiveVideo();
            StartSendVideo();
        }
        private void StartReceiveVideo()
        {           
            receiveVideo.StartReceiveVideo();
        }
        private void StartSendVideo()
        {
            sendVideo.StartSendVideo();
        }
        private void SettingCamera()
        {
            videoCaptureDiveses = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo videoCaptureDevice in videoCaptureDiveses)
            {
                tS_CB_Cameras.Items.Add(videoCaptureDevice.Name);
            }
            tS_CB_Cameras.SelectedIndex = 0;
        }
        private void BeginInitializeParams()
        {
            myChatNumber = 1;
            flagGetRequests = false;
            listUsersIp = new List<string>();
            threadGetRequests = new Thread(GetRequest);
            RequestAboutNewUser = new Udp_Sender();
            RequestsFromNewUser = new Udp_Receiver(Defines.portRequestNewUser);
            imageSize = new Point(0, 0);
            listUsersChatNumbers = new List<int>();
            sendVideo = new SendVideo(videoCaptureDiveses[tS_CB_Cameras.SelectedIndex].MonikerString, myChatNumber, pb_Video);
            receiveVideo = new ReceiveVideo(pb_Video);
        }
        private void SetRequestAboutNewUser(FlagsRequest cancelFlag, string ip)
        {
            string ipAddress = GetHostIP();
            byte[] request = new byte[6];
            request[0] = (byte)GetNumberOfIp(ipAddress, 1);
            request[1] = (byte)GetNumberOfIp(ipAddress, 2);
            request[2] = (byte)GetNumberOfIp(ipAddress, 3);
            request[3] = (byte)GetNumberOfIp(ipAddress, 4);
            request[4] = (byte)myChatNumber;
            request[request.Length - 1] = (byte)cancelFlag;
            RequestAboutNewUser.Connect(ip, Defines.portRequestNewUser);
            if (RequestAboutNewUser.Connected)
            {
                RequestAboutNewUser.SendTo(request);
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
                threadGetRequests.Start();
            }
        }
        private void GetRequest()
        {
            try
            {
                while (true)
                {
                    if (RequestsFromNewUser.AvailableData() >= 6)
                    {
                        byte[] ipBytes = RequestsFromNewUser.ReceiveTo(6);
                        flagGetRequests = true;
                        string ip = ipBytes[0].ToString() + "." + ipBytes[1].ToString() + "." + ipBytes[2].ToString() + "." + ipBytes[3].ToString();
                        int chatNumber = ipBytes[4];
                        FlagsRequest request = (FlagsRequest)ipBytes[5];
                        if ((chatNumber == 0) || (ip == GetHostIP()))
                            continue;
                        if (request == FlagsRequest.FSetInfo)
                        {
                            AddUserIpAndChatNumber(ip, chatNumber);
                        }
                        if (request == FlagsRequest.FGetInfo)
                        {
                            SetRequestAboutNewUser((byte)FlagsRequest.FSetInfo, ip);
                            AddUserIpAndChatNumber(ip, chatNumber);
                        }
                        if (request == FlagsRequest.FTrySetUser)
                        {
                            if (MessageBox.Show("New user with chat number " + chatNumber + " and user ip " + ip + " want add. Add his?", "new user", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                SetRequestAboutNewUser(FlagsRequest.FAddUser, ip);
                                AddUserIpAndChatNumber(ip, chatNumber);
                                AddNewUserInGroup(ip, chatNumber);
                            }
                        }
                        if (request == FlagsRequest.FAddUser)
                        {
                            AddNewUserInGroup(ip, chatNumber);
                        }
                        if (request == FlagsRequest.FRemoveUser)
                        {
                            RemoveUserWithGroup(ip, chatNumber);
                        }
                    }
                    else
                    {
                        flagGetRequests = false;
                    }
                }
            }
            catch(Exception error)
            {
                MessageBox.Show(error.Message);
            }
        }
        private void RemoveUserWithGroup(string ip, int chatNumber)
        {
            sendVideo.RemoveUser(ip);
            receiveVideo.RemoveUser(chatNumber);
        }
        private void AbortGroup()
        {
            for (int i = 0; i < listUsersIp.Count; i++)
            {
                sendVideo.RemoveUser(listUsersIp[i]);
                receiveVideo.RemoveUser(listUsersChatNumbers[i]);
            }
        }
        private void AddNewUserInGroup(string ip, int chatNumber)
        {
            sendVideo.AddUser(ip);
            receiveVideo.AddUser(chatNumber);
        }
        public void UpdateUsers()
        {
            lock (cb_Users)
            {
                cb_Users.Items.Clear();
                for (int i = 0; i < listUsersIp.Count; i++)
                {
                    cb_Users.Items.Add(listUsersIp[i] + "   " + listUsersChatNumbers[i].ToString());
                }
            }
        }
        private void AddUserIpAndChatNumber(string ip, int chatNumber)
        {
            if (!listUsersIp.Contains(ip))
            {
                listUsersIp.Add(ip);
                listUsersChatNumbers.Add(chatNumber);
            }
            else
            {
                int indexElement = listUsersIp.IndexOf(ip);
                listUsersChatNumbers[indexElement] = chatNumber;
            }
        }     
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopExecuteThreads();
        }
        private void StopExecuteThreads()
        {
            sendVideo.StopSendVideo();
            receiveVideo.StopReceiveVideo();
            threadGetRequests.Abort();
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
            listUsersChatNumbers.Clear();
            SetRequestAboutNewUser(FlagsRequest.FGetInfo, Defines.broadcast);
        }
        private void SetMyChatNumber()
        {
            UpdateListUsers();
            Thread.Sleep(1000);
            while (flagGetRequests);
            int myNumber = 1;
            while (listUsersChatNumbers.Contains(myNumber))
            {
                myNumber++;
            }
            myChatNumber = myNumber % 256;
            for (int i = 0; i < listUsersIp.Count; i++)
            {
                SetRequestAboutNewUser((byte)FlagsRequest.FSetInfo, listUsersIp[i]);
            }
            sendVideo.MyChatNumber = myNumber;
        }
        private void btn_Call_Click(object sender, EventArgs e)
        {
            string callUserIp = "";
            lock (cb_Users)
            {
                if ((cb_Users.Items != null) && (cb_Users.Items.Count != 0) && (cb_Users.SelectedIndex != -1))
                    callUserIp = listUsersIp[cb_Users.SelectedIndex];
            }
            if (callUserIp != "")
            {
                SetRequestAboutNewUser(FlagsRequest.FTrySetUser, callUserIp);
            }
        }
        private void btn_Abort_Call_Group_Click(object sender, EventArgs e)
        {
            for(int i = 0; i < listUsersIp.Count; i++)
            {
                SetRequestAboutNewUser(FlagsRequest.FRemoveUser, listUsersIp[i]);
            }
            AbortGroup();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SetMyChatNumber();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            UpdateUsers();
        }
    }
}
