namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Cas.Common.WPF;
    using Cas.Common.WPF.Interfaces;
    using GalaSoft.MvvmLight;

    public class AsyncExecutor : ViewModelBase
    {
        private bool _isBusy;

        private readonly IMessageBoxService _messageBoxService = new MessageBoxService();

        public bool IsBusy
        {
            get { return _isBusy; }
            private set
            {
                _isBusy = value;
                RaisePropertyChanged();
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
            catch (Exception ex)
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
    }
}