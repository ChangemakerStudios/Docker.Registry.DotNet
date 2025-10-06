using Docker.Registry.DotNet.Domain.ImageReferences;
using Docker.Registry.DotNet.Domain.Manifests;

namespace DockerRegistryExplorer.ViewModel;

public class TagViewModel : ObservableObject
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
        _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        _registryClient =
            registryClient ?? throw new ArgumentNullException(nameof(registryClient));
        _messageBoxService = messageBoxService
                             ?? throw new ArgumentNullException(nameof(messageBoxService));
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

    public AsyncExecutor Executor { get; } = new();

    public string Repository { get; }

    public string Tag { get; }

    private void CopyTag()
    {
        var parentUrl = _parent.Parent.Url.Replace("127.0.0.1", "localhost");

        var uri = new Uri(parentUrl);

        var hostname = uri.Host;

        var qualified = $"{hostname}/{Repository}:{Tag}";

        Clipboard.SetText(qualified);
    }

    private async void Delete()
    {
        if (_messageBoxService.Show(
                $"Delete tag '{Repository}:{Tag}'?",
                "Delete tag",
                MessageBoxButton.YesNo) ==
            MessageBoxResult.Yes)
        {
            var ex = await Executor.ExecuteAsync(GetManifestInternal);

            if (ex == null)
                //Refresh
                _parent.Refresh();
        }
    }

    private async Task GetManifestInternal()
    {
        //We need to get the digest of the manifest
        var manifest = await _registryClient.Manifest.GetManifest(
            Repository,
            ImageReference.Create(Tag));

        var digest = manifest.DockerContentDigest;

        if (string.IsNullOrWhiteSpace(digest))
            _messageBoxService.Show("Unable to find digest.");
        else
            await _registryClient.Manifest.DeleteManifest(
                Repository,
                ImageReference.Create(digest));
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
            result = await _registryClient.Manifest.GetManifest(
                Repository,
                ImageReference.Create(Tag));
        });

        if (ex != null)
        {
            _messageBoxService.Show(ex.Message, "Get manifest");
        }
        else
        {
            var textDialogViewModel = _scope.Resolve<TextDialogViewModel>(
                new NamedParameter("text", result.Content),
                new NamedParameter(
                    "title",
                    $"Manfiest - {Repository}:{Tag}:{result.MediaType}")
            );

            _viewService.Show(textDialogViewModel);
        }
    }

    private async void ViewManifest()
    {
        GetImageManifestResult result = null;

        var ex = await Executor.ExecuteAsync(async () =>
        {
            result = await _registryClient.Manifest.GetManifest(
                Repository,
                ImageReference.Create(Tag));
        });

        if (ex != null)
        {
            _messageBoxService.Show(ex.Message);
        }
        else
        {
            if (result.Manifest is ImageManifest2_2 manifest)
            {
                var dialogViewModel = _scope.Resolve<ManifestDialogViewModel>(
                    new TypedParameter(typeof(ImageManifest2_2), manifest),
                    new TypedParameter(GetType(), this)
                );

                _viewService.ShowDialog(dialogViewModel);
            }
            else
            {
                _messageBoxService.Show("Unsupported manifest type.");
            }
        }
    }
}