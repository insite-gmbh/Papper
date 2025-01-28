using Papper.Access;
using Papper.Attributes;
using Papper.Extensions.Metadata;
using Papper.Extensions.Notification;
using Papper.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Papper
{


    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class PlcDataMapper : IDisposable
    {
        #region Delegates
        /// <summary>
        /// This delegate is used to invoke the read operations
        /// </summary>
        /// <returns></returns>
        public delegate Task ReadOperation(IEnumerable<DataPack> reads);

        /// <summary>
        /// his delegate is used to invoke the write operations.
        /// </summary>
        /// <returns></returns>
        public delegate Task WriteOperation(IEnumerable<DataPack> writes);

        /// <summary>
        /// This delegate is used to invoke the write operations.
        /// </summary>
        /// <returns></returns>
        public delegate Task UpdateMonitoring(IEnumerable<DataPack> monitoring, bool add = true);

        /// <summary>
        /// This delegate is used to invoke the block info operations.
        /// </summary>
        /// <returns></returns>
        public delegate Task ReadBlockInfo(IEnumerable<MetaDataPack> metadatas);




        #endregion

        #region Fields
        private const int _readDataHeaderLength = 18;
        private readonly HashSet<Subscription> _subscriptions = new();
        private readonly PlcMetaDataTree _tree = new();
        private readonly ReaderWriterLockSlim _mappingsLock = new();
        private UpdateMonitoring? _updateHandler;
        private ReadBlockInfo? _blockInfoHandler;
        #endregion

        #region Properties

        public int ReadDataBlockSize { get; private set; }
        public int PduSize { get; private set; }
        internal IReadOperationOptimizer Optimizer { get; }
        internal AccessEngine Engine { get; }

        internal Dictionary<string, IEntry> EntriesByName { get; } = new Dictionary<string, IEntry>();

        #endregion


        public PlcDataMapper(int pduSize,
                     ReadBlockInfo blockInfoHandler,
                     OptimizerType optimizer = OptimizerType.Block) : this(pduSize, null, null, null, blockInfoHandler, optimizer)
        {
        }

        public PlcDataMapper(int pduSize,
                    ReadOperation readEventHandler,
                    OptimizerType optimizer = OptimizerType.Block) : this(pduSize, readEventHandler, null, null, null, optimizer)
        {
        }

        public PlcDataMapper(int pduSize,
                WriteOperation writeEventHandler,
                OptimizerType optimizer = OptimizerType.Block) : this(pduSize, null, writeEventHandler, null, null, optimizer)
        {
        }

        public PlcDataMapper(int pduSize,
                            ReadOperation readEventHandler,
                            WriteOperation writeEventHandler,
                            OptimizerType optimizer = OptimizerType.Block) : this(pduSize, readEventHandler, writeEventHandler, null, null, optimizer)
        {
        }

        public PlcDataMapper(int pduSize,
                    ReadOperation readEventHandler,
                    WriteOperation writeEventHandler,
                    UpdateMonitoring updateHandler,
                    OptimizerType optimizer = OptimizerType.Block) : this(pduSize, readEventHandler, writeEventHandler, updateHandler, null, optimizer)
        {
        }

        public PlcDataMapper(int pduSize,
                            ReadOperation? readEventHandler,
                            WriteOperation? writeEventHandler,
                            UpdateMonitoring? updateHandler,
                            ReadBlockInfo? blockInfoHandler,
                            OptimizerType optimizer = OptimizerType.Block)
        {
            PduSize = pduSize;
            _updateHandler = updateHandler;
            _blockInfoHandler = blockInfoHandler;
            Optimizer = OptimizerFactory.CreateOptimizer(optimizer);


            if (Optimizer.SymbolicAccess)
            {
                Engine = new AccessEngineSymbolic(readEventHandler, writeEventHandler, GetMapping);
            }
            else
            {
                Engine = new AccessEngineAbsolute(readEventHandler, writeEventHandler, GetMapping);
                ReadDataBlockSize = pduSize - _readDataHeaderLength;
                if (ReadDataBlockSize <= 0)
                {
                    ExceptionThrowHelper.ThrowInvalidPduSizeException(_readDataHeaderLength);
                }
            }
            PlcMetaDataTreePath.CreateAbsolutePath(PlcObjectResolver.RootNodeName);
        }

        /// <summary>
        /// Return a list of all registered Mappings
        /// </summary>
        public IEnumerable<string> Mappings => EntriesByName.Keys;

        /// <summary>
        /// Return all variable names of a mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVariablesOf(string mapping)
        {
            var result = new List<string>();
            if (EntriesByName.TryGetValue(mapping, out var entry))
            {
                return PlcObjectResolver.GetLeafs(entry.PlcObject, result);
            }

            ExceptionThrowHelper.ThrowMappingNotFoundException(mapping);
            return Array.Empty<string>();
        }

        /// <summary>
        /// Return all writeable variable names of a mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public IEnumerable<string> GetWriteableVariablesOf(string mapping)
        {
            var result = new List<string>();
            if (EntriesByName.TryGetValue(mapping, out var entry))
            {
                return PlcObjectResolver.GetLeafs(entry.PlcObject, result, true);
            }

            ExceptionThrowHelper.ThrowMappingNotFoundException(mapping);
            return Array.Empty<string>();
        }

        /// <summary>
        /// Return all writeable variable names of a mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVariableBlocksOf(string mapping, VariableListTypes variableListType)
        {
            var result = new List<string>();
            if (EntriesByName.TryGetValue(mapping, out var entry))
            {
                return PlcObjectResolver.GetAccessibleBlocks(entry.PlcObject, result, variableListType, out _, out _);
            }

            ExceptionThrowHelper.ThrowMappingNotFoundException(mapping);
            return Array.Empty<string>();
        }

        /// <summary>
        /// Return all writeable variable names of a mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <returns></returns>
        public IEnumerable<string> GetVariableBlocksWithNotAccessiblyListOf(string mapping, VariableListTypes variableListType, out List<string>? notAccessible)
        {
            var result = new List<string>();
            if (EntriesByName.TryGetValue(mapping, out var entry))
            {
                return PlcObjectResolver.GetAccessibleBlocks(entry.PlcObject, result, variableListType, out _, out notAccessible);
            }
            ExceptionThrowHelper.ThrowMappingNotFoundException(mapping);
            notAccessible = null;
            return Array.Empty<string>();
        }

        /// <summary>
        /// Add a type with an MappingAttribute to register this type as an mapping for read and write operations
        /// </summary>
        /// <param name="type">Has to be a type with at least one MappingAttribute</param>
        /// <returns></returns>
        public bool AddMapping(Type type)
        {
            if (type == null)
            {
                ExceptionThrowHelper.ThrowArgumentNullException<Type>(nameof(type));
            }

            var mappingAttributes = type!.GetTypeInfo().GetCustomAttributes<MappingAttribute>().ToList();
            if (!mappingAttributes.Any())
            {
                ExceptionThrowHelper.ThrowMappingAttributeNotFoundForTypeException(type!);
            }

            return AddMappingsInternal(type!, mappingAttributes);
        }

        /// <summary>
        /// Add a type with and a list og MappingAttributes to register this type as an mapping for read and write operations
        /// </summary>
        /// <param name="type">Could be any type</param>
        /// <param name="mappingAttributes">mappings to add</param>
        /// <returns></returns>
        public bool AddMapping(Type type, params MappingAttribute[] mappingAttributes)
        {
            if (type == null)
            {
                ExceptionThrowHelper.ThrowArgumentNullException<Type>(nameof(type));
            }

            if (!mappingAttributes.Any())
            {
                ExceptionThrowHelper.ThrowMappingAttributeNotFoundForTypeException(type!);
            }

            return AddMappingsInternal(type!, mappingAttributes);
        }

        /// <summary>
        /// Removes all mappings defined by the mapping attributes for the given type.
        /// </summary>
        /// <param name="type">Has to be a type with at least one MappingAttribute</param>
        /// <returns></returns>
        public bool RemoveMappings(Type type)
        {
            if (type == null)
            {
                ExceptionThrowHelper.ThrowArgumentNullException<Type>(nameof(type));
                return false;
            }

            var mappingAttributes = type.GetTypeInfo().GetCustomAttributes<MappingAttribute>().ToList();
            if (!mappingAttributes.Any())
            {
                ExceptionThrowHelper.ThrowMappingAttributeNotFoundForTypeException(type!);
            }

            return RemoveMappingsInternal(mappingAttributes.Select(x => x.Name));
        }

        /// <summary>
        /// Removes all given mappings.
        /// </summary>
        /// <param name="mappings">Mappings names to remove</param>
        /// <returns></returns>
        public bool RemoveMappings(IEnumerable<string> mappings) => RemoveMappingsInternal(mappings);

        /// <summary>
        /// Removes all given mappings.
        /// </summary>
        /// <param name="mappings">Mappings names to remove</param>
        /// <returns></returns>
        public bool RemoveMappings(params string[] mappings) => RemoveMappingsInternal(mappings);


        /// <summary>
        /// Read variables from an given mapping
        /// </summary>
        /// <param name="vars"></param>
        /// <returns>return a dictionary with all variables and the read value</returns>
        public Task<PlcReadResult[]> ReadAsync(params PlcReadReference[] vars) => ReadAsync(vars as IEnumerable<PlcReadReference>);

        /// <summary>
        /// Read variables from an given mapping as the mapping specified type
        /// </summary>
        /// <param name="vars"></param>
        /// <returns>return a dictionary with all variables and the read value</returns>
        public Task<PlcReadResult[]> ReadAsync(IEnumerable<PlcReadReference> vars) => Engine.ReadAsync(vars, returnRawDataResult: false);

        /// <summary>
        /// Read variables from an given mapping as byte[]
        /// </summary>
        /// <param name="vars"></param>
        /// <returns>return a dictionary with all variables and the read value</returns>
        [Obsolete("ReadBytesAsync is deprecated, please use ReadAsync and convert the value to your prefered memory layout.", error: false)]
        public Task<PlcReadResult[]> ReadBytesAsync(IEnumerable<PlcReadReference> vars) => Engine.ReadAsync(vars, returnRawDataResult: true);


        /// <summary>
        /// Write values to variables of an given mapping
        /// </summary>
        /// <param name="mapping">mapping name specified in the MappingAttribute</param>
        /// <param name="values">variable names and values to write</param>
        /// <returns>return true if all operations are succeeded</returns>
        public Task<PlcWriteResult[]> WriteAsync(params PlcWriteReference[] vars) => WriteAsync(vars as IEnumerable<PlcWriteReference>);

        /// <summary>
        /// Write values to variables of an given mapping
        /// </summary>
        /// <param name="mapping">mapping name specified in the MappingAttribute</param>
        /// <param name="values">variable names and values to write</param>
        /// <returns>return true if all operations are succeeded</returns>
        public Task<PlcWriteResult[]> WriteAsync(IEnumerable<PlcWriteReference> vars) => Engine.WriteAsync(vars);


        /// <summary>
        /// If a client supports data changes it has to call this method on any changes
        /// </summary>
        /// <param name="changed"></param>
        public void OnDataChanges(IEnumerable<DataPack> changed)
        {
            foreach (var item in _subscriptions.ToList())
            {
                item.OnDataChanged(changed);
            }
        }






        #region internal

        internal bool AddSubscription(Subscription sub) => _subscriptions.Add(sub);

        internal bool RemoveSubscription(Subscription sub) => _subscriptions.Remove(sub);

        internal Task UpdateMonitoringItemsAsync(IEnumerable<DataPack> monitoring, bool add = true) => _updateHandler != null ? _updateHandler.Invoke(monitoring, add) : Task.CompletedTask;

        internal Task ReadBlockInfos(IEnumerable<MetaDataPack> infos) => _blockInfoHandler != null ? _blockInfoHandler.Invoke(infos) : Task.CompletedTask;

        internal bool IsValidReference(PlcWatchReference watchs)
            => Engine.GetOrAddMapping(watchs.Mapping, out var entry) &&
                (entry is MappingEntry && entry.PlcObject.Get(new PlcMetaDataTreePath(watchs.Variable)) != null) ||
                (entry is RawEntry);
        #endregion


        private bool GetMapping(string mapping, [MaybeNullWhen(false)] out IEntry entry, bool allowAdd = true)
        {
            if (EntriesByName.TryGetValue(mapping, out entry))
            {
                return true;
            }

            if (allowAdd)
            {
                using (var upgradeableGuard = new UpgradeableGuard(_mappingsLock))
                {
                    if (!EntriesByName.TryGetValue(mapping, out entry))
                    {
                        entry = new RawEntry(this, mapping, 0);
                        using (upgradeableGuard.UpgradeToWriterLock())
                        {
                            EntriesByName.Add(mapping, entry);
                        }
                    }
                }
                return true;
            }

            return false;
        }

        private bool AddMappingsInternal(Type type, IEnumerable<MappingAttribute> mappingAttributes)
        {
            foreach (var mapping in mappingAttributes)
            {
                using var upgradeableGuard = new UpgradeableGuard(_mappingsLock);
                if (EntriesByName.TryGetValue(mapping.Name, out var existingMapping) && existingMapping is MappingEntry mappingEntry)
                {
                    if (mappingEntry.Mapping == mapping && mappingEntry.Type == type)
                    {
                        continue; // mapping already exists
                    }

                    return false; // mapping is invalid, because it exists for an other type
                }
                using (upgradeableGuard.UpgradeToWriterLock())
                {
                    EntriesByName.Add(mapping.Name, new MappingEntry(this, mapping, type, _tree, mapping.ObservationRate));
                }
            }
            return true;
        }

        private bool RemoveMappingsInternal(IEnumerable<string>? mappingNames)
        {
            if (mappingNames == null)
            {
                return false;
            }

            foreach (var mapping in mappingNames)
            {
                using var upgradeableGuard = new UpgradeableGuard(_mappingsLock);
                if (EntriesByName.TryGetValue(mapping, out var existingMapping) && existingMapping is MappingEntry mappingEntry)
                {
                    if (mappingEntry.Mapping.Name == mapping)
                    {
                        using (upgradeableGuard.UpgradeToWriterLock())
                        {
                            if (!EntriesByName.Remove(mapping))
                            {
                                return false;
                            }
                        }
                    }

                }
            }
            return true;
        }

        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _mappingsLock.Dispose();
                }

                Engine?.Dispose();
                _updateHandler = null;
                _blockInfoHandler = null;

                _disposedValue = true;
            }
        }

        ~PlcDataMapper()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
