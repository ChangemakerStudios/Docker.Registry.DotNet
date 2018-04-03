namespace DockerRegistryExplorer.ViewModel
{
    using System;
    using Cas.Common.WPF.Behaviors;
    using GalaSoft.MvvmLight;

    public class CloseableViewModelBase : ViewModelBase, ICloseableViewModel
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