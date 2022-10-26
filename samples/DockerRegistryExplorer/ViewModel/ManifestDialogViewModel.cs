using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

using Autofac;

using Cas.Common.WPF.Interfaces;

using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using GalaSoft.MvvmLight.CommandWpf;

namespace DockerRegistryExplorer.ViewModel
{
    public class ManifestDialogViewModel : CloseableViewModelBase
    {
        private readonly IFileDialogService _fileDialogService;

        private readonly ImageManifest2_2 _manifest;

        private readonly IMessageBoxService _messageBoxService;

        private readonly TagViewModel _parent;

        private readonly IRegistryClient _registryClient;

        private readonly ILifetimeScope _scope;

        private ManifestLayerViewModel _selectedLayer;

        public ManifestDialogViewModel(
            ILifetimeScope scope,
            IRegistryClient registryClient,
            IMessageBoxService messageBoxService,
            IFileDialogService fileDialogService,
            ImageManifest2_2 manifest,
            TagViewModel parent)
        {
            this._scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this._registryClient =
                registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            this._messageBoxService = messageBoxService
                                      ?? throw new ArgumentNullException(nameof(messageBoxService));
            this._fileDialogService = fileDialogService
                                      ?? throw new ArgumentNullException(nameof(fileDialogService));
            this._manifest = manifest ?? throw new ArgumentNullException(nameof(manifest));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));

            if (manifest.Layers != null)
                this.Layers = manifest.Layers
                    .Select(
                        l => scope.Resolve<ManifestLayerViewModel>
                        (
                            new TypedParameter(typeof(ManifestLayer), l)
                        ))
                    .ToArray();

            this.DownloadCommand = new RelayCommand(this.Download, this.CanDownload);
        }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public ICommand DownloadCommand { get; }

        public string Title => $"{this._manifest.MediaType}";

        public ManifestLayerViewModel SelectedLayer
        {
            get => this._selectedLayer;
            set
            {
                this._selectedLayer = value;
                this.RaisePropertyChanged();
            }
        }

        public ManifestLayerViewModel[] Layers { get; }

        private async void Download()
        {
            var layer = this.SelectedLayer;

            if (layer == null)
                return;

            GetBlobResponse response = null;

            var ex = await this.Executor.ExecuteAsync(
                async () =>
                {
                    response = await this._registryClient.Blobs.GetBlobAsync(
                        this._parent.Repository,
                        layer.Digest);
                });

            if (ex != null)
            {
                this._messageBoxService.Show(ex.Message);
            }
            else
            {
                using (var stream = response.Stream)
                {
                    var result = this._fileDialogService.ShowSaveFileDialog();

                    if (result != null)
                    {
                        using (var targetStream = File.Create(result.FileName))
                        {
                            await stream.CopyToAsync(targetStream);
                        }
                    }
                }

                this._messageBoxService.Show("Layer saved", "Complete");
            }
        }

        private bool CanDownload()
        {
            return this.SelectedLayer != null;
        }
    }
}