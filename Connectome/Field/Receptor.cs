using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field
{
    class Receptor : FieldCore
    {
        protected new Domain.Sensor.SensorDomain Domain
        {
            get { return base.Domain as Domain.Sensor.SensorDomain; }
        }

        public Receptor(Domain.Sensor.SensorDomain domain)
            : base(domain)
        {

        }

        protected override void ConnectionConfirmed()
        {

        }

        protected override void CalculationStep()
        {
            Domain.DataReception();
            foreach (var item in Domain.Cells)
            {
                item.Value = Domain.CellData(item.Value);
            }
            System.Threading.Thread.Sleep(10);
        }
    }
}
