using System.Threading.Tasks;

using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Authentication;
using Docker.Registry.DotNet.Registry;

namespace DockerRegistryExplorer.ViewModel
{
    public class ConnectViewModel : DialogViewModelBase
    {
        private const string DefaultEndpoint = "registry.hub.docker.com";

        private string _endpoint = DefaultEndpoint;

        private bool _isAnonymous = true;

        private string _password;

        private string _username;

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public bool IsAnonymous
        {
            get => this._isAnonymous;
            set
            {
                this._isAnonymous = value;
                this.RaisePropertyChanged();
            }
        }

        public string Endpoint
        {
            get => this._endpoint;
            set
            {
                this._endpoint = value;
                this.RaisePropertyChanged();
            }
        }

        public string Username
        {
            get => this._username;
            set
            {
                this._username = value;
                this.RaisePropertyChanged();
            }
        }

        public string Password
        {
            get => this._password;
            set
            {
                this._password = value;
                this.RaisePropertyChanged();
            }
        }

        /// <summary>
        ///     If Ok is pressed, this will have the registry client
        /// </summary>
        public IRegistryClient RegistryClient { get; private set; }

        protected override async void Ok()
        {
            var ex = await this.Executor.ExecuteAsync(this.Connect);

            if (ex == null) base.Ok();
        }

        private async Task Connect()
        {
            var configuration = new RegistryClientConfiguration(this.Endpoint);

            AuthenticationProvider authenticationProvider;

            if (this.IsAnonymous)
                authenticationProvider = new AnonymousOAuthAuthenticationProvider();
            else
                authenticationProvider =
                    new PasswordOAuthAuthenticationProvider(this.Username, this.Password);

            var client = configuration.CreateClient(authenticationProvider);

            await client.System.PingAsync();

            this.RegistryClient = client;
        }
    }
}