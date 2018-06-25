namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Linq;
    using System.Windows.Input;
    using Autofac;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using DockerExplorer.Extensions;
    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.CommandWpf;

    public class RepositoryViewModel : ViewModelBase
    {
        private readonly RegistryViewModel _parent;
        private readonly IRegistryClient _registryClient;
        private readonly ILifetimeScope _lifetimeScope;
        private TagViewModel[] _tags;

        public RepositoryViewModel(string name, RegistryViewModel parent, IRegistryClient registryClient, ILifetimeScope lifetimeScope)
        {
            _parent = parent ?? throw new ArgumentNullException(nameof(parent));
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            Name = name;

            Refresh();

            RefreshCommand = new RelayCommand(Refresh);
        }

        public ICommand RefreshCommand { get; }

        public TagViewModel[] Tags
        {
            get { return _tags; }
            private set
            {
                _tags = value; 
                RaisePropertyChanged();
            }
        }

        public string Name { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public RegistryViewModel Parent
        {
            get { return _parent; }
        }

        public void Refresh()
        {
            if (CanRefresh())
            {
                Executor.ExecuteAsync(async () =>
                {
                    var tags = await _registryClient.Tags.ListImageTagsAsync(Name, new ListImageTagsParameters());

                    if (tags.Tags == null)
                    {
                        Tags = new TagViewModel[]{};
                    }
                    else
                    {
                        Tags = tags.Tags
                            .Select(t => _lifetimeScope.Resolve<TagViewModel>
                            (
                                new NamedParameter("repository", Name),
                                new NamedParameter("tag", t),
                                new TypedParameter(GetType(), this)
                            ))
                            .OrderByDescending(t => t.Tag)
                            .ToArray();    
                    }

                    

                }).IgnoreAsync();
            }
        }

        private bool CanRefresh()
        {
            return !Executor.IsBusy;
        }

    }
}