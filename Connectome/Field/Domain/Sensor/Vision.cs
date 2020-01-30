using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Sensor
{
    class Vision : SensorDomain
    {
        private int Width { get; set; }
        private int Height { get; set; }

        public Vision(Location center, int width, int height)
            : base(center, new Shape.Rectangular(width, height, 0), width * height, true)
        {
            Width = width; Height = height;
        }

        public override void InnerStep(ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity)
        {
            double[] buf = new double[Width * Height];
            Components.Imaging.Camera.Instance.GetFrame(Width, Height, out buf);
            for (int i = 0; i < Count; i++)
            {
                int idx = (int)ID[i];
                value[idx] = signal[idx] = buf[i];
            }
        }
    }
}
