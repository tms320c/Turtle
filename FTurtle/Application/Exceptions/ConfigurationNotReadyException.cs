using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FTurtle.Application.Exceptions
{
    public class ConfigurationNotReadyException: ApplicationException
    {
        public ConfigurationNotReadyException() {}
        public ConfigurationNotReadyException(string message) : base(message) {}
        public ConfigurationNotReadyException(string message, Exception inner) : base(message, inner) {}
        public ConfigurationNotReadyException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
