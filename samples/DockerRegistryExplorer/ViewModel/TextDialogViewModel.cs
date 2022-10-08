namespace DockerRegistryExplorer.ViewModel
{
    using GalaSoft.MvvmLight;

    public class TextDialogViewModel : ViewModelBase
    {
        public TextDialogViewModel(string text, string title = "Text")
        {
            Text = text;
            Title = title;
        }

        public string Text { get; }

        public string Title { get; }
    }
}