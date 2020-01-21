using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Sensor
{
    abstract class SensorDomain : DomainCore
    {
        public SensorDomain(Location center, double extent, int count)
            : base(CellInfomation.CellType.Sensor, center, extent, count)
        {
        }

        public abstract void DataReception();
        public abstract double CellData(double prevValue, int index = 0);

    }
}
