using System.Windows;
using System.Windows.Controls;
using OMNOS.Pages; // << IMPORTANTE: Certifique-se que este namespace contém suas páginas
using System.Diagnostics;
using System.Threading.Tasks;

namespace OMNOS.Screens
{
    public partial class MainWindow : Window
    {
        public int LoadingAnimationDuration { get; set; } = 1500; // Ajuste conforme necessário

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (s, e) => UpdateSidebarState();
            // Tenta carregar a página inicial. Certifique-se que 'InitialPage' existe.
            if (MainFrame.Content == null) // Evita recarregar se já houver conteúdo (ex: design time)
            {
                SimulateLoadingAndNavigate(new InitialPage());
            }
        }

        private void ToggleSidebarButton_StateChanged(object sender, RoutedEventArgs e)
        {
            UpdateSidebarState();
        }

        private void InitialPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Certifique-se que a classe 'InitialPage' existe no namespace OMNOS.Pages
            NavigateToPage(new InitialPage());
        }

        private void SecondPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Certifique-se que a classe 'SecondPage' existe no namespace OMNOS.Pages
            // Se o texto do botão no XAML é "Produtos", talvez a página seja 'ProdutosPage'?
            NavigateToPage(new SecondPage()); // Ajuste o nome da classe se necessário
        }

        private void ThirdPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new TerceiraPagina());
            Debug.WriteLine("ThirdPageButton clicado - Navegando para TerceiraPagina.");
        }

        private void FourthPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new QuartaPag());
            Debug.WriteLine("FourthPageButton clicado - Navegando para QuartaPag.");
        }

        private void FifthPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new QuintaPag());
            Debug.WriteLine("FifthPageButton clicado - Navegando para QuintaPag.");
        }

        private void SixthPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new SextaPag());
            Debug.WriteLine("SixthPageButton clicado - Navegando para SextaPag.");
        }

        private void SeventhPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Não linkado conforme solicitado.
            // NavigateToPage(new Page()); // Placeholder
            Debug.WriteLine("SeventhPageButton clicado - Nenhuma página linkada.");
            // Se desejar, pode mostrar uma mensagem ao usuário:
            // MessageBox.Show("Página ainda não implementada.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EighthPageButton_Click(object sender, RoutedEventArgs e)
        {
            // Não linkado conforme solicitado.
            // NavigateToPage(new Page()); // Placeholder
            Debug.WriteLine("EighthPageButton clicado - Nenhuma página linkada.");
            // Se desejar, pode mostrar uma mensagem ao usuário:
            // MessageBox.Show("Página ainda não implementada.", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void NavigateToPage(Page page)
        {
            if (page == null)
            {
                Debug.WriteLine("Tentativa de navegar para uma página nula.");
                return;
            }

            // Evita navegar para a mesma página múltiplas vezes ou se já estiver lá
            if (MainFrame.Content?.GetType() == page.GetType())
            {
                Debug.WriteLine($"Já está na página: {page.GetType().Name}. Navegação ignorada.");
                return;
            }

            MainFrame.Visibility = Visibility.Collapsed;
            if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Visible;

            await Task.Delay(100); // Pequeno delay para a UI mostrar o placeholder

            MainFrame.Navigate(page);

            // Não é ideal adicionar o handler toda vez. Considere fazer isso uma vez.
            // No entanto, para garantir que o placeholder desapareça após cada navegação:
            void NavigatedHandler(object sender, System.Windows.Navigation.NavigationEventArgs e)
            {
                MainFrame.Visibility = Visibility.Visible;
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                MainFrame.Navigated -= NavigatedHandler; // Remove o handler para evitar múltiplas execuções
            }
            MainFrame.Navigated += NavigatedHandler;
        }

        private async void SimulateLoadingAndNavigate(Page page)
        {
            if (page == null) return;
            if (MainFrame.Content?.GetType() == page.GetType()) return;


            await Dispatcher.InvokeAsync(() =>
            {
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Visible;
                MainFrame.Visibility = Visibility.Collapsed;
            });

            await Task.Yield();

            await Task.Delay(LoadingAnimationDuration);

            await Dispatcher.InvokeAsync(() =>
            {
                MainFrame.Navigate(page);
                // O placeholder e a visibilidade do MainFrame serão tratados pelo NavigateToPage/Navigated event
                // No entanto, para o carregamento inicial, fazemos aqui:
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                MainFrame.Visibility = Visibility.Visible;
            });
        }

        private TextBlock GetButtonTextElement(Button button, string textBlockName)
        {
            if (button == null) return null;
            // Tenta encontrar dentro do conteúdo direto se for um Viewbox
            if (button.Content is Viewbox vb && vb.Child is StackPanel sp)
            {
                foreach (var child in sp.Children)
                {
                    if (child is TextBlock tb && tb.Name == textBlockName)
                    {
                        return tb;
                    }
                }
            }
            // Fallback se o TextBlock for nomeado diretamente no escopo do botão (menos provável com Viewbox)
            // ou se o template for mais complexo e FindName for necessário no contexto do botão aplicado.
            // Esta linha pode não funcionar como esperado para elementos dentro de um DataTemplate ou ControlTemplate complexo
            // sem o contexto correto, mas a lógica do Viewbox acima é mais direcionada para sua estrutura XAML.
            return button.Template?.FindName(textBlockName, button) as TextBlock;
        }

        private void UpdateSidebarState()
        {
            if (ToggleSidebarButton == null) return;

            bool isCurrentlyExpanded = ToggleSidebarButton.IsChecked.GetValueOrDefault(true);

            TextBlock initialPageText = GetButtonTextElement(InitialPageButton, "InitialPageButtonText");
            TextBlock secondPageText = GetButtonTextElement(SecondPageButton, "SecondPageButtonText");
            TextBlock thirdPageText = GetButtonTextElement(ThirdPageButton, "ThirdPageButtonText");
            TextBlock fourthPageText = GetButtonTextElement(FourthPageButton, "FourthPageButtonText");
            TextBlock fifthPageText = GetButtonTextElement(FifthPageButton, "FifthPageButtonText");
            TextBlock sixthPageText = GetButtonTextElement(SixthPageButton, "SixthPageButtonText");
            TextBlock seventhPageText = GetButtonTextElement(SeventhPageButton, "SeventhPageButtonText");
            TextBlock eighthPageText = GetButtonTextElement(EighthPageButton, "EighthPageButtonText");

            var allButtonTexts = new[] {
                initialPageText, secondPageText, thirdPageText, fourthPageText,
                fifthPageText, sixthPageText, seventhPageText, eighthPageText
            };

            var allButtons = new[] {
                InitialPageButton, SecondPageButton, ThirdPageButton, FourthPageButton,
                FifthPageButton, SixthPageButton, SeventhPageButton, EighthPageButton
            };

            if (isCurrentlyExpanded)
            {
                SidebarColumn.Width = new GridLength(0.15, GridUnitType.Star);
                SidebarColumn.MinWidth = 200; // Ou seu MaxWidth se preferir um tamanho fixo ao expandir
                if (OmnosTitleText != null) OmnosTitleText.Visibility = Visibility.Visible;

                foreach (var tb in allButtonTexts)
                {
                    if (tb != null) tb.Visibility = Visibility.Visible;
                }
                foreach (var btn in allButtons)
                {
                    if (btn != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.HorizontalContentAlignment = HorizontalAlignment.Left;
                    }
                }
                if (ToggleSidebarButton != null)
                {
                    ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Right;
                }
            }
            else // Sidebar está recolhida
            {
                SidebarColumn.Width = new GridLength(70); // Ajuste para o tamanho dos ícones
                SidebarColumn.MinWidth = 70;
                if (OmnosTitleText != null) OmnosTitleText.Visibility = Visibility.Collapsed;

                foreach (var tb in allButtonTexts)
                {
                    if (tb != null) tb.Visibility = Visibility.Collapsed;
                }
                foreach (var btn in allButtons)
                {
                    if (btn != null)
                    {
                        btn.Visibility = Visibility.Visible;
                        btn.HorizontalContentAlignment = HorizontalAlignment.Center;
                    }
                }
                if (ToggleSidebarButton != null)
                {
                    ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }
    }
}