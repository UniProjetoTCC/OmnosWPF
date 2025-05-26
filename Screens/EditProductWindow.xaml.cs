using System.Windows;
using System.Globalization;
using OMNOS.Pages;

namespace OMNOS.Screens
{
    public partial class EditProductWindow : Window
    {
        public Product ProductToEdit { get; private set; }

        public EditProductWindow(Product product)
        {
            InitializeComponent();
            ProductToEdit = product;
            LoadProductData();

            if (Application.Current.MainWindow != this && Application.Current.MainWindow != null)
            {
                Owner = Application.Current.MainWindow;
            }
        }

        private void LoadProductData()
        {
            if (ProductToEdit != null)
            {
                CodeTextBox.Text = ProductToEdit.Code;
                NameTextBox.Text = ProductToEdit.Name;
                PriceTextBox.Text = ProductToEdit.Price.ToString("F2", CultureInfo.CurrentCulture);
                StockTextBox.Text = ProductToEdit.Stock.ToString("F2", CultureInfo.CurrentCulture);
                CategoriaTextBox.Text = ProductToEdit.Categoria; // Carregar categoria
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // ... (validações existentes para Code, Name, Price, Stock) ...
            if (string.IsNullOrWhiteSpace(CodeTextBox.Text))
            {
                MessageBox.Show("O campo 'Código' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                CodeTextBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(NameTextBox.Text))
            {
                MessageBox.Show("O campo 'Nome' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                NameTextBox.Focus();
                return;
            }

            double price;
            if (!double.TryParse(PriceTextBox.Text.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Any, CultureInfo.CurrentCulture, out price) || price < 0)
            {
                MessageBox.Show("Valor de 'Preço' inválido. Por favor, insira um número positivo (ex: 10,50).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                PriceTextBox.Focus();
                return;
            }

            double stock;
            if (!double.TryParse(StockTextBox.Text.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator), NumberStyles.Any, CultureInfo.CurrentCulture, out stock) || stock < 0)
            {
                MessageBox.Show("Valor de 'Estoque' inválido. Por favor, insira um número positivo (ex: 100,00).", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                StockTextBox.Focus();
                return;
            }
            // Validação simples para categoria (opcional)
            if (string.IsNullOrWhiteSpace(CategoriaTextBox.Text))
            {
                MessageBox.Show("O campo 'Categoria' não pode estar vazio.", "Erro de Validação", MessageBoxButton.OK, MessageBoxImage.Error);
                CategoriaTextBox.Focus();
                return;
            }


            ProductToEdit.Code = CodeTextBox.Text;
            ProductToEdit.Name = NameTextBox.Text;
            ProductToEdit.Price = price;
            ProductToEdit.Stock = stock;
            ProductToEdit.Categoria = CategoriaTextBox.Text; // Salvar categoria

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