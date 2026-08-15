using Cas.Common.WPF;

using Serilog;

namespace DockerRegistryExplorer.ViewModel;

public class AsyncExecutor : ObservableObject
{
    private readonly IMessageBoxService _messageBoxService = new MessageBoxService();

    private bool _isBusy;

    public bool IsBusy
    {
        get => _isBusy;
        private set
        {
            _isBusy = value;
            OnPropertyChanged();
        }
    }

    public async Task<Exception?> ExecuteAsync(Func<Task> action)
    {
        try
        {
            IsBusy = true;

            await action();

            return null;
        }
        catch (Exception ex) when (LogError(ex))
        {
            _messageBoxService.Show(ex.Message, "Error");

            return ex;
        }
        finally
        {
            IsBusy = false;

            CommandManager.InvalidateRequerySuggested();
        }
    }

    private static bool LogError(Exception ex)
    {
        Log.Error(ex, "Failure Executing Task");

        return true;
    }
}
