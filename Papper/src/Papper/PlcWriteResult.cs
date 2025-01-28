using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Papper
{
    /// <summary>
    /// This structure represents the result of a write command. It includes the written meta data and 
    /// the <see cref="ExecutionResult"/> of the write operation.
    /// </summary>
    public struct PlcWriteResult : IEquatable<PlcWriteResult>
    {
        private readonly int _dot;
        private static readonly Regex _regexSplitByDot = new("[.]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

        /// <summary>
        /// Full address is composed of mapping and variable
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// The result of an Executed action
        /// </summary>
        public ExecutionResult ActionResult { get; }

        /// <summary>
        /// mapping part of the address
        /// </summary>
        public string Mapping => _dot == -1 ? Address : Address.Substring(0, _dot);

        /// <summary>
        /// variable part of the address.
        /// </summary>
        public string Variable => _dot == -1 ? string.Empty : Address.Substring(_dot + 1);


        /// <summary>
        /// Create an instance of a PlcWriteResult
        /// </summary>
        /// <param name="address">The address of a variable. e.g. [Mapping].[Variable]</param>
        /// <param name="executionResult">The result itself. <see cref="ExecutionResult"/></param>
        public PlcWriteResult(string address, ExecutionResult executionResult)
        {
            Address = address;
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

        public override bool Equals(object? obj) => obj is PlcWriteResult result && Equals(result);
        public bool Equals(PlcWriteResult other) => _dot == other._dot && Address == other.Address && ActionResult == other.ActionResult && Mapping == other.Mapping && Variable == other.Variable;

        public override int GetHashCode()
        {
            var hashCode = 150809288;
            hashCode = hashCode * -1521134295 + _dot.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + ActionResult.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mapping);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Variable);
            return hashCode;
        }

        public static bool operator ==(PlcWriteResult left, PlcWriteResult right) => left.Equals(right);
        public static bool operator !=(PlcWriteResult left, PlcWriteResult right) => !(left == right);
    }
}