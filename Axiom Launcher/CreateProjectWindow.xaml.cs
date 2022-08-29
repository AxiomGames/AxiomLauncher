﻿using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;

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
            ProjectDirectoryTextBox.Text += Path.DirectorySeparatorChar + ProjectNameTextBox.Text;
            LastPath = ProjectDirectoryTextBox.Text;

            MainWindow.instance.AddProjectButton(ProjectNameTextBox.Text, ProjectDirectoryTextBox.Text, IncludeAssetsCheckbox.IsChecked);
            Close();
        }

        private void FindClick(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = !string.IsNullOrEmpty(LastPath) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : LastPath;
            dialog.ValidateNames = false;
            dialog.CheckFileExists = false;
            dialog.CheckPathExists = true;

            LastPath = dialog.InitialDirectory;

            if (dialog.ShowDialog() != false)
            {
                ProjectDirectoryTextBox.Text = dialog.FileName;
            }
        }
    }
}
