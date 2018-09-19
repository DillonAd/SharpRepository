using NUnit.Framework;
using SharpRepository.Tests.Integration.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SharpRepository.Tests.Integration
{
    [SetUpFixture]
    public class ContainerService
    {
        private static readonly HashSet<ContainerBase> Containers;

        static ContainerService()
        {
            if (Containers != null)
            {
                return;
            }

            Containers = new HashSet<ContainerBase>()
            {
                new CouchDbContainer(),
                new MongoDbContainer(),
                new RavenDbContainer(),
                new SqlServerContainer()
            };
        }

        public static async Task Start()
        {
            var tasks = new List<Task>();

            foreach (var container in Containers)
            {
                if (container.Started)
                {
                    continue;
                }

                tasks.Add(container.Start());
            }

            await Task.WhenAll(tasks);
        }

        [OneTimeTearDown]
        public static void TearDown()
        {
            foreach (var container in Containers)
            {
                container.Dispose();
            }
        }
    }
}