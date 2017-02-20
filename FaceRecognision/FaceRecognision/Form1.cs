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

        public Form1()
        {
            InitializeComponent();


        }

        private void StartRecognision(object sender, EventArgs e)
        {

            _cap = new Capture();

            Task.Factory.StartNew(BackgroundWorkerDoWork, null);
            Thread.Sleep(0);
        }
        void BackgroundWorkerDoWork(object args)
        {
            while (true)
            {
                this.BeginInvoke(new MethodInvoker(delegate
                {
                    /*imgCamUser.Image = _cap.QueryFrame();
                    this.InvokeIfRequired(() => imgCamUser.Image = _cap.QueryFrame());
                   // Console.Write("Pracuje");*/


                    _cascadeClassifier = new CascadeClassifier(Application.StartupPath + "\\haarcascade_frontalface_default.xml");
                    using (var imageFrame = _cap.QueryFrame().ToImage<Bgr, Byte>())
                    {
                        if (imageFrame != null)
                        {
                            var grayframe = imageFrame.Convert<Gray, byte>();
                            var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, Size.Empty); //the actual face detection happens here
                            foreach (var face in faces)
                            {
                                imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                            }
                        }
                        imgCamUser.Image = imageFrame;
                    }
                })
                );
                Thread.Sleep(300);
            }
        }


    }
    public static class ControlExtensions
    {
        public static void InvokeIfRequired(this Control control, Action action)
        {
            if (control.InvokeRequired)
                control.Invoke(action);
            else
                action();
        }
        public static void InvokeIfRequired<T>(this Control control, Action<T> action, T parameter)
        {
            if (control.InvokeRequired)
                control.Invoke(action, parameter);
            else
                action(parameter);
        }
    }

}
