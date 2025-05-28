using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OMNOS.Data;
using System.Windows.Controls.Primitives; // Para DatePickerTextBox
using System.Windows.Media;             // Para VisualTreeHelper e Brushes

namespace OMNOS.Pages
{
    public partial class QuartaPag : Page, INotifyPropertyChanged
    {
        public ICollectionView StockMovementsView { get; private set; }
        public ObservableCollection<StockMovementLogEntry> PagedStockMovementsList { get; set; }

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
                if (TotalPages > 0) { if (newPage < 1) newPage = 1; if (newPage > TotalPages) newPage = TotalPages; }
                else { newPage = 1; }
                if (_currentPage != newPage) { _currentPage = newPage; OnPropertyChanged(); RefreshPagedView(); }
                else if (_currentPage == newPage && (PagedStockMovementsList == null || PagedStockMovementsList.Count == 0) && _filteredItemCount > 0 && _currentPage > 0) { RefreshPagedView(); }
            }
        }
        public int TotalPages { get => _totalPages; private set { _totalPages = value; OnPropertyChanged(); } }
        public string PageInfoText => TotalPages > 0 && _filteredItemCount > 0 ? $"Página {CurrentPage} de {TotalPages}" : (_filteredItemCount == 0 ? "Nenhuma movimentação" : "Página 1 de 1");
        public bool CanGoPrevious => CurrentPage > 1;
        public bool CanGoNext => CurrentPage < TotalPages;

        public QuartaPag()
        {
            InitializeComponent();
            AppData.EnsureDataLoaded(); // Garante que AppData.MasterStockMovementsList seja inicializada e populada.

            PagedStockMovementsList = new ObservableCollection<StockMovementLogEntry>();

            // A verificação e atribuição para AppData.MasterStockMovementsList foram removidas daqui.
            // Ela é inicializada no construtor estático de AppData e nunca será null.

            StockMovementsView = CollectionViewSource.GetDefaultView(AppData.MasterStockMovementsList);

            if (StockMovementsDataGrid != null) StockMovementsDataGrid.ItemsSource = PagedStockMovementsList;

            this.DataContext = this;
        }

        private void QuartaPag_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPagedView();
            EnsureDatePickerPlaceholderCleared(MovementDatePicker);
        }

        public static T FindVisualChild<T>(DependencyObject parent) where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T tChild)
                {
                    foundChild = tChild;
                    break;
                }
                else
                {
                    foundChild = FindVisualChild<T>(child);
                    if (foundChild != null) break;
                }
            }
            return foundChild;
        }

        private void ClearDatePickerTextIfPlaceholder(DatePicker datePicker)
        {
            if (datePicker != null && datePicker.SelectedDate == null)
            {
                var textBox = FindVisualChild<DatePickerTextBox>(datePicker);
                if (textBox != null)
                {
                    if (!string.IsNullOrEmpty(textBox.Text))
                    {
                        textBox.Text = string.Empty;
                    }
                }
            }
        }

        private void EnsureDatePickerPlaceholderCleared(DatePicker dp)
        {
            if (dp == null) return;

            if (dp.IsLoaded && dp.ApplyTemplate())
            {
                ClearDatePickerTextIfPlaceholder(dp);
            }
            else
            {
                RoutedEventHandler loadedHandler = null;
                loadedHandler = (s, e_args) =>
                {
                    dp.Loaded -= loadedHandler;
                    if (dp.ApplyTemplate())
                    {
                        ClearDatePickerTextIfPlaceholder(dp);
                    }
                };
                dp.Loaded += loadedHandler;
            }
        }

        internal void MovementDatePicker_SelectedDateChanged_ClearPlaceholder(object sender, SelectionChangedEventArgs e)
        {
            if (sender is DatePicker dp)
            {
                if (dp.SelectedDate == null)
                {
                    ClearDatePickerTextIfPlaceholder(dp);
                }
            }
        }

        private void RefreshPagedView()
        {
            if (StockMovementsView == null) return;

            List<StockMovementLogEntry> itemsToPaginate = StockMovementsView.Cast<StockMovementLogEntry>().ToList();
            _filteredItemCount = itemsToPaginate.Count;

            int newTotalPages = (_filteredItemCount + PageSize - 1) / PageSize;
            TotalPages = (newTotalPages == 0) ? 1 : newTotalPages;

            if (_currentPage > TotalPages) _currentPage = TotalPages;
            if (_currentPage < 1) _currentPage = 1;
            OnPropertyChanged(nameof(CurrentPage));

            PagedStockMovementsList.Clear();
            int skip = (_currentPage - 1) * PageSize;
            var pageItems = itemsToPaginate.Skip(skip).Take(PageSize);
            foreach (var item in pageItems) PagedStockMovementsList.Add(item);

            UpdatePaginationControlsState();
            UpdateFooterStatus();
        }

        private void UpdatePaginationControlsState()
        {
            OnPropertyChanged(nameof(CanGoPrevious));
            OnPropertyChanged(nameof(CanGoNext));
            OnPropertyChanged(nameof(PageInfoText));
        }

        private void ApplyFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            string searchTerm = SearchTermTextBox.Text.ToLowerInvariant().Trim();
            DateTime? selectedDate = MovementDatePicker.SelectedDate;
            string selectedMovementTypeDisplay = (MovementTypeComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            StockMovementsView.Filter = item =>
            {
                if (item is StockMovementLogEntry log)
                {
                    bool matchesSearchTerm = true;
                    if (!string.IsNullOrWhiteSpace(searchTerm))
                    {
                        matchesSearchTerm = (log.ProductSku?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                                            (log.ProductName?.ToLowerInvariant().Contains(searchTerm) ?? false) ||
                                            (log.Reason?.ToLowerInvariant().Contains(searchTerm) ?? false);
                    }

                    bool matchesDate = true;
                    if (selectedDate.HasValue)
                    {
                        matchesDate = log.MovementDate.Date == selectedDate.Value.Date;
                    }

                    bool matchesMovementType = true;
                    if (!string.IsNullOrEmpty(selectedMovementTypeDisplay) && selectedMovementTypeDisplay != "Todos")
                    {
                        matchesMovementType = log.MovementTypeDisplay.Equals(selectedMovementTypeDisplay, StringComparison.OrdinalIgnoreCase);
                    }
                    return matchesSearchTerm && matchesDate && matchesMovementType;
                }
                return false;
            };

            if (CurrentPage != 1) CurrentPage = 1;
            else RefreshPagedView();
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            SearchTermTextBox.Text = string.Empty;
            MovementDatePicker.SelectedDate = null;
            EnsureDatePickerPlaceholderCleared(MovementDatePicker);
            MovementTypeComboBox.SelectedIndex = 0;

            if (StockMovementsView.Filter != null)
            {
                StockMovementsView.Filter = null;
            }

            if (CurrentPage != 1) CurrentPage = 1;
            else RefreshPagedView();
        }

        private void StockMovementsDataGrid_Sorting(object sender, DataGridSortingEventArgs e)
        {
            string sortBy = e.Column.SortMemberPath;
            if (string.IsNullOrEmpty(sortBy) || StockMovementsView == null) return;

            ListSortDirection direction;
            if (e.Column.SortDirection == null || e.Column.SortDirection == ListSortDirection.Descending)
            {
                direction = ListSortDirection.Ascending;
            }
            else
            {
                direction = ListSortDirection.Descending;
            }

            StockMovementsView.SortDescriptions.Clear();
            StockMovementsView.SortDescriptions.Add(new SortDescription(sortBy, direction));

            StockMovementsView.Refresh();

            e.Column.SortDirection = direction;

            foreach (var col in StockMovementsDataGrid.Columns.Where(c => c.SortMemberPath != sortBy))
            {
                col.SortDirection = null;
            }

            if (CurrentPage != 1) CurrentPage = 1;
            else RefreshPagedView();

            e.Handled = true;
        }

        private void PreviousPageButton_Click(object sender, RoutedEventArgs e) { if (CanGoPrevious) CurrentPage--; }
        private void NextPageButton_Click(object sender, RoutedEventArgs e) { if (CanGoNext) CurrentPage++; }

        private void UpdateFooterStatus()
        {
            if (FooterStartItemRun == null || FooterEndItemRun == null ||
                FooterTotalFilteredItemsRun == null || FooterMovimentacaoSingularPluralRun == null ||
                FooterFilterStatusRun == null)
                return;

            int startItem = 0, endItem = 0;
            string movimentacaoText = " movimentação(ões)";

            if (_filteredItemCount > 0 && PagedStockMovementsList.Count > 0)
            {
                startItem = (_currentPage - 1) * PageSize + 1;
                endItem = startItem + PagedStockMovementsList.Count - 1;
            }

            if (_filteredItemCount == 1) movimentacaoText = " movimentação";
            else if (_filteredItemCount > 1) movimentacaoText = " movimentações";
            else if (AppData.MasterStockMovementsList == null || !AppData.MasterStockMovementsList.Any()) movimentacaoText = " movimentação";
            else movimentacaoText = " movimentações";

            FooterStartItemRun.Text = startItem.ToString();
            FooterEndItemRun.Text = endItem.ToString();
            FooterTotalFilteredItemsRun.Text = _filteredItemCount.ToString();
            FooterMovimentacaoSingularPluralRun.Text = movimentacaoText;

            string filterStatusText = "";
            bool isMasterListEmpty = AppData.MasterStockMovementsList == null || !AppData.MasterStockMovementsList.Any();
            bool isFilterActive = StockMovementsView?.Filter != null;

            if (isFilterActive && _filteredItemCount < (AppData.MasterStockMovementsList?.Count ?? 0) && !isMasterListEmpty)
                filterStatusText = " (filtrado)";
            else if (_filteredItemCount == 0 && isFilterActive && !isMasterListEmpty)
                filterStatusText = " (Nenhum resultado para o filtro)";
            else if (_filteredItemCount == 0 && isMasterListEmpty)
            {
                filterStatusText = " (Nenhuma movimentação registrada)";
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