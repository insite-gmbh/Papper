using System;


namespace Papper.Attributes
{
    /// <devdoc>
    ///    <para>Specifies whether the property this attribute is bound to
    ///       is read-only or read/write.</para>
    /// </devdoc>
    [AttributeUsage(AttributeTargets.All)]
    public sealed class NotAccessibleAttribute : Attribute
    {

        /// <devdoc>
        ///    <para>
        ///       Specifies that the property this attribute is bound to is read-only and
        ///       cannot be modified in the server explorer. This <see langword='static '/>field is
        ///       read-only.
        ///    </para>
        /// </devdoc>
        public static readonly NotAccessibleAttribute Yes = new(true);

        /// <devdoc>
        ///    <para>
        ///       Specifies that the property this attribute is bound to is read/write and can
        ///       be modified at design time. This <see langword='static '/>field is read-only.
        ///    </para>
        /// </devdoc>
        public static readonly NotAccessibleAttribute No = new(false);

        /// <devdoc>
        ///    <para>
        ///       Specifies the default value for the <see cref='System.ComponentModel.NotAccessibleAttribute'/> , which is <see cref='System.ComponentModel.NotAccessibleAttribute.No'/>, that is,
        ///       the property this attribute is bound to is read/write. This <see langword='static'/> field is read-only.
        ///    </para>
        /// </devdoc>
        public static readonly NotAccessibleAttribute Default = No;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.ReadOnlyAttribute'/> class.
        ///    </para>
        /// </devdoc>
        public NotAccessibleAttribute(bool isNotAccessible) => IsNotAccessible = isNotAccessible;

        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the property this attribute is bound to is
        ///       read-only.
        ///    </para>
        /// </devdoc>
        public bool IsNotAccessible { get; } = false;

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        public override bool Equals(object? value)
        {
            if (this == value)
            {
                return true;
            }

            return value is NotAccessibleAttribute other && other.IsNotAccessible == IsNotAccessible;
        }

        /// <devdoc>
        ///    <para>
        ///       Returns the hashcode for this object.
        ///    </para>
        /// </devdoc>
        public override int GetHashCode() => base.GetHashCode();

    }
}
