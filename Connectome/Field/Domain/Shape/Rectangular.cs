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

        public override bool CheckBorder(ref Location loc)
        {
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
