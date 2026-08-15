using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Application.Authentication;

namespace DockerRegistryExplorer.ViewModel;

public class ConnectViewModel : DialogViewModelBase
{
    private const string DefaultEndpoint = "https://registry-1.docker.io";

    private string _endpoint = DefaultEndpoint;

    private bool _isAnonymous = true;

    private string? _password;

    private string? _username;

    public AsyncExecutor Executor { get; } = new();

    public bool IsAnonymous
    {
        get => _isAnonymous;
        set
        {
            _isAnonymous = value;
            OnPropertyChanged();
        }
    }

    public string Endpoint
    {
        get => _endpoint;
        set
        {
            _endpoint = value;
            OnPropertyChanged();
        }
    }

    public string? Username
    {
        get => _username;
        set
        {
            _username = value;
            OnPropertyChanged();
        }
    }

    public string? Password
    {
        get => _password;
        set
        {
            _password = value;
            OnPropertyChanged();
        }
    }

    /// <summary>
    ///     If Ok is pressed, this will have the registry client
    /// </summary>
    public IRegistryClient? RegistryClient { get; private set; }

    protected override async void Ok()
    {
        var ex = await Executor.ExecuteAsync(Connect);

        if (ex == null) base.Ok();
    }

    private async Task Connect()
    {
        var configuration = new RegistryClientConfiguration(Endpoint);

        if (!IsAnonymous
            && !string.IsNullOrWhiteSpace(Username)
            && !string.IsNullOrWhiteSpace(Password))
            configuration.UsePasswordOAuthAuthentication(Username, Password);

        var client = configuration.CreateClient();

        await client.System.Ping();

        RegistryClient = client;
    }
}
