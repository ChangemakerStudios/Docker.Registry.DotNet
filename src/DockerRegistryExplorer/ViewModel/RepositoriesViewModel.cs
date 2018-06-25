namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Autofac;
    using Cas.Common.WPF.Interfaces;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using DockerExplorer.Extensions;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class RepositoriesViewModel : ViewModelBase
    {
        private readonly IRegistryClient _registryClient;
        private readonly RegistryViewModel _parent;
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ITextEditService _textEditService;
        private ObservableCollection<RepositoryViewModel> _repositories = new ObservableCollection<RepositoryViewModel>();

        public RepositoriesViewModel(IRegistryClient registryClient, RegistryViewModel parent, ILifetimeScope lifetimeScope, ITextEditService textEditService)
        {
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _textEditService = textEditService ?? throw new ArgumentNullException(nameof(textEditService));

            LoadAllRepositoriesCommand = new RelayCommand(LoadAllRepositories);
            LoadRepositoryCommand = new RelayCommand(LoadRepository);
        }

        public ICommand LoadAllRepositoriesCommand { get; }
        public ICommand LoadRepositoryCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get { return _repositories; }
            private set
            {
                _repositories = value; 
                RaisePropertyChanged();
            }
        }


        private void LoadAllRepositories()
        {
            if (!Executor.IsBusy)
            {
                Executor.ExecuteAsync(async () =>
                {
                    var catalog = await _registryClient.Catalog.GetCatalogAsync(new CatalogParameters()
                    {

                    });

                    var repositories = catalog.Repositories
                        .Select(r => _lifetimeScope.Resolve<RepositoryViewModel>
                        (
                            new NamedParameter("name", r),
                            new TypedParameter(typeof(RegistryViewModel), _parent)
                        ))
                        .OrderBy(e => e.Name);

                    Repositories = new ObservableCollection<RepositoryViewModel>(repositories);

                    Console.WriteLine("Done");

                }).IgnoreAsync();
            }
        }

        private void LoadRepository()
        {
            if (!Executor.IsBusy)
            {
                Executor.ExecuteAsync(() =>
                {
                    string name = null;

                    _textEditService.EditText("", "Repository name", "Add Repository", s => name = s);

                    if (!string.IsNullOrEmpty(name))
                    {
                        var repository = _lifetimeScope.Resolve<RepositoryViewModel>
                        (
                            new NamedParameter("name", name),
                            new TypedParameter(typeof(RegistryViewModel), _parent)
                        );

                        Repositories.Add(repository);
                    }

                    return Task.CompletedTask;

                }).IgnoreAsync();
            }
        }

        public void Refresh()
        {
            
        }

        private bool CanRefresh()
        {
            return !Executor.IsBusy;
        }
    }
}