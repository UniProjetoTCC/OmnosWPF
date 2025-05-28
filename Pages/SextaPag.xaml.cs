using System.Windows;
using System.Windows.Controls;

namespace OMNOS.Pages
{
    public partial class SextaPag : Page
    {
        public SextaPag()
        {
            InitializeComponent();
        }

        private void Enable2faCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (QrCodePanel != null)
            {
                if (Enable2faCheckBox.IsChecked == true)
                {
                    QrCodePanel.Visibility = Visibility.Visible;
                }
                else
                {
                    QrCodePanel.Visibility = Visibility.Collapsed;
                    // Opcional: Limpar o campo de código de verificação se desabilitado
                    if (VerificationCodeTextBox != null)
                    {
                        VerificationCodeTextBox.Text = string.Empty;
                    }
                }
            }
        }

        private void VerifyCodeButton_Click(object sender, RoutedEventArgs e)
        {
            // Em um aplicativo real, aqui você implementaria a lógica para verificar o código 2FA
            // com o segredo associado ao QR Code gerado.
            // Para esta demonstração, apenas exibiremos uma mensagem.

            if (string.IsNullOrWhiteSpace(VerificationCodeTextBox.Text))
            {
                MessageBox.Show("Por favor, insira o código de verificação.", "Código Necessário", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Simulação de verificação
            if (VerificationCodeTextBox.Text.Length == 6 && VerificationCodeTextBox.Text.All(char.IsDigit))
            {
                MessageBox.Show($"Código '{VerificationCodeTextBox.Text}' seria verificado.\nAutenticação de Dois Fatores (DEMO) ativada com sucesso!",
                                "2FA Ativado (Demonstração)", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Código de verificação inválido. Deve conter 6 dígitos.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}