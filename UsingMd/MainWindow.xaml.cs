using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using System;
using System.ComponentModel;
using System.IO;
using Microsoft.Win32;
using Markdig;
using System.Threading.Tasks;

namespace UsingMd
{
    public partial class MainWindow : Window
    {
        private bool isDocumentModified = false;
        private int currentThemeIndex = 0;
        private readonly string[] themes = { "Theme1.xaml", "Theme2.xaml", "Theme3.xaml" };
        private bool isLightMode = true;
        private string currentMarkdownText = string.Empty;

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            markdownTextBox.TextChanged += (s, e) =>
            {
                currentMarkdownText = markdownTextBox.Text;
                UpdatePreview(currentMarkdownText);
                isDocumentModified = true;
            };
        }

        private async void InitializeWebView()
        {
            try
            {
                await webViewEditor.EnsureCoreWebView2Async();
                string htmlFilePath = @"C:\Users\keanu\source\repos\UsingMd\UsingMd\initialContent.html";
                if (File.Exists(htmlFilePath))
                {
                    webViewEditor.NavigateToString(File.ReadAllText(htmlFilePath));
                }
                webViewEditor.CoreWebView2InitializationCompleted += WebViewEditor_CoreWebView2InitializationCompleted;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}");
            }
        }

        private void WebViewEditor_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                webViewEditor.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        markdownTextBox.Text = e.TryGetWebMessageAsString();
                        isDocumentModified = true;
                    });
                };

                // Now it's safe to call UpdateWebViewTheme
                UpdateWebViewTheme();
            }
            else
            {
                MessageBox.Show($"WebView2 initialization failed: {e.InitializationException.Message}");
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                webViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }
        }

        private void UpdatePreview(string markdownText)
        {
            if (!string.IsNullOrEmpty(markdownText))
            {
                Dispatcher.Invoke(() =>
                {
                    string htmlContent = ConvertMarkdownToHtml(markdownText);
                    string script = $@"
                        document.body.innerHTML = `<div contenteditable='true' id='editor'>{htmlContent.Replace("`", "\\`")}</div>`;
                        var script = document.createElement('script');
                        script.src = 'editor.js';
                        document.body.appendChild(script);";
                    webViewEditor.CoreWebView2.ExecuteScriptAsync(script);
                });
            }
        }

        private static string ConvertMarkdownToHtml(string markdownText) => Markdown.ToHtml(markdownText, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                webViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText").ContinueWith(t =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        File.WriteAllText(saveFileDialog.FileName, t.Result.Trim('"'));
                        isDocumentModified = false;
                    });
                });
            }
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileContent = File.ReadAllText(openFileDialog.FileName);
                markdownTextBox.Text = fileContent;
                currentMarkdownText = fileContent; // Update the current markdown text
                webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.getElementById('editor').innerText = `{fileContent.Replace("\n", "\\n")}`;");
                isDocumentModified = false;
            }
        }

        private void InsertMarkdownSyntax(string syntax) => webViewEditor.CoreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, `{syntax}`);");

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            markdownTextBox.Clear();
            currentMarkdownText = string.Empty; // Clear the current markdown text
            webViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText = '';");
            isDocumentModified = true;
        }

        private void Bold_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("**Bold**");

        private void Italic_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("*Italic*");

        private void Copy_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy');");

        private void Cut_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('cut');");

        private void Paste_Click(object sender, RoutedEventArgs e) => webViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste');");

        private void SourceCodeMode_Click(object sender, RoutedEventArgs e)
        {
            webViewEditor.Visibility = Visibility.Collapsed;
            markdownTextBox.Visibility = Visibility.Visible;
        }

        private void MarkdownMode_Click(object sender, RoutedEventArgs e)
        {
            webViewEditor.Visibility = Visibility.Visible;
            markdownTextBox.Visibility = Visibility.Collapsed;
            UpdatePreview(markdownTextBox.Text);
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
            // Store the current Markdown content
            StoreCurrentMarkdown();

            // Cycle through themes
            currentThemeIndex = (currentThemeIndex + 1) % themes.Length;
            var newTheme = themes[currentThemeIndex];

            var newThemeDict = new ResourceDictionary { Source = new Uri(newTheme, UriKind.Relative) };

            // Remove the current theme dictionary and add the new one
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var modeDict = dictionaries[1]; // Keep the mode dictionary
            dictionaries.Clear();
            dictionaries.Add(newThemeDict);
            dictionaries.Add(modeDict);

            // Update WebView background color if defined in the new theme
            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                webViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }

            // Ensure WebView is initialized before updating theme
            Dispatcher.Invoke(() =>
            {
                if (webViewEditor.CoreWebView2 != null)
                {
                    UpdateWebViewTheme();
                }
            });
        }

        private void ChangeMode_Click(object sender, RoutedEventArgs e)
        {
            // Store the current Markdown content
            StoreCurrentMarkdown();

            // Toggle between light and dark modes
            isLightMode = !isLightMode;
            var newMode = isLightMode ? "LightMode.xaml" : "DarkMode.xaml";

            var newModeDict = new ResourceDictionary { Source = new Uri(newMode, UriKind.Relative) };

            // Remove the current mode dictionary and add the new one
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = dictionaries[0]; // Keep the theme dictionary
            dictionaries.Clear();
            dictionaries.Add(themeDict);
            dictionaries.Add(newModeDict);

            // Update WebView background color if defined in the new mode
            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                webViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }

            // Ensure WebView is initialized before updating mode
            Dispatcher.Invoke(() =>
            {
                if (webViewEditor.CoreWebView2 != null)
                {
                    // No need to update WebViewTheme here since we're only changing the mode
                    ReloadMarkdownContent();
                }
            });
        }

        private void StoreCurrentMarkdown()
        {
            Dispatcher.Invoke(() =>
            {
                if (webViewEditor.CoreWebView2 != null)
                {
                    webViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText").ContinueWith(t =>
                    {
                        currentMarkdownText = t.Result.Trim('"');
                    });
                }
            });
        }

        private void ReloadMarkdownContent()
        {
            Dispatcher.Invoke(() =>
            {
                if (webViewEditor.CoreWebView2 != null)
                {
                    UpdatePreview(currentMarkdownText);
                }
            });
        }

        private void UpdateWebViewTheme()
        {
            if (Application.Current.Resources["BackgroundColor"] is SolidColorBrush backgroundColorBrush &&
                Application.Current.Resources["TextColor"] is SolidColorBrush textColorBrush)
            {
                string backgroundColor = ColorToHex(backgroundColorBrush.Color);
                string textColor = ColorToHex(textColorBrush.Color);

                string script = $@"
                    document.documentElement.style.setProperty('--background-color', '{backgroundColor}');
                    document.documentElement.style.setProperty('--color', '{textColor}');
                ";

                webViewEditor.CoreWebView2.ExecuteScriptAsync(script).ContinueWith(t =>
                {
                    // Reapply the stored Markdown content
                    ReloadMarkdownContent();
                });
            }
        }

        private static string ColorToHex(Color color)
        {
            return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
        }

        private void ToggleMenu_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    case "FileButton": FileMenu.IsOpen = !FileMenu.IsOpen; break;
                    case "EditButton": EditMenu.IsOpen = !EditMenu.IsOpen; break;
                    case "ViewButton": ViewMenu.IsOpen = !ViewMenu.IsOpen; break;
                    case "HelpButton": HelpMenu.IsOpen = !HelpMenu.IsOpen; break;
                }
            }
        }

        private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Markdown Editor\nVersion 1.0\nDeveloped by Kurizaki", "About");

        private CustomPopupPlacement[] PopupPlacementCallback(Size popupSize, Size targetSize, Point offset)
        {
            return new CustomPopupPlacement[]
            {
                new CustomPopupPlacement(new Point(0, targetSize.Height), PopupPrimaryAxis.Horizontal),
                new CustomPopupPlacement(new Point(targetSize.Width, targetSize.Height), PopupPrimaryAxis.Horizontal)
            };
        }
    }
}
