using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using OMNOS.Screens; // Para acessar EditProductWindow

namespace OMNOS.Pages
{
    public class Product : INotifyPropertyChanged
    {
        private string _code;
        public string Code { get => _code; set { if (_code != value) { _code = value; OnPropertyChanged(nameof(Code)); } } }

        private string _name;
        public string Name { get => _name; set { if (_name != value) { _name = value; OnPropertyChanged(nameof(Name)); } } }

        private string _categoria;
        public string Categoria { get => _categoria; set { if (_categoria != value) { _categoria = value; OnPropertyChanged(nameof(Categoria)); } } }

        private double _price;
        public double Price { get => _price; set { if (_price != value) { _price = value; OnPropertyChanged(nameof(Price)); } } }

        private double _stock;
        public double Stock { get => _stock; set { if (_stock != value) { _stock = value; OnPropertyChanged(nameof(Stock)); } } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class SecondPage : Page
    {
        public ObservableCollection<Product> MasterProductsList { get; set; }
        public ICollectionView ProductsView { get; private set; }

        public SecondPage()
        {
            InitializeComponent();
            MasterProductsList = new ObservableCollection<Product>();
            LoadSampleProducts();

            ProductsView = CollectionViewSource.GetDefaultView(MasterProductsList);
            ProductsDataGrid.ItemsSource = ProductsView;

            UpdateFooterStatus();
        }

        private void LoadSampleProducts()
        {
            MasterProductsList.Clear();
            MasterProductsList.Add(new Product { Code = "22", Name = "Coca-Cola Classic 2L", Categoria = "Refrigerantes", Price = 7.50, Stock = 120.00 });
            MasterProductsList.Add(new Product { Code = "23", Name = "Pepsi Black 2L", Categoria = "Refrigerantes", Price = 7.00, Stock = 90.00 });
            MasterProductsList.Add(new Product { Code = "24", Name = "Guaran� Antarctica 2L", Categoria = "Refrigerantes", Price = 6.80, Stock = 150.00 });
            MasterProductsList.Add(new Product { Code = "25", Name = "�gua Mineral Crystal 500ml", Categoria = "Bebidas N�o Alco�licas", Price = 2.50, Stock = 300.00 });
            MasterProductsList.Add(new Product { Code = "26", Name = "Chocolate Hershey's Barra Ao Leite", Categoria = "Chocolates", Price = 5.20, Stock = 80.00 });
            MasterProductsList.Add(new Product { Code = "27", Name = "Salgadinho Doritos Queijo Nacho 140g", Categoria = "Salgadinhos", Price = 8.90, Stock = 70.00 });
            MasterProductsList.Add(new Product { Code = "28", Name = "Biscoito Oreo Original 90g", Categoria = "Biscoitos", Price = 3.50, Stock = 110.00 });
            MasterProductsList.Add(new Product { Code = "29", Name = "Sab�o em P� Omo Lavagem Perfeita 800g", Categoria = "Limpeza", Price = 12.75, Stock = 50.00 });
            MasterProductsList.Add(new Product { Code = "30", Name = "Shampoo Pantene Restaura��o 400ml", Categoria = "Higiene Pessoal", Price = 18.90, Stock = 65.00 });
            MasterProductsList.Add(new Product { Code = "31", Name = "Leite Integral Italac TetraPak 1L", Categoria = "Latic�nios", Price = 4.20, Stock = 200.00 });
            MasterProductsList.Add(new Product { Code = "32", Name = "P�o de Forma Pullman Tradicional 480g", Categoria = "Padaria", Price = 9.50, Stock = 40.00 });
            MasterProductsList.Add(new Product { Code = "33", Name = "Pizza Sadia Congelada Calabresa 460g", Categoria = "Congelados", Price = 19.90, Stock = 30.00 });
            MasterProductsList.Add(new Product { Code = "34", Name = "Cerveja Heineken Long Neck 330ml", Categoria = "Bebidas Alco�licas", Price = 5.75, Stock = 150.00 });
            MasterProductsList.Add(new Product { Code = "35", Name = "Arroz Agulhinha Tipo 1 Tio Jo�o 5kg", Categoria = "Gr�os", Price = 28.00, Stock = 100.00 });
            MasterProductsList.Add(new Product { Code = "36", Name = "Feij�o Carioca Kicaldo Pacote 1kg", Categoria = "Gr�os", Price = 9.20, Stock = 120.00 });
            MasterProductsList.Add(new Product { Code = "37", Name = "Milho Verde Quero Lata Drenado 170g", Categoria = "Enlatados", Price = 3.10, Stock = 180.00 });
            MasterProductsList.Add(new Product { Code = "38", Name = "Azeite Extra Virgem Gallo Vidro 500ml", Categoria = "Mercearia", Price = 38.50, Stock = 45.00 });
            MasterProductsList.Add(new Product { Code = "39", Name = "Caf� Torrado e Mo�do Pil�o Pacote 500g", Categoria = "Mercearia", Price = 16.00, Stock = 90.00 });
            MasterProductsList.Add(new Product { Code = "40", Name = "Requeij�o Cremoso Vigor Copo 200g", Categoria = "Latic�nios", Price = 7.80, Stock = 75.00 });
            MasterProductsList.Add(new Product { Code = "41", Name = "Suco de Laranja Del Valle N�ctar 1L", Categoria = "Bebidas N�o Alco�licas", Price = 6.90, Stock = 85.00 });
            MasterProductsList.Add(new Product { Code = "42", Name = "Desinfetante Pinho Sol Original Frasco 1L", Categoria = "Limpeza", Price = 9.90, Stock = 60.00 });
            MasterProductsList.Add(new Product { Code = "43", Name = "Creme Dental Colgate Total 12 Clean Mint 90g", Categoria = "Higiene Pessoal", Price = 5.50, Stock = 200.00 });
            MasterProductsList.Add(new Product { Code = "44", Name = "Margarina Qualy com Sal Pote 500g", Categoria = "Latic�nios", Price = 8.70, Stock = 70.00 });
            MasterProductsList.Add(new Product { Code = "45", Name = "Macarr�o Espaguete Barilla N.5 Pacote 500g", Categoria = "Massas", Price = 7.20, Stock = 130.00 });
            MasterProductsList.Add(new Product { Code = "46", Name = "Molho de Tomate Heinz Tradicional Sach� 300g", Categoria = "Molhos", Price = 2.95, Stock = 160.00 });
            MasterProductsList.Add(new Product { Code = "47", Name = "Iogurte Grego Vigor Natural Pote 100g", Categoria = "Latic�nios", Price = 3.80, Stock = 90.00 });
            MasterProductsList.Add(new Product { Code = "48", Name = "Batata Palha Elma Chips Tradicional Pacote 100g", Categoria = "Salgadinhos", Price = 9.50, Stock = 55.00 });
            MasterProductsList.Add(new Product { Code = "49", Name = "Lasanha Congelada Seara Bolonhesa 600g", Categoria = "Congelados", Price = 22.50, Stock = 25.00 });
            MasterProductsList.Add(new Product { Code = "50", Name = "Vinho Tinto Chileno Casillero Del Diablo Cabernet 750ml", Categoria = "Bebidas Alco�licas", Price = 48.00, Stock = 35.00 });
            MasterProductsList.Add(new Product { Code = "51", Name = "Farinha de Trigo Dona Benta Tipo 1 Pacote 1kg", Categoria = "Mercearia", Price = 5.30, Stock = 100.00 });
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            int nextCode = 1;
            if (MasterProductsList.Any())
            {
                // Tenta obter o maior c�digo num�rico da lista e incrementa
                // Se todos os c�digos n�o forem num�ricos ou a lista estiver vazia, come�a com 1 (ou o primeiro n�mero encontrado + 1)
                var maxCode = MasterProductsList
                    .Select(p => int.TryParse(p.Code, out int c) ? (int?)c : null)
                    .Where(c => c.HasValue)
                    .Max();
                nextCode = (maxCode ?? 0) + 1;
            }

            Product newProduct = new Product
            {
                Code = nextCode.ToString("D2"), // Preenche c�digo sugerido
                Name = "",                     // Nome em branco para entrada do usu�rio
                Categoria = "Diversos",        // Categoria padr�o, usu�rio pode alterar
                Price = 0,                     // Pre�o padr�o
                Stock = 0                      // Estoque padr�o
            };

            EditProductWindow addProductDialog = new EditProductWindow(newProduct);
            addProductDialog.Title = "Adicionar Novo Produto"; // Altera o t�tulo da janela para o modo de adi��o

            bool? result = addProductDialog.ShowDialog();

            if (result == true)
            {
                // O objeto newProduct foi preenchido/modificado pelo di�logo
                MasterProductsList.Add(newProduct);
                UpdateFooterStatus();

                if (ProductsDataGrid.Items.Count > 0)
                {
                    ProductsDataGrid.ScrollIntoView(newProduct);
                    ProductsDataGrid.SelectedItem = newProduct;
                }
                MessageBox.Show($"Produto '{newProduct.Name}' adicionado com sucesso!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            // Se result for false ou null, o usu�rio cancelou e n�o fazemos nada com newProduct.
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string searchText = SearchTextBox.Text.ToLowerInvariant().Trim();

            if (string.IsNullOrWhiteSpace(searchText))
            {
                ProductsView.Filter = null; // Limpa o filtro se a busca estiver vazia
            }
            else
            {
                ProductsView.Filter = item =>
                {
                    if (item is Product product)
                    {
                        // Verifica se o texto de busca est� contido no nome, c�digo OU CATEGORIA do produto
                        return (product.Name?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (product.Code?.ToLowerInvariant().Contains(searchText) ?? false) ||
                               (product.Categoria?.ToLowerInvariant().Contains(searchText) ?? false);
                    }
                    return false;
                };
            }
            UpdateFooterStatus();
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteButton && deleteButton.DataContext is Product productToDelete)
            {
                MessageBoxResult confirm = MessageBox.Show(
                    $"Tem certeza que deseja excluir o produto '{productToDelete.Name}' (C�digo: {productToDelete.Code})?",
                    "Confirmar Exclus�o",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (confirm == MessageBoxResult.Yes)
                {
                    MasterProductsList.Remove(productToDelete);
                    UpdateFooterStatus();
                    MessageBox.Show($"Produto '{productToDelete.Name}' exclu�do!", "Sucesso", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button editButton && editButton.DataContext is Product productToEdit)
            {
                EditProductWindow editWindow = new OMNOS.Screens.EditProductWindow(productToEdit);
                // O t�tulo padr�o da EditProductWindow ("Editar Produto") j� � adequado aqui.

                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    MessageBox.Show($"Produto '{productToEdit.Name}' atualizado com sucesso!", "Edi��o Conclu�da", MessageBoxButton.OK, MessageBoxImage.Information);
                    ProductsDataGrid.SelectedItem = null;
                    ProductsDataGrid.UnselectAllCells();
                }
            }
        }

        private void UpdateFooterStatus()
        {
            int itemsInViewCount = 0;
            if (ProductsView != null)
            {
                foreach (var _ in ProductsView)
                {
                    itemsInViewCount++;
                }
            }

            if (FooterStatusText != null)
            {
                string filterStatus = (ProductsView != null && ProductsView.Filter != null && itemsInViewCount < MasterProductsList.Count) ? " (filtrado)" : "";
                FooterStatusText.Text = $"Mostrando {itemsInViewCount} de {MasterProductsList.Count} produto(s){filterStatus}";
            }
        }
    }
}