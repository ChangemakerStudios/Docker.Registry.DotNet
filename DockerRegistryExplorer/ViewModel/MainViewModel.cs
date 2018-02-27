namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using Autofac;
    using Cas.Common.WPF.Interfaces;
    using Docker.Registry.DotNet;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class MainViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly IMessageBoxService _messageBoxService;
        private readonly ITextEditService _textEditService;
        private readonly ObservableCollection<RegistryViewModel> _registries = new ObservableCollection<RegistryViewModel>();

        public MainViewModel(ILifetimeScope lifetimeScope, IMessageBoxService messageBoxService, ITextEditService textEditService)
        {
            _lifetimeScope = lifetimeScope;
            _messageBoxService = messageBoxService;
            _textEditService = textEditService;

            RefreshCommand = new RelayCommand(Refresh);
            ConnectCommand = new RelayCommand(Connect);
        }

        public ICommand RefreshCommand { get; }
        public ICommand ConnectCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        private void Connect()
        {
            try
            {
                string endpoint = null;

                _textEditService.EditText("http://10.0.4.44:5000/", "Docker Registry Endpoint", "Add Registry Endpoint", s => endpoint = s);

                if (!string.IsNullOrEmpty(endpoint))
                {
                    var configuration = new RegistryClientConfiguration(new Uri(endpoint));

                    var registryClient = configuration.CreateClient();

                    //await registryClient.System.PingAsync();

                    var childScope = _lifetimeScope.BeginLifetimeScope(builder =>
                        {
                            builder.RegisterInstance(registryClient);
                        });

                    var registry = childScope.Resolve<RegistryViewModel>
                        (
                            new NamedParameter("url", endpoint)
                        );

                    _registries.Add(registry);
                }
            }
            catch (Exception ex)
            {
                _messageBoxService.Show(ex.Message, "Error");
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