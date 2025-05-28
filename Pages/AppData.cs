using OMNOS.Pages; // Certifique-se que o namespace para Product e StockMovementLogEntry está correto
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace OMNOS.Data
{
    public static class AppData
    {
        public static ObservableCollection<Product> MasterProductsList { get; private set; }
        public static ObservableCollection<StockMovementLogEntry> MasterStockMovementsList { get; private set; }

        private static bool _isDataLoaded = false;
        private static readonly object _loadLock = new object();
        private static readonly Random _random = new Random(); // Random para estoque

        static AppData()
        {
            MasterProductsList = new ObservableCollection<Product>();
            MasterStockMovementsList = new ObservableCollection<StockMovementLogEntry>();
        }

        public static void EnsureDataLoaded()
        {
            if (!_isDataLoaded)
            {
                lock (_loadLock)
                {
                    if (!_isDataLoaded)
                    {
                        LoadRealProducts(); // Alterado para carregar produtos reais
                        LoadSampleStockMovements(); 
                        _isDataLoaded = true;
                    }
                }
            }
        }

        public static void LogStockMovement(Product product, int quantityChanged, StockMovementType type, string reason, int oldStock = -1, int newStock = -1)
        {
            if (product == null) return;

            int absoluteQuantity = Math.Abs(quantityChanged);
            if (absoluteQuantity == 0) return; 

            var logEntry = new StockMovementLogEntry
            {
                ProductSku = product.SKU,
                ProductName = product.Name,
                Quantity = absoluteQuantity,
                MovementType = type,
                Reason = reason,
                // Opcional: OldStockLevel = oldStock, NewStockLevel = newStock
            };
            MasterStockMovementsList.Insert(0, logEntry); 
        }

        private static void LoadSampleStockMovements()
        {
            if (!MasterProductsList.Any()) return;

            string[] reasonsEntrada = { "Recebimento de Fornecedor", "Ajuste de Inventário (Entrada)", "Devolução de Cliente" };
            string[] reasonsSaida = { "Venda", "Ajuste de Inventário (Quebra)", "Transferência Interna", "Uso Interno" };

            int productsToLogFor = Math.Min(MasterProductsList.Count, 10); 

            for (int i = 0; i < productsToLogFor; i++)
            {
                Product product = MasterProductsList[_random.Next(MasterProductsList.Count)]; // Pega um produto aleatório da lista real
                int numberOfMovements = _random.Next(1, 4); 

                for (int j = 0; j < numberOfMovements; j++)
                {
                    StockMovementType type = (_random.Next(0, 2) == 0) ? StockMovementType.Entrada : StockMovementType.Saida;
                    
                    // Tenta gerar uma quantidade que faça sentido com o estoque, mas permite adicionar se o estoque for baixo
                    int quantity;
                    if (type == StockMovementType.Saida && product.Stock > 0)
                    {
                        quantity = _random.Next(1, Math.Max(2, product.Stock / 2)); // Saída de até metade do estoque
                    }
                    else
                    {
                        quantity = _random.Next(5, 51); // Entrada de 5 a 50
                    }
                    if (quantity == 0) quantity = 1;


                    string reason = type == StockMovementType.Entrada ?
                                      reasonsEntrada[_random.Next(reasonsEntrada.Length)] :
                                      reasonsSaida[_random.Next(reasonsSaida.Length)];
                    
                    // Simula a atualização de estoque para o propósito do log de exemplo
                    // int stockBefore = product.Stock;
                    // if (type == StockMovementType.Entrada) product.Stock += quantity;
                    // else product.Stock = Math.Max(0, product.Stock - quantity);
                    // int stockAfter = product.Stock;


                    MasterStockMovementsList.Add(new StockMovementLogEntry
                    {
                        ProductSku = product.SKU,
                        ProductName = product.Name,
                        Quantity = quantity,
                        MovementType = type,
                        Reason = reason,
                        MovementDate = DateTime.UtcNow.AddDays(-_random.Next(0, 30)).AddHours(-_random.Next(0, 24))
                        // Opcional: OldStockLevel = stockBefore, NewStockLevel = stockAfter
                    });
                }
            }
            var sortedMovements = MasterStockMovementsList.OrderByDescending(m => m.MovementDate).ToList();
            MasterStockMovementsList.Clear();
            foreach (var movement in sortedMovements)
            {
                MasterStockMovementsList.Add(movement);
            }
        }

        // MÉTODO ATUALIZADO PARA CARREGAR PRODUTOS REAIS
        private static void LoadRealProducts()
        {
            MasterProductsList.Clear();
            var skuCounters = new Dictionary<string, int>();

            var productsToAdd = new List<ProductData>
            {
                // Mercearia
                new ProductData { Name = "Arroz Agulhinha Tipo 1 Camil 5kg", Categoria = "Mercearia", Price = 28.90, Stock = _random.Next(30, 150) },
                new ProductData { Name = "Feijão Carioca Kicaldo Pacote 1kg", Categoria = "Mercearia", Price = 8.79, Stock = _random.Next(50, 200) },
                new ProductData { Name = "Açúcar Refinado União Pacote 1kg", Categoria = "Mercearia", Price = 5.49, Stock = _random.Next(80, 250) },
                new ProductData { Name = "Óleo de Soja Liza PET 900ml", Categoria = "Mercearia", Price = 7.29, Stock = _random.Next(60, 180) },
                new ProductData { Name = "Café Torrado e Moído Pilão Tradicional Almofada 500g", Categoria = "Mercearia", Price = 18.99, Stock = _random.Next(40, 120) },
                new ProductData { Name = "Macarrão Espaguete Barilla Nº7 Pacote 500g", Categoria = "Mercearia", Price = 6.99, Stock = _random.Next(70, 220) },
                new ProductData { Name = "Molho de Tomate Heinz Tradicional Sachê 300g", Categoria = "Mercearia", Price = 3.20, Stock = _random.Next(50, 150) },
                new ProductData { Name = "Farinha de Trigo Dona Benta Tradicional Pacote 1kg", Categoria = "Mercearia", Price = 4.99, Stock = _random.Next(60, 180) },
                new ProductData { Name = "Leite Condensado Moça Integral Lata 395g", Categoria = "Mercearia", Price = 7.89, Stock = _random.Next(30, 100) },
                
                // Laticínios e Frios
                new ProductData { Name = "Leite Integral UHT Italac Caixa 1L", Categoria = "Laticínios", Price = 4.89, Stock = _random.Next(100, 300) },
                new ProductData { Name = "Iogurte Nestlé Grego Tradicional Pote 100g", Categoria = "Laticínios", Price = 3.15, Stock = _random.Next(50, 150) },
                new ProductData { Name = "Queijo Mussarela Fatiado Sadia Pacote 150g", Categoria = "Frios", Price = 12.90, Stock = _random.Next(20, 80) },
                new ProductData { Name = "Manteiga com Sal Aviação Pote 200g", Categoria = "Laticínios", Price = 14.50, Stock = _random.Next(30, 90) },
                
                // Bebidas
                new ProductData { Name = "Refrigerante Coca-Cola Original Garrafa PET 2L", Categoria = "Bebidas", Price = 9.99, Stock = _random.Next(50, 200) },
                new ProductData { Name = "Água Mineral sem Gás Minalba Garrafa 1,5L", Categoria = "Bebidas", Price = 3.00, Stock = _random.Next(80, 250) },
                new ProductData { Name = "Suco de Laranja Integral Prats Garrafa 900ml", Categoria = "Bebidas", Price = 11.50, Stock = _random.Next(30, 100) },
                new ProductData { Name = "Cerveja Brahma Duplo Malte Lata 350ml", Categoria = "Bebidas Alcoólicas", Price = 3.79, Stock = _random.Next(100, 300) },

                // Higiene Pessoal
                new ProductData { Name = "Sabonete em Barra Dove Original Unidade 90g", Categoria = "Higiene", Price = 3.50, Stock = _random.Next(60, 180) },
                new ProductData { Name = "Shampoo Pantene Restauração Frasco 400ml", Categoria = "Higiene", Price = 22.90, Stock = _random.Next(30, 90) },
                new ProductData { Name = "Creme Dental Colgate Total 12 Clean Mint Tubo 90g", Categoria = "Higiene", Price = 8.99, Stock = _random.Next(50, 150) },
                
                // Limpeza
                new ProductData { Name = "Detergente Líquido Limpol Neutro Frasco 500ml", Categoria = "Limpeza", Price = 2.79, Stock = _random.Next(70, 200) },
                new ProductData { Name = "Sabão em Pó Omo Lavagem Perfeita Caixa 1,6kg", Categoria = "Limpeza", Price = 25.90, Stock = _random.Next(30, 100) },
                new ProductData { Name = "Água Sanitária Ypê Frasco 1L", Categoria = "Limpeza", Price = 3.99, Stock = _random.Next(50, 160) },

                // Hortifruti (Exemplos)
                new ProductData { Name = "Banana Prata Dúzia", Categoria = "Hortifruti", Price = 8.00, Stock = _random.Next(20, 60) },
                new ProductData { Name = "Maçã Fuji Unidade (Aprox. 150g)", Categoria = "Hortifruti", Price = 1.80, Stock = _random.Next(30, 100) }
            };

            foreach (var pd in productsToAdd)
            {
                Product p = new Product
                {
                    Name = pd.Name,
                    Categoria = pd.Categoria,
                    Price = pd.Price,
                    Stock = pd.Stock
                };

                string prefixoCat = "GER"; // Prefixo geral padrão
                if (!string.IsNullOrWhiteSpace(p.Categoria))
                {
                    var palavras = p.Categoria.Split(new[] { ' ', '(', ')' }, StringSplitOptions.RemoveEmptyEntries);
                    if (palavras.Length > 0)
                    {
                        // Pega as 3 primeiras letras da primeira palavra da categoria
                        prefixoCat = new string(palavras[0].Where(char.IsLetterOrDigit).ToArray()).ToUpperInvariant();
                        if (prefixoCat.Length > 3) prefixoCat = prefixoCat.Substring(0, 3);
                        else if (prefixoCat.Length < 3) prefixoCat = prefixoCat.PadRight(3, 'X');
                        if (string.IsNullOrWhiteSpace(prefixoCat)) prefixoCat = "GER"; // Fallback se o prefixo ficar vazio
                    }
                }

                if (!skuCounters.ContainsKey(prefixoCat))
                {
                    skuCounters[prefixoCat] = 0;
                }
                skuCounters[prefixoCat]++;
                p.SKU = $"{prefixoCat}-{skuCounters[prefixoCat]:D3}";

                MasterProductsList.Add(p);
            }
        }

        // Classe auxiliar para facilitar a criação dos dados dos produtos
        private class ProductData
        {
            public string Name { get; set; }
            public string Categoria { get; set; }
            public double Price { get; set; }
            public int Stock { get; set; }
        }
    }
}