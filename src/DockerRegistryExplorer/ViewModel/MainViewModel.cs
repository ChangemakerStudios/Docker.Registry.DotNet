namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Autofac;
    using Cas.Common.WPF.Interfaces;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class MainViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IViewService _viewService;
        private readonly ObservableCollection<RegistryViewModel> _registries = new ObservableCollection<RegistryViewModel>();

        public MainViewModel(ILifetimeScope lifetimeScope, IViewService viewService)
        {
            _lifetimeScope = lifetimeScope;
            _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));

            RefreshCommand = new RelayCommand(Refresh);
            ConnectCommand = new RelayCommand(Connect);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ConnectCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        private void Connect()
        {
            var viewModel = _lifetimeScope.Resolve<ConnectViewModel>();

            if (_viewService.ShowDialog(viewModel) == true)
            {
                var registryClient = viewModel.RegistryClient;

                var childScope = _lifetimeScope.BeginLifetimeScope(builder =>
                {
                    builder.RegisterInstance(registryClient);
                });

                var registry = childScope.Resolve<RegistryViewModel>
                (
                    new NamedParameter("url", viewModel.Endpoint)
                );

                _registries.Add(registry);
            }
        }

        private void Refresh()
        {
            foreach (var registry in Registries)
            {
                //TODO: Refresh
            }
        }

        public IEnumerable<RegistryViewModel> Registries => _registries;
    }
}