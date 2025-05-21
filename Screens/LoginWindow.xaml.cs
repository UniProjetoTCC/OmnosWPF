using System.Windows;
using System.Windows.Input;

namespace LoginApp
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
            HomeWindow home = new HomeWindow();
            home.Show();
            this.Close();
        }


        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            await Task.Delay(100);

            MessageBox.Show("Bem-vindo, USER!", "Login realizado", MessageBoxButton.OK, MessageBoxImage.Information);

            // Cria a nova janela
            HomeWindow home = new HomeWindow();

            // Se a janela de login estiver ocupando toda a tela, força maximizar a nova janela
            if (this.WindowState == WindowState.Maximized ||
                (this.Width >= SystemParameters.WorkArea.Width && this.Height >= SystemParameters.WorkArea.Height))
            {
                home.WindowState = WindowState.Maximized;
            }

            home.Show();
            this.Close();
        }


    }
}
