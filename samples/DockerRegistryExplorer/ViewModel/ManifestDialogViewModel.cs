namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using Autofac;
    using Cas.Common.WPF.Interfaces;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using GalaSoft.MvvmLight.CommandWpf;

    public class ManifestDialogViewModel : CloseableViewModelBase
    {
        private readonly ILifetimeScope _scope;
        private readonly IRegistryClient _registryClient;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IFileDialogService _fileDialogService;
        private readonly ImageManifest2_2 _manifest;
        private readonly TagViewModel _parent;
        private ManifestLayerViewModel _selectedLayer;

        public ManifestDialogViewModel(
            ILifetimeScope scope, 
            IRegistryClient registryClient,
            IMessageBoxService messageBoxService, 
            IFileDialogService fileDialogService,
            ImageManifest2_2 manifest,
            TagViewModel parent)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            _fileDialogService = fileDialogService ?? throw new ArgumentNullException(nameof(fileDialogService));
            _manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));

            if (manifest.Layers != null)
            {
                Layers = manifest.Layers
                    .Select(l => scope.Resolve<ManifestLayerViewModel>
                    (
                        new TypedParameter(typeof(ManifestLayer), l)
                    ))
                    .ToArray();
            }

            DownloadCommand = new RelayCommand(Download, CanDownload);
        }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public ICommand DownloadCommand { get; }

        public string Title => $"{_manifest.MediaType}";

        private async void Download()
        {
            var layer = SelectedLayer;

            if (layer == null)
                return;

            GetBlobResponse response = null;

            var ex = await Executor.ExecuteAsync(async () =>
            {
                response = await _registryClient.Blobs.GetBlobAsync(_parent.Repository, layer.Digest);
            });

            if (ex != null)
            {
                _messageBoxService.Show(ex.Message);
            }
            else
            {
                using (var stream = response.Stream)
                {
                    var result = _fileDialogService.ShowSaveFileDialog();

                    if (result != null)
                    {
                        using (var targetStream = File.Create(result.FileName))
                        {
                            stream.CopyTo(targetStream);
                        }
                    }
                }

                _messageBoxService.Show("Layer saved", "Complete");
            }
        }

        private bool CanDownload()
        {
            return SelectedLayer != null;
        }

        public ManifestLayerViewModel SelectedLayer
        {
            get => _selectedLayer;
            set
            {
                _selectedLayer = value; 
                RaisePropertyChanged();
            }
        }

        public ManifestLayerViewModel[] Layers { get; }
    }
}