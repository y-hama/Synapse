using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field
{
    class Area : FieldCore
    {
        public Area(Domain.Transporter.TransporterDomain domain)
            : base(domain)
        {

        }

        protected override void CurrentStep()
        {
            RNdArray value = new RNdArray(CoreObjects.Infomation.Value.Data);
            RNdArray signal = new RNdArray(CoreObjects.Infomation.Signal.Data);
            RNdArray potential = new RNdArray(CoreObjects.Infomation.Potential.Data);
            RNdArray activity = new RNdArray(CoreObjects.Infomation.Activity.Data);
            RNdArray weight = new RNdArray(CoreObjects.Infomation.Weight.Data);
            RNdArray connectionCount = new RNdArray(CoreObjects.Infomation.ConnectionCount.Data);
            RNdArray connectionIndex = new RNdArray(CoreObjects.Infomation.ConnectionIndex.Data);
            RNdArray connectionStartPosition = new RNdArray(CoreObjects.Infomation.ConnectionStartPosition.Data);

            (Domain as Domain.Transporter.TransporterDomain).InnerStep
                (ref value, ref signal, ref potential, ref activity,
                 ref weight,
                 ref connectionCount, ref connectionIndex, ref connectionStartPosition);

            for (int i = 0; i < Domain.Count; i++)
            {
                int idx = (int)Domain.ID[i];
                CoreObjects.Infomation.Value[idx] = value[idx];
                CoreObjects.Infomation.Signal[idx] = signal[idx];
                CoreObjects.Infomation.Potential[idx] = potential[idx];
                CoreObjects.Infomation.Activity[idx] = activity[idx];
                //int csidx = (int)connectionStartPosition[idx];
                //int cnctcnt = (int)connectionCount[idx];
                //for (int j = 0; j < cnctcnt; j++)
                //{
                //    CoreObjects.Infomation.Weight[csidx + j] = weight[csidx + j];
                //}
            }
        }
    }
}
