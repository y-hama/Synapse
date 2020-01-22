using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field
{
    abstract class FieldCore
    {
        protected Domain.DomainCore Domain { get; set; }
        public int Count { get { return Domain.Count; } }

        public Location AreaMinState { get { return Domain.AreaCorner.AreaMinState; } }
        public Location AreaMaxState { get { return Domain.AreaCorner.AreaMaxState; } }

        public FieldCore(Domain.DomainCore domain)
        {
            Domain = domain;
        }
    }
}
