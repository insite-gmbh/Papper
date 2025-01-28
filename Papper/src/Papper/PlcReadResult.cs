using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;

namespace Papper
{
    /// <summary>
    /// This structure represents the result of a read command. It includes the read meta data and 
    /// the <see cref="ExecutionResult"/> of the read operation.
    /// </summary>
    public struct PlcReadResult : IEquatable<PlcReadResult>
    {
        private readonly int _dot;
        private static readonly Regex _regexSplitByDot = new("[.]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

        /// <summary>
        /// Full address is composed of mapping and variable
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// read value
        /// </summary>
        public object? Value { get; }

        /// <summary>
        /// The result of an Executed action
        /// </summary>
        public ExecutionResult ActionResult { get; }

        /// <summary>
        /// mapping part of the address
        /// </summary>
        public string Mapping => _dot == -1 ? Address : Address[.._dot];

        /// <summary>
        /// variable part of the address.
        /// </summary>
        public string Variable => _dot == -1 ? string.Empty : Address[(_dot + 1)..];


        /// <summary>
        /// Create an instance of a <see cref="PlcReadResult"/>
        /// </summary>
        /// <param name="address">The address of a variable. e.g. [Mapping].[Variable]</param>
        /// <param name="value">The read value.</param>
        /// <param name="executionResult">The result itself. <see cref="ExecutionResult"/></param>
        public PlcReadResult(string address, object? value, ExecutionResult executionResult)
        {
            Address = address ?? ExceptionThrowHelper.ThrowArgumentNullException<string>(nameof(address));
            Value = value;
            ActionResult = executionResult;
            if (address == null)
            {
                _dot = -1;
            }
            else
            {
                Match firstMatch = _regexSplitByDot.Match(address);
                _dot = firstMatch.Success ? firstMatch.Index : -1;
            }
        }


        /// <summary>
        /// Determine if this result is part of the given mapping
        /// </summary>
        /// <param name="mapping">mapping to test</param>
        /// <returns></returns>
        public bool IsPartOfMapping(string mapping)
        {
            Match firstMatch = _regexSplitByDot.Match(Address);
            if (!firstMatch.Success) return false;
            return mapping.AsSpan().SequenceEqual(Address.AsSpan()[..firstMatch.Index]);
        }

        public override bool Equals(object? obj) => obj is PlcReadResult result &&
                                                    Address == result.Address &&
                                                    EqualityComparer<object?>.Default.Equals(Value, result.Value) &&
                                                    ActionResult == result.ActionResult &&
                                                    Mapping == result.Mapping &&
                                                    Variable == result.Variable;

        public override int GetHashCode()
        {
            return HashCode.Combine(Address, Value, ActionResult, Mapping, Variable);
        }

        public static bool operator ==(PlcReadResult left, PlcReadResult right) => left.Equals(right);

        public static bool operator !=(PlcReadResult left, PlcReadResult right) => !(left == right);

        public bool Equals(PlcReadResult other) => Address == other.Address &&
                                                    EqualityComparer<object?>.Default.Equals(Value, other.Value) &&
                                                    ActionResult == other.ActionResult &&
                                                    Mapping == other.Mapping &&
                                                    Variable == other.Variable;
    }
}