using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Diagnostics;
using System;
using System.IO;

namespace Axiom_Launcher
{
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        class Project
        {
            public string name;
            public string directory;
            public Guid guid;

            public Project(string name, string directory, Guid guid)
            {
                this.name = name;
                this.directory = directory;
                this.guid = guid;
            }
        }

        private Button projectButton;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            projectButton = ProjectsPanel.Children[0] as Button;
            ProjectsPanel.Children.Clear();
        }

        private void CreateProjectButton(string projectName, string projectDirectory)
        { 
            Guid buttonGUID = Guid.NewGuid();

            Button button = new Button();
            button.Content = projectName;
            button.Height = projectButton.Height;
            button.Width = projectButton.Width;
            button.Margin = new Thickness(5);

            button.HorizontalAlignment = HorizontalAlignment.Left;
            button.VerticalAlignment = VerticalAlignment.Top;
            button.Tag = buttonGUID;

            Project project = new Project(projectName, projectDirectory, buttonGUID);

            button.ContextMenu = new ContextMenu();
            
            {
                MenuItem deleteMenuItem = new MenuItem();
                deleteMenuItem.Header = "Delete Project";
                deleteMenuItem.Tag = project;
                deleteMenuItem.PreviewMouseDown += ProjectDelete;
                button.ContextMenu.Items.Add(deleteMenuItem);
            }
            { 
                MenuItem menuItem = new MenuItem();
                menuItem.Header = "Open Project Folder";
                menuItem.Tag = project;
                menuItem.PreviewMouseDown += ProjectGoToFile;
                button.ContextMenu.Items.Add(menuItem);
            }
           
            ProjectsPanel.Children.Add(button);
        }


        public void AddProjectButton(string projectName, string projectDirectory, bool? useDefaultAssets)
        {
            CreateProjectButton(projectName, projectDirectory); // also adds to pannel

            // Create Template Project
            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            return;

            Directory.CreateDirectory(projectDirectory + Path.DirectorySeparatorChar + "Assets");
            var cppSrcFolder = Directory.CreateDirectory(projectDirectory + Path.DirectorySeparatorChar + "CPPSource");
            var csharpSrcFonlder = Directory.CreateDirectory(projectDirectory + Path.DirectorySeparatorChar + "CSharpSource");

            File.Create(cppSrcFolder.FullName + "");

            if (useDefaultAssets == true)
            {

            }

        }
        
        private void CreateNewClickButtonClick(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow createProjectWindow = new CreateProjectWindow();
            createProjectWindow.Show();
        }

        private void ProjectDelete(object sender, MouseButtonEventArgs e)
        {
            Project project = (e.Source as MenuItem).Tag as Project;

            System.Collections.IList list = ProjectsPanel.Children;

            for (int i = 0; i < list.Count; i++)
            {
                Button button = list[i] as Button;
                if ((Guid)button.Tag == project.guid)
                {
                    ProjectsPanel.Children.RemoveAt(i);
                    break;
                }
            }
        }

        private void ProjectGoToFile(object sender, RoutedEventArgs e)
        {
            Project project = (e.Source as MenuItem).Tag as Project;
            Process.Start(new ProcessStartInfo(project.directory) { UseShellExecute = true });
        }

        private void UpdateEngineButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsClick(object s, RoutedEventArgs e)
        { 
            
        }
    }
}
