using System;
using System.Globalization;

namespace Papper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class MappingAttribute : Attribute
    {
        //public MappingAttribute()
        //{
        //    Name = string.Empty;
        //    Selector = string.Empty;
        //    Offset = 0;
        //    ObservationRate = 0;
        //}

        public MappingAttribute(string name, string selector, int offset = 0, int observationRate = 0)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ExceptionThrowHelper.ThrowArgumentCouldNotBeNullOrWhitespaceException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(selector))
            {
                ExceptionThrowHelper.ThrowArgumentCouldNotBeNullOrWhitespaceException(nameof(selector));
            }

            Name = name;
            Selector = selector;
            Offset = offset;
            ObservationRate = observationRate;
        }


        /// <summary>
        /// The symbolic name of the data block. This is the name you will use to access the data block in the plc.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Selector
        {
            get;
            private set;
        }

        public int Offset
        {
            get;
            private set;
        }

        public int ObservationRate
        {
            get;
            private set;
        }

        // overload operator + 
        public static bool operator ==(MappingAttribute? a, MappingAttribute? b)
        {
            bool rc;

            if (ReferenceEquals(a, b))
            {
                rc = true;
            }
            else if ((a is null) || (b is null))
            {
                rc = false;
            }
            else
            {
                rc = (string.Compare(a.Name, b.Name, StringComparison.Ordinal) == 0 &&
                      string.Compare(a.Selector, b.Selector, StringComparison.Ordinal) == 0 &&
                      a.Offset == b.Offset &&
                      a.ObservationRate == b.ObservationRate);
            }

            return rc;
        }



        public static bool operator !=(MappingAttribute? p1, MappingAttribute? p2) => !(p1 == p2);

        public override bool Equals(object? obj)
        {
            var rc = false;
            if (obj is MappingAttribute p2)
            {
                rc = this == p2;
            }
            return rc;
        }

        public override int GetHashCode() => string.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}", Name, Selector, Offset).GetHashCode();
    }
}
