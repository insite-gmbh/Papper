﻿using Papper.Internal;
using Papper.Types;
using System;
using System.Collections.Generic;

namespace Papper
{
    public class MappingEntryProvider
    {
        private readonly PlcMetaDataTree _tree = new PlcMetaDataTree();
        private readonly Dictionary<Type, MappingEntry> _entries = new Dictionary<Type, MappingEntry>();

        internal class MappingEntry
        {
            public PlcObject PlcObject { get; private set; }
            public Dictionary<string, Tuple<int, PlcObject>> Variables { get; private set; }
            public Dictionary<string, PlcObjectBinding> Bindings { get; private set; }
            public PlcObjectBinding BaseBinding { get; private set; }

            public MappingEntry(PlcObject plcObject)
            {
                PlcObject = plcObject ?? throw new ArgumentNullException(nameof(plcObject));
                BaseBinding = new PlcObjectBinding(new PlcRawData(plcObject.ByteSize), plcObject, 0, 0, true);
                Variables = new Dictionary<string, Tuple<int, PlcObject>>();
                Bindings = new Dictionary<string, PlcObjectBinding>();
            }
        }

        internal MappingEntry GetMappingEntryForType(Type type)
        {
            if (!_entries.TryGetValue(type, out var mappingEntry))
            {
                mappingEntry = new MappingEntry(PlcObjectResolver.GetMapping(type.Name, _tree, type, true));
                _entries.Add(type, mappingEntry);
            }
            return mappingEntry;
        }

        internal bool UpdateVariables(MappingEntry mappingEntry, string variable)
        {
            if (PlcObjectResolver.AddPlcObjects(mappingEntry.PlcObject, mappingEntry.Variables, new List<string> { variable }))
            {
                if (mappingEntry.Variables.TryGetValue(variable, out var accessObject))
                {
                    mappingEntry.Bindings.Add(variable, new PlcObjectBinding(mappingEntry.BaseBinding.RawData, accessObject.Item2, accessObject.Item1 + accessObject.Item2.ByteOffset, 0));
                    return true;
                }
            }
            return false;
        }
    }
}