using NUnit.Framework;
namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class MongoDbContainer : ContainerBase
    {
        public MongoDbContainer() 
            : base("mongo", "latest", "sr-mongodb", 27017) { }
    }
}
