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
using InputBox;
using System.Text.RegularExpressions;

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
        private List<string> listUsersName;
        private Point imageSize;
        private ReceiveVideo receiveVideo;
        private bool flagGetRequests;
        private AutoResetEvent eventThreadGetRequests;
        private AutoResetEvent eventThreadSendVideo;
        private AutoResetEvent eventThreadReceiveVideo;
        private string myUserName;

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SettingCamera();
            BeginInitializeParams();
            StartGetRequest();
            StartReceiveVideo();
            StartSendVideo();
            eventThreadGetRequests.Set();
            SetMyChatNumber();
            updateListUsersTimer.Start();
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
            tS_CB_Cameras.Items.Clear();
            foreach (FilterInfo videoCaptureDevice in videoCaptureDiveses)
            {
                tS_CB_Cameras.Items.Add(videoCaptureDevice.Name);
            }
            if (tS_CB_Cameras.Items.Count != 0)
            {
                tS_CB_Cameras.SelectedIndex = 0;
            }
        }
        private void BeginInitializeParams()
        {
            myChatNumber = 1;
            flagGetRequests = false;
            //myUserName = GetUserName();
            //SetUserNameOnForm();
            eventThreadGetRequests = new AutoResetEvent(false);
            eventThreadSendVideo = new AutoResetEvent(false);
            eventThreadReceiveVideo = new AutoResetEvent(false);
            listUsersIp = new List<string>();
            threadGetRequests = new Thread(GetRequest);
            RequestAboutNewUser = new Udp_Sender();
            RequestsFromNewUser = new Udp_Receiver(Defines.portRequestNewUser);
            imageSize = new Point(0, 0);
            listUsersChatNumbers = new List<int>();
            listUsersName = new List<string>();
            if (tS_CB_Cameras.SelectedIndex != -1)
            {
                sendVideo = new SendVideo(videoCaptureDiveses[tS_CB_Cameras.SelectedIndex].MonikerString, myChatNumber, pb_Video, eventThreadReceiveVideo, eventThreadSendVideo);
            }
            else
            {
                sendVideo = new SendVideo("default", myChatNumber, pb_Video, eventThreadReceiveVideo, eventThreadSendVideo);
            }
            receiveVideo = new ReceiveVideo(pb_Video, eventThreadGetRequests, eventThreadReceiveVideo);          
        }
        private void SetUserNameOnForm()
        {
            lblUserName.Text = myUserName;
        }
        private string GetUserName()
        {
            string name = "";
            InputBox.InputBox inputBox = new InputBox.InputBox();
            inputBox.TextMessage = "Input name(minimum " + Defines.minLenghtName.ToString() + " characters, A-Z, a-z, 0-9):";
            inputBox.TextMaxLenght = Defines.maxLenghtName;
            name = inputBox.getString();
            while (!CorrectName(name))
            {
                MessageBox.Show("You have entered the correct name. The name must contain at least " + Defines.minLenghtName.ToString() + " characters including numbers and letters of the English language.", "Not a valid input.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                name = inputBox.getString();
            }
            return name;
        }
        private bool CorrectName(string name)
        {
            bool result = true;
            Regex reg = new Regex(@"^[a-zA-Z_0-9]{1,20}$");
            if (name != null)
            {
                if (name == "") result = false;
                if ((name.Length > 20) || (name.Length < Defines.minLenghtName)) result = false;
                if (!reg.IsMatch(name)) result = false;
            }
            else
            {
                result = false;
            }
            return result;           
        }
        private void SetRequestAboutNewUser(FlagsRequest numberRequest, string ip)
        {
            string ipAddress = GetHostIP();
            byte[] request = new byte[Defines.RequestSize];
            request[0] = (byte)GetNumberOfIp(ipAddress, 1);
            request[1] = (byte)GetNumberOfIp(ipAddress, 2);
            request[2] = (byte)GetNumberOfIp(ipAddress, 3);
            request[3] = (byte)GetNumberOfIp(ipAddress, 4);
            request[4] = (byte)myChatNumber;
            request[5] = (byte)numberRequest;
            /*byte[] nameInBytes = TranslateNameInBytes(myUserName, Defines.maxLenghtName);
            for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
            {
                request[i] = nameInBytes[j];
            }*/
            RequestAboutNewUser.Connect(ip, Defines.portRequestNewUser);
            if (RequestAboutNewUser.Connected)
            {
                RequestAboutNewUser.SendTo(request);
            }
        }
        private byte[] TranslateNameInBytes(string userName, int maxLenghtName)
        {
            byte[] nameInBytes = Encoding.ASCII.GetBytes(userName);
            byte[] result = new byte[maxLenghtName];
            for (int i = 0; i < maxLenghtName; i++)
            {
                if (i < nameInBytes.Length)
                {
                    result[i] = nameInBytes[i];
                }
                else
                {
                    result[i] = 0;
                }
            }
            return result;
        }
        private string TranslateBytesInName(byte[] nameInBytes)
        {
            int lastElem = -1;
            for (int i = nameInBytes.Length - 1; i > 0; i--)
            {
                if (nameInBytes[i] != 0)
                {
                    lastElem = i;
                }
            }
            if (lastElem == -1)
            {
                return Defines.defaultName;
            }
            byte[] clearNameInByte = new byte[lastElem + 1];
            for (int i = 0; i < lastElem + 1; i++)
            {
                clearNameInByte[i] = nameInBytes[i];
            }
            return Encoding.ASCII.GetString(clearNameInByte);
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
            while (true)
            {
                eventThreadGetRequests.WaitOne();
                eventThreadSendVideo.Set();
                if (RequestsFromNewUser.AvailableData() >= Defines.RequestSize)
                {
                    byte[] request = RequestsFromNewUser.ReceiveTo(Defines.RequestSize);
                    flagGetRequests = true;
                    string ip = request[0].ToString() + "." + request[1].ToString() + "." + request[2].ToString() + "." + request[3].ToString();
                    int chatNumber = request[4];
                    FlagsRequest numberRequest = (FlagsRequest)request[5];
                    /*byte[] nameInBytes = new byte[Defines.maxLenghtName];
                    for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
                    {
                        nameInBytes[j] = request[i];
                    }
                    string name = TranslateBytesInName(nameInBytes);*/
                    if ((chatNumber == 0) || (ip == GetHostIP()))
                    {
                        continue;
                    }
                    if (numberRequest == FlagsRequest.FSetInfo)
                    {
                        AddUserIpAndChatNumber(ip, chatNumber);
                    }
                    if (numberRequest == FlagsRequest.FGetInfo)
                    {
                        SetRequestAboutNewUser((byte)FlagsRequest.FSetInfo, ip);
                        AddUserIpAndChatNumber(ip, chatNumber);
                    }
                    if (numberRequest == FlagsRequest.FTrySetUser)
                    {
                        if (MessageBox.Show("New user with chat number " + chatNumber + " and user ip " + ip + " want add. Add his?", "new user", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                        {
                            SetRequestAboutNewUser(FlagsRequest.FAddUser, ip);
                            AddUserIpAndChatNumber(ip, chatNumber);
                            AddNewUserInGroup(ip, chatNumber);
                        }
                    }
                    if (numberRequest == FlagsRequest.FAddUser)
                    {
                        AddNewUserInGroup(ip, chatNumber);
                    }
                    if (numberRequest == FlagsRequest.FRemoveUser)
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
                lock (listUsersIp)
                {
                    lock (listUsersChatNumbers)
                    {
                        cb_Users.Items.Clear();
                        for (int i = 0; i < listUsersIp.Count; i++)
                        {
                            cb_Users.Items.Add("ip " + listUsersIp[i] + " chat number " + listUsersChatNumbers[i].ToString());
                        }
                    }
                }
            }
        }
        private void AddUserIpAndChatNumber(string ip, int chatNumber)
        {
            lock (listUsersIp)
            {
                lock (listUsersChatNumbers)
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
        private void button2_Click(object sender, EventArgs e)
        {
            UpdateUsers();
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SettingCamera();
        }
        private void tS_CB_Cameras_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((tS_CB_Cameras.SelectedIndex != -1) && (sendVideo != null))
            {
                sendVideo.StopSendVideo();
                sendVideo.MonikerStringVideo = videoCaptureDiveses[tS_CB_Cameras.SelectedIndex].MonikerString;
                sendVideo.StartSendVideo();
            }
        }
        private void updateListUsersTimer_Tick(object sender, EventArgs e)
        {
            UpdateUsers();
        }
        private void newNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*myUserName = GetUserName();
            SetUserNameOnForm();*/
        }
    }
}
