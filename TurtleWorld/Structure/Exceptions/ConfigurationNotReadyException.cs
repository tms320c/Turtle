using System;
using System.Runtime.Serialization;

namespace TurtleWorld.Structure.Exceptions
{
    public class ConfigurationNotReadyException: ApplicationException
    {
        public ConfigurationNotReadyException() {}
        public ConfigurationNotReadyException(string message) : base(message) {}
        public ConfigurationNotReadyException(string message, Exception inner) : base(message, inner) {}
        public ConfigurationNotReadyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
