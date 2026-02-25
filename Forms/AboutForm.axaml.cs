using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using LiveMarkdown.Avalonia;

namespace Agenda.Forms;

public partial class AboutForm : UserControl
{
    public AboutForm()
    {
        InitializeComponent();
        var markdownBuilder = new ObservableStringBuilder();
        
        this.MarkdownContent.MarkdownBuilder = markdownBuilder;
        markdownBuilder.Append(Settings.TextAbout);
    }
}