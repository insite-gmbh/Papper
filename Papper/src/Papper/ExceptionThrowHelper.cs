using System;
using System.Collections.Generic;

namespace Papper
{
#pragma warning disable CA1303 // Do not pass literals as localized parameters
    internal static class ExceptionThrowHelper
    {
        public static T ThrowArgumentNullException<T>(string argName) => throw new ArgumentNullException(argName);

        public static void ThrowArgumentNullException(string argName) => throw new ArgumentNullException(argName);

        public static void ThrowInvalidPduSizeException(int minimumSize) => throw new ArgumentException($"PDU size have to be greater then {minimumSize}", nameof(minimumSize));

        public static void ThrowInvalidMappingNameException(string name) => throw new ArgumentException($"The given mapping name {name} is not declared!", nameof(name));

        public static void ThrowMappingNotFoundException(string name) => throw new KeyNotFoundException($"The mapping {name} does not exist.");

        public static void ThrowMappingAttributeNotFoundForTypeException(Type type) => throw new ArgumentException($"The given type {type}  has no MappingAttribute", nameof(type));

        public static void ThrowMappingAttributeNotFoundForPlcTypeNameException(string type) => throw new ArgumentException($"The given type {type}  has no MappingAttribute", nameof(type));

        public static void ThrowInvalidVariableException(string variable) => throw new InvalidVariableException(variable);

        public static void ThrowArrayIndexExeption(int index) => throw new IndexOutOfRangeException($"PlcArrayError (unknown index {index})");

        public static void ThrowArgumentOutOfRangeException(string argumentName) => throw new ArgumentOutOfRangeException(argumentName);

        public static void ThrowArgumentCouldNotBeNullOrWhitespaceException(string argumentName) => throw new ArgumentException("The given argument could not be null or whitespace.", argumentName);

        public static void ThrowMultipleDetectionsAreNotSupportedException() => throw new InvalidOperationException($"More than one detection run at the same time is not supported!");

        public static void ThrowOperationNotAllowedForCurrentChangeDetectionStrategy() => throw new InvalidOperationException("This operation is not allowed for this change detection strategy");

        public static void ThrowChildNodeException(string name, bool exists) => throw new ArgumentException($"TreeNode: A child with name {name} {(exists ? "already exists" : "does not exist")}!");

        public static void ThrowAttemptToAssignNewParentException() => throw new ArgumentException("TreeNode: Attempt to assign a new parent.");

        public static void ThrowInvalidNodePathCollectionException() => throw new ArgumentException("Path: Node collection must not contain a separator!");

        public static void ThrowEmptyNodePathCollectionException() => throw new Exception("Path: Cannot step down in empty path!");

        public static void ThrowUnknownOptimizrException(OptimizerType type) => throw new ArgumentException($"Unknown optimizer type <{type}> given!");

        public static void ThrowObjectDisposedException(string objectName) => throw new ObjectDisposedException(objectName);

        public static void ThrowNotSupportedException() => throw new NotSupportedException();
    }
#pragma warning restore CA1303 // Do not pass literals as localized parameters
}
