namespace DockerRegistryExplorer.ViewModel
{
    using System;
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

        public TagViewModel(
            ILifetimeScope scope,
            IRegistryClient registryClient, 
            IMessageBoxService messageBoxService,
            IViewService viewService,
            string repository, 
            string tag)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));
            Repository = repository;
            Tag = tag;

            GetManifestCommand = new RelayCommand(GetManifest);
            ViewManifestCommand = new RelayCommand(ViewManifest);
        }

        public ICommand GetManifestCommand { get; }

        public ICommand ViewManifestCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        private async void GetManifest()
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