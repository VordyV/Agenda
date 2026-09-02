using System;
using System.Diagnostics;
using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using LiveMarkdown.Avalonia;

namespace Agenda.Forms;

public partial class AboutForm : Form
{
    public AboutForm()
    {
        InitializeComponent();
        this.TextBlockAuthor.Text = Settings.Author;
        this.TextBlockReleaseYear.Text = Settings.ReleaseYear;
        this.TextBlockLicense.Text = Settings.License;
        
        Version version = Assembly.GetEntryAssembly()?.GetName().Version;
        this.LabelVersion.Content = $"Version {version}";

        this.TextBlockAbout.Markdown = Settings.TextAbout;
    }

    private void IconButtonGitHub_OnClick(object? sender, RoutedEventArgs e)
    {
        TopLevel.GetTopLevel(this)?.Launcher.LaunchUriAsync(new Uri(Settings.GithubUrl));
    }

    private void IconButtonReportBug_OnClick(object? sender, RoutedEventArgs e)
    {
        TopLevel.GetTopLevel(this)?.Launcher.LaunchUriAsync(new Uri(Settings.BugReportUrl));
    }
}