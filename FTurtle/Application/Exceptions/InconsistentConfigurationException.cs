using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Application.Exceptions
{
    public class InconsistentConfigurationException : ApplicationException
    {
        public InconsistentConfigurationException() {}
        public InconsistentConfigurationException(string message) : base(message) {}
        public InconsistentConfigurationException(string message, Exception inner) : base(message, inner) {}
        public InconsistentConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
