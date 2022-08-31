using System.Windows;
using System.Windows.Controls;
using System.IO;
using Axiom_Launcher.SaveLoad;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Diagnostics;
using System.Windows.Input;

namespace Axiom_Launcher
{
    public partial class MainWindow : Window
    {
        public static MainWindow instance { get; private set; }
        private const string CMAKE_GAME_NAME = "GAME_NAME";

        internal static string EnginePath = string.Empty;
        public static string ExePath => EnginePath + "/build/AxiomEditor/Debug/AxiomEditor.exe";

        private static string cmakeListsTxt;
        private static string CMakeListsText
        {
            get 
            {
                if (string.IsNullOrEmpty(cmakeListsTxt))
                {
                    cmakeListsTxt = File.ReadAllText("Project Template/CMakeLists.txt");
                }
                return cmakeListsTxt;
            }
        }

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

        private readonly Button projectButton;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
            
            projectButton = ProjectsPanel.Children[0] as Button;
            ProjectsPanel.Children.Clear();
            SearchProjects();
        }

        private static String GetDirectoryName(String directory)
        {
            int i = directory.Length - 1;
            while (i-- > 2)
            {
                if (directory[i] == '\\' || directory[i] == '/')
                    break;
            }
            return directory.Substring(i + 1, directory.Length - i - 1);
        }

        private static bool IsProject(string directory)
        {
            return Directory.Exists(directory) &&
                Directory.Exists(directory + "/Assets") && 
                Directory.Exists(directory + "/CPPSource");
        }

        private void SearchProjects()
        {
            string projectsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "/Axiom Games"; 
            if (Directory.Exists(projectsPath))
            {
                foreach (var dir in Directory.GetDirectories(projectsPath))
                {
                    if (IsProject(dir))
                    {
                        CreateProject(GetDirectoryName(dir), dir, false);
                    }
                }
            }
        }

#region Project
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
            button.Click += EngineStart;
            Project project = new Project(projectName, projectDirectory, buttonGUID);
            button.Tag = project;

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

        public void CreateProject(string projectName, string projectDirectory, bool? useDefaultAssets)
        {
            EnsureEnginePath();

            CreateProjectButton(projectName, projectDirectory); // also adds button to pannel

            // Create Template Project
            if (!Directory.Exists(projectDirectory))
            {
                Directory.CreateDirectory(projectDirectory);
            }

            Directory.CreateDirectory(projectDirectory + "/Assets");
            Directory.CreateDirectory(projectDirectory + "/CSharpSource");

            string axiomInclude = EnginePath + "/Axiom/src/";
            string projectInclude;

            // create Folders
            {
                string cppSrcFolder = Directory.CreateDirectory(projectDirectory + "/CPPSource").FullName;

                string sourceSrc = Directory.CreateDirectory(cppSrcFolder + "/src").FullName;
                projectInclude = Directory.CreateDirectory(cppSrcFolder + "/include").FullName;
                Directory.CreateDirectory(cppSrcFolder + "/lib");


                using StreamWriter mainCPPStream = File.CreateText(sourceSrc + "/Main.cpp");
                mainCPPStream.Write("#include <iostream>\n\nint main()\n{\n\n}");

                using StreamWriter cmakeStream = File.CreateText(cppSrcFolder + "/CMakeLists.txt");
                cmakeStream.Write(CMakeListsText.Replace(CMAKE_GAME_NAME, projectName));
            }

            // copy header files to project's include folder recursively

            void CopyHeadersRec(string path)
            {
                string newAxiom = axiomInclude + path;
                string newProject = projectInclude + "/" + path;

                Directory.CreateDirectory(newProject);

                foreach (string file in Directory.GetFiles(newAxiom))
                {
                    if (Path.GetExtension(file) == ".hpp")
                    {
                        using StreamWriter writer = File.CreateText(newProject + Path.GetFileName(file));
                        writer.Write(File.ReadAllText(file));
                    }
                }

                foreach (string directory in Directory.GetDirectories(newAxiom))
                {
                    CopyHeadersRec(path + "/" + GetDirectoryName(directory) + "/");
                }
            }

            CopyHeadersRec("Axiom/");
            axiomInclude = EnginePath + "/AxiomEditor/src/";
            CopyHeadersRec("AxiomEditor/");

            if (useDefaultAssets.Value == true)
            {

            }
        }

        private void ProjectDelete(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Are you sure to delete Project?", "Warning", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

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

            Directory.Delete(project.directory);
        }

        private void ProjectGoToFile(object sender, RoutedEventArgs e)
        {
            Project project = (e.Source as MenuItem).Tag as Project;
            Process.Start(new ProcessStartInfo(project.directory) { UseShellExecute = true });
        }
        #endregion

        #region Engine
        public static void EnsureEnginePath()
        {
            if (PlayerPrefs.TryGetStringValue("EnginePath", out EnginePath) || !string.IsNullOrEmpty(EnginePath)) return;

            // look common folders
            string repoPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/source/repos/Axiom";
            if (Directory.Exists(repoPath))
            {
                EnginePath = repoPath;
                PlayerPrefs.SetString("EnginePath", EnginePath);
            }
            else
            {
            search_engine_path:
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    if (IsProject(dialog.FileName))
                    {
                        EnginePath = dialog.FileName;
                        PlayerPrefs.SetString("EnginePath", EnginePath);
                    }
                    else if (MessageBox.Show("Axiom engine path is wrong!\n" +
                                             "you want to search again?",
                                             "Warning", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        goto search_engine_path;
                }
            }
        }

        private void EngineStart(object sender, RoutedEventArgs e)
        {
            Project project = (e.Source as Button).Tag as Project;

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.Arguments = @$"-editor -{project.directory}";
            startInfo.FileName = ExePath;

            Process.Start(startInfo);

            //Task.Delay(5000).ContinueWith(t =>Application.Current.Shutdown());
        }
        #endregion 
        private void CreateNewProjectButtonClick(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow createProjectWindow = new CreateProjectWindow();
            createProjectWindow.Owner = this;
            createProjectWindow.Show();
        }

        private void UpdateEngineButtonClick(object sender, RoutedEventArgs e)
        {

        }

        private void SettingsClick(object s, RoutedEventArgs e)
        { 
            
        }
    }
}
