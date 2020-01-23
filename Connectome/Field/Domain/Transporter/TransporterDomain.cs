using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Transporter
{
    abstract class TransporterDomain : DomainCore
    {
        public TransporterDomain(Location center, double areasize, int count, int connectcount)
            : base(CellInfomation.CellType.Synapse, center, areasize, count, connectcount)
        {

        }

        public abstract void InnerStep(
            ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity,
            ref RNdArray weight,
            ref RNdArray connectionCount, ref RNdArray connectionIndex, ref RNdArray connectionStartPosition);
    }
}
