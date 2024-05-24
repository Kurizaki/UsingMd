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
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            markdownTextBox.TextChanged += MarkdownTextBox_TextChanged;
        }

        private void MarkdownTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string markdownText = markdownTextBox.Text;
            UpdatePreview(markdownText);
            isDocumentModified = true;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
                {
                    webViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
                }
                else
                {
                    MessageBox.Show("WebViewBackgroundColor resource not found or is not a Color.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void WebViewEditor_DOMContentLoaded(object sender, CoreWebView2DOMContentLoadedEventArgs e)
        {
            webViewEditor.CoreWebView2.AddWebResourceRequestedFilter("*", CoreWebView2WebResourceContext.All);
        }

        private void WebViewEditor_WebMessageReceived(object sender, CoreWebView2WebMessageReceivedEventArgs e)
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

            string cssBackgroundColor = cssBackgroundColors[currentThemeIndex];
            string cssTextColor = cssTextColors[currentThemeIndex];
            string script = $@"
        document.body.innerHTML = `<div contenteditable='true' id='editor'>{htmlContent.Replace("`", "\\`")}</div>`;
        document.body.style.setProperty('--background-color', '{cssBackgroundColor}');
        document.body.style.setProperty('--color', '{cssTextColor}');
        document.body.style.backgroundColor = '{cssBackgroundColor}';
        document.body.style.color = '{cssTextColor}';

        var tables = document.getElementsByTagName('table');
        for (var i = 0; i < tables.length; i++) {{
            tables[i].style.borderCollapse = 'collapse';
            tables[i].style.width = '100%';
            tables[i].style.border = '1px solid #ddd';
        }}
        var thtd = document.querySelectorAll('th, td');
        for (var i = 0; i < thtd.length; i++) {{
            thtd[i].style.border = '1px solid #ddd';
            thtd[i].style.padding = '8px';
            thtd[i].style.textAlign = 'left';
        }}
        var th = document.getElementsByTagName('th');
        for (var i = 0; i < th.length; i++) {{
            th[i].style.backgroundColor = '#f2f2f2';
        }}
    ";

            webViewEditor.CoreWebView2.ExecuteScriptAsync(script);
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

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*"
            };
            if (saveFileDialog.ShowDialog() == true)
            {
                string fileName = saveFileDialog.FileName;
                webViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText")
                    .ContinueWith(t =>
                    {
                        string markdownText = t.Result.Trim('"');
                        File.WriteAllText(fileName, markdownText);
                        isDocumentModified = false;
                    });
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
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
                webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.getElementById('editor').innerText = `{fileContent.Replace("\n", "\\n")}`;");
                isDocumentModified = false;
            }
        }

        private void InsertMarkdownSyntax(string syntax)
        {
            webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, `{syntax}`);");
        }

        private void Bold_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("**Bold**");

        private void Italic_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("*Italic*");

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            markdownTextBox.Clear();
            webViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText = '';");
            isDocumentModified = true;
        }

        private void Copy_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy');");

        private void Cut_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('cut');");

        private void Paste_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste');");

        private void SourceCodeMode_Click(object sender, RoutedEventArgs e)
        {
            webViewEditor.Visibility = Visibility.Collapsed;
            markdownTextBox.Visibility = Visibility.Visible;
            markdownTextBox.Text = markdownTextBox.Text.Trim();
        }

        private void UsingMdMode_Click(object sender, RoutedEventArgs e)
        {
            webViewEditor.Visibility = Visibility.Visible;
            markdownTextBox.Visibility = Visibility.Collapsed;
            string markdownText = markdownTextBox.Text;
            UpdatePreview(markdownText);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
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
                Resources["WebViewBackgroundColor"] = new SolidColorBrush(newColor);
                Resources["DarkBackgroundBrush"] = new SolidColorBrush((Color)Resources[themeColors[(currentThemeIndex + 2) % themeColors.Length]]);
                Resources["ForegroundColor"] = new SolidColorBrush((Color)Resources[cssTextColors[currentThemeIndex]]);

                string cssBackgroundColor = cssBackgroundColors[currentThemeIndex];
                string cssTextColor = cssTextColors[currentThemeIndex];
                string script = $@"
                    document.body.style.setProperty('--background-color', '{cssBackgroundColor}');
                    document.body.style.setProperty('--color', '{cssTextColor}');
                    document.body.style.backgroundColor = '{cssBackgroundColor}';
                    document.body.style.color = '{cssTextColor}';
                ";

                webViewEditor.CoreWebView2.ExecuteScriptAsync(script);
            }
        }

        private CustomPopupPlacement[] FileMenu_CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            double horizontalOffset = -6;
            var belowAndAligned = new CustomPopupPlacement(new Point(horizontalOffset, targetSize.Height), PopupPrimaryAxis.Horizontal);
            return new[] { belowAndAligned };
        }

        private CustomPopupPlacement[] EditMenu_CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            double horizontalOffset = -6;
            var belowAndAligned = new CustomPopupPlacement(new Point(horizontalOffset, targetSize.Height), PopupPrimaryAxis.Horizontal);
            return new[] { belowAndAligned };
        }

        private CustomPopupPlacement[] ViewMenu_CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            double horizontalOffset = -6;
            var belowAndAligned = new CustomPopupPlacement(new Point(horizontalOffset, targetSize.Height), PopupPrimaryAxis.Horizontal);
            return new[] { belowAndAligned };
        }

        private CustomPopupPlacement[] HelpMenu_CustomPopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            double horizontalOffset = -6;
            var belowAndAligned = new CustomPopupPlacement(new Point(horizontalOffset, targetSize.Height), PopupPrimaryAxis.Horizontal);
            return new[] { belowAndAligned };
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                switch (button.Name)
                {
                    case "FileButton":
                        FileMenu.IsOpen = !FileMenu.IsOpen;
                        break;
                    case "EditButton":
                        EditMenu.IsOpen = !EditMenu.IsOpen;
                        break;
                    case "ViewButton":
                        ViewMenu.IsOpen = !ViewMenu.IsOpen;
                        break;
                    case "HelpButton":
                        HelpMenu.IsOpen = !HelpMenu.IsOpen;
                        break;
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Markdown Editor\nVersion 1.0\nDeveloped by Kurizaki", "About");
        }
    }
}