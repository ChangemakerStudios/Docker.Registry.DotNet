using Docker.Registry.DotNet.Domain.Models;

namespace DockerRegistryExplorer.ViewModel;

public class RepositoryViewModel : ObservableObject
{
    private readonly ILifetimeScope _lifetimeScope;

    private readonly IRegistryClient _registryClient;

    private TagViewModel[] _tags = [];

    public RepositoryViewModel(
        string name,
        RegistryViewModel parent,
        IRegistryClient registryClient,
        ILifetimeScope lifetimeScope)
    {
        Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
        _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        Name = name;

        Refresh();

        RefreshCommand = new RelayCommand(Refresh);
    }

    public ICommand RefreshCommand { get; }

    public TagViewModel[] Tags
    {
        get => _tags;
        private set
        {
            _tags = value;
            OnPropertyChanged();
        }
    }

    public string Name { get; }

    public AsyncExecutor Executor { get; } = new();

    public RegistryViewModel Parent { get; }

    public void Refresh()
    {
        if (!CanRefresh()) return;

        Executor.ExecuteAsync(ListImagesTags).IgnoreAsync();
    }

    private async Task ListImagesTags()
    {
        var tags = await _registryClient.Tags.ListTags(
            Name,
            new ListTagsParameters());

        if (tags.Tags == null) Tags = [];
        else
            Tags = tags.Tags.Select(t => _lifetimeScope.Resolve<TagViewModel>(
                    new NamedParameter("repository", Name),
                    new NamedParameter("tag", t.Value),
                    new TypedParameter(GetType(), this)))
                .OrderByDescending(t => t.Tag)
                .ToArray();
    }

    private bool CanRefresh()
    {
        return !Executor.IsBusy;
    }
}