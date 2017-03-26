﻿using System;
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

namespace VideoChat
{
    public partial class Form1 : Form
    {
        private FilterInfoCollection videoCaptureDiveses;
        private VideoCaptureDevice finalVideo;
        private Udp_Server udp_Server;
        private Udp_Server getsRequest;
        private Thread threadGetRequests;

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
            getsRequest = new Udp_Server(9002);
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
            while (true)
            {

            }
        }
        private void FinalVideo_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            Bitmap picture = (Bitmap)eventArgs.Frame.Clone();
            byte[] pictureInByte = ImageToByteArray(picture);       
            udp_Server.SendTo(pictureInByte);
            pictureBox1.Image = picture;          
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
                threadGetRequests.Abort();
            }
        }
    }
}
