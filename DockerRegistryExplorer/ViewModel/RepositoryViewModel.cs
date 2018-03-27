namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Linq;
    using Autofac;
    using Docker.Registry.DotNet;
    using Docker.Registry.DotNet.Models;
    using DockerExplorer.Extensions;
    using GalaSoft.MvvmLight;

    public class RepositoryViewModel : ViewModelBase
    {
        private readonly IRegistryClient _registryClient;
        private readonly ILifetimeScope _lifetimeScope;
        private TagViewModel[] _tags;

        public RepositoryViewModel(string name, IRegistryClient registryClient, ILifetimeScope lifetimeScope)
        {
            _registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            Name = name;

            Refresh();
        }

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

        public void Refresh()
        {
            if (CanRefresh())
            {
                Executor.ExecuteAsync(async () =>
                {
                    var tags = await _registryClient.Tags.ListImageTagsAsync(Name, new ListImageTagsParameters());

                    Tags = tags.Tags
                        .Select(t => _lifetimeScope.Resolve<TagViewModel>
                        (
                            new NamedParameter("tag", t)
                        ))
                        //.OrderBy(t => t.Tag)
                        .ToArray();

                }).IgnoreAsync();
            }
        }

        private bool CanRefresh()
        {
            return !Executor.IsBusy;
        }

    }
}