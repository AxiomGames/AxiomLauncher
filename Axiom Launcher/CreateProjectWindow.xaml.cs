using System.Windows;
using System.Windows.Controls;

namespace Axiom_Launcher
{
    /// <summary>
    /// Interaction logic for CreateProjectWindow.xaml
    /// </summary>
    public partial class CreateProjectWindow : Window
    {
        public CreateProjectWindow()
        {
            InitializeComponent();
        }

        // Create Project
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            MainWindow.instance.AddProjectButton(ProjectNameTextBox.Text);
            Close();
        }

        //Close
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
