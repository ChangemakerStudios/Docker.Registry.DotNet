namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using Autofac;
    using GalaSoft.MvvmLight;

    public class RegistryViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RegistryViewModel(string url, ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            Url = url;

            Children = new ViewModelBase[]
            {
                lifetimeScope.Resolve<RepositoriesViewModel>()
            };
        }

        public string Url { get; }

        public ViewModelBase[] Children { get; }
    }
}