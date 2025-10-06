using System.Threading.Tasks;

using Docker.Registry.DotNet;
using Docker.Registry.DotNet.Application.Authentication;

namespace DockerRegistryExplorer.ViewModel
{
    public class ConnectViewModel : DialogViewModelBase
    {
        private const string DefaultEndpoint = "https://registry-1.docker.io";

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
                this.OnPropertyChanged();
            }
        }

        public string Endpoint
        {
            get => this._endpoint;
            set
            {
                this._endpoint = value;
                this.OnPropertyChanged();
            }
        }

        public string Username
        {
            get => this._username;
            set
            {
                this._username = value;
                this.OnPropertyChanged();
            }
        }

        public string Password
        {
            get => this._password;
            set
            {
                this._password = value;
                this.OnPropertyChanged();
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

            if (!this.IsAnonymous && this.Username != null && this.Password != null)
                configuration.UsePasswordOAuthAuthentication(this.Username, this.Password);

            var client = configuration.CreateClient();

            await client.System.Ping();

            this.RegistryClient = client;
        }
    }
}