using System;
using System.IO;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Axiom_Launcher
{
    public partial class CreateProjectWindow : Window
    {
        static string LastPath = null;
        static readonly string axiomGamesPath;

        static CreateProjectWindow()
        {
            axiomGamesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Axiom Games";
            if (!Directory.Exists(axiomGamesPath))
            {
                Directory.CreateDirectory(axiomGamesPath);
            }
        }

        public CreateProjectWindow()
        {
            InitializeComponent();
            ProjectDirectoryTextBox.Text = axiomGamesPath;
        }

        private void CreateClick(object sender, RoutedEventArgs e)
        {
            if (Path.GetFileNameWithoutExtension(ProjectDirectoryTextBox.Text) != ProjectNameTextBox.Text)
            {
                ProjectDirectoryTextBox.Text += Path.DirectorySeparatorChar + ProjectNameTextBox.Text;
            }
            LastPath = ProjectDirectoryTextBox.Text;

            MainWindow.instance.CreateProject(ProjectNameTextBox.Text, ProjectDirectoryTextBox.Text, IncludeAssetsCheckbox.IsChecked);
            Close();
        }

        private void FindClick(object sender, RoutedEventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = !string.IsNullOrEmpty(LastPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : LastPath;
            dialog.IsFolderPicker = true;
            LastPath = dialog.InitialDirectory;
            
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ProjectDirectoryTextBox.Text = dialog.FileName;
            }
        }
    }
}
