using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    class Rectangular : ShapeCore
    {
        public override double LocalMinArea
        {
            get
            {
                return Math.Min(Math.Min(AreaSize.X, AreaSize.Y), AreaSize.Z);
            }
        }

        private Location AreaSize { get; set; }

        public Rectangular(double width, double height, double depth)
        {
            AreaSize = new Location(width, height, depth);
        }

        public override Location.LocationCornerSet AreaCorner(Location center)
        {
            Location expand = new Location(AreaSize.X, AreaSize.Y, AreaSize.Z);
            return new Location.LocationCornerSet(center - expand, center + expand);
        }

        public override Location GetAlignmentLocation(int index)
        {
            int j = (int)(index / AreaSize.X);
            int i = (int)(index - j * AreaSize.X);
            return new Location(i - AreaSize.X / 2, j - AreaSize.Y / 2, AreaSize.Z);
        }

        public override bool CheckBorder(ref Location loc)
        {
            loc.X *= AreaSize.X / 2;
            loc.Y *= AreaSize.Y / 2;
            loc.Z *= AreaSize.Z / 2;
            if (Math.Abs(loc.X) < Math.Abs(AreaSize.X)
             && Math.Abs(loc.Y) < Math.Abs(AreaSize.Y)
             && Math.Abs(loc.Z) < Math.Abs(AreaSize.Z))
            {
                return true;
            }
            return false;
        }
    }
}
