using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain.Transporter
{
    class SynapseConnection : TransporterDomain
    {
        public SynapseConnection(Location center, double extent, int count, double axonLength)
            : base(center, extent, count, axonLength)
        {
        }
    }
}
