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

        public Location Center { get; private set; }

        public List<int> ID { get; set; } = new List<int>();
        public int Count { get; private set; }

        public Location.LocationCornerSet AreaCorner { get; private set; }

        public DomainCore(CellInfomation.CellType type, Location center, double areasize, int count, double axonLength)
        {
            Center = center;
            Count = count;
            List<Location> list = new List<Location>(new Location[count]);
            for (int i = 0; i < count; i++)
            {
                list[i] = new Location(random, areasize);
            }
            if (type == CellInfomation.CellType.Synapse && count > 1)
            {
                List<bool> check = new List<bool>(new bool[count]);
                int cnt = check.FindAll(x => x == false).Count;
                while (check.Contains(false))
                {
                    for (int i = 0; i < count; i++)
                    {
                        if (!check[i])
                        {
                            list[i] = new Location(random, areasize);
                        }
                        check[i] = false;
                        for (int j = 0; j < count; j++)
                        {
                            if (i == j) { continue; }
                            if (list[i].DistanceTo(list[j]) < axonLength * areasize)
                            {
                                check[i] = true;
                                break;
                            }
                        }
                    }
                    cnt = check.FindAll(x => x == false).Count;
                }
            }
            for (int i = 0; i < count; i++)
            {
                var cell = new CellInfomation(type, list[i] + center);
                ID.Add(cell.ID);
                CoreObjects.Cells.Add(cell);
            }

            Location expand = new Location(areasize, areasize, areasize);
            AreaCorner = new Location.LocationCornerSet(center - expand, center + expand);
        }
    }
}
