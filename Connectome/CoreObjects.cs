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

        public static Location.LocationCornerSet AreaCorner { get; set; }

        public static double AxsonConnectionAverage { get; set; }
        public static double AxsonConnectionMax { get; set; }
        public static double AxsonConnectionMin { get; set; }

        public static List<Field.FieldCore> Fields { get; set; } = new List<Field.FieldCore>();
        public static List<Field.Domain.CellInfomation> Cells { get; set; } = new List<Field.Domain.CellInfomation>();

        public static class Infomation
        {

        }
    }
}
