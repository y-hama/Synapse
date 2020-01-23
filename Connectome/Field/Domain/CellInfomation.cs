using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connectome.Field.Domain
{
    public class CellInfomation
    {
        public enum IgnitionState
        {
            Stable,
            Ignition,
            Overshoot,
            Cooling,
        }

        public enum CellType
        {
            Synapse,
            Sensor,
        }

        public new string ToString()
        {
            return string.Format("id{0,5}, v:{1,10}, s:{2,10}, p:{3,10}, a:{4,10}, st:{5,10}, {6}",
                ID,
                Math.Round(Value, 6),
                Math.Round(Signal, 6),
                Math.Round(Potential, 6),
                Math.Round(Activity, 6),
                State, Location.ToString());
        }

        private static int cellidseed { get; set; } = 0;
        public int ID { get; private set; }

        public double Value { get; set; }
        public double Signal { get; set; }
        public double Potential { get; set; }
        public double Activity { get; set; }
        private IgnitionState state { get; set; }
        public IgnitionState State
        {
            get
            {
                if (Type == CellType.Synapse)
                {
                    return state;
                }
                else { return IgnitionState.Ignition; }
            }
            set { state = state; }
        }

        public CellType Type { get; private set; }

        public Location Location { get; private set; }
        public Location LocalLocation { get; set; }
        public double AxsonLength { get; set; }

        public List<CellInfomation> ConnectedCells { get; set; } = new List<CellInfomation>();

        public CellInfomation(CellType type, Location location)
        {
            ID = cellidseed;
            cellidseed++;
            Type = type; Location = location;
        }

        public CellInfomation Clone()
        {
            return new CellInfomation(Type, Location)
            {
                ID = ID,
                state = state,
                Value = Value,
                Signal = Signal,
                Potential = Potential,
                Activity = Activity,
                AxsonLength = AxsonLength,
                ConnectedCells = new List<Domain.CellInfomation>(ConnectedCells.ToArray()),
            };
        }
    }
}
