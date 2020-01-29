using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Style
{
    class Area : FieldCore
    {
        public Area(Domain.Transporter.TransporterDomain domain)
            : base(domain)
        {

        }

        protected override void CurrentStep()
        {
            RNdArray state = new RNdArray(CoreObjects.Infomation.State.Data);
            RNdArray value = new RNdArray(CoreObjects.Infomation.Value.Data);
            RNdArray signal = new RNdArray(CoreObjects.Infomation.Signal.Data);
            RNdArray potential = new RNdArray(CoreObjects.Infomation.Potential.Data);
            RNdArray activity = new RNdArray(CoreObjects.Infomation.Activity.Data);
            RNdArray weight = new RNdArray(CoreObjects.Infomation.Weight.Data);
            RNdArray connectionCount = new RNdArray(CoreObjects.Infomation.ConnectionCount.Data);
            RNdArray connectionIndex = new RNdArray(CoreObjects.Infomation.ConnectionIndex.Data);
            RNdArray connectionStartPosition = new RNdArray(CoreObjects.Infomation.ConnectionStartPosition.Data);

            (Domain as Domain.Transporter.TransporterDomain).InnerStep
                (ref state, ref value, ref signal, ref potential, ref activity,
                 ref weight,
                 connectionCount, connectionStartPosition, connectionIndex);

            Tasks.ForStep(0, Domain.Count, i =>
            {
                int idx = (int)Domain.ID[i];
                CoreObjects.Infomation.State[idx] = state[idx];
                CoreObjects.Infomation.Value[idx] = value[idx];
                CoreObjects.Infomation.Signal[idx] = signal[idx];

                var p = potential[idx];
                var a = activity[idx];
                Field.Domain.DomainCore.Calc_PotentialandActivity(signal[idx], value[idx], ref p, ref a);
                CoreObjects.Infomation.Potential[idx] = p;
                CoreObjects.Infomation.Activity[idx] = a;

                int csidx = (int)connectionStartPosition[idx];
                int cnctcnt = (int)connectionCount[idx];
                Tasks.ForStep(0, cnctcnt, j =>
                {
                    CoreObjects.Infomation.Weight[csidx + j] = weight[csidx + j];
                });
            });
        }
    }
}
