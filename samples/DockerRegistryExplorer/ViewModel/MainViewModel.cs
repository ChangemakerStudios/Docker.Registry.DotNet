using System.Collections.ObjectModel;

namespace DockerRegistryExplorer.ViewModel;

public class MainViewModel : ObservableObject
{
    private readonly ILifetimeScope _lifetimeScope;

    private readonly ObservableCollection<RegistryViewModel> _registries = new();

    private readonly IViewService _viewService;

    public MainViewModel(ILifetimeScope lifetimeScope, IViewService viewService)
    {
        _lifetimeScope = lifetimeScope;
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
        foreach (var registry in Registries) registry.Refresh();
    }
}