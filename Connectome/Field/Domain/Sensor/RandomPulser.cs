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
                if (!check1)
                {
                    signal[idx] = value[idx] = 0.5 * random.NextDouble();
                }
                else
                {
                    signal[idx] = value[idx] = 0;//random.NextDouble();
                }

                ps = (signal[idx] - ps);
                activity[idx] = 0.9 * activity[idx] + (1 - 0.9) * (ps > 0 ? ps : 0);
                pv = (value[idx] - pv);
                potential[idx] = 0.9 * potential[idx] + (1 - 0.9) * (pv > 0 ? pv : 0);
            }
            if (!check1 && cnt > 10) { check1 = true; }
            else { cnt++; }
        }
    }
}
