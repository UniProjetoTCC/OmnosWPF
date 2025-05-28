using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OMNOS.Screens;
using System.Globalization;

namespace OMNOS.Pages
{
    public class Funcionario : INotifyPropertyChanged

    {
        private string _senha = string.Empty; // ATENÇÃO: Em um app real, armazene o HASH da senha, não a senha em si!
        public string Senha
        {
            get => _senha;
            set { if (_senha != value) { _senha = value; OnPropertyChanged(nameof(Senha)); } }
        }

        private string _id;
        public string Id { get => _id; set { if (_id != value) { _id = value; OnPropertyChanged(nameof(Id)); } } }

        private string _nome;
        public string Nome { get => _nome; set { if (_nome != value) { _nome = value; OnPropertyChanged(nameof(Nome)); } } }

        // Telefone REMOVIDO
        // private string _telefone;
        // public string Telefone { get => _telefone; set { if (_telefone != value) { _telefone = value; OnPropertyChanged(nameof(Telefone)); } } }

        private string _cargo;
        public string Cargo { get => _cargo; set { if (_cargo != value) { _cargo = value; OnPropertyChanged(nameof(Cargo)); } } }

        private string _email;
        public string Email { get => _email; set { if (_email != value) { _email = value; OnPropertyChanged(nameof(Email)); } } }

        // Senha REMOVIDA
        // private string _senha; 
        // public string Senha { get => _senha; set { if (_senha != value) { _senha = value; OnPropertyChanged(nameof(Senha)); } } }

        private bool _podeRealizarTransacoes;
        public bool PodeRealizarTransacoes { get => _podeRealizarTransacoes; set { if (_podeRealizarTransacoes != value) { _podeRealizarTransacoes = value; OnPropertyChanged(nameof(PodeRealizarTransacoes)); } } }

        private bool _podeGerarRelatorios;
        public bool PodeGerarRelatorios { get => _podeGerarRelatorios; set { if (_podeGerarRelatorios != value) { _podeGerarRelatorios = value; OnPropertyChanged(nameof(PodeGerarRelatorios)); } } }

        private bool _podeGerenciarProdutos;
        public bool PodeGerenciarProdutos { get => _podeGerenciarProdutos; set { if (_podeGerenciarProdutos != value) { _podeGerenciarProdutos = value; OnPropertyChanged(nameof(PodeGerenciarProdutos)); } } }

        private bool _podeAlterarEstoque;
        public bool PodeAlterarEstoque { get => _podeAlterarEstoque; set { if (_podeAlterarEstoque != value) { _podeAlterarEstoque = value; OnPropertyChanged(nameof(PodeAlterarEstoque)); } } }

