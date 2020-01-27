using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome
{
    static class CoreObjects
    {
        public static bool IsTerminated { get; set; } = false;
        public static int TimeScale { get; set; } = 0;

        public static int LatestSequenceIndex { get; set; } = 0;
        public static int MaxStepOverCount { get { return Fields.Select(x => x.StepOver).Max(); } }
        public static DateTime LatestSequenceTime { get; set; } = DateTime.Now;
        public static double FPS { get; set; } = 0;

        public static Location.LocationCornerSet AreaCorner { get; set; }

        public static double AxsonConnectionAverage { get; set; }
        public static double AxsonConnectionMax { get; set; }
        public static double AxsonConnectionMin { get; set; }

        public static List<Field.Style.FieldCore> Fields { get; set; } = new List<Field.Style.FieldCore>();

        public static List<Field.Domain.CellInfomation> Cells { get; set; } = new List<Field.Domain.CellInfomation>();
        public static int Count { get; set; }

        public static class Infomation
        {
            public static Components.RNdArray State { get; set; }

            public static Components.RNdArray Value { get; set; }
            public static Components.RNdArray Signal { get; set; }
            public static Components.RNdArray Potential { get; set; }
            public static Components.RNdArray Activity { get; set; }

            public static Components.RNdArray Weight { get; set; }
            public static Components.RNdArray ConnectionCount { get; set; }
            public static Components.RNdArray ConnectionIndex { get; set; }
            public static Components.RNdArray ConnectionStartPosition { get; set; }
        }
    }
}
