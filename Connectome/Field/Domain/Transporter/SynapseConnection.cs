using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Transporter
{
    class SynapseConnection : TransporterDomain
    {
        public SynapseConnection(Location center, double areasize, int count, int connectcount)
            : base(center, areasize, count, connectcount)
        {

        }

        public override void InnerStep(
            ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity,
            ref RNdArray weight,
            ref RNdArray connectionCount, ref RNdArray connectionIndex, ref RNdArray connectionStartPosition)
        {
            for (int i = 0; i < Count; i++)
            {
                int idx = (int)ID[i];
                value[idx] += 0.002;
                if (value[idx] > 1) { value[idx] = 0.5; }
            }
        }
    }
}
