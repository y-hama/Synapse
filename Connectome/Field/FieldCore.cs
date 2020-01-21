using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field
{
    abstract class FieldCore
    {
        public Domain.DomainCore Domain { get; protected set; }

        public FieldCore(Domain.DomainCore domain)
        {
            Domain = domain;
        }

        public void CreateConnection(List<FieldCore> fields)
        {
            foreach (var cell in Domain.Cells.FindAll(x => x.Type == Field.Domain.CellInfomation.CellType.Synapse))
            {
                foreach (var item in Domain.Cells.FindAll(x => x != cell))
                {
                    if (cell.Location.DistanceTo(item.Location) < cell.AxsonLength)
                    {
                        cell.ConnectedCells.Add(item);
                    }
                }
                foreach (var field in fields)
                {
                    foreach (var item in field.Domain.Cells)
                    {
                        if (cell.Location.DistanceTo(item.Location) < cell.AxsonLength)
                        {
                            cell.ConnectedCells.Add(item);
                        }
                    }
                }
            }

            ConnectionConfirmed();
        }

        protected abstract void ConnectionConfirmed();

        public void Start()
        {
            new System.Threading.Thread(() => { while (!CoreObjects.IsTerminated) { CalculationStep(); } }).Start();
        }

        protected abstract void CalculationStep();
    }
}
