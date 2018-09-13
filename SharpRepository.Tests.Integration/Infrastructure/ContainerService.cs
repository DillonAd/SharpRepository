using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpRepository.Tests.Integration.Infrastructure
{
    public abstract class ContainerService : IDisposable
    {
        public event OutputReceivedEventHandler OnOutputReceived; 
        public delegate void OutputReceivedEventHandler(object sender, string value);    

        private readonly IDockerClient _client;
        private readonly string _imageName;
        private readonly string _tag;
        private readonly string _containerName;
        private readonly int _port;
        private readonly string _containerId;

        public ContainerService(string uri, string imageName, string tag, string containerName, int port)
        {
            _imageName = imageName;
            _tag = tag;
            _containerName = containerName;
            _port = port;

            _client = new DockerClientConfiguration(new Uri(uri))
                .CreateClient();

            _client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = _imageName,
                Tag = _tag
            },
                new AuthConfig(),
                new Progress<JSONMessage>(msg => WriteLine(msg.ProgressMessage)));

            _containerId = CreateContainer().Result;

            _client.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).Wait();

            WriteLine($"{_containerName} started");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if(disposing)
            {

                _client.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
                
                _client.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters()
                {
                    Force = true
                }).Wait();

                _client.Dispose();

                WriteLine($"{_containerName} stopped");
            }
        }
        
        private async Task<string> CreateContainer()
        {
            var container = _client.Containers
                .ListContainersAsync(new ContainersListParameters() { All = true })
                .Result
                .FirstOrDefault(c => c.Names.Contains($"/{_imageName}"));

            if (container != null)
            {
                return container.ID;
            }
            var createContainerParams = new CreateContainerParameters
            {
                Name = _containerName,
                Image = $"{_imageName}"
            };

            var portBindings = new Dictionary<string, IList<PortBinding>>()
            {
                {
                    "8080/tcp",
                    new List<PortBinding>()
                    {
                        new PortBinding()
                        {
                            HostPort = "8080"
                        }
                    }
                }
            };

            createContainerParams.HostConfig = new HostConfig()
            {
                PortBindings = portBindings
            };

            var createResponse = await _client.Containers.CreateContainerAsync(createContainerParams);
            return createResponse.ID;
        }

        private void WriteLine(string output)
        {
            OnOutputReceived?.Invoke(this, output);
        }
    }
}
