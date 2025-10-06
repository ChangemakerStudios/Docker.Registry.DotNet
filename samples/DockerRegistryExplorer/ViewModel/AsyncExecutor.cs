using Serilog;

namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Threading.Tasks;

    using Cas.Common.WPF;

    public class AsyncExecutor : ObservableObject
    {
        private bool _isBusy;

        private readonly IMessageBoxService _messageBoxService = new MessageBoxService();

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public async Task<Exception> ExecuteAsync(Func<Task> action)
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

        private bool LogError(Exception ex)
        {
            Log.Error(ex, "Failure Executing Task");

            return true;
        }
    }
}