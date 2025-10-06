using System.Collections.ObjectModel;

using Docker.Registry.DotNet.Domain.Catalogs;

using Serilog;

namespace DockerRegistryExplorer.ViewModel;

public class RepositoriesViewModel : ObservableObject
{
    private readonly ILifetimeScope _lifetimeScope;

    private readonly RegistryViewModel _parent;

    private readonly IRegistryClient _registryClient;

    private readonly ITextEditService _textEditService;

    private ObservableCollection<RepositoryViewModel> _repositories = new();

    public RepositoriesViewModel(
        IRegistryClient registryClient,
        RegistryViewModel parent,
        ILifetimeScope lifetimeScope,
        ITextEditService textEditService)
    {
        _registryClient =
            registryClient ?? throw new ArgumentNullException(nameof(registryClient));
        _parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _lifetimeScope =
            lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        _textEditService = textEditService
                           ?? throw new ArgumentNullException(nameof(textEditService));

        LoadAllRepositoriesCommand = new RelayCommand(LoadAllRepositories);
        LoadRepositoryCommand = new RelayCommand(LoadRepository);
    }

    public ICommand LoadAllRepositoriesCommand { get; }

    public ICommand LoadRepositoryCommand { get; }

    public AsyncExecutor Executor { get; } = new();

    public ObservableCollection<RepositoryViewModel> Repositories
    {
        get => _repositories;
        private set
        {
            _repositories = value;
            OnPropertyChanged();
        }
    }

    private void LoadAllRepositories()
    {
        if (Executor.IsBusy) return;

        Executor.ExecuteAsync(GetCatalog).IgnoreAsync();
    }

    private async Task GetCatalog()
    {
        var catalog =
            await _registryClient.Catalog.GetCatalog(new CatalogParameters());

        var repositories = catalog.Repositories.Select(r => _lifetimeScope.Resolve<RepositoryViewModel>(
                new NamedParameter("name", r),
                new TypedParameter(typeof(RegistryViewModel), _parent)))
            .OrderBy(e => e.Name)
            .ToList();

        Repositories = new ObservableCollection<RepositoryViewModel>(repositories);

        Log.Debug("Done Getting Catalog {@Repositories}", repositories);
    }

    private void LoadRepository()
    {
        if (Executor.IsBusy) return;

        Executor.ExecuteAsync(LoadRepositoryInternal).IgnoreAsync();
    }

    private Task LoadRepositoryInternal()
    {
        string name = null;

        _textEditService.EditText(
            "",
            "Repository name",
            "Add Repository",
            s => name = s);

        if (!string.IsNullOrEmpty(name))
        {
            var repository = _lifetimeScope.Resolve<RepositoryViewModel>(new NamedParameter("name", name),
                new TypedParameter(typeof(RegistryViewModel), _parent));

            Repositories.Add(repository);
        }

        return Task.CompletedTask;
    }

    public void Refresh()
    {
    }

    private bool CanRefresh()
    {
        return !Executor.IsBusy;
    }
}