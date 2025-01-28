using System.Collections.Generic;

namespace Papper
{
    public struct ChangeResult : System.IEquatable<ChangeResult>
    {
        /// <summary>
        /// True if the current read was canceled
        /// </summary>
        public bool IsCanceled { get; }

        /// <summary>
        /// True if is complete
        /// </summary>
        public bool IsCompleted { get; }

        /// <summary>
        /// The <see cref="PlcReadResult"/> that was read
        /// </summary>
        public IEnumerable<PlcReadResult>? Results { get; }

        public ChangeResult(IEnumerable<PlcReadResult>? result, bool isCancelled, bool isCompleted)
        {
            Results = result;
            IsCanceled = isCancelled;
            IsCompleted = isCompleted;
        }

        public override bool Equals(object? obj) => obj is ChangeResult result &&
                                                    IsCanceled == result.IsCanceled &&
                                                    IsCompleted == result.IsCompleted &&
                                                    EqualityComparer<IEnumerable<PlcReadResult>?>.Default.Equals(Results, result.Results);

        public override int GetHashCode()
        {
            return System.HashCode.Combine(IsCanceled, IsCompleted, Results);
        }

        public static bool operator ==(ChangeResult left, ChangeResult right) => left.Equals(right);

        public static bool operator !=(ChangeResult left, ChangeResult right) => !(left == right);

        public bool Equals(ChangeResult other) => IsCanceled == other.IsCanceled &&
                                                    IsCompleted == other.IsCompleted &&
                                                    EqualityComparer<IEnumerable<PlcReadResult>?>.Default.Equals(Results, other.Results);
    }
}
