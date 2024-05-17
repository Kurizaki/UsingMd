using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using Markdig;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UsingMd
{
    public partial class MainWindow : Window
    {
        private bool isDocumentModified = false;
        private int currentThemeIndex = 0;
        private readonly string[] themeColors = { "Theme1Color", "Theme2Color", "Theme3Color", "Theme4Color", "Theme5Color", "Theme6Color" };
        private readonly string[] cssBackgroundColors = { "#FFB3B3", "#FFE6E6", "#B3FFCC", "#E6FFEB", "#B3D9FF", "#E6F3FF" };
        private readonly string[] cssTextColors = { "#2C3E50", "#34495E", "#16A085", "#27AE60", "#2980B9", "#8E44AD" };

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
            Closing += MainWindow_Closing;
        }

        private async void InitializeWebView()
        {
            await webViewEditor.EnsureCoreWebView2Async();

            string htmlFilePath = @"C:\Users\keanu\source\repos\UsingMd\UsingMd\initialContent.html";
            if (!File.Exists(htmlFilePath))
            {
                MessageBox.Show("initialContent.html not found.");
                return;
            }

            string htmlContent = File.ReadAllText(htmlFilePath);
            webViewEditor.NavigateToString(htmlContent);

            webViewEditor.CoreWebView2.WebMessageReceived += WebViewEditor_WebMessageReceived;
            webViewEditor.CoreWebView2.DOMContentLoaded += WebViewEditor_DOMContentLoaded;
        }



        private void WebViewEditor_DOMContentLoaded(object? sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            webViewEditor.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        }

        private void WebViewEditor_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string markdownText = e.TryGetWebMessageAsString();
            markdownTextBox.Text = markdownText;
            UpdatePreview(markdownText);
            isDocumentModified = true;
        }

        private void UpdatePreview(string markdownText)
        {
            string htmlContent = ConvertMarkdownToHtml(markdownText);
            if (string.IsNullOrWhiteSpace(htmlContent))
            {
                htmlContent = "<p>No content to display</p>";
            }

            webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.body.innerHTML = `{htmlContent.Replace("`", "\\`")}`;");
        }

        private static string ConvertMarkdownToHtml(string markdownText)
        {
            if (string.IsNullOrWhiteSpace(markdownText))
            {
                return string.Empty;
            }

            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
            return Markdown.ToHtml(markdownText, pipeline);
        }

        private void SaveFile_Click(object? sender, RoutedEventArgs? e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                webViewEditor.CoreWebView2.ExecuteScriptAsync("document.body.innerText")
                    .ContinueWith(t =>
                    {
                        string markdownText = t.Result.Trim('"');
                        File.WriteAllText(fileName, markdownText);
                        isDocumentModified = false;
                    });
            }
        }

        private void OpenFile_Click(object? sender, RoutedEventArgs? e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*"
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                string fileContent = File.ReadAllText(fileName);
                markdownTextBox.Text = fileContent;
                webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.body.innerText = `{fileContent.Replace("\n", "\\n")}`;");
                isDocumentModified = false;
            }
        }

        private void InsertMarkdownSyntax(string syntax)
        {
            webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, `{syntax}`);");
        }

        private void Bold_Click(object? sender, RoutedEventArgs? e) => InsertMarkdownSyntax("**Bold**");

        private void Italic_Click(object? sender, RoutedEventArgs? e) => InsertMarkdownSyntax("*Italic*");

        private void NewFile_Click(object? sender, RoutedEventArgs? e)
        {
            markdownTextBox.Clear();
            webViewEditor.CoreWebView2.ExecuteScriptAsync("document.body.innerText = '';");
            isDocumentModified = true;
        }

        private void Copy_Click(object? sender, RoutedEventArgs? e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy');");

        private void Cut_Click(object? sender, RoutedEventArgs? e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('cut');");

        private void Paste_Click(object? sender, RoutedEventArgs? e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste');");

        private void SourceCodeMode_Click(object? sender, RoutedEventArgs? e)
        {
            webViewEditor.Visibility = Visibility.Collapsed;
            markdownTextBox.Visibility = Visibility.Visible;
            markdownTextBox.Text = markdownTextBox.Text.Trim();
        }

        private void UsingMdMode_Click(object? sender, RoutedEventArgs? e)
        {
            webViewEditor.Visibility = Visibility.Visible;
            markdownTextBox.Visibility = Visibility.Collapsed;
            string markdownText = markdownTextBox.Text;
            UpdatePreview(markdownText);
        }

        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            if (isDocumentModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exiting?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    SaveFile_Click(sender, new RoutedEventArgs());
                }
                else if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            currentThemeIndex = (currentThemeIndex + 1) % themeColors.Length;
            if (Resources[themeColors[currentThemeIndex]] is Color newColor)
            {
                Resources["BackgroundColor"] = new SolidColorBrush(newColor);
                Resources["WebViewBackgroundColor"] = new SolidColorBrush((Color)Resources[themeColors[(currentThemeIndex + 1) % themeColors.Length]]);
                Resources["DarkBackgroundBrush"] = new SolidColorBrush((Color)Resources[themeColors[(currentThemeIndex + 2) % themeColors.Length]]);
                Resources["ForegroundColor"] = new SolidColorBrush((Color)Resources[cssTextColors[currentThemeIndex]]);

                string cssBackgroundColor = cssBackgroundColors[currentThemeIndex];
                string cssTextColor = cssTextColors[currentThemeIndex];
                webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.body.style.setProperty('--background-color', '{cssBackgroundColor}'); document.body.style.setProperty('--color', '{cssTextColor}');");
            }
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                var menuName = button.Content + "Menu";
                if (FindName(menuName) is Popup popup)
                {
                    popup.IsOpen = !popup.IsOpen;
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Markdown Editor\nVersion 1.0\nDeveloped by YourName", "About");
        }
    }
}
