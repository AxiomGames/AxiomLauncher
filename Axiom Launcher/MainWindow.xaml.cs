using System.Windows;
using System.Windows.Controls;

namespace Axiom_Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow instance;

        public MainWindow()
        {
            instance = this;
            InitializeComponent();
        }

        public void AddProjectButton(string projectName)
        {
            Button old = ProjectsPanel.Children[0] as Button;
            Button button = new Button();
            button.Content = projectName;
            button.Height = old.Height;
            button.Width = old.Width;
            button.Margin = new Thickness(0, -old.Height-43, 0,0) ;
            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Top;
            ProjectsPanel.Children.Add(button);
        }

        /// <summary> Create new project </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateProjectWindow createProjectWindow = new CreateProjectWindow();
            createProjectWindow.Show();
        }

        // Update Engine
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }
    }
}
