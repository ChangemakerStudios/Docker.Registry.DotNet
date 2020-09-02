namespace DockerRegistryExplorer.View
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for ConnectView.xaml
    /// </summary>
    public partial class ConnectView 
    {
        public ConnectView()
        {
            InitializeComponent();
        }

        private void ConnectView_OnLoaded(object sender, RoutedEventArgs e)
        {
            EndpointTextBox.Focus();
        }
    }
}
