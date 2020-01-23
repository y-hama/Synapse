using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectomeVisualizer
{
    static class Visualization
    {
        public static double R { get; set; } = 2;
        public static double Th { get; set; } = 0;
        public static double Ph { get; set; } = 0;
        public static double Cx { get; set; } = 0;
        public static double Cy { get; set; } = 0;
        public static double Cz { get; set; } = 2;
        public static void CalcCameraPos()
        {
            CameraLocation.X = R * Math.Sin(-Th) * Math.Cos(-Ph);
            CameraLocation.Y = R * Math.Sin(-Th) * Math.Sin(-Ph);
            CameraLocation.Z = R * Math.Cos(-Th);
        }

        public static Connectome.Location CameraLocation { get; set; } = new Connectome.Location();
        public static Connectome.Location ViewLocation { get; set; } = new Connectome.Location();

        public static int ImageSize { get; set; }

        public static Forms.MainForm Form { get; set; }

        private static bool RequestSave { get; set; } = false;
        private static int SaveImageIndex { get; set; } = 0;

        public static bool RequestDrawCell { get; set; } = true;
        public static bool RequestDrawEdgeLine { get; set; } = false;

        public static void RequestImage(bool requestSave = false)
        {
            RequestSave = requestSave;
            Connectome.Core.RequestLatestState();
        }

        public static void Core_DataUpload(Connectome.Core.DataUploadEventArgs e)
        {
            var image = Connectome.Visualize.Imaging(e, Visualization.CameraLocation, Visualization.ViewLocation, Visualization.ImageSize, RequestDrawCell, RequestDrawEdgeLine);
            Form.SetImage(image);
            if (RequestSave)
            {
                int index = SaveImageIndex++;
                System.Drawing.Bitmap temporary = (System.Drawing.Bitmap)image.Bitmap.Clone();
                new System.Threading.Thread(() =>
                {
                    var dinfo = new System.IO.DirectoryInfo("img");
                    if (!dinfo.Exists) { dinfo.Create(); }
                    string name = index.ToString();
                    while (name.Length < 10)
                    {
                        name = "0" + name;
                    }
                    temporary.Save(dinfo.FullName + @"\f" + name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                }).Start();
            }
        }
    }
}
