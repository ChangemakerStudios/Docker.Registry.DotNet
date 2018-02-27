namespace DockerRegistryExplorer
{
    using Autofac;
    using Cas.Common.WPF;
    using Cas.Common.WPF.Interfaces;
    using ViewModel;

    public static class ContainerFactory
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();

            
            //view service registrations
            builder.RegisterViewModel<MainViewModel, MainWindow>();

            //view models
            builder.RegisterType<RegistryViewModel>();
            builder.RegisterType<RepositoriesViewModel>();
            builder.RegisterType<RepositoryViewModel>();
            builder.RegisterType<TagViewModel>();

            //Services
            builder.RegisterType<MessageBoxService>().As<IMessageBoxService>().SingleInstance();
            builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
            builder.RegisterType<TextEditService>().As<ITextEditService>().SingleInstance();
            builder.RegisterType<ViewService>().As<IViewService>().SingleInstance();

            return builder.Build();
        }
    }
}