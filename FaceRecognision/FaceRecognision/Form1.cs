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
using System.IO;

namespace FaceRecognision
{
    public partial class Form1 : Form
    {

        private Capture _cap;
        private CascadeClassifier _cascadeClassifier;
        private CascadeClassifier _cascadeClassifier2;
        private static bool doWork = true;
        private static PersonsEntities persons = new PersonsEntities();
        public Image<Bgr, byte> ImageFrame;

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
                          //  var eyes = _cascadeClassifier2.DetectMultiScale(grayframe, 1.1, 10, Size.Empty);
                            foreach (var face in faces)
                            {
                                imageFrame.Draw(face, new Bgr(Color.BurlyWood), 3); //the detected face(s) is highlighted here using a box that is drawn around it/them

                            }
                           /* foreach (var eye in eyes)
                            {
                                imageFrame.Draw(eye, new Bgr(Color.Blue), 1);
                            }*/
                        }
                        ImageFrame = imageFrame.Copy();
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

        private void CaptureFace(object sender, EventArgs e)
        {
            doWork = false;
           
            var faceToSave = new Image<Gray, byte>(ImageFrame.Bitmap);
            Byte[] file;


            var username = "test_subject";
            var filePath = Application.StartupPath + String.Format("/{0}.bmp", username);
            faceToSave.ToBitmap().Save(filePath);
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }

            int id = 0;
            if(persons.Faces.Count() > 0)
            {
                id = (from faces in persons.Faces
                      where faces.Id != null
                      select faces.Id).Max();
            }
            id++;
            Faces face = new Faces();
            face.Id = id;
            face.UserName = username;
            face.FaceSample = file ;

            persons.Faces.Add(face);
            persons.SaveChanges();  //no updates 
            //save data

            RecognizerEngine r = new RecognizerEngine(null);
            r.TrainRecognizer(face);
 


        }

        private void RecognizeFace(object sender, EventArgs e)
        {
            doWork = false;

            var faceToSave = new Image<Gray, byte>(ImageFrame.Bitmap);
            Byte[] file;


            var username = "test_subject";
            var filePath = Application.StartupPath + String.Format("/{0}.bmp", username);
            faceToSave.ToBitmap().Save(filePath);
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = new BinaryReader(stream))
                {
                    file = reader.ReadBytes((int)stream.Length);
                }
            }


            RecognizerEngine r = new RecognizerEngine(null);
            int result = r.RecognizeUser(faceToSave);
        }
    }
}
    
