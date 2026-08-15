using Docker.Registry.DotNet.Domain.Manifests;

namespace DockerRegistryExplorer.ViewModel;

public class ManifestLayerViewModel : ObservableObject
{
    private readonly IMessageBoxService _messageBoxService;

    private readonly ManifestLayer _model;

    private readonly IRegistryClient _registryClient;

    public ManifestLayerViewModel(
        IRegistryClient registryClient,
        IMessageBoxService messageBoxService,
        ManifestLayer model)
    {
        _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
        _messageBoxService = messageBoxService ?? throw new ArgumentNullException(nameof(messageBoxService));
        _model = model ?? throw new ArgumentNullException(nameof(model));
    }

    public AsyncExecutor Executor { get; } = new();

    public string MediaType => _model.MediaType ?? string.Empty;

    public long Size => _model.Size;

    public string Digest => _model.Digest ?? string.Empty;
}