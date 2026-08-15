namespace DockerRegistryExplorer.ViewModel;

public abstract class DialogViewModelBase : CloseableViewModelBase
{
    protected DialogViewModelBase()
    {
        OkCommand = new RelayCommand(Ok, CanOk);
    }

    public ICommand OkCommand { get; }

    protected virtual void Ok()
    {
        RaiseCloseEventArgs(true);
    }

    protected virtual bool CanOk()
    {
        return true;
    }
}