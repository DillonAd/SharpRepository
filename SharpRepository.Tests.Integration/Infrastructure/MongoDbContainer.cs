using NUnit.Framework;
namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class MongoDbContainer : ContainerService
    {
        public MongoDbContainer() : base(@"npipe://./pipe/docker_engine", "mongo", "latest", "sr-mongodb", 27017) { }
    }
}
