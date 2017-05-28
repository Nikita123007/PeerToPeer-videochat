using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VideoChat
{
    class ReceiveVideo
    {
        private Udp_Receiver udp_Receiver;
        private Thread threadReceiveVideo;
        private PictureBox pb_Video;
        private Dictionary<int, Queue<byte[]>> QueuesUsersPackages;
        private AutoResetEvent nextEventThread;
        private AutoResetEvent thisEventThread;
        public List<InfoReceiveUser> listUsers;

      /*  public List<int> ListUsersChatNumbers
        {
            get
            {
                return listUsersChatNumbers;
            }
            set
            {
                listUsersChatNumbers = value;
            }
        }*/
        public PictureBox Pb_Video
        {
            set
            {
                pb_Video = value;
            }
        }

        public ReceiveVideo(PictureBox pb_Video, AutoResetEvent nextEventThread, AutoResetEvent thisEventThread)
        {
            udp_Receiver = new Udp_Receiver(Defines.startPortsUsers);
            udp_Receiver.Timeout = Defines.ReceiveTimeout;
            listUsers = new List<InfoReceiveUser>();
            Pb_Video = pb_Video;
            threadReceiveVideo = new Thread(ReseiveDataOfImages);
            InitialiseQueues();
            this.nextEventThread = nextEventThread;
            this.thisEventThread = thisEventThread;
        }
        private void InitialiseQueues()
        {
            QueuesUsersPackages = new Dictionary<int, Queue<byte[]>>();
            foreach (InfoReceiveUser user in listUsers)
            {
                QueuesUsersPackages.Add(user.chatNumber, new Queue<byte[]>());
            }
        }
        public void StartReceiveVideo()
        {
            if ((threadReceiveVideo != null) && (!threadReceiveVideo.IsAlive))
            {                
                threadReceiveVideo.Start();
            }
        }
        private void ReseiveDataOfImages()
        {
            while (true)
            {
                thisEventThread.WaitOne();
                nextEventThread.Set();
                if (udp_Receiver.AvailableData() >= Defines.lengthDgram)
                {
                    byte[] userPackage = udp_Receiver.ReceiveTo(Defines.lengthDgram);
                    lock (listUsers)
                    {
                        if (QueuesUsersPackages.ContainsKey(userPackage[Defines.lengthDgram - 2]))
                        {
                            QueuesUsersPackages[userPackage[Defines.lengthDgram - 2]].Enqueue(userPackage);
                            TryGetImageUser(userPackage[Defines.lengthDgram - 2]);
                        }
                    }
                }
            }
        }
        private void TryGetImageUser(byte userChatNumber)
        {
            if (QueuesUsersPackages[userChatNumber].Count > Defines.MaxPackagesOnOneImage)
            {
                int pointer = 0;
                byte[] imageInBytes = new byte[Defines.lengthDgram * Defines.MaxPackagesOnOneImage + 1];
                thisEventThread.WaitOne();
                byte[] currentPackage = QueuesUsersPackages[userChatNumber].Dequeue();
                nextEventThread.Set();
                if (currentPackage[Defines.lengthDgram - 1] == 1)
                {
                    for (int i = 0; i < Defines.lengthDgram - 2; i++)
                    {
                        imageInBytes[pointer] = currentPackage[i];
                        pointer++;
                    }
                    while ((QueuesUsersPackages[userChatNumber].Count > 0) && (QueuesUsersPackages[userChatNumber].Peek()[Defines.lengthDgram - 1] == 0))
                    {
                        thisEventThread.WaitOne();
                        currentPackage = QueuesUsersPackages[userChatNumber].Dequeue();
                        nextEventThread.Set();
                        for (int i = 0; i < Defines.lengthDgram - 2; i++)
                        {
                            imageInBytes[pointer] = currentPackage[i];
                            pointer++;
                        }
                    }
                    Image image = ByteArrayToImage(imageInBytes);
                    SetImageOnForm(new Bitmap(image, pb_Video.Width / listUsers.Count, pb_Video.Width / listUsers.Count * image.Height / image.Width), userChatNumber);
                }
            }           
        }
        private void SetImageOnForm(Bitmap image, int userChatNumber)
        {
            int index = -1;
            for(int i = 0; i < listUsers.Count; i++)
            {
                if (listUsers[i].chatNumber == userChatNumber)
                {
                    index = i;
                }
            }
            Graphics g = pb_Video.CreateGraphics();
            g.DrawImage(image, index * (pb_Video.Width / listUsers.Count), (pb_Video.Height - image.Height)/2);
        }
        /*private byte[] RevieseBytesForUdp(Udp_Client udp_Client)
        {
            int pointer = 0;
            byte[] data = new byte[lengthDgram];
            try
            {
                bool firstPackage = false;
                while (!firstPackage)
                {
                    data = udp_Client.ReceiveTo(lengthDgram);
                    if ((data[lengthDgram - 1] == 1) && (data[lengthDgram - 2] == 1) && (data[lengthDgram - 3] == 1))
                    {
                        firstPackage = true;
                    }
                }
                byte[] tempResult = new byte[data[0] * lengthDgram];
                for (int i = data[0]; i > 0; i--)
                {
                    data = udp_Client.ReceiveTo(lengthDgram);
                    for (int index = 0; index < data.Length; index++)
                    {
                        tempResult[pointer] = data[index];
                        pointer++;
                    }
                }
                return tempResult;
                //return ClearCancelBytes(ref tempResult);
            }
            catch (Exception er)
            {
                MessageBox.Show(er.Message);
                return data;
            }
        }*/
        /*private byte[] ClearCancelBytes(ref byte[] pictureInBytes)
        {
            int len = pictureInBytes.Length;
            for (int i = len - 1; i > 0; i--)
            {
                if (pictureInBytes[i] != 0)
                {
                    len = i + 1;
                    break;
                }
            }
            byte[] result = new byte[len];
            for (int i = 0; i < len; i++)
            {
                result[i] = pictureInBytes[i];
            }
            return result;
        }*/
        public Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
        public void StopReceiveVideo()
        {
            if (threadReceiveVideo != null)
            {
                threadReceiveVideo.Abort();
            }
        }
        public void RemoveUser(string ip)
        {
            lock (listUsers)
            {
                lock (QueuesUsersPackages)
                {
                    InfoReceiveUser user = GetUser(ip);
                    if (user != null)
                    {
                        QueuesUsersPackages.Remove(user.chatNumber);
                        listUsers.Remove(user);
                    }
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void AddUser(string ip, string name, int chatNumber)
        {
            lock (listUsers)
            {
                lock (QueuesUsersPackages)
                {
                    InfoReceiveUser user = GetUser(ip);
                    if (user == null)
                    {
                        listUsers.Add(new InfoReceiveUser(ip, name, chatNumber));
                        QueuesUsersPackages.Add(chatNumber, new Queue<byte[]>());
                    }
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void AddCallUser(string ip)
        {
            lock (listUsers)
            {
                InfoReceiveUser user = GetUser(ip);
                if (user != null){
                    user.answerUser = false;
                    user.callUser = true;
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void RemoveCallUser(string ip)
        {
            lock (listUsers)
            {
                InfoReceiveUser user = GetUser(ip);
                if (user != null)
                {
                    user.callUser = false;
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void AddAnswerUser(string ip)
        {
            lock (listUsers)
            {
                InfoReceiveUser user = GetUser(ip);
                if (user != null)
                {
                    user.answerUser = true;
                    user.callUser = false;
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void RemoveAnswerUser(string ip)
        {
            lock (listUsers)
            {
                InfoReceiveUser user = GetUser(ip);
                if (user != null)
                {
                    user.answerUser = false;
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void ClearListUsers()
        {
            lock (listUsers)
            {
                lock (QueuesUsersPackages)
                {
                    listUsers.Clear();
                    QueuesUsersPackages.Clear();
                    ClearVideo();
                }
            }
        }
        private void SetUsersNameUnderVideo()
        {
            lock (listUsers)
            {
                Font font = new Font(Defines.familyName, Defines.emSize);
                int count = listUsers.Count;
                for (int i = 0; i < count; i++)
                {
                    string name = listUsers[i].name;
                    Graphics g = pb_Video.CreateGraphics();
                    g.DrawString(name, font, Brushes.Black, (pb_Video.Width * (i + 1) / count) - (pb_Video.Width / count / 2) - (name.Length * Defines.emSize / 2), pb_Video.Height - Defines.emSize * 2);
                }
            }
        }
        private void SetUsersImagesCall()
        {
            lock (listUsers)
            {
                foreach (InfoReceiveUser user in listUsers)
                {
                    if (user.callUser)
                    {
                        Bitmap imageCall = new Bitmap(Defines.pathCallImage);
                        imageCall = new Bitmap(imageCall, pb_Video.Width / listUsers.Count, pb_Video.Width / listUsers.Count * imageCall.Height / imageCall.Width);
                        int numberUser = listUsers.IndexOf(user);
                        AddImageOnVideo(imageCall, numberUser * (pb_Video.Width / listUsers.Count), (pb_Video.Height - imageCall.Height) / 2);
                        user.callButton = new Point(numberUser * (pb_Video.Width / listUsers.Count) + imageCall.Width / 2, (pb_Video.Height - imageCall.Height) / 2 + (int)(imageCall.Height * Defines.reletionSizeInCallImage));
                    }
                }
            }
        }
        private void SetUsersImagesAnswer()
        {
            lock (listUsers)
            {
                foreach (InfoReceiveUser user in listUsers)
                {
                    if (user.answerUser)
                    {
                        Bitmap imageAnswer = new Bitmap(Defines.pathAnswerImage);
                        imageAnswer = new Bitmap(imageAnswer, pb_Video.Width / listUsers.Count, pb_Video.Width / listUsers.Count * imageAnswer.Height / imageAnswer.Width);
                        int numberUser = listUsers.IndexOf(user);
                        AddImageOnVideo(imageAnswer, numberUser * (pb_Video.Width / listUsers.Count), (pb_Video.Height - imageAnswer.Height) / 2);
                        user.answerDownButton = new Point(numberUser * (pb_Video.Width / listUsers.Count) + (int)(imageAnswer.Width * Defines.reletionSizeWidthDownInAnswerImage), (pb_Video.Height - imageAnswer.Height) / 2 + (int)(imageAnswer.Height * Defines.reletionSizeHeightInAnswerImage));
                        user.answerUpButton = new Point(numberUser * (pb_Video.Width / listUsers.Count) + (int)(imageAnswer.Width * Defines.reletionSizeWidthUpInAnswerImage), (pb_Video.Height - imageAnswer.Height) / 2 + (int)(imageAnswer.Height * Defines.reletionSizeHeightInAnswerImage));
                    }
                }
            }
        }
        private void ClearVideo()
        {
            Graphics g = pb_Video.CreateGraphics();
            g.Clear(Color.White);
        }
        private void AddImageOnVideo(Image image, float x, float y)
        {
            Graphics g = pb_Video.CreateGraphics();
            g.DrawImage(image, x, y);
        }
        private void UpdateStaticComponentVideo()
        {
            SetUsersImagesAnswer();
            SetUsersImagesCall();
            SetUsersNameUnderVideo();
        }
        private InfoReceiveUser GetUser(string ip)
        {
            InfoReceiveUser user = null;
            foreach (InfoReceiveUser valueUser in listUsers)
            {
                if (valueUser.ip == ip)
                {
                    user = valueUser;
                }
            }
            return user;
        }
    }
}
