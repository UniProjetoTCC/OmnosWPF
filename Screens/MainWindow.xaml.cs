using System.Windows;
using System.Windows.Controls;
using OMNOS.Pages; // << IMPORTANTE: Certifique-se que este namespace cont�m suas p�ginas
using System.Diagnostics;
using System.Threading.Tasks;

namespace OMNOS.Screens
{
    public partial class MainWindow : Window
    {
        public int LoadingAnimationDuration { get; set; } = 1500; // Ajuste conforme necess�rio

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += (s, e) => UpdateSidebarState();
            // Tenta carregar a p�gina inicial. Certifique-se que 'InitialPage' existe.
            if (MainFrame.Content == null) // Evita recarregar se j� houver conte�do (ex: design time)
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
            // Se o texto do bot�o no XAML � "Produtos", talvez a p�gina seja 'ProdutosPage'?
            NavigateToPage(new SecondPage()); // Ajuste o nome da classe se necess�rio
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
            // N�o linkado conforme solicitado.
            // NavigateToPage(new Page()); // Placeholder
            Debug.WriteLine("SeventhPageButton clicado - Nenhuma p�gina linkada.");
            // Se desejar, pode mostrar uma mensagem ao usu�rio:
            // MessageBox.Show("P�gina ainda n�o implementada.", "Informa��o", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EighthPageButton_Click(object sender, RoutedEventArgs e)
        {
            // N�o linkado conforme solicitado.
            // NavigateToPage(new Page()); // Placeholder
            Debug.WriteLine("EighthPageButton clicado - Nenhuma p�gina linkada.");
            // Se desejar, pode mostrar uma mensagem ao usu�rio:
            // MessageBox.Show("P�gina ainda n�o implementada.", "Informa��o", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async void NavigateToPage(Page page)
        {
            if (page == null)
            {
                Debug.WriteLine("Tentativa de navegar para uma p�gina nula.");
                return;
            }

            // Evita navegar para a mesma p�gina m�ltiplas vezes ou se j� estiver l�
            if (MainFrame.Content?.GetType() == page.GetType())
            {
                Debug.WriteLine($"J� est� na p�gina: {page.GetType().Name}. Navega��o ignorada.");
                return;
            }

            MainFrame.Visibility = Visibility.Collapsed;
            if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Visible;

            await Task.Delay(100); // Pequeno delay para a UI mostrar o placeholder

            MainFrame.Navigate(page);

            // N�o � ideal adicionar o handler toda vez. Considere fazer isso uma vez.
            // No entanto, para garantir que o placeholder desapare�a ap�s cada navega��o:
            void NavigatedHandler(object sender, System.Windows.Navigation.NavigationEventArgs e)
            {
                MainFrame.Visibility = Visibility.Visible;
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                MainFrame.Navigated -= NavigatedHandler; // Remove o handler para evitar m�ltiplas execu��es
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
                // O placeholder e a visibilidade do MainFrame ser�o tratados pelo NavigateToPage/Navigated event
                // No entanto, para o carregamento inicial, fazemos aqui:
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                MainFrame.Visibility = Visibility.Visible;
            });
        }

        private TextBlock GetButtonTextElement(Button button, string textBlockName)
        {
            if (button == null) return null;
            // Tenta encontrar dentro do conte�do direto se for um Viewbox
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
            // Fallback se o TextBlock for nomeado diretamente no escopo do bot�o (menos prov�vel com Viewbox)
            // ou se o template for mais complexo e FindName for necess�rio no contexto do bot�o aplicado.
            // Esta linha pode n�o funcionar como esperado para elementos dentro de um DataTemplate ou ControlTemplate complexo
            // sem o contexto correto, mas a l�gica do Viewbox acima � mais direcionada para sua estrutura XAML.
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
            else // Sidebar est� recolhida
            {
                SidebarColumn.Width = new GridLength(70); // Ajuste para o tamanho dos �cones
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