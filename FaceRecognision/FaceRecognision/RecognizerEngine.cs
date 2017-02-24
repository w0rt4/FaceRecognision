using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Face;
using System.Windows.Forms;


namespace FaceRecognision
{
    class RecognizerEngine
    {
        private FaceRecognizer _faceRecognizer;
        private String _recognizerFilePath;

        public RecognizerEngine(String recognizerFilePath)
        {
            _recognizerFilePath = Application.StartupPath + "\\recognizer.yaml";
            _faceRecognizer = new EigenFaceRecognizer(80, double.PositiveInfinity);
        }

        public bool TrainRecognizer(List<Faces> faces)
        {
            
                var faceImages = new Image<Gray, byte>[faces.Count];
                var faceLabels = new int[faces.Count];
                int counter = 0;
                foreach (var face in faces)
                {
                    Stream stream = new MemoryStream();
                    stream.Write(face.FaceSample, 0,face.FaceSample.Length);
                    var faceImage = new Image<Gray, byte>(new Bitmap(stream));
                    faceImages[counter] = faceImage.Resize(100,100,Inter.Cubic);
                    faceLabels[counter] = face.Id;
                    counter++;
                }
                _faceRecognizer.Train(faceImages, faceLabels);
                _faceRecognizer.Save(_recognizerFilePath);
            
            return true;
           
        }

        public void LoadRecognizerData()
        {
            _faceRecognizer.Load(_recognizerFilePath);
        }

        public int RecognizeUser(Image<Gray, byte> userImage)
        {
          /*  Stream stream = new MemoryStream();
            stream.Write(userImage, 0, userImage.Length);
            var faceImage = new Image<Gray, byte>(new Bitmap(stream));*/
            _faceRecognizer.Load(_recognizerFilePath);

            var result = _faceRecognizer.Predict(userImage.Resize(100, 100, Inter.Cubic));
            return result.Label;
        }
    }
}
