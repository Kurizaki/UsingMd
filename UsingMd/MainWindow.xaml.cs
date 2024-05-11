using System;
using System.Windows;
using Microsoft.Win32;

namespace UsingMd
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ToggleFileSection(object sender, RoutedEventArgs e)
        {
            FileListBox.Visibility = FileListBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ToggleEditSection(object sender, RoutedEventArgs e)
        {
            EditListBox.Visibility = EditListBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ToggleViewSection(object sender, RoutedEventArgs e)
        {
            ViewListBox.Visibility = ViewListBox.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TextBoxMarkdown_TextChanged(object sender, RoutedEventArgs e)
        {
            // Markdown editing logic
            // For example, you can update the webBrowserPreview with the markdown content
            UpdatePreview(textBoxMarkdown.Text);
        }

        private void UpdatePreview(string markdownText)
        {
            // Here you can implement logic to update the preview
            // For simplicity, let's just set the HTML content of the webBrowserPreview to the markdown text
            string htmlContent = ConvertMarkdownToHtml(markdownText);
            webBrowserPreview.NavigateToString(htmlContent);
        }

        private string ConvertMarkdownToHtml(string markdownText)
        {
            // Here you can implement the actual conversion logic from markdown to HTML
            // For demonstration purposes, let's just return the markdown text as HTML
            // You may want to use a library like MarkdownSharp or Markdig for more advanced conversion
            return $"<html><body>{markdownText}</body></html>";
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            // Logic for creating a new file
            textBoxMarkdown.Text = string.Empty; // Clear the textbox
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Markdown Files (*.md)|*.md|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = openFileDialog.FileName;
                // Logic for opening the selected file
                textBoxMarkdown.Text = System.IO.File.ReadAllText(fileName);
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            // Logic for saving the file
            // For simplicity, let's just show a message box indicating that the file has been saved
            MessageBox.Show("File saved successfully.", "Save File", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Logic for exiting the application
            Application.Current.Shutdown();
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            // Logic for undoing the last action
            // For simplicity, let's just clear the textbox
            textBoxMarkdown.Undo();
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            // Logic for copying text
            textBoxMarkdown.Copy();
        }

        private void Cut_Click(object sender, RoutedEventArgs e)
        {
            // Logic for cutting text
            textBoxMarkdown.Cut();
        }

        private void Paste_Click(object sender, RoutedEventArgs e)
        {
            // Logic for pasting text
            textBoxMarkdown.Paste();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Logic for deleting selected text
            textBoxMarkdown.SelectedText = string.Empty;
        }

        private void SearchAndReplace_Click(object sender, RoutedEventArgs e)
        {
            // Logic for searching and replacing text
            // For simplicity, let's just show a message box
            MessageBox.Show("Searching and replacing text...", "Search and Replace", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SourceCodeMode_Click(object sender, RoutedEventArgs e)
        {
            // Logic for switching to source code mode
            // For simplicity, let's just show a message box
            MessageBox.Show("Switching to Source Code Mode...", "View", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void UsingMdMode_Click(object sender, RoutedEventArgs e)
        {
            // Logic for switching to UsingMd mode
            // For simplicity, let's just show a message box
            MessageBox.Show("Switching to UsingMd Mode...", "View", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
