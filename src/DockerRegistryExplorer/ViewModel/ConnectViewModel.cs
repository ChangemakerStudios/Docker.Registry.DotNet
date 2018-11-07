namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Authentication;

    public class ConnectViewModel : DialogViewModelBase
    {
        private const string DefaultEndpoint = "registry.hub.docker.com";

        private bool _isAnonymous = true;
        
        
        private string _endpoint = DefaultEndpoint;
        private string _username;
        private string _password;

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        protected override async void Ok()
        {
            var ex = await Executor.ExecuteAsync(async () =>
            {
                var configuration = new RegistryClientConfiguration(Endpoint);

                AuthenticationProvider authenticationProvider;

                if (IsAnonymous)
                {
                    authenticationProvider = new AnonymousOAuthAuthenticationProvider();
                }
                else
                {
                    authenticationProvider = new PasswordOAuthAuthenticationProvider(Username, Password);
                }

                var client = configuration.CreateClient(authenticationProvider);

                await client.System.PingAsync();

                RegistryClient = client;
            });

            if (ex == null)
            {
                base.Ok();
            }
        }

        public bool IsAnonymous
        {
            get { return _isAnonymous; }
            set
            {
                _isAnonymous = value; 
                RaisePropertyChanged();
            }
        }

        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value; 
                RaisePropertyChanged();
            }
        }

        public string Username
        {
            get { return _username; }
            set
            {
                _username = value; 
                RaisePropertyChanged();
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value; 
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// If Ok is pressed, this will have the registry client
        /// </summary>
        public IRegistryClient RegistryClient { get; private set; } 
    }
}