using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;

namespace Components.Imaging
{
    public class Camera
    {
        public static Camera Instance { get; } = new Camera();
        private Camera()
        {
            capture = new VideoCapture(0);
            if (!capture.IsOpened()) { throw new Exception(); }
        }

        private Mat PrevFrame { get; set; }

        private VideoCapture capture { get; set; }

        public void GetFrame(int width, int height, out double[] buf)
        {
            buf = new double[width * height];
            var frame = new Mat();
            if (capture.Read(frame))
            {
                frame = frame.Resize(new Size(width, height));
                Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                unsafe
                {
                    byte* p = (byte*)frame.Data;
                    for (int i = 0; i < width * height; i++)
                    {
                        buf[i] = (double)p[i] / byte.MaxValue;
                    }
                }
                PrevFrame = frame.Clone();
            }
        }

        public void GetDiffFrame(int width, int height, out double[] buf)
        {
            buf = new double[width * height];
            var frame = new Mat();
            if (capture.Read(frame))
            {
                frame = frame.Resize(new Size(width, height));
                Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                if (PrevFrame != null)
                {
                    unsafe
                    {
                        byte* p = (byte*)frame.Data;
                        byte* pv = (byte*)PrevFrame.Data;
                        for (int i = 0; i < width * height; i++)
                        {
                            double diff = p[i] - pv[i];
                            if ((Math.Abs(diff) / byte.MaxValue) > 0.25)
                            {
                                buf[i] = 1;
                            }
                        }
                    }
                }
                if (PrevFrame == null)
                {
                    PrevFrame = frame.Clone();
                }
                else
                {
                    double weight = 0.25;
                    Cv2.AddWeighted(PrevFrame, weight, frame, 1 - weight, 0, PrevFrame);
                }
            }
        }
    }
}
