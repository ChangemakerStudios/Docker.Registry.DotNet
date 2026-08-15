namespace DockerRegistryExplorer.ViewModel;

public class TextDialogViewModel : ObservableObject
{
    public TextDialogViewModel(string text, string title = "Text")
    {
        Text = text;
        Title = title;
    }

    public string Text { get; }

    public string Title { get; }
}