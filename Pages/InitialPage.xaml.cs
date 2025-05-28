using System;
using System.Collections.Generic;
using System.Globalization; // Para CultureInfo na formatação de moeda
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media; // Para Brushes
using Material.Icons;      // Para MaterialIconKind no C# (se não estiver global)
using Material.Icons.WPF;  // Para MaterialIconKind no C# (se não estiver global)
using OMNOS.Data;          // Para AppData
using OMNOS.Screens;       // Para MainWindow (navegação)

namespace OMNOS.Pages
{
    public partial class InitialPage : Page
    {
        // Adicione as seguintes declarações de campo no início da classe InitialPage
        private TextBlock TotalProdutosValue;
        private TextBlock ValorTotalEstoqueValue;
        private TextBlock ProdutosBaixoEstoqueValue;

        // Classes de modelo para os dados de placeholder das listas visuais
        public class TopProductPlaceholder
        {
            public string Name { get; set; }
            public string SalesInfo { get; set; }
        }

        public class LatestSalePlaceholder
        {
            public MaterialIconKind StatusIcon { get; set; }
            public Brush StatusColor { get; set; }
            public string Description { get; set; }
            public string OperatorItems { get; set; }
            public double Value { get; set; }
            public string Time { get; set; }
        }

        public class CriticalStockPlaceholderItem
        {
            public string ProductName { get; set; }
            public int CurrentStock { get; set; }
            public int MinStock { get; set; }
        }

        public class AlertPlaceholder
        {
            public MaterialIconKind Icon { get; set; }
            public Brush Color { get; set; }
            public string Message { get; set; }
        }

        // Propriedades para vincular aos ItemsControls no XAML
        public List<TopProductPlaceholder> TopSellingProductsPlaceholder { get; set; }
        public List<LatestSalePlaceholder> LatestSalesPlaceholder { get; set; }
        public List<CriticalStockPlaceholderItem> CriticalStockPlaceholder { get; set; }
        public List<AlertPlaceholder> AlertsPlaceholder { get; set; }

        public InitialPage()
        {
            InitializeComponent();
            TotalProdutosValue = (TextBlock)FindName("TotalProdutosValue");
            ValorTotalEstoqueValue = (TextBlock)FindName("ValorTotalEstoqueValue");
            ProdutosBaixoEstoqueValue = (TextBlock)FindName("ProdutosBaixoEstoqueValue");
            LoadPlaceholderData();    // Carrega os dados visuais de exemplo
            this.DataContext = this; // Define o DataContext para os bindings das listas no XAML
        }

        private void InitialPage_Loaded(object sender, RoutedEventArgs e)
        {
            AtualizarKPIsReais(); // Carrega os dados reais para os KPIs principais
        }

        private void AtualizarKPIsReais()
        {
            AppData.EnsureDataLoaded(); // Garante que os dados principais da aplicação estejam carregados

            if (TotalProdutosValue != null)
            {
                TotalProdutosValue.Text = AppData.MasterProductsList?.Count.ToString() ?? "0";
            }

            if (ValorTotalEstoqueValue != null)
            {
                double valorTotal = 0;
                if (AppData.MasterProductsList?.Any() == true)
                {
                    // Tratamento para evitar exceção se Price ou Stock forem problemáticos (ex: NaN, Infinity)
                    try
                    {
                        valorTotal = AppData.MasterProductsList.Where(p => !double.IsNaN(p.Price) && !double.IsInfinity(p.Price) && p.Stock >= 0)
                                             .Sum(p => p.Price * p.Stock);
                    }
                    catch (OverflowException) // Caso a soma seja muito grande
                    {
                        valorTotal = double.MaxValue;
                    }
                }
                ValorTotalEstoqueValue.Text = valorTotal.ToString("C", CultureInfo.GetCultureInfo("pt-BR"));
            }

            if (ProdutosBaixoEstoqueValue != null)
            {
                int limiteBaixoEstoque = 10;
                int produtosBaixo = 0;
                if (AppData.MasterProductsList?.Any() == true)
                {
                    produtosBaixo = AppData.MasterProductsList.Count(p => p.Stock < limiteBaixoEstoque && p.Stock > 0);
                }
                ProdutosBaixoEstoqueValue.Text = produtosBaixo.ToString();
            }
        }

