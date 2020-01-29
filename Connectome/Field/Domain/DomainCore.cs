using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain
{
    abstract class DomainCore
    {
        protected static Random random { get; set; } = new Random();

        public Location Center { get; private set; }

        private List<int> _ID { get; set; } = new List<int>();
        public RNdArray ID { get; private set; }

        public int Count { get; private set; }

        public Location.LocationCornerSet AreaCorner { get; private set; }

        public DomainCore(CellInfomation.CellType type, Location center, Shape.ShapeCore shape, int count, int connectcount, double defaultAxonLength = 0)
        {
            Center = center;
            Count = count;
            List<Location> list = new List<Location>(new Location[count]);
            List<double> axonLength = new List<double>(new double[count]);
            List<int> cnnctcnt = new List<int>(new int[count]);
            Tasks.ForStep(0, count, i =>
            {
                list[i] = new Location(random, shape.CheckBorder);
            });
            if (type == CellInfomation.CellType.Synapse)
            {
                int cnnctcnttarget = Math.Min(count / 2, connectcount);
                if (count > 1)
                {
                    Tasks.ForParallel(0, count, i =>
                    {
                        axonLength[i] = defaultAxonLength;
                        bool check = false;
                        double nearestdisttmp = 0, dist = 0;
                        while (!check)
                        {
                            double nearestdist = double.MaxValue;
                            cnnctcnt[i] = 0;
                            Tasks.ForStep(0, count, j =>
                            {
                                if (i == j) { return; }
                                dist = list[i].DistanceTo(list[j]);
                                if (dist < axonLength[i])
                                {
                                    cnnctcnt[i]++;
                                }
                                if (nearestdisttmp < dist && nearestdist > dist)
                                {
                                    nearestdist = dist;
                                }
                            });
                            nearestdisttmp = nearestdist;
                            if (cnnctcnt[i] >= cnnctcnttarget)
                            {
                                check = true;
                            }
                            else
                            {
                                axonLength[i] = nearestdist;
                            }
                        }
                    });
                }
                else
                {
                    list[0] = center;
                    Tasks.ForStep(0, count, i =>
                    {
                        axonLength[i] = (defaultAxonLength + shape.LocalMinArea) / 2;
                    });
                }
            }
            Tasks.ForStep(0, count, i =>
            {
                var cell = new CellInfomation(type, list[i] + center) { AxsonLength = axonLength[i] };
                _ID.Add(cell.ID);
                CoreObjects.Cells.Add(cell);
            });
            ID = new RNdArray(_ID.ToArray());

            AreaCorner = shape.AreaCorner(center);
        }

        public static void Calc_PotentialandActivity(Real signal, Real value, ref Real potential, ref Real activity)
        {
            double offsetm = 0.01;
            double adda = 0;
            if (signal > 0.5) { adda = 1; }
            activity += adda - offsetm;
            if (activity < 0) { activity = 0; } else if (activity > 1) { activity = 1; }

            double rho = 0.5;
            potential = rho * potential + (1 - rho) * value;
        }
    }
}
