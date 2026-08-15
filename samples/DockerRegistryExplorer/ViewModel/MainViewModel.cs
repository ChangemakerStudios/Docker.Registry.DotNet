using System.Collections.ObjectModel;

namespace DockerRegistryExplorer.ViewModel;

public class MainViewModel : ObservableObject, IDisposable
{
    private readonly ILifetimeScope _lifetimeScope;

    private readonly ObservableCollection<RegistryViewModel> _registries = new();

    private readonly List<ILifetimeScope> _registryScopes = new();

    private readonly IViewService _viewService;

    public MainViewModel(ILifetimeScope lifetimeScope, IViewService viewService)
    {
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        _viewService = viewService ?? throw new ArgumentNullException(nameof(viewService));

        RefreshCommand = new RelayCommand(Refresh);
        ConnectCommand = new RelayCommand(Connect);
    }

    public ICommand RefreshCommand { get; }

    public ICommand ConnectCommand { get; }

    public AsyncExecutor Executor { get; } = new();

    public IEnumerable<RegistryViewModel> Registries => _registries;

    private void Connect()
    {
        var viewModel = _lifetimeScope.Resolve<ConnectViewModel>();

        if (_viewService.ShowDialog(viewModel) ?? false)
        {
            var registryClient = viewModel.RegistryClient;

            if (registryClient == null) return;

            var childScope = _lifetimeScope.BeginLifetimeScope(builder =>
            {
                builder.RegisterInstance(registryClient);
            });

            var registry = childScope.Resolve<RegistryViewModel>
            (
                new NamedParameter("url", viewModel.Endpoint)
            );

            _registryScopes.Add(childScope);
            _registries.Add(registry);
        }
    }

    private void Refresh()
    {
        foreach (var registry in Registries) registry.Refresh();
    }

    public void Dispose()
    {
        // child scopes are not disposed automatically by their parent --
        // Autofac disposes this view model with the root container on exit
        foreach (var scope in _registryScopes) scope.Dispose();

        _registryScopes.Clear();
        _registries.Clear();
    }
}