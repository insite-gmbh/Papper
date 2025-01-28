using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Papper
{
    /// <summary>
    /// This class is used to define write operations.
    /// </summary>
    public struct PlcWriteReference : IPlcReference, IEquatable<PlcWriteReference>
    {
        private readonly int _dot;
        private static readonly Regex _regexSplitByDot = new("[.]{1}(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))", RegexOptions.Compiled);

        /// mapping part of the address
        /// </summary>
        public string Mapping => _dot == -1 ? Address : Address.Substring(0, _dot);

        /// <summary>
        /// variable part of the address.
        /// </summary>
        public string Variable => _dot == -1 ? string.Empty : Address.Substring(_dot + 1);


        /// <summary>
        /// Full address is composed of mapping and variable
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Value to write
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Create <see cref="PlcWriteReference"/> from an address and a value
        /// </summary>
        /// <param name="address"> [Mapping].[Variable]</param>
        /// <param name="value">Value to write</param>
        /// <returns>An instance of a <see cref="PlcWriteReference"/></returns>
        public static PlcWriteReference FromAddress(string address, object? value) => new(address, value);

        /// <summary>
        /// Create <see cref="PlcWriteReference"/> from another reference and a value
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PlcWriteReference FromPlcReference(IPlcReference reference, object? value) => new(reference.Address, value);


        public PlcWriteReference(string address, object? value)
        {
            Address = address;
            Value = value ?? ExceptionThrowHelper.ThrowArgumentNullException<object>(nameof(value));
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
        /// Create a couple of <see cref="PlcWriteReference"/> by a given variable root, and some sub variables of the root, with the value to write.
        /// This method can used if you will write more than one variable of the same data block.
        /// </summary>
        /// <param name="root">Rootpart of a variable</param>
        /// <param name="variables"><see cref="KeyValuePair{TKey, TValue}"/> of variable and value</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PlcWriteReference"/></returns>
        public static IEnumerable<PlcWriteReference> FromRoot(string root, params KeyValuePair<string, object>[] variables)
            => FromRoot(root, variables as IEnumerable<KeyValuePair<string, object>>);

        /// <summary>
        /// Create a couple of <see cref="PlcWriteReference"/> by a given variable root, and some sub variables of the root, with the value to write.
        /// This method can used if you will write more than one variable of the same data block.
        /// </summary>
        /// <param name="root">Rootpart of a variable</param>
        /// <param name="variables"><see cref="IEnumerable{KeyValuePair{TKey, TValue}}"/> of variable and value</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PlcWriteReference"/></returns>
        public static IEnumerable<PlcWriteReference> FromRoot(string root, IEnumerable<KeyValuePair<string, object>> variables)
        {
            if (variables == null)
            {
                yield break;
            }

            foreach (var variable in variables)
            {
                yield return FromAddress($"{root}.{variable.Key}", variable.Value);
            }
        }

        /// <summary>
        /// Create a couple of <see cref="PlcWriteReference"/> by a given variable root, and some sub variables of the root, with the value to write.
        /// This method can used if you will write more than one variable of the same data block.
        /// </summary>
        /// <param name="root">Rootpart of a variable</param>
        /// <param name="variables"><see cref="KeyValuePair{TKey, TValue}"/> of variable and value</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PlcWriteReference"/></returns>
        public static IEnumerable<PlcWriteReference> FromRoot(string root, params (string, object)[] variables)
            => FromRoot(root, variables as IEnumerable<(string, object)>);

        /// <summary>
        /// Create a couple of <see cref="PlcWriteReference"/> by a given variable root, and some sub variables of the root, with the value to write.
        /// This method can used if you will write more than one variable of the same data block.
        /// </summary>
        /// <param name="root">Rootpart of a variable</param>
        /// <param name="variables"><see cref="IEnumerable{KeyValuePair{TKey, TValue}}"/> of variable and value</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="PlcWriteReference"/></returns>
        public static IEnumerable<PlcWriteReference> FromRoot(string root, IEnumerable<(string variable, object value)> variables)
        {
            if (variables == null)
            {
                yield break;
            }

            foreach (var variable in variables)
            {
                yield return FromAddress($"{root}.{variable.variable}", variable.value);
            }
        }

        public override bool Equals(object? obj) => obj is PlcWriteReference reference && Equals(reference);
        public bool Equals(PlcWriteReference other) => _dot == other._dot && Mapping == other.Mapping && Variable == other.Variable && Address == other.Address && EqualityComparer<object>.Default.Equals(Value, other.Value);

        public override int GetHashCode()
        {
            var hashCode = 794000764;
            hashCode = hashCode * -1521134295 + _dot.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Mapping);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Variable);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Address);
            hashCode = hashCode * -1521134295 + EqualityComparer<object>.Default.GetHashCode(Value);
            return hashCode;
        }

        public static bool operator ==(PlcWriteReference left, PlcWriteReference right) => left.Equals(right);
        public static bool operator !=(PlcWriteReference left, PlcWriteReference right) => !(left == right);
    }
}