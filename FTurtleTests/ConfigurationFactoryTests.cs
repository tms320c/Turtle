using System;
using System.Collections.Generic;
using System.Linq;
using FTurtle;
using FTurtle.Application;
using FTurtle.Domain;
using FTurtle.Infrastructure;
using FTurtleTests.Tools;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace FTurtleTests
{
    public class ConfigurationFactoryTests
    {
        private readonly ITestOutputHelper _output;

        public ConfigurationFactoryTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ShouldThrowExceptionIfBuilderKindIsNotSupported()
        {
            _ = Assert.Throws<NotImplementedException>(() => ConfigurationFactory.GetBuilder("unknown"));
        }

        [Fact]
        public void ShouldThrowExceptionIfConfigurationDoesNotExist()
        {
            var ex = Assert.Throws<ArgumentException>(() => ConfigurationFactory.GetConfiguration("unknown"));
            Assert.Equal("kind", ex.ParamName);
        }

        [Fact]
        public void ReturnsActionForKnownBuilder()
        {
            var builder = ConfigurationFactory.GetBuilder("standard");
            Assert.IsType<Action<string>>(builder);
        }

        [Fact]
        public void ReturnsConfigurationForKnownBuilder()
        {
            var builder = ConfigurationFactory.GetBuilder("mocked", () => new MockBuilder());
            Assert.IsType<Action<string>>(builder);
            var config = ConfigurationFactory.GetConfiguration("mocked");
            Assert.IsType<MockConfig>(config);
        }

        [Fact]
        public void GetKnownBuilderFromCache()
        {
            var builder1 = ConfigurationFactory.GetBuilder("mocked", () => new MockBuilder());
            Assert.IsType<Action<string>>(builder1);

            var builder2 = ConfigurationFactory.GetBuilder("mocked", () => new MockBuilder());
            Assert.IsType<Action<string>>(builder2);

            var builder3 = ConfigurationFactory.GetBuilder("mocked1", () => new MockBuilder());
            Assert.IsType<Action<string>>(builder3);

            Assert.Equal(builder1, builder2);
            Assert.NotEqual(builder1, builder3);
        }

    }

    // The logic is too complicated for Substitute<>.
    internal class MockConfig : IConfiguration
    {
        public MockConfig()
        {
            Board = null;
            Start = new Position();
            Target = new Position();
            Moves = null;
        }
        public IBoard Board { get; }
        public Position Start { get; }
        public Position Target { get; }
        public IEnumerable<string> Moves { get; }
    }

    internal class MockBuilder : IConfigBuilder
    {
        public void Build(string rawLine)
        {
            return;
        }

        public IConfiguration Get()
        {
            return new MockConfig();
        }
    }
}


