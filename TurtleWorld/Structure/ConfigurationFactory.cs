using System;
using System.Collections.Concurrent;

namespace TurtleWorld.Structure
{
    /// <summary>
    /// Builds the configuration. Keeps the builders in "secret" and provides clients with
    /// either building delegate, or ready configuration.
    /// </summary>
    public static class ConfigurationFactory
    {
        // ConcurrentDictionary as a first step to multithreading; and I love GetOrAdd method.
        private static readonly ConcurrentDictionary<string, IConfigBuilder> Builders = new ConcurrentDictionary<string, IConfigBuilder>();

        /// <summary>
        /// Creates or reuses config builders.
        /// It is responsibility of a builder to be thread safe and re entrant.
        /// </summary>
        /// <param name="kind">What to build or get</param>
        /// <param name="creator">Delegate that creates an instance</param>
        /// <returns>Builder delegate</returns>
        public static Action<string> GetBuilder(string kind = "standard", Func<IConfigBuilder> creator = null)
        {
            return Builders.GetOrAdd(kind, 
                creator?.Invoke() ?? kind.ToLower() switch
                {
                    "standard" => new StandardConfigBuilder(),
                    _ => ThrowIt(kind)
                }).Build;
        }

        public static IConfiguration GetConfiguration(string kind = "standard")
        {
            if (!Builders.TryGetValue(kind, out var builder))
            {
                throw new ArgumentException($"Configuration '{kind}' does not exists.", nameof(kind));
            }

            return builder.Get(); // may throw (e.g. if the configuration is incomplete)
        }

        private static IConfigBuilder ThrowIt(string kind)
        {
            throw new NotImplementedException("'{kind}' builder is not supported currently");
        }
    }
}
