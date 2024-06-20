using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Win32;
using Markdig;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace UsingMd
{
    public partial class MainWindow : Window
    {
        // Variables to track document modification, theme index, themes array, mode, and markdown text
        private bool _isDocumentModified;
        private int _currentThemeIndex;
        private readonly string[] _themes = { "Theme1.xaml", "Theme2.xaml", "Theme3.xaml" };
        private bool _isLightMode = true;
        private string _currentMarkdownText = string.Empty;
        private readonly string _projectDirectoryPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));

        // Constructor initializes the MainWindow, sets up event handlers, and initializes WebView
        public MainWindow()
        {
            InitializeComponent();
            InitializeWebView();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            MarkdownTextBox.TextChanged += (_, _) => { _currentMarkdownText = MarkdownTextBox.Text; _isDocumentModified = true; };
        }

        /// <summary>
        /// Initializes the WebView and loads the initial HTML content.
        /// </summary>
        private async void InitializeWebView()
        {
            try
            {
                // Ensures WebView2 core is initialized
                await WebViewEditor.EnsureCoreWebView2Async();

                // Loads the initial HTML content from a file
                string htmlFilePath = Path.Combine(_projectDirectoryPath, "initialContent.html");
                if (File.Exists(htmlFilePath))
                {
                    WebViewEditor.NavigateToString(await File.ReadAllTextAsync(htmlFilePath));
                }
                else
                {
                    MessageBox.Show($"File not found: {htmlFilePath}");
                }

                // Sets up event handler for WebView initialization completion
                WebViewEditor.CoreWebView2InitializationCompleted += WebViewEditor_CoreWebView2InitializationCompleted;
                WebViewEditor.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
                LoadMarkdownToWeb();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView2: {ex.Message}");
            }
        }

        /// <summary>
        /// Makes tables in the WebView editable.
        /// </summary>
        private void LoadMarkdownToWeb()
        {
            Dispatcher.InvokeAsync(() =>
            {
                // JavaScript code to make tables in WebView content editable
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
            });
        }

        /// <summary>
        /// Handles WebView initialization completion event.
        /// </summary>
        private void WebViewEditor_CoreWebView2InitializationCompleted(object sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            if (e.IsSuccess)
            {
                // Sets up WebMessageReceived event handler
                WebViewEditor.CoreWebView2.WebMessageReceived += (_, args) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        string markdownContent = args.TryGetWebMessageAsString();
                        if (!string.IsNullOrEmpty(markdownContent))
                        {
                            MarkdownTextBox.Text = markdownContent;
                            _currentMarkdownText = markdownContent;
                        }
                    });
                };
                UpdateWebViewTheme();
            }
            else
            {
                MessageBox.Show($"WebView2 initialization failed: {e.InitializationException.Message}");
            }
        }

        /// <summary>
        /// Handles MainWindow Loaded event.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Sets the WebView background color based on application resources
            if (Application.Current.Resources["WebViewBackgroundColor"] is Color webViewBackgroundColor)
            {
                WebViewEditor.DefaultBackgroundColor = System.Drawing.Color.FromArgb(webViewBackgroundColor.A, webViewBackgroundColor.R, webViewBackgroundColor.G, webViewBackgroundColor.B);
            }
            WebViewEditor.Visibility = Visibility.Collapsed;
            MarkdownTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Converts Markdown text to HTML.
        /// </summary>
        private static string ConvertMarkdownToHtml(string markdownText) => Markdown.ToHtml(markdownText, new MarkdownPipelineBuilder().UseAdvancedExtensions().Build());

        /// <summary>
        /// Saves the current Markdown text to a file.
        /// </summary>
        private async void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            await SynchronizeMarkdown();
            var saveFileDialog = new SaveFileDialog { Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, MarkdownTextBox.Text);
                _isDocumentModified = false;
            }
        }

        /// <summary>
        /// Opens a Markdown file and loads its content.
        /// </summary>
        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog { Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileContent = File.ReadAllText(openFileDialog.FileName);
                MarkdownTextBox.Text = fileContent;
                _currentMarkdownText = fileContent;
                LoadMarkdownToWeb();
                _isDocumentModified = false;
            }
        }

        /// <summary>
        /// Inserts Markdown syntax at the current selection in the text box.
        /// </summary>
        private async void InsertMarkdownSyntax(string startSyntax, string endSyntax = "")
        {
            int selectionStart = MarkdownTextBox.SelectionStart;
            int selectionLength = MarkdownTextBox.SelectionLength;
            string selectedText = MarkdownTextBox.SelectedText;
            string newText = startSyntax + selectedText + endSyntax;
            MarkdownTextBox.Text = MarkdownTextBox.Text.Remove(selectionStart, selectionLength).Insert(selectionStart, newText);
            MarkdownTextBox.SelectionStart = selectionStart + newText.Length;
            MarkdownTextBox.Focus();
            _currentMarkdownText = MarkdownTextBox.Text;
            await SynchronizeMarkdown();
            LoadMarkdownToWeb();
        }

        /// <summary>
        /// Clears the text box to create a new file.
        /// </summary>
        private async void NewFile_Click(object sender, RoutedEventArgs e)
        {
            MarkdownTextBox.Clear();
            await SynchronizeMarkdown();
            _currentMarkdownText = string.Empty;
            _isDocumentModified = true;
            LoadMarkdownToWeb();
        }

        /// <summary>
        /// Makes selected text bold.
        /// </summary>
        private void Bold_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("**", "**");

        /// <summary>
        /// Makes selected text italic.
        /// </summary>
        private void Italic_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("*", "*");

        /// <summary>
        /// Inserts a code snippet block.
        /// </summary>
        private void CodeSnippet_Click(object sender, RoutedEventArgs e) => InsertMarkdownSyntax("```\n", "\n```");

        /// <summary>
        /// Copies selected text.
        /// </summary>
        private void Copy_Click(object sender, RoutedEventArgs e) => Clipboard.SetText(MarkdownTextBox.SelectedText);

        /// <summary>
        /// Cuts selected text.
        /// </summary>
        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(MarkdownTextBox.SelectedText);
            MarkdownTextBox.SelectedText = string.Empty;
        }

        /// <summary>
        /// Pastes text from the clipboard.
        /// </summary>
        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            int selectionStart = MarkdownTextBox.SelectionStart;
            MarkdownTextBox.Text = MarkdownTextBox.Text.Insert(selectionStart, Clipboard.GetText());
            MarkdownTextBox.SelectionStart = selectionStart + Clipboard.GetText().Length;
        }

        /// <summary>
        /// Synchronizes the current Markdown text with the WebView.
        /// </summary>
        private async Task SynchronizeMarkdown()
        {
            if (WebViewEditor.CoreWebView2 == null)
            {
                await WebViewEditor.EnsureCoreWebView2Async(null);
            }

            // JavaScript code to convert HTML content to Markdown format
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

            if (WebViewEditor.CoreWebView2 != null)
            {
                var tcs = new TaskCompletionSource<string>();

                void WebMessageReceivedHandler(object sender, CoreWebView2WebMessageReceivedEventArgs args)
                {
                    string markdownContent = args.TryGetWebMessageAsString();
                    if (!string.IsNullOrEmpty(markdownContent) && !markdownContent.StartsWith("Error:"))
                    {
                        tcs.TrySetResult(markdownContent);
                    }
                    else
                    {
                        tcs.TrySetException(new Exception($"Error retrieving markdown content: {markdownContent}"));
                    }

                    WebViewEditor.CoreWebView2.WebMessageReceived -= WebMessageReceivedHandler;
                }

                WebViewEditor.CoreWebView2.WebMessageReceived += WebMessageReceivedHandler;

                try
                {
                    await WebViewEditor.CoreWebView2.ExecuteScriptAsync(script);
                    _currentMarkdownText = await tcs.Task;
                    MarkdownTextBox.Text = _currentMarkdownText;
                }
                catch (Exception ex)
                {
                    WebViewEditor.CoreWebView2.WebMessageReceived -= WebMessageReceivedHandler;
                    MessageBox.Show($"Error executing script: {ex.Message}");
                }
            }
            else
            {
                MarkdownTextBox.Text = string.Empty;
            }
        }

        /// <summary>
        /// Switches to Markdown mode.
        /// </summary>
        private void MarkdownMode_Click(object sender, RoutedEventArgs e)
        {
            LoadMarkdownToWeb();
            string htmlContent = ConvertMarkdownToHtml(MarkdownTextBox.Text);
            string script = $@"
                document.body.innerHTML = `<div contenteditable='true' id='editor'>{htmlContent.Replace("`", "\\`")}</div>`;
            ";
            WebViewEditor.CoreWebView2.ExecuteScriptAsync(script);
            WebViewEditor.Visibility = Visibility.Visible;
            MarkdownTextBox.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Switches to source code mode.
        /// </summary>
        private async void SourceCodeMode_Click(object sender, RoutedEventArgs e)
        {
            await SynchronizeMarkdown();
            WebViewEditor.Visibility = Visibility.Collapsed;
            MarkdownTextBox.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Handles MainWindow Closing event.
        /// </summary>
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            if (_isDocumentModified)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before exiting?", "Unsaved Changes", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes) { SaveFile_Click(sender, new RoutedEventArgs()); }
                else if (result == MessageBoxResult.Cancel) { e.Cancel = true; }
            }
        }

        /// <summary>
        /// Changes the theme.
        /// </summary>
        private void ChangeTheme_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(async () =>
            {
                await SynchronizeMarkdown();
                _currentThemeIndex = (_currentThemeIndex + 1) % _themes.Length;
                var newTheme = _themes[_currentThemeIndex];
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
                UpdateWebViewTheme();
                LoadMarkdownToWeb();
            });
        }

        /// <summary>
        /// Changes the display mode (light/dark).
        /// </summary>
        private void ChangeMode_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.Invoke(async () =>
            {
                await SynchronizeMarkdown();
                _isLightMode = !_isLightMode;
                var newMode = _isLightMode ? "LightMode.xaml" : "DarkMode.xaml";
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
                UpdateWebViewTheme();
                LoadMarkdownToWeb();
            });
        }

        /// <summary>
        /// Updates the WebView theme based on the current application theme.
        /// </summary>
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
                WebViewEditor.CoreWebView2.ExecuteScriptAsync(script).ContinueWith(_ => { LoadMarkdownToWeb(); });
            }
        }

        /// <summary>
        /// Converts a Color object to a hex string.
        /// </summary>
        private static string ColorToHex(Color color) => $"#{color.R:X2}{color.G:X2}{color.B:X2}";

        /// <summary>
        /// Toggles the visibility of the menu.
        /// </summary>
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

        /// <summary>
        /// Displays an About dialog.
        /// </summary>
        private void About_Click(object sender, RoutedEventArgs e) => MessageBox.Show("Markdown Editor\nVersion 1.0\nDeveloped by Kurizaki", "About");

        /// <summary>
        /// Provides custom popup placement for menus.
        /// </summary>
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
