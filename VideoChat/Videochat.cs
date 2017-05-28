using System;
using System.Drawing;
using System.Windows.Forms;
using AForge.Video.DirectShow;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using System.Net;

namespace VideoChat
{

    public delegate void delegateUpdateListUsers();
    public partial class Videochat : Form
    {
        private SendVideo sendVideo;
        private ReceiveVideo receiveVideo;
        private FilterInfoCollection videoCaptureDiveses;              
        private GetRequests getRequests;
        private AutoResetEvent eventThreadSendVideo;
        private AutoResetEvent eventThreadReceiveVideo;
        public event delegateUpdateListUsers eventUpdateListUsers;

        public Videochat()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            SettingCamera();
            BeginInitializeParams();
            StartGetRequests();
            StartReceiveVideo();
            StartSendVideo();
            getRequests.UpdateUsers();
            eventThreadSendVideo.Set();
        }
        private void StartGetRequests()
        {
            getRequests.StartGetRequests();
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
            LoadSetting();
            eventThreadSendVideo = new AutoResetEvent(false);
            eventThreadReceiveVideo = new AutoResetEvent(false);            
            if (tS_CB_Cameras.SelectedIndex != -1)
            {
                sendVideo = new SendVideo(videoCaptureDiveses[tS_CB_Cameras.SelectedIndex].MonikerString, GetHostIP(), pb_Video, eventThreadReceiveVideo, eventThreadSendVideo);
            }
            else
            {
                sendVideo = new SendVideo("default", GetHostIP(), pb_Video, eventThreadReceiveVideo, eventThreadSendVideo);
            }
            receiveVideo = new ReceiveVideo(pb_Video, eventThreadSendVideo, eventThreadReceiveVideo);
            eventUpdateListUsers += UpdateListUsers;
            getRequests = new GetRequests(sendVideo, receiveVideo, eventUpdateListUsers, pb_Video, lblUserName.Text);
        }
        private void SetUserNameOnForm(string name)
        {
            lblUserName.Text = name;
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
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            getRequests.AbortGroup();
            StopExecuteThreads();
            SaveSetting();
        }
        private void StopExecuteThreads()
        {
            sendVideo.StopSendVideo();
            receiveVideo.StopReceiveVideo();
            getRequests.StopGetRequests();
        }
        private void btn_Call_Click(object sender, EventArgs e)
        {
            string callUserIp = "";
            lock (cb_Users)
            {
                if ((cb_Users.Items != null) && (cb_Users.Items.Count != 0) && (cb_Users.SelectedIndex != -1))
                {
                    callUserIp = getRequests.listUsersIp[cb_Users.SelectedIndex];
                }
            }
            if ((callUserIp != "") && (!sendVideo.ContainUser(callUserIp)))
            {
                getRequests.Call(callUserIp);
            }
        }
        private void btn_Abort_Call_Group_Click(object sender, EventArgs e)
        {
            getRequests.AbortGroup();
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
        private void newNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string myUserName = GetUserName();
            SetUserNameOnForm(myUserName);
            SaveSetting();
            getRequests.myUserName = myUserName;
            lock (getRequests.listUsersIp)
            {
                for (int i = 0; i < getRequests.listUsersIp.Count; i++)
                {
                    getRequests.SendRequest(FlagsRequest.FSetInfo, getRequests.listUsersIp[i]);
                }
            }
        }
        public void UpdateListUsers()
        {
            lock (cb_Users)
            {
                lock (getRequests.listUsersNames)
                {
                    lock (getRequests.listUsersIp)
                    {
                        if (cb_Users.InvokeRequired) cb_Users.BeginInvoke(new Action(() => { cb_Users.Items.Clear(); }));
                        else cb_Users.Items.Clear();
                        string[] recordsUsers = new string[getRequests.listUsersIp.Count];
                        for (int i = 0; i < getRequests.listUsersIp.Count; i++)
                        {
                            recordsUsers[i] = getRequests.listUsersNames[i] + ": " + getRequests.listUsersIp[i];
                        }
                        if (cb_Users.InvokeRequired) cb_Users.BeginInvoke(new Action(() => { cb_Users.Items.AddRange(recordsUsers); }));
                        else cb_Users.Items.AddRange(recordsUsers);
                    }
                }
            }
        }
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetParamsQuality();
        }
        private void SetParamsQuality()
        {
            byte reducingQuality;
            GetParamsQualityes(cb_Quality.Text, out reducingQuality);
            Defines.reducingQuality = reducingQuality;
        }
        private void GetParamsQualityes(string quality, out byte reducingQuality)
        {
            switch (quality)
            {
                case nameof(Qualityes.High):
                    reducingQuality = 1;
                    break;
                case nameof(Qualityes.Medium):
                    reducingQuality = 3;
                    break;
                case nameof(Qualityes.Small):
                    reducingQuality = 5;
                    break;
                default:
                    reducingQuality = 5;
                    break;
            }
        }
        private void SaveSetting()
        {
            SettingVideoChat settingForm = GetSetting();
            XmlSerializer formatter = new XmlSerializer(settingForm.GetType());
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            using (FileStream fs = new FileStream(Defines.pathSettingFile, FileMode.Create))
            {
                formatter.Serialize(fs, settingForm, ns);
            }
        }
        private void LoadSetting()
        {
            try
            {
                SettingVideoChat settingForm = new SettingVideoChat();
                XmlSerializer formatter = new XmlSerializer(settingForm.GetType());
                using (FileStream fs = new FileStream(Defines.pathSettingFile, FileMode.Open))
                {
                    settingForm = (SettingVideoChat)formatter.Deserialize(fs);
                }
                if (CheckFormSetting(settingForm))
                {
                    SetSetting(settingForm);
                }
                else
                {
                    SetSetting(Defines.defaultSetting);
                }
            }
            catch (Exception)
            {
                SetSetting(Defines.defaultSetting);
            }
        }
        private bool CheckFormSetting(SettingVideoChat settingForm)
        {
            bool result = true;
            if (!CorrectName(settingForm.Name))
            {
                result = false;
            }
            return result;
        }
        private SettingVideoChat GetSetting()
        {
            SettingVideoChat settingForm = new SettingVideoChat();
            settingForm.Name = lblUserName.Text;
            settingForm.Quality = (Qualityes)cb_Quality.SelectedIndex;
            return settingForm;
        }
        private void SetSetting(SettingVideoChat settingForm)
        {
            lblUserName.Text = settingForm.Name;
            cb_Quality.Text = (settingForm.Quality).ToString();
            SetParamsQuality();
        }
        private void pb_Video_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (InfoReceiveUser user in receiveVideo.listUsers)
            {
                if (user.callUser)
                {
                    if (CheckMouse(e.X, e.Y, user.callButton))
                    {
                        getRequests.SendRequest(FlagsRequest.FNoAddUser, user.ip);
                        receiveVideo.RemoveUser(user.ip);
                        return;
                    }
                }
                if (user.answerUser)
                {
                    if (CheckMouse(e.X, e.Y, user.answerDownButton))
                    {
                        getRequests.SendRequest(FlagsRequest.FNoAddUser, user.ip);
                        receiveVideo.RemoveUser(user.ip);
                        return;
                    }
                    if (CheckMouse(e.X, e.Y, user.answerUpButton))
                    {
                        getRequests.SendRequest(FlagsRequest.FAddUser, user.ip);
                        sendVideo.AddUser(user.ip);
                        receiveVideo.RemoveAnswerUser(user.ip);
                        return;
                    }
                }
            }
        }
        public bool CheckMouse(int mouseX, int mouseY, Point point)
        {
            return ((Math.Abs(mouseX - point.X) < 40) && (Math.Abs(mouseY - point.Y) < 40));
        }
        public string GetHostIP()
        {
            string Host = Dns.GetHostName();
            return Dns.GetHostByName(Host).AddressList[0].ToString();
        }
    }
}
