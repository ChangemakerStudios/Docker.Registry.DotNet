using System.Runtime.InteropServices;
using System.Security.Policy;

namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Autofac;
    using Cas.Common.WPF.Interfaces;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class TagViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _scope;
        private readonly IRegistryClient _registryClient;
        private readonly IMessageBoxService _messageBoxService;
        private readonly IViewService _viewService;
        private readonly RepositoryViewModel _parent;

        public TagViewModel(
            ILifetimeScope scope,
            IRegistryClient registryClient, 
            IMessageBoxService messageBoxService,
            IViewService viewService,
            RepositoryViewModel parent,
            string repository, 
            string tag)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            Repository = repository;
            Tag = tag;

            CopyTagCommand = new RelayCommand(CopyTag);
            GetManifestCommand = new RelayCommand(GetManifest);
            ViewManifestCommand = new RelayCommand(ViewManifest);
            DeleteCommand = new RelayCommand(Delete, CanDelete);
        }

        public ICommand GetManifestCommand { get; }
        public ICommand ViewManifestCommand { get; }
        public ICommand DeleteCommand { get; }

        public ICommand CopyTagCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        private void CopyTag()
        {
            Uri uri = new Uri(_parent.Parent.Url);

            string hostname = uri.Host;

            string qualified = $"{hostname}/{Repository}:{Tag}";

            Clipboard.SetText(qualified);
        }

        private async void Delete()
        {
            if (_messageBoxService.Show($"Delete tag '{Repository}:{Tag}'?", "Delete tag", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                var ex = await Executor.ExecuteAsync(async () =>
                    {
                        //We need to get the digest of the manifest
                        var manifest = await _registryClient.Manifest.GetManifestAsync(Repository, Tag);

                        string digest = manifest.DockerContentDigest;

                        if (string.IsNullOrWhiteSpace(digest))
                        {
                            _messageBoxService.Show("Unable to find digest.");
                        }
                        else
                        {
                            await _registryClient.Manifest.DeleteManifestAsync(Repository, digest);    
                        }
                    });

                if (ex == null)
                {
                    //Refresh
                    _parent.Refresh();
                }
            }
        }

        private bool CanDelete()
        {
            return !Executor.IsBusy;
        }

        private async void GetManifest()
        {
            GetImageManifestResult result = null;

            var ex = await Executor.ExecuteAsync(async () =>
            {
                result = await _registryClient.Manifest.GetManifestAsync(Repository, Tag);
            });

            if (ex != null)
            {
                _messageBoxService.Show(ex.Message, "Get manifest");
            }
            else
            {
                var textDialogViewModel = _scope.Resolve<TextDialogViewModel>(
                    new NamedParameter("text", result.Content),
                    new NamedParameter("title", $"Manfiest - {Repository}:{Tag}:{result.MediaType}")
                    );

                _viewService.Show(textDialogViewModel);
            }
        }

        private async void ViewManifest()
        {
            GetImageManifestResult result = null;

            var ex = await Executor.ExecuteAsync(async () =>
            {
                result = await _registryClient.Manifest.GetManifestAsync(Repository, Tag);
            });

            if (ex != null)
            {
                _messageBoxService.Show(ex.Message);
            }
            else
            {
                var manifest = result.Manifest as ImageManifest2_2;

                if (manifest == null)
                {
                    _messageBoxService.Show("Unsupported manifest type.");
                }
                else
                {
                    var dialogViewModel = _scope.Resolve<ManifestDialogViewModel>
                    (
                        new TypedParameter(typeof(ImageManifest2_2), manifest),
                        new TypedParameter(GetType(), this)
                    );

                    _viewService.ShowDialog(dialogViewModel);
                }
            }
        }

        public string Repository { get; }

        public string Tag { get; }

        
    }
}