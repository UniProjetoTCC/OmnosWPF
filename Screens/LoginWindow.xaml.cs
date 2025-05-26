using System.Windows;
using System.Windows.Input;

namespace OMNOS.Screens
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

      

        // Evento para clicar e arrastar a janela pela barra superior customizada
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }

        // Evento do botão "X" para fechar a janela
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Clique em "Forgot Password?"
        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Funcionalidade de recuperação de senha ainda não implementada.");
        }
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            
            MainWindow home = new MainWindow();
            home.Show();
            this.Close();
        }


    }
}
