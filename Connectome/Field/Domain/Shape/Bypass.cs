using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    class Bypass : ShapeCore
    {

        private Location Start { get; set; }
        private Location End { get; set; }
        private Location v { get; set; }
        private double Thickness { get; set; }

        public Bypass(Location start, Location end, double thickness)
        {
            Start = start; End = end; Thickness = thickness;
            v = (end - Start).Normalize();
        }

        public override Location.LocationCornerSet AreaCorner(Location center)
        {
            return new Location.LocationCornerSet(center + Start, center + End);
        }

        public override bool CheckBorder(ref Location loc)
        {
            loc.X = (End.X - Start.X) != 0 ? (End.X - Start.X) * (loc.X + 1) / 2 + Start.X : loc.X + Start.X;
            loc.Y = (End.Y - Start.Y) != 0 ? (End.Y - Start.Y) * (loc.Y + 1) / 2 + Start.Y : loc.Y + Start.Y;
            loc.Z = (End.Z - Start.Z) != 0 ? (End.Z - Start.Z) * (loc.Z + 1) / 2 + Start.Z : loc.Z + Start.Z;

            double ts = (v.X * (loc.X - End.X) + v.Y * (loc.Y - End.Y) + v.Z * (loc.Z - End.Z)) / (v.X * v.X + v.Y * v.Y + v.Z * v.Z);
            if (ts < Thickness) { return true; }
            return false;
        }
    }
}