        private bool _podeGerenciarPromocoes;
        public bool PodeGerenciarPromocoes { get => _podeGerenciarPromocoes; set { if (_podeGerenciarPromocoes != value) { _podeGerenciarPromocoes = value; OnPropertyChanged(nameof(PodeGerenciarPromocoes)); } } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class QuintaPag : Page
    {
        public ObservableCollection<Funcionario> MasterFuncionariosList { get; set; }
        public ICollectionView FuncionariosView { get; private set; }
        private int _nextFuncionarioIdPart = 1;

        public QuintaPag()
        {
            InitializeComponent();
            MasterFuncionariosList = new ObservableCollection<Funcionario>();
            LoadSampleFuncionarios();

            FuncionariosView = CollectionViewSource.GetDefaultView(MasterFuncionariosList);
            if (FuncionariosDataGrid != null)
            {
                FuncionariosDataGrid.ItemsSource = FuncionariosView;
            }
            UpdateFooterStatus();
        }

        private string GenerateNewFuncionarioId()
        {
            string newId = $"FNC{_nextFuncionarioIdPart:D3}";
            _nextFuncionarioIdPart++;
            while (MasterFuncionariosList.Any(f => f.Id == newId))
            {
                newId = $"FNC{_nextFuncionarioIdPart:D3}";
                _nextFuncionarioIdPart++;
            }
            return newId;
        }

        private void LoadSampleFuncionarios()
        {
            MasterFuncionariosList.Clear();
            _nextFuncionarioIdPart = 1;

            MasterFuncionariosList.Add(new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "Ana Clara Silva",
                Email = "ana.silva@omnos.com",
                Cargo = "Gerente Geral",
                PodeRealizarTransacoes = true,
                PodeGerarRelatorios = true,
                PodeGerenciarProdutos = true,
                PodeAlterarEstoque = true,
                PodeGerenciarPromocoes = true
            });
            MasterFuncionariosList.Add(new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "Bruno Oliveira Costa",
                Email = "bruno.costa@omnos.com",
                Cargo = "Vendedor Líder",
                PodeRealizarTransacoes = true,
                PodeGerarRelatorios = true,
                PodeGerenciarProdutos = false,
                PodeAlterarEstoque = false,
                PodeGerenciarPromocoes = true
            });
            MasterFuncionariosList.Add(new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "Carlos Eduardo Dias",
                Email = "carlos.dias@omnos.com",
                Cargo = "Chefe de Estoque",
                PodeRealizarTransacoes = false,
                PodeGerarRelatorios = true,
                PodeGerenciarProdutos = true,
                PodeAlterarEstoque = true,
                PodeGerenciarPromocoes = false
            });
            MasterFuncionariosList.Add(new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "Daniela Fernandes Rocha",
                Email = "daniela.rocha@omnos.com",
                Cargo = "Operadora de Caixa",
                PodeRealizarTransacoes = true,
                PodeGerarRelatorios = false,
                PodeGerenciarProdutos = false,
                PodeAlterarEstoque = false,
                PodeGerenciarPromocoes = false
            });
            MasterFuncionariosList.Add(new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "Eduardo Pereira Martins",
                Email = "eduardo.martins@omnos.com",
                Cargo = "Assistente Administrativo",
                PodeRealizarTransacoes = false,
                PodeGerarRelatorios = true,
                PodeGerenciarProdutos = false,
                PodeAlterarEstoque = false,
                PodeGerenciarPromocoes = false
            });
            UpdateFooterStatus();
        }

        private void AddFuncionarioButton_Click(object sender, RoutedEventArgs e)
        {
            Funcionario novoFuncionario = new Funcionario
            {
                Id = GenerateNewFuncionarioId(),
                Nome = "",
                Email = "",
                // Senha = "", // REMOVIDO
                // Telefone = "", // REMOVIDO
                Cargo = "Funcionário",
                PodeRealizarTransacoes = false,
                PodeGerarRelatorios = false,
                PodeGerenciarProdutos = false,
                PodeAlterarEstoque = false,
                PodeGerenciarPromocoes = false
            };

            AddEditFuncionarioWindow dialog = new AddEditFuncionarioWindow(novoFuncionario, true);
            dialog.Title = "Adicionar Novo Funcionário";

            if (dialog.ShowDialog() == true)
            {
                if (MasterFuncionariosList.Any(f => f.Id.Equals(novoFuncionario.Id, System.StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Já existe um funcionário com o ID '{novoFuncionario.Id}'.\nPor favor, utilize um ID diferente.", "ID Duplicado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                MasterFuncionariosList.Add(novoFuncionario);
                UpdateFooterStatus();
                if (FuncionariosDataGrid.Items.Count > 0)
                {
                    FuncionariosDataGrid.ScrollIntoView(novoFuncionario);
                    FuncionariosDataGrid.SelectedItem = novoFuncionario;
                }
                MessageBox.Show($"Funcionário '{novoFuncionario.Nome}' adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void EditFuncionarioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button editButton && editButton.DataContext is Funcionario funcionarioParaEditar)
            {
                AddEditFuncionarioWindow dialog = new AddEditFuncionarioWindow(funcionarioParaEditar, false);
                dialog.Title = "Editar Funcionário";

                if (dialog.ShowDialog() == true)
                {
                    FuncionariosDataGrid.SelectedItem = null;
                    FuncionariosDataGrid.UnselectAllCells();
                    MessageBox.Show($"Funcionário '{funcionarioParaEditar.Nome}' atualizado!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void DeleteFuncionarioButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.DataContext is Funcionario funcionarioParaExcluir)
            {
                MessageBoxResult confirm = MessageBox.Show(
                    $"Tem certeza que deseja excluir o funcionário '{funcionarioParaExcluir.Nome}' (ID: {funcionarioParaExcluir.Id})?",
                    "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    MasterFuncionariosList.Remove(funcionarioParaExcluir);
                    UpdateFooterStatus();
                    MessageBox.Show($"Funcionário '{funcionarioParaExcluir.Nome}' excluído!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void PerformSearch()
        {
            string searchText = SearchTextBox.Text.ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                FuncionariosView.Filter = null;
            }
            else
            {
                FuncionariosView.Filter = item =>
                {
                    if (item is Funcionario func)
                    {
                        return (func.Nome?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (func.Email?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (func.Id?.ToLowerInvariant().Contains(searchText) ?? false);
                    }
                    return false;
                };
            }
            UpdateFooterStatus();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // PerformSearch(); // Descomente para busca dinâmica
        }

        private void UpdateFooterStatus()
        {
            int itemsInViewCount = 0;
            if (FuncionariosView != null)
            {
                foreach (var _ in FuncionariosView) { itemsInViewCount++; }
            }

            if (FooterStatusText != null)
            {
                string filterStatus = (FuncionariosView?.Filter != null && itemsInViewCount < MasterFuncionariosList.Count) ? " (filtrado)" : "";
                FooterStatusText.Text = $"Mostrando {itemsInViewCount} de {MasterFuncionariosList.Count} funcionário(s){filterStatus}";
            }
        }
    }
}