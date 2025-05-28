using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using System.Globalization;
using OMNOS.Data; // Para AppData
using System.Windows.Media; // Para Brushes

namespace OMNOS.Pages
{
    public partial class TerceiraPagina : Page, INotifyPropertyChanged
    {
        private Product _searchedProduct;
        public Product SearchedProduct
        {
            get => _searchedProduct;
            set { _searchedProduct = value; OnPropertyChanged(); UpdateProductInfoPanel(); }
        }

        public ObservableCollection<Product> LowStockProductsList { get; set; }
        public string InitialSkuToSearch { get; set; }

        public TerceiraPagina()
        {
            InitializeComponent();
            AppData.EnsureDataLoaded();

            LowStockProductsList = new ObservableCollection<Product>();
            if (LowStockDataGrid != null)
            {
                LowStockDataGrid.ItemsSource = LowStockProductsList;
            }
        }

        private void TerceiraPagina_Loaded(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(InitialSkuToSearch))
            {
                if (SearchProductTextBox != null)
                {
                    SearchProductTextBox.Text = InitialSkuToSearch;
                }
                PerformProductSearch();
                InitialSkuToSearch = null;
            }
            if (LowStockThresholdTextBox != null && string.IsNullOrWhiteSpace(LowStockThresholdTextBox.Text))
            {
                LowStockThresholdTextBox.Text = "10"; // Valor padrão para o threshold
            }
        }

        private void ClearActionStatus()
        {
            if (StockActionStatusText != null)
            {
                StockActionStatusText.Text = string.Empty;
                StockActionStatusText.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowActionStatus(string message, bool isError = false)
        {
            if (StockActionStatusText != null)
            {
                StockActionStatusText.Text = message;
                StockActionStatusText.Foreground = isError ? Brushes.Red : Brushes.Green; // Vermelho para erro, Verde para sucesso
                StockActionStatusText.Visibility = Visibility.Visible;
            }
        }

        private void PerformProductSearch()
        {
            if (SearchProductTextBox == null) return;
            string searchTerm = SearchProductTextBox.Text.Trim();
            ClearActionStatus();

            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                SearchedProduct = null;
                if (SearchProductErrorText != null)
                {
                    SearchProductErrorText.Text = "Por favor, insira um SKU ou Nome para buscar.";
                    SearchProductErrorText.Visibility = Visibility.Visible;
                }
                if (ProductInfoPanel != null) ProductInfoPanel.Visibility = Visibility.Collapsed;
                return;
            }

            SearchedProduct = AppData.MasterProductsList.FirstOrDefault(p =>
                                     (p.SKU?.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) ?? false) ||
                                     (p.Name?.ToLowerInvariant().Contains(searchTerm.ToLowerInvariant()) ?? false));

            if (SearchedProduct == null)
            {
                if (SearchProductErrorText != null)
                {
                    SearchProductErrorText.Text = $"Nenhum produto encontrado para o termo '{searchTerm}'.";
                    SearchProductErrorText.Visibility = Visibility.Visible;
                }
                if (ProductInfoPanel != null) ProductInfoPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (SearchProductErrorText != null) SearchProductErrorText.Visibility = Visibility.Collapsed;
                // UpdateProductInfoPanel é chamado pelo setter de SearchedProduct
            }
        }

