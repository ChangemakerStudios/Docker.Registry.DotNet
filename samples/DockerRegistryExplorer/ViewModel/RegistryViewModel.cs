namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Autofac;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class RegistryViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RegistryViewModel(string url, ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            Url = url;

            Children = new ViewModelBase[]
            {
                lifetimeScope.Resolve<RepositoriesViewModel>(
                    new TypedParameter(GetType(), this))
            };

            RefreshCommand = new RelayCommand(Refresh);
        }

        public ICommand RefreshCommand { get; }

        public void Refresh()
        {
            foreach (var child in Children.OfType<RepositoryViewModel>())
            {
                child.Refresh();
            }
        }

        public string Url { get; }

        public ViewModelBase[] Children { get; }
    }
}