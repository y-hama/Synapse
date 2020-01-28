using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Shape
{
    abstract class ShapeCore
    {
        public abstract double LocalMinArea { get; }

        public abstract bool CheckBorder(ref Location loc);

        public abstract Location.LocationCornerSet AreaCorner(Location center);
    }
}
