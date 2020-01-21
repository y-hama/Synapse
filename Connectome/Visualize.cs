using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Connectome
{
    public static class Visualize
    {

        public static Bitmap Imaging(Core.DataUploadEventArgs e, Location camera, Location view, int size)
        {
            Location.SetWorldMatrix(camera, view);
            double areawidth = Math.Sqrt(2) * Math.Max(1, Math.Max(Math.Abs(e.AreaCorner.Min), Math.Abs(e.AreaCorner.Max)));

            Bitmap bitmap = new Bitmap(size, size);
            Graphics g = Graphics.FromImage(bitmap);
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, size, size));

            if (e.Infomations.Count > 0)
            {
                foreach (var item in e.Infomations)
                {
                    item.LocalLocation = Location.GetConvertedLocation(item.Location);
                }
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

                double sizeoder = Math.Max(5, size / 100), sizemin = 0.5;
                foreach (var cell in e.Infomations)
                {
                    float x = (float)(size * (cell.LocalLocation.X + areawidth) / (2 * areawidth));
                    float y = (float)(size * (cell.LocalLocation.Y + areawidth) / (2 * areawidth));

                    double itemorder = (cell.LocalLocation.Z - near) / (areasize);
                    double signal = (Math.Max(0, Math.Min(1, cell.Value)));
                    double zodr = (1 - itemorder);

                    float elemsize = (float)(sizeoder * ((1 - sizemin) * zodr + sizemin));
                    RectangleF rect = new RectangleF(x - elemsize / 2, y - elemsize / 2, elemsize, elemsize);
                    byte brightness = (byte)(byte.MaxValue * signal);
                    Color c = Color.Red;
                    switch (cell.Type)
                    {
                        case Field.Domain.CellInfomation.CellType.Synapse:
                            c = Color.FromArgb(0, 0, brightness);
                            break;
                        case Field.Domain.CellInfomation.CellType.Sensor:
                            c = Color.FromArgb(brightness, 0, brightness);
                            break;
                        default:
                            break;
                    }
                    g.FillEllipse(new SolidBrush(c), rect);
                    g.DrawEllipse(Pens.DimGray, rect);
                }
            }
            return bitmap;
        }
    }
}
