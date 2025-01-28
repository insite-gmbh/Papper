using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using static Papper.PlcDataMapper;

namespace Papper.Access
{
    internal partial class AccessEngineSymbolic : AccessEngine
    {

        public AccessEngineSymbolic(ReadOperation? readEventHandler,
                                    WriteOperation? writeEventHandler,
                                    GetMapping? getMapping) : base(readEventHandler, writeEventHandler, getMapping)
        {
        }
        internal override Dictionary<Execution, DataPack> UpdateableItems(List<Execution> executions, bool onlyOutdated, Func<IEnumerable<string>, DateTime, bool>? forceUpdate = null)
        {
            return executions.Where(exec => !onlyOutdated ||
                                            exec.ValidationTimeMs <= 0 ||
                                            exec.PlcRawData.LastUpdate.AddMilliseconds(exec.ValidationTimeMs) < DateTime.Now ||
                                            (forceUpdate != null && forceUpdate(exec.PlcRawData.References.Keys, exec.PlcRawData.LastUpdate)))
                             .Select(x => x.CreateReadDataPack())
                             .ToDictionary(x => x.Key, x => x.Value);
        }


        internal override List<Execution> DetermineExecutions<T>(IEnumerable<T> vars)
        {
            return vars.GroupBy(x => x.Mapping)
                                .Select((execution) => GetOrAddMapping(execution.Key, out var entry)
                                                                ? (execution, entry)
                                                                : (null, null))
                                .Where(x => x.execution != null)
                                .SelectMany(x => x.entry.GetOperations(x.execution.Select(exec => exec.Variable).ToList()))
                                .ToList();
        }


        internal override bool GetOrAddMapping(string mapping, [MaybeNullWhen(false)] out IEntry entry)
        {
            if (_getMapping == null)
            {
                entry = null!;
                return false;
            }
            return _getMapping.Invoke(mapping, out entry, false);
        }


    }
}
