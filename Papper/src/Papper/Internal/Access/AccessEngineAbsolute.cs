using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;

namespace Papper.Access
{
    internal partial class AccessEngineAbsolute(PlcDataMapper.ReadOperation? readEventHandler,
                                                PlcDataMapper.WriteOperation? writeEventHandler,
                                                AccessEngine.GetMapping? getMapping) : AccessEngine(readEventHandler, writeEventHandler, getMapping)
    {
        internal override Dictionary<Execution, DataPack> UpdateableItems(List<Execution> executions, bool onlyOutdated, Func<IEnumerable<string>, DateTime, bool>? forceUpdate = null)
        {
            return executions.Where(exec => !onlyOutdated ||
                                            exec.ValidationTimeMs <= 0 ||
                                            exec.PlcRawData.LastUpdate.AddMilliseconds(exec.ValidationTimeMs) < DateTime.Now ||
                                            (forceUpdate != null && forceUpdate(exec.PlcRawData.References.Keys, exec.PlcRawData.LastUpdate)))
                             .Select(x => x.CreateReadDataPack())
                             .ToDictionary(x => x.Key, x => x.Value);
        }

        internal override List<Execution> DetermineExecutions<T>(IEnumerable<T> plcReferences)
        {
            return [.. plcReferences.GroupBy(plcReference => plcReference.Mapping)
                                .Select((plcReferenceGroup) => GetOrAddMapping(plcReferenceGroup.Key, out IEntry? entry)
                                                                ? (plcReferenceGroup, entry)
                                                                : (null, null))
                                .Where(x => x.plcReferenceGroup != null)
                                .SelectMany(x => x.entry.GetOperations([.. x.plcReferenceGroup.Select(plcReference => plcReference.Variable)]))];
        }

        internal override bool GetOrAddMapping(string mapping, [MaybeNullWhen(false)] out IEntry entry)
        {
            if (_getMapping == null)
            {
                entry = null!;
                return false;
            }
            bool isAbsoluteMapping = false;

            // If the mapping is an absolute mapping we can create an entry
            switch (mapping)
            {
                case "IB":
                case "FB":
                case "QB":
                case "TM":
                case "CT":
                case var s when Regex.IsMatch(s, "^DB\\d+$"):
                    {
                        isAbsoluteMapping = true;
                    }
                    break;
            }

            return _getMapping.Invoke(mapping, out entry, isAbsoluteMapping);
        }

    }
}
