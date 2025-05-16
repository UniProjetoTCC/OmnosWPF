using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OmnosWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window    
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GoToNextScreen(object sender, RoutedEventArgs e)
        {
            Window1 newWindow = new Window1();
            newWindow.Show();
            this.Close(); // ou use Hide() se quiser manter a janela aberta
        }
    }
}