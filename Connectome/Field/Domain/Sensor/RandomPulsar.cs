using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Sensor
{
    class RandomPulsar : SensorDomain
    {
        public RandomPulsar(Location center, double extent, int count)
            : base(center, extent, count)
        {
        }

        public override void DataReception()
        {
        }

        public override double CellData(double prevValue, int index)
        {
            return (0.5) * prevValue + (1 - 0.5) * random.NextDouble();
        }
    }
}
