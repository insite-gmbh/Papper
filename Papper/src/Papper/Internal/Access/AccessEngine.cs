using Papper.Extensions.Notification;
using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Threading.Tasks;
using static Papper.PlcDataMapper;

namespace Papper.Access
{
    internal abstract class AccessEngine : IDisposable
    {
        internal delegate bool GetMapping(string mapping, [MaybeNullWhen(false)] out IEntry entry, bool allowAdd = true);


        protected readonly ReadOperation? _readEventHandler;
        protected readonly WriteOperation? _writeEventHandler;
        protected readonly GetMapping? _getMapping;
        protected List<Execution>? _executions;


        public AccessEngine(ReadOperation? readEventHandler,
                            WriteOperation? writeEventHandler,
                            GetMapping? getMapping)
        {
            _readEventHandler = readEventHandler;
            _writeEventHandler = writeEventHandler;
            _getMapping = getMapping;
        }


        public virtual void Dispose()
        {

        }

        internal Task<PlcReadResult[]> ReadAsync(IEnumerable<PlcReadReference> vars, bool returnRawDataResult)
             => ReadExecutionsAsync(DetermineExecutions(vars), returnRawDataResult: returnRawDataResult);


        internal abstract Task<PlcReadResult[]> ReadExecutionsAsync(List<Execution> executions,
                                                                    bool onlyOutDated = true,
                                                                    DateTime? changedAfter = null,
                                                                    Func<IEnumerable<string>, DateTime, bool>? forceUpdate = null,
                                                                    Func<IEnumerable<KeyValuePair<string, PlcObjectBinding>>, IEnumerable<KeyValuePair<string, PlcObjectBinding>>>? filter = null,
                                                                    bool returnRawDataResult = false);
        internal abstract PlcReadResult[] ConvertExecutionsToReadResults(IEnumerable<Execution> executions,
                                                             Dictionary<Execution, DataPack> needUpdate,
                                                             DateTime? changedAfter = null,
                                                             Func<IEnumerable<KeyValuePair<string, PlcObjectBinding>>, IEnumerable<KeyValuePair<string, PlcObjectBinding>>>? filter = null,
                                                             bool returnRawDataResult = false);
        internal abstract PlcReadResult[] ReadVariablesFromExecutionCache(IEnumerable<string> variables, List<Execution> executions);

        internal abstract PlcReadResult[] CreatePlcReadResults(IEnumerable<Execution> executions, IEnumerable<DataPack> packs);
        internal abstract Task<PlcWriteResult[]> WriteAsync(IEnumerable<PlcWriteReference> vars);
        internal abstract List<Execution> DetermineExecutions<T>(IEnumerable<T> vars) where T : IPlcReference;
        internal abstract Dictionary<Execution, DataPack> UpdateableItems(List<Execution> executions, bool onlyOutdated, Func<IEnumerable<string>, DateTime, bool>? forceUpdate = null);

        internal abstract bool GetOrAddMapping(string mapping, [MaybeNullWhen(false)] out IEntry entry);




        protected Task ReadFromPlcAsync(IEnumerable<DataPack> packs) => _readEventHandler != null ? _readEventHandler.Invoke(packs) : Task.CompletedTask;
        protected Task WriteToPlcAsync(IEnumerable<DataPack> packs) => _writeEventHandler != null ? _writeEventHandler.Invoke(packs) : Task.CompletedTask;


        [Conditional("DEBUG")]
        private static void DebugOutPut(string format, params object[] attributes) => Debug.WriteLine(format, attributes);
    }
}
