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
        RNdArray Value { get; set; }
        RNdArray Signal { get; set; }
        RNdArray Potential { get; set; }
        RNdArray Activity { get; set; }
        RNdArray State { get; set; }

        RNdArray Weight { get; set; }

        RNdArray IndexPosition { get; set; }


        public Area(Domain.Transporter.TransporterDomain domain)
            : base(domain)
        {
            Potential = new RNdArray(domain.Count);
            Activity = new RNdArray(domain.Count);
            State = new RNdArray(domain.Count);
        }

        protected override void ConnectionConfirmed()
        {
            Value = new RNdArray(CoreObjects.Cells.Count);
            Signal = new RNdArray(CoreObjects.Cells.Count);
            int pos = 0;
            IndexPosition = new RNdArray(Domain.Count);
            for (int i = 0; i < Domain.Count; i++)
            {
                IndexPosition[i] = pos;
                pos += Domain.Cells[i].ConnectedCells.Count;
            }
        }

        protected override void CalculationStep()
        {
            Value.CopyBy(CoreObjects.Cells.Select(x => x.Value).ToArray());
            Signal.CopyBy(CoreObjects.Cells.Select(x => x.Signal).ToArray());
            Potential.CopyBy(Domain.Cells.Select(x => x.Potential).ToArray());
            Activity.CopyBy(Domain.Cells.Select(x => x.Activity).ToArray());
            State.CopyBy(Domain.Cells.Select(x => (double)x.State).ToArray());



            for (int i = 0; i < Domain.Count; i++)
            {
                Domain.Cells[i].Value = Signal[i];
                Domain.Cells[i].Signal = Signal[i];
                Domain.Cells[i].Potential = Potential[i];
                Domain.Cells[i].Activity = Activity[i];
                Domain.Cells[i].State = (Domain.CellInfomation.IgnitionState)Enum.ToObject(typeof(Domain.CellInfomation.IgnitionState), (int)State[i]);
            }

            System.Threading.Thread.Sleep(0);
        }
    }
}
