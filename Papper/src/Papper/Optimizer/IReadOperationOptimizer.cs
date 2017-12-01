﻿using System;
using System.Collections.Generic;
using Papper.Helper;
using Papper.Types;

namespace Papper.Optimizer
{
    internal interface IReadOperationOptimizer
    {
        IEnumerable<PlcRawData> CreateRawReadOperations(string selector, IEnumerable<KeyValuePair<string, Tuple<int, PlcObject>>> objects, int readDataBlockSize);
    }
}