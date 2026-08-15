namespace DockerRegistryExplorer.ViewModel;

public class RegistryViewModel : ObservableObject
{
    public RegistryViewModel(string url, ILifetimeScope lifetimeScope)
    {
        Url = url;

        Children =
        [
            lifetimeScope.Resolve<RepositoriesViewModel>(
                new TypedParameter(GetType(), this))
        ];

        RefreshCommand = new RelayCommand(Refresh);
    }

    public ICommand RefreshCommand { get; }

    public string Url { get; }

    public ObservableObject[] Children { get; }

    public void Refresh()
    {
        foreach (var child in Children.OfType<RepositoriesViewModel>()) child.Refresh();
    }
}