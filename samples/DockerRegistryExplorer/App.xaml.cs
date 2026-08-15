namespace DockerRegistryExplorer
{
    using ViewModel;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            using (var container = ContainerFactory.Build())
            {
                var viewService = container.Resolve<IViewService>();

                var mainViewModel = container.Resolve<MainViewModel>();

                viewService.ShowDialog(mainViewModel);
            }
        }
    }
}
