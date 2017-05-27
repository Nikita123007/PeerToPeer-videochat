using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;

namespace VideoChat
{
    class GetRequests
    {
        private Thread threadGetRequests;
        private AutoResetEvent nextEventThread;
        private AutoResetEvent thisEventThread;
        private Udp_Receiver RequestsFromNewUser;
        private Udp_Sender RequestAboutNewUser;
        public int myChatNumber;
        public List<string> listUsersIp;
        public List<int> listUsersChatNumbers;
        public List<string> listUsersName;
        public string myUserName;
        private bool flagGetRequests;
        private SendVideo sendVideo;
        private ReceiveVideo receiveVideo;
        public event delegateUpdateListUsers eventUpdateListUsers;

        public GetRequests(string myUserName, AutoResetEvent nextEventThread, AutoResetEvent thisEventThread, SendVideo sendVideo, ReceiveVideo receiveVideo, delegateUpdateListUsers eventUpdateListUsers)
        {
            myChatNumber = Defines.DefaultChatNumber;
            RequestAboutNewUser = new Udp_Sender();
            RequestsFromNewUser = new Udp_Receiver(Defines.portRequestNewUser);
            this.nextEventThread = nextEventThread;
            this.thisEventThread = thisEventThread;
            listUsersChatNumbers = new List<int>();
            listUsersName = new List<string>();
            listUsersIp = new List<string>();
            this.myUserName = myUserName;
            flagGetRequests = false;
            this.sendVideo = sendVideo;
            this.receiveVideo = receiveVideo;
            threadGetRequests = new Thread(GetRequest);
            this.eventUpdateListUsers = eventUpdateListUsers;
        }
        public void StartGetRequests()
        {
            if ((threadGetRequests == null) || (!threadGetRequests.IsAlive))
            {
                threadGetRequests.Start();
            }
        }
        public void StopGetRequests()
        {
            if ((threadGetRequests != null) && (threadGetRequests.IsAlive))
            {
                threadGetRequests.Abort();
            }
        }
        public void SendRequest(FlagsRequest numberRequest, string ip)
        {
            string ipAddress = GetHostIP();
            byte[] request = new byte[Defines.RequestSize];
            request[0] = (byte)GetNumberOfIp(ipAddress, 1);
            request[1] = (byte)GetNumberOfIp(ipAddress, 2);
            request[2] = (byte)GetNumberOfIp(ipAddress, 3);
            request[3] = (byte)GetNumberOfIp(ipAddress, 4);
            request[4] = (byte)myChatNumber;
            request[5] = (byte)numberRequest;
            byte[] nameInBytes = TranslateNameInBytes(myUserName, Defines.maxLenghtName);
            for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
            {
                request[i] = nameInBytes[j];
            }
            RequestAboutNewUser.Connect(ip, Defines.portRequestNewUser);
            if (RequestAboutNewUser.Connected)
            {
                RequestAboutNewUser.SendTo(request);
            }
        }
        private void GetRequest()
        {
            while (true)
            {
                thisEventThread.WaitOne();
                nextEventThread.Set();
                if (RequestsFromNewUser.AvailableData() >= Defines.RequestSize)
                {
                    byte[] request = RequestsFromNewUser.ReceiveTo(Defines.RequestSize);
                    flagGetRequests = true;
                    string ip = request[0].ToString() + "." + request[1].ToString() + "." + request[2].ToString() + "." + request[3].ToString();
                    int chatNumber = request[4];
                    FlagsRequest numberRequest = (FlagsRequest)request[5];
                    byte[] nameInBytes = new byte[Defines.maxLenghtName];
                    for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
                    {
                        nameInBytes[j] = request[i];
                    }
                    string name = TranslateBytesInName(nameInBytes);
                    if ((chatNumber == 0) || (ip == GetHostIP()))
                    {
                        continue;
                    }
                    if (numberRequest == FlagsRequest.FSetInfo)
                    {
                        AddUserIpAndChatNumber(ip, chatNumber, name);
                    }
                    if (numberRequest == FlagsRequest.FGetInfo)
                    {
                        SendRequest((byte)FlagsRequest.FSetInfo, ip);
                        AddUserIpAndChatNumber(ip, chatNumber, name);
                    }
                    if (numberRequest == FlagsRequest.FTrySetUser)
                    {
                        if (!sendVideo.ContainUser(ip))
                        {
                            if (MessageBox.Show("User: " + name + " with chat number " + chatNumber + " and ip " + ip + " want add. Add his?", "Call", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
                            {
                                SendRequest(FlagsRequest.FAddUser, ip);
                                AddUserIpAndChatNumber(ip, chatNumber, name);
                                AddNewUserInGroup(ip, chatNumber);
                            }
                            else
                            {
                                SendRequest(FlagsRequest.FNoAddUser, ip);
                                AddUserIpAndChatNumber(ip, chatNumber, name);
                            }
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
                    if (numberRequest == FlagsRequest.FNoAddUser)
                    {
                        MessageBox.Show(name + " didn't pick up the phone.", "Answer ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    flagGetRequests = false;
                }
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
            for (int i = nameInBytes.Length - 1; i >= 0; i--)
            {
                if (nameInBytes[i] != 0)
                {
                    lastElem = i;
                    break;
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
        private void RemoveUserWithGroup(string ip, int chatNumber)
        {
            sendVideo.RemoveUser(ip);
            receiveVideo.RemoveUser(chatNumber);
        }
        public void AbortGroup()
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
        private void AddUserIpAndChatNumber(string ip, int chatNumber, string name)
        {
            lock (listUsersName)
            {
                lock (listUsersIp)
                {
                    lock (listUsersChatNumbers)
                    {
                        if (!listUsersIp.Contains(ip))
                        {
                            listUsersIp.Add(ip);
                            listUsersChatNumbers.Add(chatNumber);
                            listUsersName.Add(name);
                            eventUpdateListUsers();
                        }
                        else
                        {
                            int indexElement = listUsersIp.IndexOf(ip);
                            if ((listUsersChatNumbers[indexElement] != chatNumber) || (listUsersName[indexElement] != name))
                            {
                                listUsersChatNumbers[indexElement] = chatNumber;
                                listUsersName[indexElement] = name;
                                eventUpdateListUsers();
                            }
                        }
                    }
                }
            }
        }
        private void UpdateUsers()
        {
            lock (listUsersName)
            {
                lock (listUsersIp)
                {
                    lock (listUsersChatNumbers)
                    {
                        listUsersIp.Clear();
                        listUsersChatNumbers.Clear();
                        listUsersName.Clear();
                        SendRequest(FlagsRequest.FGetInfo, Defines.broadcast);
                    }
                }
            }
        }
        public void SetMyChatNumber()
        {
            UpdateUsers();
            Thread.Sleep(1000);
            while (flagGetRequests) ;
            int myNumber = 1;
            while (listUsersChatNumbers.Contains(myNumber))
            {
                myNumber++;
            }
            myNumber = myNumber % 256;
            lock (listUsersIp)
            {
                for (int i = 0; i < listUsersIp.Count; i++)
                {
                    SendRequest((byte)FlagsRequest.FSetInfo, listUsersIp[i]);
                }
            }
            myChatNumber = myNumber;
            sendVideo.MyChatNumber = myNumber;
        }
        private int GetNumberOfIp(string ipAddress, int number)
        {
            string[] numbersOfIp = new string[4] { "", "", "", "" };
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
