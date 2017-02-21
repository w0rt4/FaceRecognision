using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Threading;

namespace FaceRecognision
{
    public partial class Form1 : Form
    {

        private Capture _cap;
        private CascadeClassifier _cascadeClassifier;
        private CascadeClassifier _cascadeClassifier2;
        private static bool doWork = true;

        public Form1()
        {
            InitializeComponent();
            _cap = new Capture();


        }

        private void StartRecognision(object sender, EventArgs e)
        {
            doWork = true;

            Task.Factory.StartNew(BackgroundWorkerDoWork, null);
            Thread.Sleep(0);
        }
        void BackgroundWorkerDoWork(object args)
        {
            while (doWork)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    /*imgCamUser.Image = _cap.QueryFrame();
                    this.InvokeIfRequired(() => imgCamUser.Image = _cap.QueryFrame());
                   // Console.Write("Pracuje");*/


                    _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "\\haarcascade_frontalface_default.xml");
                    _cascadeClassifier2 = new CascadeClassifier(Application.StartupPath + "\\haarcascade_eye.xml");
                    using (var imageFrame = _cap.QueryFrame().ToImage<Bgr, Byte>())
                    {
                        if (imageFrame != null)
                        {
                            var grayframe = imageFrame.Convert<Gray, byte>();
                            var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                            var eyes = _cascadeClassifier2.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                            foreach (var face in faces)
                            {
                                imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                            }
                            foreach (var eye in eyes)
                            {
                                imageFrame.Draw(eye, new Bgr(Color.Blue), 1);
                            }
                        }
                        imgCamUser.Image = imageFrame;
                    }
                })
                );
                Thread.Sleep(300);
            }
        }

        private void StopRecognision(object sender, EventArgs e)
        {
            doWork = false;
            imgCamUser.Image = null;
        }
    }
}
