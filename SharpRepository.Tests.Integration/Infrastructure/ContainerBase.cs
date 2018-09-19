using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SharpRepository.Tests.Integration.Infrastructure
{
    public abstract class ContainerBase
    {
        public event OutputReceivedEventHandler OnOutputReceived;
        public delegate void OutputReceivedEventHandler(object sender, string value);

        private readonly IDockerClient _client;
        private readonly string _imageName;
        private readonly string _tag;
        private readonly string _containerName;
        private readonly int _port;
        
        public bool Started { get; private set; }
        public string ContainerId { get; private set; }

        protected ContainerBase(string imageName, string tag, string containerName, int port)
        {
            _imageName = imageName;
            _tag = tag;
            _containerName = containerName;
            _port = port;

            _client = new DockerClientConfiguration(new Uri(@"npipe://./pipe/docker_engine"))
                .CreateClient();
        }

        public async Task Start()
        {
            if (Started)
            {
                return;
            }

            Started = true;

            await _client.Images.CreateImageAsync(new ImagesCreateParameters()
                {
                    FromImage = _imageName,
                    Tag = _tag
                },
                new AuthConfig(),
                new Progress<JSONMessage>(msg => WriteLine(msg.ProgressMessage)));

            ContainerId = await CreateContainer();
            
            await _client.Containers.StartContainerAsync(ContainerId, new ContainerStartParameters());

            WriteLine($"{_containerName} started");
        }

        //public abstract Task<bool> IsStarted();

        public void Dispose()
        {
            Dispose(true);
        }

        protected void Dispose(bool disposing)
        {
            if (!disposing || string.IsNullOrEmpty(ContainerId))
            {
                return;
            }

            _client.Containers.StopContainerAsync(ContainerId, new ContainerStopParameters()).Wait();

            _client.Containers.RemoveContainerAsync(ContainerId, new ContainerRemoveParameters()
            {
                Force = true
            }).Wait();

            _client.Dispose();

            WriteLine($"{_containerName} stopped");
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
                    $"{_port}/tcp",
                    new List<PortBinding>()
                    {
                        new PortBinding()
                        {
                            HostPort = $"{_port}"
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

        protected void WriteLine(string output)
        {
            OnOutputReceived?.Invoke(this, output);
        }
    }
}
