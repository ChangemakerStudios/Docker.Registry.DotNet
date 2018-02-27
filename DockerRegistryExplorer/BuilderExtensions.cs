namespace DockerRegistryExplorer
{
    using System.Windows;
    using Autofac;
    using Cas.Common.WPF;

    public static class BuilderExtensions
    {
        public static void RegisterViewModel<TViewModel, TView>(this ContainerBuilder builder)
            where TView : Window, new()
        {
            //Registry the view model itself
            builder.RegisterType<TViewModel>();

            //Make a registration for the view model service
            builder.RegisterInstance(ViewServiceRegistrationFactory.Create<TViewModel, TView>());
        }
    }
}