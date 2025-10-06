namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using Cas.Common.WPF.Behaviors;

    public class CloseableViewModelBase : ObservableObject, ICloseableViewModel
    {
        public virtual bool CanClose()
        {
            return true;
        }

        public virtual void Closed()
        {
        }

        protected void RaiseCloseEventArgs(bool? dialogResult)
        {
            Close?.Invoke(this, new CloseEventArgs(dialogResult));
        }

        public event EventHandler<CloseEventArgs> Close;
    }
}