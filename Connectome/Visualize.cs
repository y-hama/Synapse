using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace Connectome
{
    public static class Visualize
    {
        public class Image
        {
            public Bitmap Bitmap { get; private set; }
            public TimeSpan CreateTimeSpan { get; private set; }

            public TimeSpan ProcessTimeSpan { get; set; }
            public int ProcessFrameCount { get; set; }
            public double FPS { get; set; }

            public Image(Bitmap bitmap, TimeSpan timespan)
            {
                Bitmap = bitmap;
                CreateTimeSpan = timespan;
            }
        }

        public static Image Imaging(Core.DataUploadEventArgs e, Location camera, Location view, int size, bool drawCell = true, bool drawConnectEdge = false)
        {
            DateTime start = DateTime.Now;
            Location.SetWorldMatrix(camera, view);
            double areawidth = Math.Max(1, Math.Max(Math.Abs(e.AreaCorner.Min), Math.Abs(e.AreaCorner.Max)));

            Bitmap bitmap = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, size, size));

            if (e.Infomations.Count > 0)
            {
                foreach (var item in e.Infomations)
                {
                    item.LocalLocation = Location.GetConvertedLocation(item.Location);
                }
                //Parallel.ForEach(e.Infomations, new ParallelOptions()
                //{
                //    MaxDegreeOfParallelism = 1,
                //}, item =>
                //{
                //    item.LocalLocation = Location.GetConvertedLocation(item.Location);
                //});
                e.Infomations.Sort((x, y) =>
                {
                    if (x.LocalLocation.Z > y.LocalLocation.Z)
                    {
                        return -1;
                    }
                    else
                    if (x.LocalLocation.Z < y.LocalLocation.Z)
                    {
                        return 1;
                    }
                    else
                    {
                        return 0;
                    }
                });
                List<double> z_order = new List<double>(e.Infomations.Select(x => x.LocalLocation.Z).ToArray());
                double near = z_order.Min();
                double far = z_order.Max();
                double areasize = far == near ? 1 : far - near;

                double sizeoder = Math.Max(10, size / 50), sizemin = 0.75;

                Pen linePen = new Pen(Color.FromArgb(128, 128, 128));
                foreach (var cell in e.Infomations)
                {
                    float x = (float)(size * (cell.LocalLocation.X + areawidth) / (2 * areawidth));
                    float y = (float)(size * (cell.LocalLocation.Y + areawidth) / (2 * areawidth));

                    double itemorder = (cell.LocalLocation.Z - near) / (areasize);
                    double value = (Math.Max(0, Math.Min(1, cell.Value)));
                    double signal = (Math.Max(0, Math.Min(1, cell.Signal)));
                    double zodr = (1 - itemorder);
                    byte alpha = (byte)(zodr * byte.MaxValue);

                    double oparat = 1;
                    if (drawConnectEdge)
                    {
                        double alpx = 0.9;
                        oparat = 0.3;
                        if (cell.Type == Field.Domain.CellInfomation.CellType.Synapse)
                        {
                            double max = cell.ConnectionWeight.Max();
                            double min = cell.ConnectionWeight.Min();
                            double sum = cell.ConnectionWeight.Sum();
                            double delta = max - min;
                            double alp = 0;
                            for (int i = 0; i < cell.ConnectedCells.Count; i++)
                            {
                                int id = cell.ConnectedCells[i].ID;
                                var edgec = e.Infomations.Find(n => n.ID == id);
                                float xx = (float)(size * (edgec.LocalLocation.X + areawidth) / (2 * areawidth));
                                float yy = (float)(size * (edgec.LocalLocation.Y + areawidth) / (2 * areawidth));
                                if (delta == 0) { alp = (alpx * cell.ConnectionWeight[i] + (1 - alpx)); }
                                else { alp = ((alpx * (cell.ConnectionWeight[i]) / delta) + (1 - alpx)); }
                                PointF s = new PointF(xx, yy), t = new PointF(x, y);
                                if (s != t)
                                {
                                    LinearGradientBrush gb = new LinearGradientBrush(s, t, Color.FromArgb((byte)(alpha * alp), Color.DarkGreen), Color.FromArgb((byte)(alpha * alp), Color.DarkMagenta));
                                    g.DrawLine(new Pen(gb), s, t);
                                }
                            }
                        }
                    }
                    if (drawCell)
                    {
                        alpha = (byte)(alpha * oparat);
                        float elemsize = (float)(sizeoder * ((1 - sizemin) * zodr + sizemin));
                        RectangleF rect = new RectangleF(x - elemsize / 2, y - elemsize / 2, elemsize, elemsize);
                        byte valuebrightness = (byte)(byte.MaxValue * value);
                        byte sigbrightness = (byte)(byte.MaxValue * signal);
                        Color c = Color.FromArgb(255, Color.Black);
                        switch (cell.Type)
                        {
                            case Field.Domain.CellInfomation.CellType.Synapse:
                                c = Color.FromArgb(valuebrightness / 4, valuebrightness / 4, valuebrightness);
                                break;
                            case Field.Domain.CellInfomation.CellType.Sensor:
                                c = Color.FromArgb(valuebrightness, 0, valuebrightness / 2);
                                break;
                            default:
                                break;
                        }
                        g.FillEllipse(new SolidBrush(Color.FromArgb(alpha, c)), rect);
                        g.DrawEllipse(new Pen(Color.FromArgb(alpha, Color.FromArgb(sigbrightness, 64, 64 + sigbrightness / 4))), rect);
                    }
                }
            }

            return new Image(bitmap, (DateTime.Now - start))
            {
                ProcessFrameCount = e.ProcessFrame,
                ProcessTimeSpan = e.ProcessTime,
                FPS = e.FPS,
            };
        }
    }
}
