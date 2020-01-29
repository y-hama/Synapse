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
        public RandomPulser(Location center, Shape.ShapeCore shape, int count)
            : base(center, shape, count)
        {

        }
        bool check1 = false;
        bool check2 = false;
        int cnt = 0;
        public override void InnerStep(ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity)
        {
            for (int i = 0; i < Count; i++)
            {
                int idx = (int)ID[i];

                float ps = signal[idx];
                float pv = value[idx];
                if (cnt > 10) { signal[idx] = value[idx] = 1; }
                else { signal[idx] = value[idx] = 0; }
            }
            if (!check1 && cnt < 0) { cnt = random.Next(10, 20); }
            else { cnt--; }
        }
    }
}