        private void UpdateProductInfoPanel()
        {
            if (ProductInfoPanel != null && FoundProductSkuText != null && FoundProductNameText != null && CurrentStockText != null)
            {
                ClearActionStatus();
                if (SearchedProduct != null)
                {
                    FoundProductSkuText.Text = SearchedProduct.SKU;
                    FoundProductNameText.Text = SearchedProduct.Name;
                    CurrentStockText.Text = SearchedProduct.Stock.ToString();
                    ProductInfoPanel.Visibility = Visibility.Visible;
                    StockChangeReasonTextBox.Clear();
                    SetStockQuantityTextBox.Clear();
                    AddStockQuantityTextBox.Clear();
                    RemoveStockQuantityTextBox.Clear();
                    if (SearchProductErrorText != null) SearchProductErrorText.Visibility = Visibility.Collapsed;
                }
                else
                {
                    ProductInfoPanel.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SearchProductButton_Click(object sender, RoutedEventArgs e)
        {
            PerformProductSearch();
        }

        // Validação genérica para o campo de motivo
        private bool ValidateReasonField()
        {
            if (string.IsNullOrWhiteSpace(StockChangeReasonTextBox.Text))
            {
                ShowActionStatus("O campo 'Motivo da Movimentação' é obrigatório.", true);
                StockChangeReasonTextBox.Focus();
                return false;
            }
            return true;
        }

        // Validação genérica para campos de quantidade
        private bool ValidateQuantityField(TextBox quantityTextBox, string fieldName, out int quantity, bool allowZeroForSet = false)
        {
            quantity = 0;
            if (quantityTextBox == null || string.IsNullOrWhiteSpace(quantityTextBox.Text))
            {
                ShowActionStatus($"O campo '{fieldName}' é obrigatório.", true);
                quantityTextBox?.Focus();
                return false;
            }
            if (!int.TryParse(quantityTextBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out quantity))
            {
                ShowActionStatus($"Valor inválido para '{fieldName}'. Insira um número inteiro.", true);
                quantityTextBox.Focus();
                return false;
            }
            if (!allowZeroForSet && quantity <= 0) // Para Adicionar/Remover, quantidade deve ser > 0
            {
                ShowActionStatus($"'{fieldName}' deve ser um número inteiro positivo maior que zero.", true);
                quantityTextBox.Focus();
                return false;
            }
            if (allowZeroForSet && quantity < 0) // Para Definir, quantidade deve ser >= 0
            {
                ShowActionStatus($"'{fieldName}' deve ser um número inteiro não negativo.", true);
                quantityTextBox.Focus();
                return false;
            }
            return true;
        }


        private void SetStockButton_Click(object sender, RoutedEventArgs e)
        {
            ClearActionStatus();
            if (SearchedProduct == null) { ShowActionStatus("Primeiro, busque um produto.", true); return; }

            // Valida campo de quantidade
            if (!ValidateQuantityField(SetStockQuantityTextBox, "Definir Qtd. Total", out int newQuantity, allowZeroForSet: true)) return;

            // Valida campo de motivo (agora obrigatório para Definir também)
            if (!ValidateReasonField()) return;

            int oldStock = SearchedProduct.Stock;
            int quantityChanged = newQuantity - oldStock;

            if (quantityChanged == 0)
            {
                ShowActionStatus($"O estoque do produto '{SearchedProduct.Name}' já é {newQuantity}. Nenhuma alteração foi feita.");
                // Não precisa limpar SetStockQuantityTextBox aqui, pois UpdateProductInfoPanel fará se a ação for bem-sucedida
                return;
            }

            StockMovementType movementType = quantityChanged > 0 ? StockMovementType.Entrada : StockMovementType.Saida;
            string reason = StockChangeReasonTextBox.Text.Trim(); // Pega o motivo do TextBox

            SearchedProduct.Stock = newQuantity;
            AppData.LogStockMovement(SearchedProduct, Math.Abs(quantityChanged), movementType, reason);

            ShowActionStatus($"Estoque do produto '{SearchedProduct.Name}' definido para {newQuantity}. Motivo: {reason}", false);
            UpdateProductInfoPanel(); // Limpa campos e atualiza info
        }

        private void AddStockButton_Click(object sender, RoutedEventArgs e)
        {
            ClearActionStatus();
            if (SearchedProduct == null) { ShowActionStatus("Primeiro, busque um produto.", true); return; }

            // Valida campo de quantidade
            if (!ValidateQuantityField(AddStockQuantityTextBox, "Adicionar Qtd.", out int quantityToAdd)) return;

            // Valida campo de motivo
            if (!ValidateReasonField()) return;

            string reason = StockChangeReasonTextBox.Text.Trim();
            SearchedProduct.Stock += quantityToAdd;
            AppData.LogStockMovement(SearchedProduct, quantityToAdd, StockMovementType.Entrada, reason);

            ShowActionStatus($"{quantityToAdd} unidade(s) adicionada(s) ao estoque de '{SearchedProduct.Name}'. Novo estoque: {SearchedProduct.Stock}. Motivo: {reason}", false);
            UpdateProductInfoPanel(); // Limpa campos e atualiza info
        }

        private void RemoveStockButton_Click(object sender, RoutedEventArgs e)
        {
            ClearActionStatus();
            if (SearchedProduct == null) { ShowActionStatus("Primeiro, busque um produto.", true); return; }

            // Valida campo de quantidade
            if (!ValidateQuantityField(RemoveStockQuantityTextBox, "Remover Qtd.", out int quantityToRemove)) return;

            // Valida campo de motivo
            if (!ValidateReasonField()) return;

            if (SearchedProduct.Stock >= quantityToRemove)
            {
                string reason = StockChangeReasonTextBox.Text.Trim();
                SearchedProduct.Stock -= quantityToRemove;
                AppData.LogStockMovement(SearchedProduct, quantityToRemove, StockMovementType.Saida, reason);

                ShowActionStatus($"{quantityToRemove} unidade(s) removida(s) do estoque de '{SearchedProduct.Name}'. Novo estoque: {SearchedProduct.Stock}. Motivo: {reason}", false);
                UpdateProductInfoPanel(); // Limpa campos e atualiza info
            }
            else { ShowActionStatus("Quantidade a remover é maior que o estoque atual.", true); }
        }

        private void GenerateLowStockReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (LowStockReportErrorText != null) LowStockReportErrorText.Visibility = Visibility.Collapsed;
            if (LowStockDataGridContainer != null) LowStockDataGridContainer.Visibility = Visibility.Collapsed;
            LowStockProductsList.Clear();

            if (LowStockThresholdTextBox != null && int.TryParse(LowStockThresholdTextBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out int threshold) && threshold >= 0)
            {
                var lowStockItems = AppData.MasterProductsList.Where(p => p.Stock < threshold).OrderBy(p => p.Stock).ToList();

                foreach (var item in lowStockItems) { LowStockProductsList.Add(item); }

                if (!lowStockItems.Any())
                {
                    if (LowStockReportErrorText != null)
                    {
                        LowStockReportErrorText.Text = $"Nenhum produto encontrado com estoque abaixo de {threshold}.";
                        LowStockReportErrorText.Visibility = Visibility.Visible;
                    }
                }
                else
                {
                    if (LowStockDataGridContainer != null) LowStockDataGridContainer.Visibility = Visibility.Visible;
                }
            }
            else
            {
                if (LowStockReportErrorText != null)
                {
                    LowStockReportErrorText.Text = "Por favor, insira um limite de estoque válido (número inteiro não negativo).";
                    LowStockReportErrorText.Visibility = Visibility.Visible;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
