using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Sensor
{
    class RandomPulser : SensorDomain
    {
        public RandomPulser(Location center, double areasize, int count)
            : base(center, areasize, count)
        {

        }

        public override void InnerStep(ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity)
        {
            for (int i = 0; i < Count; i++)
            {
                int idx = (int)ID[i];
                value[idx] = random.NextDouble();
            }
        }
    }
}
