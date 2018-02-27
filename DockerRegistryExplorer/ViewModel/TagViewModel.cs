namespace DockerRegistryExplorer.ViewModel
{
    using GalaSoft.MvvmLight;

    public class TagViewModel : ViewModelBase
    {
        public string Tag { get; }

        public TagViewModel(string tag)
        {
            Tag = tag;
        }
    }
}