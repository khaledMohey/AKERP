using System.IO;
using System.Text.Json;
using System.Windows;

namespace AKERP.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        var appUrl = ResolveServerUrl();

        try
        {
            await WebView.EnsureCoreWebView2Async();
            WebView.CoreWebView2.Settings.AreDevToolsEnabled = true;
            WebView.Source = new Uri(appUrl);
            StatusText.Text = $"وضع إيجار محلي — متصل: {appUrl}";
        }
        catch (Exception ex)
        {
            StatusText.Text = "تعذر الاتصال بالسيرفر";
            MessageBox.Show(
                $"تأكد أن سيرفر AKERP شغال على الشبكة، وعدّل ملف server.json بجانب البرنامج.\n\nالعنوان الحالي: {appUrl}\n\n{ex.Message}",
                "AKERP Desktop",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }

    private static string ResolveServerUrl()
    {
        try
        {
            var path = Path.Combine(AppContext.BaseDirectory, "server.json");
            if (File.Exists(path))
            {
                using var doc = JsonDocument.Parse(File.ReadAllText(path));
                if (doc.RootElement.TryGetProperty("ServerUrl", out var url) &&
                    !string.IsNullOrWhiteSpace(url.GetString()))
                {
                    return url.GetString()!.Trim().TrimEnd('/');
                }
            }
        }
        catch
        {
            // fall through to default
        }

        return "http://127.0.0.1:5088";
    }
}
