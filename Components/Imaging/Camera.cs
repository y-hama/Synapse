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

        private const string VIDEOFILE_NAME = "_videoframe.mp4";

        private Mat PrevFrame { get; set; }
        private Mat PrevDiffFrame { get; set; }


        private VideoCapture capture { get; set; }

        public void SaveVideo(int TimeLength)
        {
            List<Mat> frames = new List<Mat>();
            var size = new Size(640, 480);

            var start = DateTime.Now;
            while ((DateTime.Now - start).TotalMilliseconds < TimeLength / 2)
            {
                var mat = new Mat();
                if (capture.Read(mat))
                {
                    mat = mat.Resize(size);
                    frames.Add(mat);
                    System.Threading.Thread.Sleep(1);
                }
            }

            var frame_rate = frames.Count;
            var fmt = VideoWriter.FourCC('m', 'p', '4', 'v');
            var writer = new VideoWriter(VIDEOFILE_NAME, fmt, frame_rate, size);
            for (int i = 0; i < frames.Count; i++)
            {
                writer.Write(frames[i].Clone());
            }
            for (int i = frames.Count - 1; i >= 0; i--)
            {
                writer.Write(frames[i].Clone());
            }
            writer.Release();
        }

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
            var diff = new Mat();
            if (capture.Read(frame))
            {
                frame = frame.Resize(new Size(width, height));
                Cv2.CvtColor(frame, frame, ColorConversionCodes.BGR2GRAY);
                if (PrevFrame != null)
                {
                    diff = frame.Clone();
                    Cv2.Absdiff(frame, PrevFrame, diff);
                    Cv2.Threshold(diff, diff, byte.MaxValue * 0.25, byte.MaxValue, ThresholdTypes.Binary);

                    Point[][] contours;
                    HierarchyIndex[] hierarchyIndexes;
                    Cv2.FindContours(diff.Clone(), out contours, out hierarchyIndexes, RetrievalModes.List, ContourApproximationModes.ApproxTC89KCOS);
                    if (contours.Length > 0)
                    {
                        var points = new List<Point>();
                        for (int idx_cnt = 0; idx_cnt < contours.GetLength(0); ++idx_cnt)
                        {
                            if (hierarchyIndexes[idx_cnt].Parent != -1) { continue; }
                            points.AddRange(contours[idx_cnt]);
                        }
                        Cv2.DrawContours(diff, new List<List<Point>>(new List<Point>[] { new List<Point>(Cv2.ConvexHull(points.ToArray())) }), 0, new Scalar(byte.MaxValue), -1);
                    }
                    var masked = diff.Clone();
                    frame.CopyTo(masked, diff);
                    Cv2.BitwiseOr(diff, PrevDiffFrame, diff);
                    Cv2.AddWeighted(masked, 0.5, diff, 0.5, 0, diff);

                    if (PrevFrame != null)
                    {
                        unsafe
                        {
                            byte* p = (byte*)frame.Data;
                            byte* pv = (byte*)diff.Data;
                            for (int i = 0; i < width * height; i++)
                            {
                                buf[i] = (double)pv[i] / byte.MaxValue;
                            }
                        }
                    }
                }
                if (PrevFrame == null)
                {
                    PrevFrame = frame.Clone();
                    PrevDiffFrame = new Mat(PrevFrame.Size(), PrevFrame.Type());
                }
                else
                {
                    double weight = 0.75;
                    Cv2.AddWeighted(PrevFrame, weight, frame, 1 - weight, 0, PrevFrame);
                    PrevDiffFrame = diff.Clone();
                }
            }
        }
    }
}
