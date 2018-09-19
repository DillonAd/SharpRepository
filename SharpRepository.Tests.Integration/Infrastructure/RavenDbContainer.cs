namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class RavenDbContainer : ContainerBase
    {
        public RavenDbContainer() : base("ravendb/ravendb", "latest", "sr-ravendb", 8080)
        {
        }
    }
}
