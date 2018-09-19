using NUnit.Framework;
using System;

namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class SourceAttribute : TestCaseSourceAttribute
    {
        static SourceAttribute()
        {
            ContainerService.Start().Wait();
        }

        public SourceAttribute(Type type, string source) : base(type, source) { }
    }
}
