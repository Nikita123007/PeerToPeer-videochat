using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.Net;
using System.Drawing;

namespace VideoChat
{
    class GetRequests
    {
        private Thread threadGetRequests;
        private Udp_Receiver RequestsReceiver;
        private Udp_Sender RequestSender;
        public List<string> listUsersIp;
        public List<string> listUsersNames;
        public string myUserName;
        private SendVideo sendVideo;
        private ReceiveVideo receiveVideo;
        public event delegateUpdateListUsers eventUpdateListUsers;
        public PictureBox pb_Video;

        public GetRequests(SendVideo sendVideo, ReceiveVideo receiveVideo, delegateUpdateListUsers eventUpdateListUsers, PictureBox pb_Video, string myUserName)
        {
            listUsersNames = new List<string>();
            listUsersIp = new List<string>();
            this.myUserName = myUserName;
            this.sendVideo = sendVideo;
            this.receiveVideo = receiveVideo;
            threadGetRequests = new Thread(GetRequest);
            this.eventUpdateListUsers = eventUpdateListUsers;
            this.pb_Video = pb_Video;
        }
        public void StartGetRequests()
        {
            if ((threadGetRequests == null) || (!threadGetRequests.IsAlive))
            {
                RequestSender = new Udp_Sender();
                RequestsReceiver = new Udp_Receiver(Defines.portGetRequests);
                threadGetRequests.Start();
            }
        }
        public void StopGetRequests()
        {
            if ((threadGetRequests != null) && (threadGetRequests.IsAlive))
            {
                RequestSender.Close();
                RequestsReceiver.Close();
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
            request[4] = (byte)numberRequest;
            byte[] nameInBytes = TranslateNameInBytes(myUserName, Defines.maxLenghtName);
            for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
            {
                request[i] = nameInBytes[j];
            }
            RequestSender.Connect(ip, Defines.portGetRequests);
            if (RequestSender.Connected)
            {
                RequestSender.SendTo(request);
            }
        }
        private void GetRequest()
        {
            while (true)
            {
                byte[] request = RequestsReceiver.ReceiveTo(Defines.RequestSize);
                string ip = request[0].ToString() + "." + request[1].ToString() + "." + request[2].ToString() + "." + request[3].ToString();
                if (ip == GetHostIP())
                {
                    continue;
                }
                FlagsRequest numberRequest = (FlagsRequest)request[4];
                byte[] nameInBytes = new byte[Defines.maxLenghtName];
                for (int i = Defines.startPositionNameInRequest, j = 0; (i < Defines.RequestSize) && (j < nameInBytes.Length); i++, j++)
                {
                    nameInBytes[j] = request[i];
                }
                string name = TranslateBytesInName(nameInBytes);               
                if (numberRequest == FlagsRequest.FSetInfo)
                {
                    AddUserIpAndChatNumber(ip, name);
                }
                if (numberRequest == FlagsRequest.FGetInfo)
                {
                    SendRequest((byte)FlagsRequest.FSetInfo, ip);
                    AddUserIpAndChatNumber(ip, name);
                }
                if (numberRequest == FlagsRequest.FTrySetUser)
                {
                    if (!sendVideo.ContainUser(ip))
                    {
                        receiveVideo.AddUser(ip, name);
                        receiveVideo.AddAnswerUser(ip);
                    }
                }
                if (numberRequest == FlagsRequest.FAddUser)
                {
                    AddNewUserInGroup(ip, name);
                }
                if (numberRequest == FlagsRequest.FRemoveUser)
                {
                    RemoveUserWithGroup(ip);
                }
                if (numberRequest == FlagsRequest.FNoAddUser)
                {
                    receiveVideo.RemoveCallUser(ip);
                    RemoveUserWithGroup(ip);
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
        private void RemoveUserWithGroup(string ip)
        {
            sendVideo.RemoveUser(ip);
            receiveVideo.RemoveUser(ip);
        }
        public void AbortGroup()
        {
            lock (listUsersIp)
            {
                for (int i = 0; i < listUsersIp.Count; i++)
                {
                    SendRequest(FlagsRequest.FRemoveUser, listUsersIp[i]);
                }
                sendVideo.ClearListUsers();
                receiveVideo.ClearListUsers();
            }
        }
        private void AddNewUserInGroup(string ip, string name)
        {
            sendVideo.AddUser(ip);
            receiveVideo.AddUser(ip, name);
        }
        private void AddUserIpAndChatNumber(string ip, string name)
        {
            lock (listUsersNames)
            {
                lock (listUsersIp)
                {
                    if (!listUsersIp.Contains(ip))
                    {
                        listUsersIp.Add(ip);
                        listUsersNames.Add(name);
                        eventUpdateListUsers();
                    }
                    else
                    {
                        int indexElement = listUsersIp.IndexOf(ip);
                        if (listUsersNames[indexElement] != name)
                        {
                            listUsersNames[indexElement] = name;
                            eventUpdateListUsers();
                        }
                    }
                }
            }
        }
        public void UpdateUsers()
        {
            lock (listUsersNames)
            {
                lock (listUsersIp)
                {
                    listUsersIp.Clear();
                    listUsersNames.Clear();
                    SendRequest(FlagsRequest.FGetInfo, Defines.broadcast);
                }
            }
        }
        public int GetNumberOfIp(string ipAddress, int number)
        {
            string[] numbersOfIp = ipAddress.Split('.');
            return Convert.ToInt32(numbersOfIp[number - 1]);
        }
        public string GetHostIP()
        {
            string Host = Dns.GetHostName();
            return Dns.GetHostByName(Host).AddressList[0].ToString();
        }
        public void Call(string callUserIp)
        {
            SendRequest(FlagsRequest.FTrySetUser, callUserIp);
            int indexUser = listUsersIp.IndexOf(callUserIp);
            receiveVideo.AddUser(callUserIp, listUsersNames[indexUser]);
            receiveVideo.AddCallUser(callUserIp);
        }
    }
}
