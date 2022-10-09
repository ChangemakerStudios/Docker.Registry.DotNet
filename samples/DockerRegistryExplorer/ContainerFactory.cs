using Serilog;

namespace DockerRegistryExplorer
{
    using Autofac;
    using Cas.Common.WPF;
    using Cas.Common.WPF.Interfaces;
    using View;
    using ViewModel;

    public static class ContainerFactory
    {
        public static IContainer Build()
        {
            var builder = new ContainerBuilder();
            
            //view service registrations
            builder.RegisterViewModel<MainViewModel, MainWindow>();
            builder.RegisterViewModel<ConnectViewModel, ConnectView>();
            builder.RegisterViewModel<TextDialogViewModel, TextDialogView>();
            builder.RegisterViewModel<ManifestDialogViewModel, ManfiestDialogView>();

            //view models
            builder.RegisterType<RegistryViewModel>();
            builder.RegisterType<RepositoriesViewModel>();
            builder.RegisterType<RepositoryViewModel>();
            builder.RegisterType<TagViewModel>();
            builder.RegisterType<TextDialogViewModel>();
            builder.RegisterType<ManifestDialogViewModel>();
            builder.RegisterType<ManifestLayerViewModel>();

            //Services
            builder.RegisterType<MessageBoxService>().As<IMessageBoxService>().SingleInstance();
            builder.RegisterType<FileDialogService>().As<IFileDialogService>().SingleInstance();
            builder.RegisterType<TextEditService>().As<ITextEditService>().SingleInstance();
            builder.RegisterType<ViewService>().As<IViewService>().SingleInstance();

            builder.Register(
                    s =>
                    {
                        Log.Logger = new LoggerConfiguration().MinimumLevel.Verbose().WriteTo
                            .Console()
                            .WriteTo
                            .Debug().Enrich.FromLogContext().CreateLogger();

                        Log.Information("Started Up");

                        return Log.Logger;
                    }).As<ILogger>().SingleInstance()
                .AutoActivate();

            return builder.Build();
        }
    }
}