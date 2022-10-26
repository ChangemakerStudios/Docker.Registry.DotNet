using Docker.Registry.DotNet.Registry;

namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using Cas.Common.WPF.Interfaces;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using GalaSoft.MvvmLight;

    public class ManifestLayerViewModel : ViewModelBase
    {
        private readonly IRegistryClient _registryClient;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ManifestLayer _model;

        public ManifestLayerViewModel(
            IRegistryClient registryClient, 
            IMessageBoxService messageBoxService,
            ManifestLayer model)
        {
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            _model = model ?? throw new ArgumentNullException(nameof(model));
        }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public string MediaType => _model.MediaType;

        public long Size => _model.Size;

        public string Digest => _model.Digest;
    }
}