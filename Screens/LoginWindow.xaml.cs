using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace OMNOS.Screens
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                // Permite arrastar a janela se o clique não for em um controle interativo dentro do painel
                // ou se for diretamente na área de fundo/janela.
                if (e.Source == this ||
                    (e.OriginalSource is System.Windows.Controls.Border) ||
                    (e.OriginalSource is System.Windows.Controls.Grid) ||
                    (e.OriginalSource is System.Windows.Shapes.Path) || // Para ícones
                    (e.OriginalSource is System.Windows.Controls.TextBlock && ((TextBlock)e.OriginalSource).Name == null)) // Textos não nomeados
                {
                    try
                    {
                        this.DragMove();
                    }
                    catch (System.InvalidOperationException)
                    {
                        // Ignora exceção se DragMove for chamado de forma inadequada
                    }
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ForgotPassword_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Funcionalidade de recuperação de senha ainda não implementada.", "Recuperar Senha", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            // Adicionar lógica de validação real aqui
            // string username = UsernameBox.Text;
            // string password = PasswordBox.Password;
            // if (IsValidUser(username, password)) { ... }

            MainWindow home = new MainWindow();
            home.Show();
            this.Close();
        }

        private void CreateAccount_Click(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Funcionalidade de criação de conta ainda não implementada.", "Criar Conta", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}