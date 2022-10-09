using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

using Autofac;

using Docker.Registry.DotNet.Models;
using Docker.Registry.DotNet.Registry;

using DockerExplorer.Extensions;

using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace DockerRegistryExplorer.ViewModel
{
    public class RepositoryViewModel : ViewModelBase
    {
        private readonly ILifetimeScope _lifetimeScope;

        private readonly IRegistryClient _registryClient;

        private TagViewModel[] _tags;

        public RepositoryViewModel(
            string name,
            RegistryViewModel parent,
            IRegistryClient registryClient,
            ILifetimeScope lifetimeScope)
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
            this._registryClient = registryClient ?? throw new ArgumentNullException(nameof(registryClient));
            this._lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            this.Name = name;

            this.Refresh();

            this.RefreshCommand = new RelayCommand(this.Refresh);
        }

        public ICommand RefreshCommand { get; }

        public TagViewModel[] Tags
        {
            get => this._tags;
            private set
            {
                this._tags = value;
                this.RaisePropertyChanged();
            }
        }

        public string Name { get; }

        public AsyncExecutor Executor { get; } = new AsyncExecutor();

        public RegistryViewModel Parent { get; }

        public void Refresh()
        {
            if (!this.CanRefresh()) return;

            this.Executor.ExecuteAsync(this.ListImagesTags).IgnoreAsync();
        }

        private async Task ListImagesTags()
        {
            var tags = await this._registryClient.Tags.ListImageTagsAsync(
                this.Name,
                new ListImageTagsParameters());

            if (tags.Tags == null) this.Tags = new TagViewModel[] { };
            else
                this.Tags = tags.Tags.Select(
                        t => this._lifetimeScope.Resolve<TagViewModel>(
                            new NamedParameter("repository", this.Name),
                            new NamedParameter("tag", t),
                            new TypedParameter(this.GetType(), this)))
                    .OrderByDescending(t => t.Tag)
                    .ToArray();
        }

        private bool CanRefresh()
        {
            return !this.Executor.IsBusy;
        }
    }
}