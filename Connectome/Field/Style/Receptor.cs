using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Style
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

            Tasks.ForStep(0, Domain.Count, i =>
            {
                int idx = (int)Domain.ID[i];
                CoreObjects.Infomation.Value[idx] = value[idx];
                CoreObjects.Infomation.Signal[idx] = signal[idx];

                var p = potential[idx];
                var a = activity[idx];
                Field.Domain.DomainCore.Calc_PotentialandActivity(1, 1, ref p, ref a);
                CoreObjects.Infomation.Potential[idx] = p;
                CoreObjects.Infomation.Activity[idx] = a;
            });
        }

    }
}
