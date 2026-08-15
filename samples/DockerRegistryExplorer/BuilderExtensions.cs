using Cas.Common.WPF;

namespace DockerRegistryExplorer;

public static class BuilderExtensions
{
    public static void RegisterViewModel<TViewModel, TView>(this ContainerBuilder builder)
        where TViewModel : notnull
        where TView : Window, new()
    {
        //Register the view model itself
        builder.RegisterType<TViewModel>();

        //Make a registration for the view model service
        builder.RegisterInstance(ViewServiceRegistrationFactory.Create<TViewModel, TView>());
    }
}