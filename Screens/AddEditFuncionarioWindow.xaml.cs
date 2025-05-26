using System.Windows;
using OMNOS.Pages;
using System.Globalization;

namespace OMNOS.Screens
{
    public partial class AddEditFuncionarioWindow : Window
    {
        public Funcionario FuncionarioEditado { get; private set; }
        private bool _isNewFuncionario;

        public AddEditFuncionarioWindow(Funcionario funcionario, bool isNew = false)
        {
            InitializeComponent();
            FuncionarioEditado = funcionario;
            _isNewFuncionario = isNew;
            LoadFuncionarioData();

            if (Application.Current.MainWindow != this && Application.Current.MainWindow != null)
            {
                Owner = Application.Current.MainWindow;
            }

            if (!_isNewFuncionario)
            {
                IdTextBox.IsEnabled = false;
            }
        }

        private void LoadFuncionarioData()
        {
            if (FuncionarioEditado != null)
            {
                IdTextBox.Text = FuncionarioEditado.Id;
                NomeTextBox.Text = FuncionarioEditado.Nome;
                // TelefoneTextBox.Text removido
                CargoTextBox.Text = FuncionarioEditado.Cargo;
                EmailTextBox.Text = FuncionarioEditado.Email;

                // SenhaPasswordBox.Password e ConfirmarSenhaPasswordBox.Password removidos

                PodeRealizarTransacoesCheckBox.IsChecked = FuncionarioEditado.PodeRealizarTransacoes;
                PodeGerarRelatoriosCheckBox.IsChecked = FuncionarioEditado.PodeGerarRelatorios;
                PodeGerenciarProdutosCheckBox.IsChecked = FuncionarioEditado.PodeGerenciarProdutos;
                PodeAlterarEstoqueCheckBox.IsChecked = FuncionarioEditado.PodeAlterarEstoque;
                PodeGerenciarPromocoesCheckBox.IsChecked = FuncionarioEditado.PodeGerenciarPromocoes;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(IdTextBox.Text) && _isNewFuncionario)
            {
                MessageBox.Show("O campo 'ID/Matrícula' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                IdTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(NomeTextBox.Text))
            {
                MessageBox.Show("O campo 'Nome Completo' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                NomeTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(CargoTextBox.Text))
            {
                MessageBox.Show("O campo 'Cargo/Nível' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                CargoTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(EmailTextBox.Text))
            {
                MessageBox.Show("O campo 'Email (Login)' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                EmailTextBox.Focus();
                return;
            }

            // Lógica de Senha REMOVIDA

            if (_isNewFuncionario || IdTextBox.IsEnabled)
            {
                FuncionarioEditado.Id = IdTextBox.Text;
            }
            FuncionarioEditado.Nome = NomeTextBox.Text;
            // FuncionarioEditado.Telefone = TelefoneTextBox.Text; // REMOVIDO
            FuncionarioEditado.Cargo = CargoTextBox.Text;
            FuncionarioEditado.Email = EmailTextBox.Text;
            // FuncionarioEditado.Senha = ... // REMOVIDO

            FuncionarioEditado.PodeRealizarTransacoes = PodeRealizarTransacoesCheckBox.IsChecked ?? false;
            FuncionarioEditado.PodeGerarRelatorios = PodeGerarRelatoriosCheckBox.IsChecked ?? false;
            FuncionarioEditado.PodeGerenciarProdutos = PodeGerenciarProdutosCheckBox.IsChecked ?? false;
            FuncionarioEditado.PodeAlterarEstoque = PodeAlterarEstoqueCheckBox.IsChecked ?? false;
            FuncionarioEditado.PodeGerenciarPromocoes = PodeGerenciarPromocoesCheckBox.IsChecked ?? false;

            this.DialogResult = true;
            this.Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}