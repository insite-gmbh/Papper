using System;

namespace Papper.Attributes
{

    /// <summary>
    /// Validation attribute to assert a string property, field or parameter does not exceed a maximum length
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public sealed class IgnoreAttribute : Attribute
    {

        /// <devdoc>
        ///    <para>
        ///       Specifies that the property this attribute is bound to is read-only and
        ///       cannot be modified in the server explorer. This <see langword='static '/>field is
        ///       read-only.
        ///    </para>
        /// </devdoc>
        public static readonly IgnoreAttribute Yes = new(true);

        /// <devdoc>
        ///    <para>
        ///       Specifies that the property this attribute is bound to is read/write and can
        ///       be modified at design time. This <see langword='static '/>field is read-only.
        ///    </para>
        /// </devdoc>
        public static readonly IgnoreAttribute No = new(false);

        /// <devdoc>
        ///    <para>
        ///       Specifies the default value for the <see cref='System.ComponentModel.IgnoreAttribute'/> , which is <see cref='System.ComponentModel.IgnoreAttribute.No'/>, that is,
        ///       the property this attribute is bound to is read/write. This <see langword='static'/> field is read-only.
        ///    </para>
        /// </devdoc>
        public static readonly IgnoreAttribute Default = No;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.IgnoreAttribute'/> class.
        ///    </para>
        /// </devdoc>
        public IgnoreAttribute(bool isIgnored = true) => IsIgnored = isIgnored;

        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the property this attribute is bound to is
        ///       read-only.
        ///    </para>
        /// </devdoc>
        public bool IsIgnored { get; } = false;

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override bool Equals(object? value)
        {
            if (this == value)
            {
                return true;
            }

            return value is IgnoreAttribute other && other.IsIgnored == IsIgnored;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns the hashcode for this object.
        ///    </para>
        /// </devdoc>
        public override int GetHashCode() => base.GetHashCode();

    }
}
