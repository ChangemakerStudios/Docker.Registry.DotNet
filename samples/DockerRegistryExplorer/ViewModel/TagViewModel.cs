using System;
using System.Windows;
using System.Windows.Input;

using Autofac;

using Cas.Common.WPF.Interfaces;

using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerRegistryExplorer.ViewModel
{
    public class TagViewModel : ViewModelBase
    {
        private readonly IMessageBoxService _messageBoxService;

        private readonly RepositoryViewModel _parent;

        private readonly IRegistryClient _registryClient;

        private readonly ILifetimeScope _scope;

        private readonly IViewService _viewService;

        public TagViewModel(
            ILifetimeScope scope,
            IRegistryClient registryClient,
            IMessageBoxService messageBoxService,
            IViewService viewService,
            RepositoryViewModel parent,
            string repository,
            string tag)
        {
            this._scope = scope ?? throw new ArgumentNullException(nameof(scope));
            this._registryClient =
                registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            this._messageBoxService = messageBoxService
                                      ?? throw new ArgumentNullException(nameof(messageBoxService));
            this._viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this.Repository = repository;
            this.Tag = tag;

            this.CopyTagCommand = new RelayCommand(this.CopyTag);
            this.GetManifestCommand = new RelayCommand(this.GetManifest);
            this.ViewManifestCommand = new RelayCommand(this.ViewManifest);
            this.DeleteCommand = new RelayCommand(this.Delete, this.CanDelete);
        }

        public ICommand GetManifestCommand { get; }

        public ICommand ViewManifestCommand { get; }

        public ICommand DeleteCommand { get; }

        public ICommand CopyTagCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public string Repository { get; }

        public string Tag { get; }

        private void CopyTag()
        {
            var uri = new Uri(this._parent.Parent.Url);

            var hostname = uri.Host;

            var qualified = $"{hostname}/{this.Repository}:{this.Tag}";

            Clipboard.SetText(qualified);
        }

        private async void Delete()
        {
            if (this._messageBoxService.Show(
                    $"Delete tag '{this.Repository}:{this.Tag}'?",
                    "Delete tag",
                    MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                var ex = await this.Executor.ExecuteAsync(
                    async () =>
                    {
                        //We need to get the digest of the manifest
                        var manifest =
                            await this._registryClient.Manifest.GetManifestAsync(
                                this.Repository,
                                this.Tag);

                        var digest = manifest.DockerContentDigest;

                        if (string.IsNullOrWhiteSpace(digest))
                            this._messageBoxService.Show("Unable to find digest.");
                        else
                            await this._registryClient.Manifest.DeleteManifestAsync(
                                this.Repository,
                                digest);
                    });

                if (ex == null)
                    //Refresh
                    this._parent.Refresh();
            }
        }

        private bool CanDelete()
        {
            return !this.Executor.IsBusy;
        }

        private async void GetManifest()
        {
            GetImageManifestResult result = null;

            var ex = await this.Executor.ExecuteAsync(
                async () =>
                {
                    result = await this._registryClient.Manifest.GetManifestAsync(
                        this.Repository,
                        this.Tag);
                });

            if (ex != null)
            {
                this._messageBoxService.Show(ex.Message, "Get manifest");
            }
            else
            {
                var textDialogViewModel = this._scope.Resolve<TextDialogViewModel>(
                    new NamedParameter("text", result.Content),
                    new NamedParameter(
                        "title",
                        $"Manfiest - {this.Repository}:{this.Tag}:{result.MediaType}")
                );

                this._viewService.Show(textDialogViewModel);
            }
        }

        private async void ViewManifest()
        {
            GetImageManifestResult result = null;

            var ex = await this.Executor.ExecuteAsync(
                async () =>
                {
                    result = await this._registryClient.Manifest.GetManifestAsync(
                        this.Repository,
                        this.Tag);
                });

            if (ex != null)
            {
                this._messageBoxService.Show(ex.Message);
            }
            else
            {
                var manifest = result.Manifest as ImageManifest2_2;

                if (manifest == null)
                {
                    this._messageBoxService.Show("Unsupported manifest type.");
                }
                else
                {
                    var dialogViewModel = this._scope.Resolve<ManifestDialogViewModel>(
                        new TypedParameter(typeof(ImageManifest2_2), manifest),
                        new TypedParameter(this.GetType(), this)
                    );

                    this._viewService.ShowDialog(dialogViewModel);
                }
            }
        }
    }
}