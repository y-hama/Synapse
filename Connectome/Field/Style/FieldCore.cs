using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Style
{
    abstract class FieldCore
    {
        protected Domain.DomainCore Domain { get; set; }
        public int Count { get { return Domain.Count; } }

        public int StepOver { get; private set; } = 0;

        public Location AreaMinState { get { return Domain.AreaCorner.AreaMinState; } }
        public Location AreaMaxState { get { return Domain.AreaCorner.AreaMaxState; } }



        public FieldCore(Domain.DomainCore domain)
        {
            Domain = domain;
        }

        public void Start()
        {
            new System.Threading.Thread(() =>
            {
                while (!CoreObjects.IsTerminated)
                {
                    CurrentStep();
                    StepOver++;
                    if (this is Area)
                    {
                        while (!CanNextStep() && !CoreObjects.IsTerminated) { System.Threading.Thread.Sleep(1); }
                    }
                    System.Threading.Thread.Sleep(CoreObjects.TimeScale);
                }
            }).Start();
        }

        private bool CanNextStep()
        {
            bool check = false;
            var list = CoreObjects.Fields.FindAll(x => x is Area).Select(x => x.StepOver).Distinct().ToArray();

            if (list.Length == 1)
            {
                if (list[0] == this.StepOver)
                {
                    check = true;
                    if (this.StepOver > 1e+10)
                    {
                        foreach (var item in CoreObjects.Fields)
                        {
                            item.StepOver = 0;
                        }
                    }
                }
            }
            else
            {
                if (list.Max() > this.StepOver) { check = true; }
            }

            return check;
        }

        protected abstract void CurrentStep();
    }
}
