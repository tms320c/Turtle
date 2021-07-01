using System;
using System.Runtime.Serialization;

namespace TurtleWorld.Structure.Exceptions
{
    public class InconsistentConfigurationException : ApplicationException
    {
        public InconsistentConfigurationException() {}
        public InconsistentConfigurationException(string message) : base(message) {}
        public InconsistentConfigurationException(string message, Exception inner) : base(message, inner) {}
        public InconsistentConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
