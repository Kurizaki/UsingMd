using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Markdig;

namespace UsingMd
{
    public partial class MainWindow : Window
    {
        private bool isDocumentModified = false;
        private int currentThemeIndex = 0;
        private readonly string[] themes = { "Theme1.xaml", "Theme2.xaml", "Theme3.xaml" };
        private bool isLightMode = true;
        private string currentMarkdownText = string.Empty;
        string projectDirectoryPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));

        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
            this.Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            MarkdownTextBox.TextChanged += (s, e) =>
            {
                currentMarkdownText = MarkdownTextBox.Text;
                isDocumentModified = true;
            };
        }

        private async void InitializeWebView()
        {
            try
            {
                await WebViewEditor.EnsureCoreWebView2Async();

                string htmlFilePath = Path.Combine(projectDirectoryPath, "initialContent.html");
                if (File.Exists(htmlFilePath))
                {
                    WebViewEditor.NavigateToString(await File.ReadAllTextAsync(htmlFilePath));
                }
                else
                {
                    MessageBox.Show($"File not found: {htmlFilePath}");
                }

                WebViewEditor.CoreWebView2InitializationCompleted += WebViewEditor_CoreWebView2InitializationCompleted;
                WebViewEditor.CoreWebView2.Settings.IsSwipeNavigationEnabled = false; // Disable swipe navigation

                // Initialize editor script
                InitializeEditorScript();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}");
            }
        }

        private void InitializeEditorScript()
        {
            string script = @"
                document.addEventListener('DOMContentLoaded', () => {
                    function makeTableEditable(table) {
                        const rows = table.rows;
                        for (let i = 0; i < rows.length; i++) {
                            const cells = rows[i].cells;
                            for (let j = 0; j < cells.length; j++) {
                                const cell = cells[j];
                                cell.setAttribute('contenteditable', 'true');
                            }
                        }
                    }

                    const observer = new MutationObserver((mutations) => {
                        mutations.forEach((mutation) => {
                            if (mutation.type === 'childList') {
                                mutation.addedNodes.forEach((node) => {
                                    if (node.nodeName === 'TABLE') {
                                        makeTableEditable(node);
                                    }
                                });
                            }
                        });
                    });

                    observer.observe(document.body, { childList: true, subtree: true });

                    const existingTable = document.querySelector('table');
                    if (existingTable) {
                        makeTableEditable(existingTable);
                    }
                });
            ";
            WebViewEditor.CoreWebView2.ExecuteScriptAsync(script);
        }

        private void WebViewEditor_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                WebViewEditor.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        string markdownContent = e.TryGetWebMessageAsString();
                        if (!string.IsNullOrEmpty(markdownContent))
                        {
                            MarkdownTextBox.Text = markdownContent;
                            currentMarkdownText = markdownContent;
                        }
                    });
                };
                UpdateWebViewTheme();
            }
            else { MessageBox.Show($"WebView2 initialization failed: {e.InitializationException.Message}"); }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                WebViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }
            WebViewEditor.Visibility = Visibility.Collapsed;
            MarkdownTextBox.Visibility = Visibility.Visible;
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
                    WebViewEditor.CoreWebView2.ExecuteScriptAsync(script);
                });
            }
        }

        private static string ConvertMarkdownToHtml(string markdownText) => Markdown.ToHtml(markdownText, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText").ContinueWith(t =>
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
                MarkdownTextBox.Text = fileContent;
                currentMarkdownText = fileContent;
                WebViewEditor.CoreWebView2.ExecuteScriptAsync($"document.getElementById('editor').innerText = `{fileContent.Replace("\n", "\\n")}`;");
                isDocumentModified = false;
            }
        }

        private void InsertMarkdownSyntax(string syntax) => WebViewEditor.CoreWebView2.ExecuteScriptAsync($"document.execCommand('insertText', false, `{syntax}`);");

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            MarkdownTextBox.Clear();
            currentMarkdownText = string.Empty;
            WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText = '';");
            isDocumentModified = true;
        }

        private void Bold_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("**Bold**");

        private void Italic_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("*Italic*");

        private void Copy_Click(object sender, RoutedEventArgs e) => WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('copy');");

        private void Cut_Click(object sender, RoutedEventArgs e) => WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('cut');");

        private void Paste_Click(object sender, RoutedEventArgs e) => WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.execCommand('paste');");

        private async void SourceCodeMode_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the WebView2 control is initialized
            if (WebViewEditor.CoreWebView2 == null)
            {
                await WebViewEditor.EnsureCoreWebView2Async(null);
            }

            string script = @"
                (function() {
                    function convertTableToMarkdown(table) {
                        let markdownTable = '';
                        const rows = table.rows;
                        for (let i = 0; i < rows.length; i++) {
                            let rowMarkdown = '| ';
                            const cells = rows[i].cells;
                            for (let j = 0; j < cells.length; j++) {
                                rowMarkdown += cells[j].textContent.trim() + ' | ';
                            }
                            markdownTable += rowMarkdown + '\n';
                            if (i === 0) {
                                markdownTable += '|---'.repeat(cells.length) + '|\n';
                            }
                        }
                        return markdownTable;
                    }

                    function convertListToMarkdown(list, marker) {
                        let markdownList = '';
                        const items = list.children;
                        for (let i = 0; i < items.length; i++) {
                            markdownList += marker + ' ' + items[i].textContent.trim() + '\n';
                            if (items[i].querySelector('ul, ol')) {
                                markdownList += convertListToMarkdown(items[i].querySelector('ul, ol'), marker === '-' ? '-' : '1.') + '\n';
                            }
                        }
                        return markdownList;
                    }

                    function convertHtmlToMarkdown() {
                        let markdown = '';
                        const body = document.body;

                        function walk(node) {
                            if (node.nodeType === Node.ELEMENT_NODE) {
                                if (node.tagName === 'TABLE') {
                                    markdown += convertTableToMarkdown(node) + '\n\n';
                                } else if (node.tagName === 'H1') {
                                    markdown += '# ' + node.textContent.trim() + '\n\n';
                                } else if (node.tagName === 'H2') {
                                    markdown += '## ' + node.textContent.trim() + '\n\n';
                                } else if (node.tagName === 'H3') {
                                    markdown += '### ' + node.textContent.trim() + '\n\n';
                                } else if (node.tagName === 'P') {
                                    markdown += node.textContent.trim() + '\n\n';
                                } else if (node.tagName === 'UL') {
                                    markdown += convertListToMarkdown(node, '-') + '\n\n';
                                } else if (node.tagName === 'OL') {
                                    markdown += convertListToMarkdown(node, '1.') + '\n\n';
                                }
                            }
                            for (let child = node.firstChild; child; child = child.nextSibling) {
                                walk(child);
                            }
                        }

                        walk(body);
                        return markdown.trim();
                    }

                    try {
                        const markdown = convertHtmlToMarkdown();
                        window.chrome.webview.postMessage(markdown);
                    } catch (error) {
                        window.chrome.webview.postMessage(`Error: ${error.message}`);
                    }
                })();
            ";

            WebViewEditor.CoreWebView2.WebMessageReceived += (s, args) =>
            {
                Dispatcher.Invoke(() =>
                {
                    string markdownContent = args.TryGetWebMessageAsString();
                    if (!string.IsNullOrEmpty(markdownContent) && !markdownContent.StartsWith("Error:"))
                    {
                        MarkdownTextBox.Text = markdownContent;
                        currentMarkdownText = markdownContent;
                    }
                    else
                    {
                        MessageBox.Show($"Error retrieving markdown content: {markdownContent}");
                    }
                });
            };

            try
            {
                // Execute the JavaScript in the WebView and capture the return value
                await WebViewEditor.CoreWebView2.ExecuteScriptAsync(script);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing script: {ex.Message}");
            }

            // Toggle visibility of WebView and TextBox
            WebViewEditor.Visibility = Visibility.Collapsed;
            MarkdownTextBox.Visibility = Visibility.Visible;
        }

        private void MarkdownMode_Click(object sender, RoutedEventArgs e)
        {
            // Save current Markdown text to the WebView editor
            string markdownText = MarkdownTextBox.Text.Replace("\n", "\\n").Replace("\"", "\\\"");
            WebViewEditor.CoreWebView2.ExecuteScriptAsync($"document.getElementById('editor').innerText = \"{markdownText}\";");

            // Toggle visibility of WebView and TextBox
            WebViewEditor.Visibility = Visibility.Visible;
            MarkdownTextBox.Visibility = Visibility.Collapsed;

            // Update preview in WebView
            UpdatePreview(MarkdownTextBox.Text);
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isDocumentModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exiting?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes) { SaveFile_Click(sender, new RoutedEventArgs()); }
                else if (result == MessageBoxResult.Cancel) { e.Cancel = true; }
            }
        }

        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            StoreCurrentMarkdown();
            currentThemeIndex = (currentThemeIndex + 1) % themes.Length;
            var newTheme = themes[currentThemeIndex];
            var newThemeDict = new ResourceDictionary { Source = new Uri(newTheme, UriKind.Relative) };
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var modeDict = dictionaries[1];
            dictionaries.Clear();
            dictionaries.Add(newThemeDict);
            dictionaries.Add(modeDict);

            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                WebViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }
            Dispatcher.Invoke(() =>
            {
                if (WebViewEditor.CoreWebView2 != null) UpdateWebViewTheme();
            });
        }

        private void ChangeMode_Click(object sender, RoutedEventArgs e)
        {
            StoreCurrentMarkdown();
            isLightMode = !isLightMode;
            var newMode = isLightMode ? "LightMode.xaml" : "DarkMode.xaml";
            var newModeDict = new ResourceDictionary { Source = new Uri(newMode, UriKind.Relative) };
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = dictionaries[0];
            dictionaries.Clear();
            dictionaries.Add(themeDict);
            dictionaries.Add(newModeDict);

            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                WebViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }
            Dispatcher.Invoke(() =>
            {
                if (WebViewEditor.CoreWebView2 != null) ReloadMarkdownContent();
            });
        }

        private void StoreCurrentMarkdown()
        {
            Dispatcher.Invoke(() =>
            {
                if (WebViewEditor.CoreWebView2 != null)
                {
                    WebViewEditor.CoreWebView2.ExecuteScriptAsync("document.getElementById('editor').innerText").ContinueWith(t => { currentMarkdownText = t.Result.Trim('"'); });
                }
            });
        }

        private void ReloadMarkdownContent()
        {
            Dispatcher.Invoke(() =>
            {
                if (WebViewEditor.CoreWebView2 != null) UpdatePreview(currentMarkdownText);
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

                WebViewEditor.CoreWebView2.ExecuteScriptAsync(script).ContinueWith(t => { ReloadMarkdownContent(); });
            }
        }

        private static string ColorToHex(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

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
