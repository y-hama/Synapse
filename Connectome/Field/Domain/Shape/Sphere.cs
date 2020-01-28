using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    class Sphere : ShapeCore
    {

        private Location AreaSize { get; set; }

        public Sphere(double size)
        {
            AreaSize = new Location(size, 0, 0);
        }

        public override Location.LocationCornerSet AreaCorner(Location center)
        {
            Location expand = new Location(AreaSize.X, AreaSize.X, AreaSize.X);
            return new Location.LocationCornerSet(center - expand, center + expand);
        }

        public override bool CheckBorder(ref Location loc)
        {
            if (loc.DistanceTo(new Location()) < AreaSize.X)
            {
                return true;
            }
            return false;
        }
    }
}
