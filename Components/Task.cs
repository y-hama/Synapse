﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Tasks
{
    public delegate void ForStepFunction(int index);

    public static void ForStep(int start, int end, ForStepFunction func)
    {
        for (int i = start; i < end; i++) { func(i); }
    }

    public static void ForParallel(int start, int end, ForStepFunction func)
    {
        Parallel.For(start, end, i => { func(i); });
    }
}