        private void LoadPlaceholderData()
        {
            // Placeholder para Top Produtos Vendidos
            TopSellingProductsPlaceholder = new List<TopProductPlaceholder>
            {
                new TopProductPlaceholder { Name = "Refrigerante Coca-Cola 2L", SalesInfo = "25 un - R$ 249,75" },
                new TopProductPlaceholder { Name = "Arroz Camil Tipo 1 5kg", SalesInfo = "18 un - R$ 520,20" },
                new TopProductPlaceholder { Name = "Leite Integral Italac 1L", SalesInfo = "30 un - R$ 146,70" },
                new TopProductPlaceholder { Name = "Sabão em Pó Omo 1.6kg", SalesInfo = "12 un - R$ 310,80" },
                new TopProductPlaceholder { Name = "Pão Francês Unidade", SalesInfo = "55 un - R$ 41,25" }
            };
            if (TopSellingProductsItemsControl != null) TopSellingProductsItemsControl.ItemsSource = TopSellingProductsPlaceholder;

            // Placeholder para Últimas Vendas
            LatestSalesPlaceholder = new List<LatestSalePlaceholder>
            {
                new LatestSalePlaceholder { StatusIcon = MaterialIconKind.CheckCircleOutline, StatusColor = Brushes.SeaGreen, Description = "Venda #12345", OperatorItems = "Op: Carlos | Itens: 3", Value = 75.50, Time = "14:32" },
                new LatestSalePlaceholder { StatusIcon = MaterialIconKind.CashMultiple, StatusColor = Brushes.DodgerBlue, Description = "Venda #12344", OperatorItems = "Op: Ana | Itens: 1", Value = 120.00, Time = "14:28" },
                new LatestSalePlaceholder { StatusIcon = MaterialIconKind.CancelCircleOutline, StatusColor = Brushes.Tomato, Description = "Venda #12343 (Cancelada)", OperatorItems = "Op: Carlos | Itens: 2", Value = 50.25, Time = "14:15" },
                new LatestSalePlaceholder { StatusIcon = MaterialIconKind.ProgressClock, StatusColor = Brushes.Orange, Description = "Venda #12342 (Em Aberto)", OperatorItems = "Op: Bia | Itens: 5", Value = 33.80, Time = "14:05" },
            };
            if (UltimasVendasItemsControl != null) UltimasVendasItemsControl.ItemsSource = LatestSalesPlaceholder;

            // Placeholder para Estoque Crítico
            CriticalStockPlaceholder = new List<CriticalStockPlaceholderItem>
            {
                new CriticalStockPlaceholderItem { ProductName = "Óleo de Soja Liza 900ml", CurrentStock = 5, MinStock = 10 },
                new CriticalStockPlaceholderItem { ProductName = "Café Pilão Tradicional 500g", CurrentStock = 8, MinStock = 15 },
                new CriticalStockPlaceholderItem { ProductName = "Detergente Limpol Neutro 500ml", CurrentStock = 3, MinStock = 5 },
            };
            if (EstoqueCriticoItemsControl != null) EstoqueCriticoItemsControl.ItemsSource = CriticalStockPlaceholder;

            // Placeholder para Alertas
            AlertsPlaceholder = new List<AlertPlaceholder>
            {
                new AlertPlaceholder { Icon = MaterialIconKind.GiftOutline, Color = (Brush)new BrushConverter().ConvertFrom("#10B981"), Message = "Ativar promoção de Refrigerantes!"},
                new AlertPlaceholder { Icon = MaterialIconKind.ClockAlertOutline, Color = (Brush)new BrushConverter().ConvertFrom("#F59E0B"), Message = "Lembrete: Fechamento de caixa às 22:00."},
                new AlertPlaceholder { Icon = MaterialIconKind.Update, Color = (Brush)new BrushConverter().ConvertFrom("#6366F1"), Message = "Atualização do sistema agendada para 02:00."}
            };
            if (AlertsPlaceholderItemsControl != null) AlertsPlaceholderItemsControl.ItemsSource = AlertsPlaceholder;
        }

        private void NavegarPara(string pageKeyNaMainWindow)
        {
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                // Chama o método público NavigateToPage da MainWindow, passando a chave da página
                mainWindow.NavigateToPage(pageKeyNaMainWindow);
            }
            else
            {
                MessageBox.Show("Não foi possível encontrar a janela principal para navegação.", "Erro de Navegação");
            }
        }

        private void VerProdutos_Click(object sender, RoutedEventArgs e)
        {
            NavegarPara("SecondPage"); // Usa a chave "SecondPage" que definimos em MainWindow.xaml.cs
        }

        private void VerEstoque_Click(object sender, RoutedEventArgs e)
        {
            NavegarPara("ThirdPage"); // Usa a chave "ThirdPage"
        }

        private void VerHistorico_Click(object sender, RoutedEventArgs e)
        {
            NavegarPara("FourthPage"); // Usa a chave "FourthPage"
        }

        private void VerUsuarios_Click(object sender, RoutedEventArgs e)
        {
            NavegarPara("FifthPage"); // Usa a chave "FifthPage"
        }
        private void VerEstoqueCritico_Click(object sender, RoutedEventArgs e)
        {
            // Poderia navegar para a tela de estoque com um filtro pré-aplicado,
            // ou para uma tela de relatório específica de estoque baixo.
            // Por agora, vamos para a tela de estoque geral:
            NavegarPara("ThirdPage");
        }
    }
}