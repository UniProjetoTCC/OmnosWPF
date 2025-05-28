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
using OMNOS.Screens; // Para EditProductWindow
using OMNOS.Data;    // Para AppData

namespace OMNOS.Pages
{
    // A classe Product DEVE estar em um arquivo separado Product.cs 
    // no namespace OMNOS.Pages (ou OMNOS.Models com o using ajustado)

    public partial class SecondPage : Page, INotifyPropertyChanged
    {
        public ICollectionView ProductsView { get; private set; }
        public ObservableCollection<Product> PagedProductsList { get; set; }

        private const int PageSize = 20;
        private int _currentPage = 1;
        private int _filteredItemCount = 0;
        private int _totalPages = 0;

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                int newPage = value;
                if (TotalPages > 0)
                {
                    if (newPage < 1) newPage = 1;
                    if (newPage > TotalPages) newPage = TotalPages;
                }
                else
                {
                    newPage = 1;
                }

                if (_currentPage != newPage)
                {
                    _currentPage = newPage;
                    OnPropertyChanged();
                    RefreshPagedViewDataAndControls();
                }
                else if (_currentPage == newPage && (PagedProductsList == null || PagedProductsList.Count == 0) && _filteredItemCount > 0 && _currentPage > 0)
                {
                    RefreshPagedViewDataAndControls();
                }
            }
        }

        public int TotalPages
        {
            get => _totalPages;
            private set { _totalPages = value; OnPropertyChanged(); }
        }

        public string PageInfoText => TotalPages > 0 && _filteredItemCount > 0 ? $"Página {CurrentPage} de {TotalPages}" : (_filteredItemCount == 0 ? "Nenhum produto" : "Página 1 de 1");

        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public SecondPage()
        {
            InitializeComponent();
            AppData.EnsureDataLoaded();

            PagedProductsList = new ObservableCollection<Product>();
            ProductsView = CollectionViewSource.GetDefaultView(AppData.MasterProductsList);

            if (ProductsDataGrid != null)
            {
                ProductsDataGrid.ItemsSource = PagedProductsList;
            }

            this.DataContext = this;
            RefreshPagedViewDataAndControls();
        }

        private void RefreshPagedViewDataAndControls()
        {
            List<Product> itemsToPaginate;
            if (ProductsView != null)
            {
                itemsToPaginate = ProductsView.Cast<Product>().ToList();
            }
            else
            {
                itemsToPaginate = AppData.MasterProductsList.ToList();
            }

            _filteredItemCount = itemsToPaginate.Count;

            int newTotalPages = (_filteredItemCount + PageSize - 1) / PageSize;
            TotalPages = (newTotalPages == 0) ? 1 : newTotalPages;

            if (_currentPage > TotalPages)
            {
                _currentPage = TotalPages;
            }
            if (_currentPage < 1)
            {
                _currentPage = 1;
            }
            OnPropertyChanged(nameof(CurrentPage));

            PagedProductsList.Clear();
            int skip = (_currentPage - 1) * PageSize;
            var pageItems = itemsToPaginate.Skip(skip).Take(PageSize);

            foreach (var item in pageItems)
            {
                PagedProductsList.Add(item);
            }

            UpdatePaginationControlsState();
            UpdateFooterStatus();
        }

        private void UpdatePaginationControlsState()
        {
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(PageInfoText));
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            Product novoProduto = new Product
            {
                SKU = "",
                Name = "",
                Categoria = "Diversos",
                Price = 0,
                Stock = 0
            };

            EditProductWindow addProductDialog = new EditProductWindow(novoProduto, true);
            addProductDialog.Title = "Adicionar Novo Produto";

            if (addProductDialog.ShowDialog() == true)
            {
                if (string.IsNullOrWhiteSpace(novoProduto.SKU))
                {
                    MessageBox.Show("O SKU não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (AppData.MasterProductsList.Any(p => p.SKU.Equals(novoProduto.SKU, StringComparison.OrdinalIgnoreCase)))
                {
                    MessageBox.Show($"Já existe um produto com o SKU '{novoProduto.SKU}'.\nPor favor, utilize um SKU diferente.", "SKU Duplicado", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                AppData.MasterProductsList.Add(novoProduto);

                if (ProductsView.Filter != null)
                {
                    ProductsView.Refresh();
                }

                _filteredItemCount = ProductsView.Cast<Product>().Count();
                int newTotalPages = (_filteredItemCount + PageSize - 1) / PageSize;
                if (newTotalPages == 0) newTotalPages = 1;

                CurrentPage = newTotalPages;

                if (PagedProductsList.Contains(novoProduto))
                {
                    ProductsDataGrid.ScrollIntoView(novoProduto);
                    ProductsDataGrid.SelectedItem = novoProduto;
                }
                MessageBox.Show($"Produto '{novoProduto.Name}' (SKU: {novoProduto.SKU}) adicionado!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button detailsButton && detailsButton.DataContext is Product selectedProduct)
            {
                if (selectedProduct != null && !string.IsNullOrEmpty(selectedProduct.SKU))
                {
                    if (this.NavigationService != null)
                    {
                        TerceiraPagina stockPage = new TerceiraPagina();
                        stockPage.InitialSkuToSearch = selectedProduct.SKU;
                        this.NavigationService.Navigate(stockPage);
                    }
                    else
                    {
                        MessageBox.Show("Serviço de navegação não disponível.", "Erro");
                    }
                }
            }
        }

        private void PerformSearch()
        {
            string searchText = SearchTextBox.Text.ToLowerInvariant().Trim();
            if (string.IsNullOrWhiteSpace(searchText))
            {
                ProductsView.Filter = null;
            }
            else
            {
                ProductsView.Filter = item =>
                {
                    if (item is Product product)
                    {
                        return (product.Name?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (product.SKU?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (product.Categoria?.ToLowerInvariant().Contains(searchText) ?? false);
                    }
                    return false;
                };
            }

            if (CurrentPage != 1) CurrentPage = 1;
            else RefreshPagedViewDataAndControls();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            PerformSearch();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.DataContext is Product productToDelete)
            {
                MessageBoxResult confirm = MessageBox.Show($"Tem certeza que deseja excluir o produto '{productToDelete.Name}' (SKU: {productToDelete.SKU})?",
                    "Confirmar Exclusão", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    AppData.MasterProductsList.Remove(productToDelete);
                    if (ProductsView.Filter != null) ProductsView.Refresh();

                    RefreshPagedViewDataAndControls();

                    MessageBox.Show($"Produto '{productToDelete.Name}' excluído!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button editButton && editButton.DataContext is Product productToEdit)
            {
                EditProductWindow editWindow = new EditProductWindow(productToEdit, false);
                editWindow.Title = "Editar Produto";
                if (editWindow.ShowDialog() == true)
                {
                    ProductsView.Refresh();
                    RefreshPagedViewDataAndControls();
                    MessageBox.Show($"Produto '{productToEdit.Name}' atualizado com sucesso!", "Edição Concluída", MessageBoxButton.OK, MessageBoxImage.Information);

                    if (ProductsDataGrid != null)
                    {
                        ProductsDataGrid.SelectedItem = null;
                        ProductsDataGrid.UnselectAllCells();
                    }
                }
            }
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoPrevious) CurrentPage--;
        }

        private void NextPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (CanGoNext) CurrentPage++;
        }

        private void ProductsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            string sortBy = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(sortBy) || ProductsView == null) return;

            ListSortDirection direction;
            if (e.Column.SortDirection == ListSortDirection.Ascending)
            {
                direction = ListSortDirection.Descending;
            }
            else
            {
                direction = ListSortDirection.Ascending;
            }

            ProductsView.SortDescriptions.Clear();
            ProductsView.SortDescriptions.Add(new SortDescription(sortBy, direction));

            // Adicionado ProductsView.Refresh() para garantir que a UI atualize após a ordenação.
            ProductsView.Refresh();

            e.Column.SortDirection = direction;

            foreach (var col in ProductsDataGrid.Columns)
            {
                if (col.SortMemberPath != sortBy)
                {
                    col.SortDirection = null;
                }
            }

            if (CurrentPage != 1)
            {
                CurrentPage = 1;
            }
            else
            {
                RefreshPagedViewDataAndControls();
            }

            e.Handled = true;
        }

        private void UpdateFooterStatus()
        {
            if (FooterStartItemRun == null || FooterEndItemRun == null || FooterTotalFilteredItemsRun == null || FooterProdutoSingularPluralRun == null || FooterFilterStatusRun == null) return;

            int startItem = 0, endItem = 0;
            string produtoText = " produto(s)";

            if (_filteredItemCount > 0 && PagedProductsList.Count > 0)
            {
                startItem = (_currentPage - 1) * PageSize + 1;
                endItem = startItem + PagedProductsList.Count - 1;
            }

            if (_filteredItemCount == 1) produtoText = " produto";
            else if (_filteredItemCount > 1) produtoText = " produtos";
            else if (AppData.MasterProductsList.Count == 0 && _filteredItemCount == 0) produtoText = " produto";
            else produtoText = " produtos";

            FooterStartItemRun.Text = startItem.ToString();
            FooterEndItemRun.Text = endItem.ToString();
            FooterTotalFilteredItemsRun.Text = _filteredItemCount.ToString();
            FooterProdutoSingularPluralRun.Text = produtoText;

            string filterStatusText = "";
            if (ProductsView?.Filter != null && _filteredItemCount < AppData.MasterProductsList.Count && AppData.MasterProductsList.Count > 0)
                filterStatusText = " (filtrado)";
            else if (_filteredItemCount == 0 && ProductsView?.Filter != null && AppData.MasterProductsList.Count > 0)
                filterStatusText = " (Nenhum resultado para o filtro)";
            else if (_filteredItemCount == 0 && (ProductsView?.Filter == null || AppData.MasterProductsList.Count == 0))
            {
                if (AppData.MasterProductsList.Count == 0) filterStatusText = " (Lista Vazia)";
            }
            FooterFilterStatusRun.Text = filterStatusText;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            if (propertyName == nameof(CurrentPage) || propertyName == nameof(TotalPages))
            {
                UpdatePaginationControlsState();
            }
        }
    }
}