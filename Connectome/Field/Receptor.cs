using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field
{
    class Receptor : FieldCore
    {
        public Receptor(Domain.Sensor.SensorDomain domain)
            : base(domain)
        {

        }

        protected override void CurrentStep()
        {
            RNdArray value = new RNdArray(CoreObjects.Infomation.Value.Data);
            RNdArray signal = new RNdArray(CoreObjects.Infomation.Signal.Data);
            RNdArray potential = new RNdArray(CoreObjects.Infomation.Potential.Data);
            RNdArray activity = new RNdArray(CoreObjects.Infomation.Activity.Data);

            (Domain as Domain.Sensor.SensorDomain).InnerStep(ref value, ref signal, ref potential, ref activity);

            for (int i = 0; i < Domain.Count; i++)
            {
                int idx = (int)Domain.ID[i];
                CoreObjects.Infomation.Value[idx] = value[idx];
                CoreObjects.Infomation.Signal[idx] = signal[idx];
                CoreObjects.Infomation.Potential[idx] = potential[idx];
                CoreObjects.Infomation.Activity[idx] = activity[idx];
            }
        }

    }
}
