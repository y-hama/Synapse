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

        public DomainCore(CellInfomation.CellType type, Location center, double areasize, int count, int connectcount, double defaultAxonLength = 0.1)
        {
            Center = center;
            Count = count;
            List<Location> list = new List<Location>(new Location[count]);
            List<double> axonLength = new List<double>(new double[count]);
            List<int> cnnctcnt = new List<int>(new int[count]);
            for (int i = 0; i < count; i++)
            {
                list[i] = new Location(random, areasize);
            }
            if (type == CellInfomation.CellType.Synapse && count > 1)
            {
                for (int i = 0; i < count; i++)
                {
                    axonLength[i] = defaultAxonLength;
                    bool check = false;
                    while (!check)
                    {
                        cnnctcnt[i] = 0;
                        for (int j = 0; j < count; j++)
                        {
                            if (i == j) { continue; }
                            if (list[i].DistanceTo(list[j]) < axonLength[i])
                            {
                                cnnctcnt[i]++;
                            }
                        }
                        if (cnnctcnt[i] > connectcount)
                        {
                            check = true;
                        }
                        else { axonLength[i] += defaultAxonLength / 10; }
                    }
                }
            }
            for (int i = 0; i < count; i++)
            {
                var cell = new CellInfomation(type, list[i] + center) { AxsonLength = axonLength[i] };
                _ID.Add(cell.ID);
                CoreObjects.Cells.Add(cell);
            }
            ID = new RNdArray(_ID.ToArray());

            Location expand = new Location(areasize, areasize, areasize);
            AreaCorner = new Location.LocationCornerSet(center - expand, center + expand);
        }
    }
}
