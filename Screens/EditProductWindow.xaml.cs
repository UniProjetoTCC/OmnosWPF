using System.Windows;
using System.Globalization;
using OMNOS.Pages; // Assume que a classe Product está aqui e já foi atualizada com SKU e Stock como int

namespace OMNOS.Screens
{
    public partial class EditProductWindow : Window
    {
        public Product ProductToEdit { get; private set; }
        private bool _isNewProduct;

        public EditProductWindow(Product product, bool isNew = false)
        {
            InitializeComponent();
            ProductToEdit = product;
            _isNewProduct = isNew;
            LoadProductData();

            if (Application.Current.MainWindow != this && Application.Current.MainWindow != null)
            {
                Owner = Application.Current.MainWindow;
            }

            // Desabilita a edição do SKU se for um produto existente
            // Certifique-se que o TextBox do SKU no XAML tem x:Name="SkuTextBox"
            if (!_isNewProduct && SkuTextBox != null)
            {
                SkuTextBox.IsEnabled = false;
            }
            else if (SkuTextBox != null) // Se for novo, garante que está habilitado
            {
                SkuTextBox.IsEnabled = true;
            }
        }

        private void LoadProductData()
        {
            if (ProductToEdit != null)
            {
                // Certifique-se que os TextBoxes no XAML têm os x:Names corretos:
                // SkuTextBox, NameTextBox, CategoriaTextBox, PriceTextBox, StockTextBox
                SkuTextBox.Text = ProductToEdit.SKU; // Alterado de Code para SKU
                NameTextBox.Text = ProductToEdit.Name;
                CategoriaTextBox.Text = ProductToEdit.Categoria;
                PriceTextBox.Text = ProductToEdit.Price.ToString("F2", CultureInfo.CurrentCulture);
                StockTextBox.Text = ProductToEdit.Stock.ToString(); // int para string
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Validação básica
            if (string.IsNullOrWhiteSpace(SkuTextBox.Text)) // Alterado de CodeTextBox
            {
                MessageBox.Show("O campo 'SKU' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                SkuTextBox.Focus(); // Alterado de CodeTextBox
                return;
            }
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("O campo 'Nome' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(CategoriaTextBox.Text))
            {
                MessageBox.Show("O campo 'Categoria' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                CategoriaTextBox.Focus();
                return;
            }

            double price;
            if (!double.TryParse(PriceTextBox.Text.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Any, CultureInfo.CurrentCulture, out price) || price < 0)
            {
                MessageBox.Show("Valor de 'Preço' inválido. Por favor, insira um número positivo (ex: 10,50).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                PriceTextBox.Focus();
                return;
            }

            int stock;
            if (!int.TryParse(StockTextBox.Text, NumberStyles.Integer, CultureInfo.CurrentCulture, out stock) || stock < 0)
            {
                MessageBox.Show("Valor de 'Estoque' inválido. Por favor, insira um número inteiro positivo (ex: 100).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                StockTextBox.Focus();
                return;
            }

            // Atualiza o objeto ProductToEdit
            ProductToEdit.SKU = SkuTextBox.Text; // Alterado de Code
            ProductToEdit.Name = NameTextBox.Text;
            ProductToEdit.Categoria = CategoriaTextBox.Text;
            ProductToEdit.Price = price;
            ProductToEdit.Stock = stock;

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