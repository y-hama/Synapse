using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    class Bypass : ShapeCore
    {
        public override double LocalMinArea
        {
            get
            {
                return Thickness;
            }
        }

        private int CollectionCount { get; set; } = 0;

        private Location Start { get; set; }
        private Location Center { get; set; }
        private Location End { get; set; }
        private Location v { get; set; }
        private double Thickness { get; set; }

        public Bypass(Location end, double thickness)
        {
            Start = new Location(); End = end; Thickness = thickness;
            Center = (Start + End) / 2;
            v = (End - Start).Normalize();
        }

        public override Location.LocationCornerSet AreaCorner(Location center)
        {
            return new Location.LocationCornerSet(center + Start, center + End);
        }

        private void RandomToArea(Location random, out Location area)
        {
            area = new Location();
            double areamin, areamax;

            areamin = Math.Min(Start.X, End.X);
            areamax = Math.Max(Start.X, End.X);
            if (Math.Abs(areamin * areamin - areamax * areamax) < 1e-9)
            {
                area.X = Thickness * random.X;
            }
            else
            {
                area.X = ((random.X + 1) / 2) * (areamax - areamin) + areamin;
            }
            areamin = Math.Min(Start.Y, End.Y);
            areamax = Math.Max(Start.Y, End.Y);
            if (Math.Abs(areamin * areamin - areamax * areamax) < 1e-9)
            {
                area.Y = Thickness * random.Y;
            }
            else
            {
                area.Y = ((random.Y + 1) / 2) * (areamax - areamin) + areamin;
            }
            areamin = Math.Min(Start.Z, End.Z);
            areamax = Math.Max(Start.Z, End.Z);
            if (Math.Abs(areamin * areamin - areamax * areamax) < 1e-9)
            {
                area.Z = Thickness * random.Z;
            }
            else
            {
                area.Z = ((random.Z + 1) / 2) * (areamax - areamin) + areamin;
            }
        }

        public override Location GetAlignmentLocation(int index)
        {
            throw new NotImplementedException();
        }

        public override bool CheckBorder(ref Location loc)
        {
            if (CollectionCount == 0)
            {
                loc = Start;
                CollectionCount++;
                return true;
            }
            else if (CollectionCount == 1)
            {
                loc = End;
                CollectionCount++;
                return true;
            }
            else
            {
                var _loc = loc;
                RandomToArea(loc, out _loc);
                loc = _loc;

                double t = (Start + loc) * v;
                Location H = Start + t * v;
                double dist = H.DistanceTo(loc);
                if (dist < Thickness)
                { CollectionCount++; return true; }
                else
                { return false; }
            }
        }
    }
}
