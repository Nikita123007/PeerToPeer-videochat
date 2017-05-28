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
        private Dictionary<byte[], Queue<byte[]>> QueuesUsersPackages;
        private AutoResetEvent nextEventThread;
        private AutoResetEvent thisEventThread;
        public List<InfoReceiveUser> listUsers;

        public ReceiveVideo(PictureBox pb_Video, AutoResetEvent nextEventThread, AutoResetEvent thisEventThread)
        {
            udp_Receiver = new Udp_Receiver(Defines.startPortsUsers);
            udp_Receiver.Timeout = Defines.ReceiveTimeout;
            listUsers = new List<InfoReceiveUser>();
            this.pb_Video = pb_Video;
            threadReceiveVideo = new Thread(ReseiveDataOfImages);
            InitialiseQueues();
            this.nextEventThread = nextEventThread;
            this.thisEventThread = thisEventThread;
        }
        private void InitialiseQueues()
        {
            QueuesUsersPackages = new Dictionary<byte[], Queue<byte[]>>();
            foreach (InfoReceiveUser user in listUsers)
            {
                QueuesUsersPackages.Add(GetIpInBytes(user.ip), new Queue<byte[]>());
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
                        byte[] ip = new byte[4] { userPackage[Defines.lengthDgram - 5], userPackage[Defines.lengthDgram - 4], userPackage[Defines.lengthDgram - 3], userPackage[Defines.lengthDgram - 2]};
                        if (QueuesUsersPackages.ContainsKey(ip))
                        {
                            QueuesUsersPackages[ip].Enqueue(userPackage);
                            TryGetImageUser(ip);
                        }
                    }
                }
            }
        }
        private void TryGetImageUser(byte[] ip)
        {
            if (QueuesUsersPackages[ip].Count > Defines.MaxPackagesOnOneImage)
            {
                int pointer = 0;
                byte[] imageInBytes = new byte[Defines.lengthDgram * Defines.MaxPackagesOnOneImage + 1];
                thisEventThread.WaitOne();
                byte[] currentPackage = QueuesUsersPackages[ip].Dequeue();
                nextEventThread.Set();
                if (currentPackage[Defines.lengthDgram - 1] == 1)
                {
                    for (int i = 0; i < Defines.lengthDgram - 2; i++)
                    {
                        imageInBytes[pointer] = currentPackage[i];
                        pointer++;
                    }
                    while ((QueuesUsersPackages[ip].Count > 0) && (QueuesUsersPackages[ip].Peek()[Defines.lengthDgram - 1] == 0))
                    {
                        thisEventThread.WaitOne();
                        currentPackage = QueuesUsersPackages[ip].Dequeue();
                        nextEventThread.Set();
                        for (int i = 0; i < Defines.lengthDgram - 5; i++)
                        {
                            imageInBytes[pointer] = currentPackage[i];
                            pointer++;
                        }
                    }
                    Image image = ByteArrayToImage(imageInBytes);
                    SetImageOnForm(new Bitmap(image, pb_Video.Width / listUsers.Count, pb_Video.Width / listUsers.Count * image.Height / image.Width), ip);
                }
            }           
        }
        private void SetImageOnForm(Bitmap image, byte[] ip)
        {
            string ipStr = GetIpInString(ip);
            int index = listUsers.IndexOf(GetUser(ipStr));
            Graphics g = pb_Video.CreateGraphics();
            g.DrawImage(image, index * (pb_Video.Width / listUsers.Count), (pb_Video.Height - image.Height)/2);
        }
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
                        QueuesUsersPackages.Remove(GetIpInBytes(ip));
                        listUsers.Remove(user);
                    }
                }
            }
            ClearVideo();
            UpdateStaticComponentVideo();
        }
        public void AddUser(string ip, string name)
        {
            lock (listUsers)
            {
                lock (QueuesUsersPackages)
                {
                    InfoReceiveUser user = GetUser(ip);
                    if (user == null)
                    {
                        listUsers.Add(new InfoReceiveUser(ip, name));
                        QueuesUsersPackages.Add(GetIpInBytes(ip), new Queue<byte[]>());
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
        public byte[] GetIpInBytes(string ipAddress)
        {
            string[] numbersOfIp = ipAddress.Split('.');
            return new byte[4] { Convert.ToByte(numbersOfIp[0]), Convert.ToByte(numbersOfIp[1]), Convert.ToByte(numbersOfIp[2]), Convert.ToByte(numbersOfIp[3]) };
        }
        public string GetIpInString(byte[] ip)
        {
            return Convert.ToString(ip[0]) + "." + Convert.ToString(ip[1]) + "." + Convert.ToString(ip[2]) + "." + Convert.ToString(ip[3]);
        }
    }
}
