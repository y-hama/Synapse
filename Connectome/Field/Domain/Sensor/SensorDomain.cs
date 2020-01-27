using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Sensor
{
    abstract class SensorDomain : DomainCore
    {
        public SensorDomain(Location center, Shape.ShapeCore shape, int count)
            : base(CellInfomation.CellType.Sensor, center, shape, count, 0)
        {

        }

        public abstract void InnerStep(
            ref RNdArray value,
            ref RNdArray signal,
            ref RNdArray potential,
            ref RNdArray activity);
    }
}
