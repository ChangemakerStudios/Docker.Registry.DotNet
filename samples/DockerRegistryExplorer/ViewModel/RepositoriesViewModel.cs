using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Autofac;

using Cas.Common.WPF.Interfaces;

using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using DockerExplorer.Extensions;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerRegistryExplorer.ViewModel
{
    public class RepositoriesViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;

        private readonly RegistryViewModel _parent;

        private readonly IRegistryClient _registryClient;

        private readonly ITextEditService _textEditService;

        private ObservableCollection<RepositoryViewModel> _repositories =
            new ObservableCollection<RepositoryViewModel>();

        public RepositoriesViewModel(
            IRegistryClient registryClient,
            RegistryViewModel parent,
            ILifetimeScope lifetimeScope,
            ITextEditService textEditService)
        {
            this._registryClient =
                registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            this._parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this._lifetimeScope =
                lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this._textEditService = textEditService
                                    ?? throw new ArgumentNullException(nameof(textEditService));

            this.LoadAllRepositoriesCommand = new RelayCommand(this.LoadAllRepositories);
            this.LoadRepositoryCommand = new RelayCommand(this.LoadRepository);
        }

        public ICommand LoadAllRepositoriesCommand { get; }

        public ICommand LoadRepositoryCommand { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get => this._repositories;
            private set
            {
                this._repositories = value;
                this.RaisePropertyChanged();
            }
        }

        private void LoadAllRepositories()
        {
            if (!this.Executor.IsBusy)
                this.Executor.ExecuteAsync(
                    async () =>
                    {
                        var catalog =
                            await this._registryClient.Catalog.GetCatalogAsync(
                                new CatalogParameters());

                        var repositories = catalog.Repositories
                            .Select(
                                r => this._lifetimeScope.Resolve<RepositoryViewModel>(
                                    new NamedParameter("name", r),
                                    new TypedParameter(typeof(RegistryViewModel), this._parent)
                                ))
                            .OrderBy(e => e.Name);

                        this.Repositories =
                            new ObservableCollection<RepositoryViewModel>(repositories);

                        Console.WriteLine("Done");
                    }).IgnoreAsync();
        }

        private void LoadRepository()
        {
            if (!this.Executor.IsBusy)
                this.Executor.ExecuteAsync(
                    () =>
                    {
                        string name = null;

                        this._textEditService.EditText(
                            "",
                            "Repository name",
                            "Add Repository",
                            s => name = s);

                        if (!string.IsNullOrEmpty(name))
                        {
                            var repository = this._lifetimeScope.Resolve<RepositoryViewModel>(
                                new NamedParameter("name", name),
                                new TypedParameter(typeof(RegistryViewModel), this._parent)
                            );

                            this.Repositories.Add(repository);
                        }

                        return Task.CompletedTask;
                    }).IgnoreAsync();
        }

        public void Refresh()
        {
        }

        private bool CanRefresh()
        {
            return !this.Executor.IsBusy;
        }
    }
}