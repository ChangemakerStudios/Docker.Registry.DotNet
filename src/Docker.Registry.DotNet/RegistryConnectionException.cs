using System;

namespace Docker.Registry.DotNet
{
    /// <summary>
    /// Thrown when connecting to a registry fails.
    /// </summary>
    public class RegistryConnectionException : Exception
    {
        /// <inheritdoc />
        public RegistryConnectionException()
        {
        }

        /// <inheritdoc />
        public RegistryConnectionException(string message) : base(message)
        {
        }

        /// <inheritdoc />
        public RegistryConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}