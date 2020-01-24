﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Components;

namespace Connectome.Field.Domain.Transporter
{
    class SynapseConnection : TransporterDomain
    {
        public SynapseConnection(Location center, double areasize, int count, int connectcount, double defaultAxonLength = 0.1)
            : base(center, areasize, count, connectcount, defaultAxonLength)
        {

        }

        public override void InnerStep(
            ref RNdArray state, ref RNdArray value, ref RNdArray signal, ref RNdArray potential, ref RNdArray activity,
            ref RNdArray weight,
            ref RNdArray connectionCount, ref RNdArray connectionIndex, ref RNdArray connectionStartPosition)
        {
            for (int i0 = 0; i0 < Count; i0++)
            {
                int idx = (int)ID[i0];

                int start = (int)connectionStartPosition[idx];
                int count = (int)connectionCount[idx];

                int tid = -1;
                float ptlmax = 0, ptlmin = 100, ptldiff = 0, tptl = 0;
                float dp = 0;
                for (int i = 0; i < count; i++)
                {
                    tid = (int)connectionIndex[start + i];
                    dp += weight[start + i] * signal[tid];
                    tptl = potential[tid];
                    if (ptlmax < tptl) { ptlmax = tptl; }
                    if (ptlmin > tptl) { ptlmin = tptl; }
                }
                ptldiff = ptlmax - ptlmin;

                float ps = signal[idx];
                float pv = value[idx];
                if (state[idx] == 0)
                {
                    if (value[idx] > 0.5)
                    {
                        value[idx] = 0.5;
                        state[idx] = 1;
                    }
                    else
                    {
                        value[idx] += dp;
                        value[idx] *= 0.75;
                        float sum = 0;
                        if (ptldiff > 0)
                        {
                            for (int i = 0; i < count; i++)
                            {
                                tid = (int)connectionIndex[start + i];
                                weight[start + i] += (potential[tid] - ptlmin) / ptldiff;
                                sum += weight[start + i];
                            }
                            for (int i = 0; i < count; i++)
                            {
                                weight[start + i] /= sum;
                            }
                        }
                    }
                }
                else
                if (state[idx] == 1)
                {
                    if (value[idx] > 1)
                    {
                        value[idx] = 1;
                        state[idx] = 2;
                    }
                    else
                    {
                        value[idx] *= 1.1;
                    }
                }
                else
                if (state[idx] == 2)
                {
                    if (value[idx] < -0.25)
                    {
                        value[idx] = -0.25;
                        state[idx] = 3;
                    }
                    else
                    {
                        value[idx] *= 0.9;
                        value[idx] -= 0.05;
                    }
                }
                else
                if (state[idx] == 3)
                {
                    if (value[idx] >= 0)
                    {
                        value[idx] = 0;
                        state[idx] = 0;
                    }
                    else
                    {
                        value[idx] += 0.01;
                        value[idx] *= 0.9;
                    }
                }

                if (value[idx] > 0.5)
                { signal[idx] = 1; }
                else
                { signal[idx] = 0; }

                ps = (signal[idx] - ps);
                activity[idx] = 0.9 * activity[idx] + (1 - 0.9) * (ps > 0 ? ps : 0);
                pv = (value[idx] - pv);
                potential[idx] = 0.9 * potential[idx] + (1 - 0.9) * (pv > 0 ? pv : 0);
            }
        }
    }
}
