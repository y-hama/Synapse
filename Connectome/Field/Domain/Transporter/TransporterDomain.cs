using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Transporter
{
    abstract class TransporterDomain : DomainCore
    {
        public TransporterDomain(Location center, double areasize, int count, double axonLength) 
            : base(CellInfomation.CellType.Synapse, center, areasize, count, axonLength)
        {

        }
    }
}
