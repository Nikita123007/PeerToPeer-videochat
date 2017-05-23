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
        private const int lengthDgram = 65500;
        private List<int> listCurrentUsersChatNumbers;
        private const int startPortsUsers = 9010;
        private Dictionary<int, Queue<byte[]>> QueuesUsersPackage;
        private const int MaxPackagesOnOneImage = 10;

        public List<int> ListCurrentUsersChatNumbers
        {
            set
            {
                listCurrentUsersChatNumbers = value;
            }
        }
        public PictureBox Pb_Video
        {
            set
            {
                pb_Video = value;
            }
        }

        public ReceiveVideo(PictureBox pb_Video)
        {
            StartInitialise(pb_Video);
        }
        public void StartInitialise(PictureBox pb_Video)
        {
            udp_Receiver = new Udp_Receiver(startPortsUsers);
            ListCurrentUsersChatNumbers = new List<int>();
            Pb_Video = pb_Video;
            threadReceiveVideo = new Thread(new ParameterizedThreadStart(ReseiveDataOfImages));
            InitialiseQueues();
        }
        private void InitialiseQueues()
        {
            QueuesUsersPackage = new Dictionary<int, Queue<byte[]>>();
            foreach (int userNumber in listCurrentUsersChatNumbers)
            {
                QueuesUsersPackage.Add(userNumber, new Queue<byte[]>());
            }
        }
        public void StartReceiveVideo()
        {
            if ((threadReceiveVideo != null) && (!threadReceiveVideo.IsAlive))
            {                
                threadReceiveVideo.Start();
            }
        }

        private void ReseiveDataOfImages(object sender)
        {
            udp_Receiver.Timeout = 500;
            while (true)
            {
                byte[] userPackage = udp_Receiver.ReceiveTo(lengthDgram);
                lock (listCurrentUsersChatNumbers)
                {
                    if (QueuesUsersPackage.ContainsKey(userPackage[lengthDgram - 2]))
                    {
                        QueuesUsersPackage[userPackage[lengthDgram - 2]].Enqueue(userPackage);
                        TryGetImageUser(userPackage[lengthDgram - 2]);
                    }
                }
            }
        }
        private void TryGetImageUser(byte userNumber)
        {
            if (QueuesUsersPackage[userNumber].Count > MaxPackagesOnOneImage)
            {
                int pointer = 0;
                byte[] imageInBytes = new byte[lengthDgram * MaxPackagesOnOneImage];
                byte[] currentPackage = QueuesUsersPackage[userNumber].Dequeue();
                if (currentPackage[lengthDgram - 1] == 1)
                {
                    for (int i = 0; i < lengthDgram - 2; i++)
                    {
                        imageInBytes[pointer] = currentPackage[i];
                        pointer++;
                    }
                    while ((QueuesUsersPackage[userNumber].Count > 0) && (QueuesUsersPackage[userNumber].Peek()[lengthDgram - 1] == 0))
                    {
                        currentPackage = QueuesUsersPackage[userNumber].Dequeue();
                        for (int i = 0; i < lengthDgram - 2; i++)
                        {
                            imageInBytes[pointer] = currentPackage[i];
                            pointer++;
                        }
                    }
                    SetImageOnForm(ByteArrayToImage(imageInBytes), userNumber);
                }
            }           
        }
        private void SetImageOnForm(Image image, int userNumber)
        {
            int index = listCurrentUsersChatNumbers.IndexOf(userNumber);
            Graphics g = pb_Video.CreateGraphics();
            g.DrawImage(image, index * (pb_Video.Width / listCurrentUsersChatNumbers.Count), (pb_Video.Height - image.Height)/2);
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
        public bool RemoveUser(int userChatNumber)
        {
            lock (listCurrentUsersChatNumbers)
            {
                if (listCurrentUsersChatNumbers.Contains(userChatNumber))
                {
                    //StopReceiveVideo();
                    listCurrentUsersChatNumbers.Remove(userChatNumber);
                    QueuesUsersPackage.Remove(userChatNumber);
                    //StartReceiveVideo();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        public bool AddUser(int userChatNumber)
        {
            lock (listCurrentUsersChatNumbers)
            {
                if (!listCurrentUsersChatNumbers.Contains(userChatNumber))
                {
                    //StopReceiveVideo();
                    listCurrentUsersChatNumbers.Add(userChatNumber);
                    QueuesUsersPackage.Add(userChatNumber, new Queue<byte[]>());
                    //StartReceiveVideo();
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
