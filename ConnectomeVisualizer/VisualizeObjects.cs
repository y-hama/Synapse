using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectomeVisualizer
{
    static class VisualizeObjects
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
    }
}
