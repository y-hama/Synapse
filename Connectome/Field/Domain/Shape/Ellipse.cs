﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    class Ellipse : ShapeCore
    {

        public override double LocalMinArea
        {
            get
            {
                return Math.Min(Math.Min(AreaSize.X, AreaSize.Y), AreaSize.Z);
            }
        }

        private Location AreaSize { get; set; }

        public Ellipse(double width, double height, double depth)
        {
            AreaSize = new Location(width, height, depth);
        }
        public Ellipse(double size)
        {
            AreaSize = new Location(size, size, size);
        }

        public override Location.LocationCornerSet AreaCorner(Location center)
        {
            Location expand = new Location(AreaSize.X, AreaSize.Y, AreaSize.Z);
            return new Location.LocationCornerSet(center - expand, center + expand);
        }

        public override Location GetAlignmentLocation(int index)
        {
            throw new NotImplementedException();
        }

        public override bool CheckBorder(ref Location loc)
        {
            loc.X *= AreaSize.X;
            loc.Y *= AreaSize.Y;
            loc.Z *= AreaSize.Z;

            double x = loc.X / AreaSize.X;
            double y = loc.Y / AreaSize.Y;
            double z = loc.Z / AreaSize.Z;

            if (x * x + y * y + z * z < 1)
            {
                return true;
            }
            return false;
        }
    }
}
