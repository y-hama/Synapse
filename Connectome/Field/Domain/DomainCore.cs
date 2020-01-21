using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain
{
    abstract class DomainCore
    {
        protected static Random random { get; set; } = new Random();

        public Location AreaMinState { get; private set; }
        public Location AreaMaxState { get; private set; }

        public List<CellInfomation> Cells { get; protected set; } = new List<CellInfomation>();
        public Location Center { get; private set; }
        public int Count { get; private set; }
        public double AxonLength { get; private set; }

        public DomainCore(CellInfomation.CellType type, Location center, double extent, int count, double axsonLenth = 0)
        {
            Center = center;
            Count = count;
            AxonLength = axsonLenth;

            var list = new List<Location>();
            var check = new List<int>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new Location(random, extent));
                check.Add(0);
            }
            if (type == CellInfomation.CellType.Synapse)
            {
                while (check.Contains(0))
                {
                    int cnt = check.FindAll(x => x == 0).Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (check[i] == 0)
                        {
                            list[i] = new Location(random, extent);
                        }
                        check[i] = 0;
                        for (int j = 0; j < count; j++)
                        {
                            if (i == j) { continue; }
                            if (list[i].DistanceTo(list[j]) < axsonLenth)
                            {
                                check[i]++;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < count; i++)
            {
                Cells.Add(new Domain.CellInfomation(type, list[i] + center) { AxsonLength = axsonLenth });
            }
            var areasize = new Location(extent, extent, extent);
            AreaMinState = center - areasize;
            AreaMaxState = center + areasize;
        }
    }
}
