using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using OMNOS.Pages;

namespace OMNOS.Screens
{
    /// <summary>
    /// Classe auxiliar para encontrar elementos na árvore visual do WPF.
    /// </summary>
    public static class VisualTreeHelpers
    {
        public static IEnumerable<T> FindChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T typedChild)
                    {
                        yield return typedChild;
                    }

                    foreach (T childOfChild in FindChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Lógica interna para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public int LoadingAnimationDuration { get; set; } = 100;
        private bool _isUpdatingSidebarSelection = false;

        private readonly Dictionary<string, Page> _pageInstances = new Dictionary<string, Page>();

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine("MainWindow.cs: InitializeComponent() foi chamado.");

            if (this.MainFrame == null)
                Debug.WriteLine("MainWindow.cs CRÍTICO: MainFrame é NULL após InitializeComponent!");
            else
                Debug.WriteLine("MainWindow.cs SUCESSO: MainFrame NÃO é NULL após InitializeComponent.");

            MainFrame.Navigated += MainFrame_Navigated;
            Loaded += MainWindow_Loaded;
        }

        private Page GetOrCreatePage(string pageKey)
        {
            if (!_pageInstances.TryGetValue(pageKey, out Page pageInstance))
            {
                switch (pageKey)
                {
                    case "InitialPage": pageInstance = new InitialPage(); break;
                    case "SecondPage": pageInstance = new SecondPage(); break;
                    case "ThirdPage": pageInstance = new TerceiraPagina(); break;
                    case "FourthPage": pageInstance = new QuartaPag(); break;
                    case "FifthPage": pageInstance = new QuintaPag(); break;
                    case "SixthPage": pageInstance = new SextaPag(); break;
                    default:
                        Debug.WriteLine($"Página com chave '{pageKey}' desconhecida em GetOrCreatePage.");
                        return null;
                }
                if (pageInstance != null) // Adiciona ao dicionário apenas se a instância foi criada
                {
                    _pageInstances[pageKey] = pageInstance;
                }
            }
            return pageInstance;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("MainWindow.cs: Evento MainWindow_Loaded disparado.");
            if (MainFrame == null)
            {
                ShowMainFrameUnavailableError("MainFrame não inicializado no evento Loaded.");
                return;
            }

            UpdateSidebarState();

            if (MainFrame.Content == null)
            {
                if (InitialPageRadioButton != null && InitialPageRadioButton.IsChecked == true)
                {
                    if (MainFrame.Content == null)
                        InitialPageButton_Click(InitialPageRadioButton, new RoutedEventArgs());
                }
                else if (InitialPageRadioButton != null)
                {
                    InitialPageRadioButton.IsChecked = true;
                }
                else
                {
                    Page initial = GetOrCreatePage("InitialPage");
                    if (initial != null) NavigateToPage(initial);
                }
            }
            else
            {
                UpdateSidebarSelection(MainFrame.Content as Page);
            }
        }

        private void ToggleSidebarButton_StateChanged(object sender, RoutedEventArgs e)
        {
            UpdateSidebarState();
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            if (e.Content is Page page)
            {
                UpdateSidebarSelection(page);
            }
            if (MainFrame != null) MainFrame.Visibility = Visibility.Visible;
            if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
        }

        private void UpdateSidebarSelection(Page currentPage)
        {
            if (currentPage == null || _isUpdatingSidebarSelection) return;

            if (InitialPageRadioButton == null)
            {
                Debug.WriteLine("UpdateSidebarSelection: RadioButtons da Sidebar ainda não estão prontos (ex: InitialPageRadioButton é null).");
                return;
            }
            if (SidebarNavigationPanel == null)
            {
                Debug.WriteLine("MainWindow.xaml.cs: SidebarNavigationPanel (o StackPanel dos RadioButtons) é NULL. Verifique o x:Name no XAML.");
                return;
            }

            _isUpdatingSidebarSelection = true;
            RadioButton targetRadioButton = null;

            if (currentPage is InitialPage) targetRadioButton = InitialPageRadioButton;
            else if (currentPage is SecondPage) targetRadioButton = SecondPageRadioButton;
            else if (currentPage is TerceiraPagina) targetRadioButton = ThirdPageRadioButton;
            else if (currentPage is QuartaPag) targetRadioButton = FourthPageRadioButton;
            else if (currentPage is QuintaPag) targetRadioButton = FifthPageRadioButton;
            else if (currentPage is SextaPag) targetRadioButton = SixthPageRadioButton;

            var radioButtonsInSidebar = VisualTreeHelpers.FindChildren<RadioButton>(SidebarNavigationPanel);
            if (radioButtonsInSidebar != null)
            {
                foreach (var rb in radioButtonsInSidebar)
                {
                    if (rb != null) rb.IsChecked = (rb == targetRadioButton);
                }
            }

            _isUpdatingSidebarSelection = false;
            UpdateSidebarState();
        }

        public async void NavigateToPage(Page pageInstance)
        {
            if (MainFrame == null) { ShowMainFrameUnavailableError($"Tentativa de navegar para {pageInstance?.GetType().Name} mas MainFrame é NULL."); return; }
            if (pageInstance == null) { Debug.WriteLine("NavigateToPage: Tentativa de navegar para uma página nula."); return; }

            if (MainFrame.Content == pageInstance)
            {
                if (MainFrame.Visibility != Visibility.Visible) MainFrame.Visibility = Visibility.Visible;
                if (PlaceholderBackgroundGrid != null && PlaceholderBackgroundGrid.Visibility == Visibility.Visible) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                UpdateSidebarSelection(pageInstance);
                return;
            }

            bool isInitialLoadForThisPage = (MainFrame.Content == null || MainFrame.Content.GetType() != typeof(InitialPage)) && pageInstance is InitialPage;

            if (MainFrame.Visibility != Visibility.Collapsed || (PlaceholderBackgroundGrid != null && PlaceholderBackgroundGrid.Visibility != Visibility.Visible))
            {
                MainFrame.Visibility = Visibility.Collapsed;
                if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Visible;
            }

            if (isInitialLoadForThisPage)
            {
                await Task.Delay(LoadingAnimationDuration);
            }
            else
            {
                await Task.Delay(10);
            }

            MainFrame.Navigate(pageInstance);
        }

        public void NavigateToPage(string pageKey)
        {
            Page pageInstance = GetOrCreatePage(pageKey);
            if (pageInstance != null)
            {
                NavigateToPage(pageInstance);
            }
            else
            {
                ShowMainFrameUnavailableError($"Página com chave '{pageKey}' não pôde ser criada ou encontrada.");
            }
        }

        private void InitialPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("InitialPage");
            }
        }
        private void SecondPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("SecondPage");
            }
        }
        private void ThirdPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("ThirdPage");
            }
        }
        private void FourthPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("FourthPage");
            }
        }
        private void FifthPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("FifthPage");
            }
        }
        private void SixthPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton rb && rb.IsChecked == true && !_isUpdatingSidebarSelection)
            {
                NavigateToPage("SixthPage");
            }
        }

        private void ShowMainFrameUnavailableError(string customMessage = null)
        {
            string message = customMessage ?? "CRÍTICO: MainFrame é NULL ao tentar navegar. A navegação não ocorrerá.";
            Debug.WriteLine(message);
        }

        private TextBlock GetTextBlockFromControl(ContentControl control, string textBlockName)
        {
            if (control?.Content is Panel panel)
            {
                return panel.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Name == textBlockName);
            }
            return null;
        }

        private void UpdateSidebarState()
        {
            if (ToggleSidebarButton == null) { Debug.WriteLine("UpdateSidebarState: ToggleSidebarButton é null."); return; }
            bool isCurrentlyExpanded = ToggleSidebarButton.IsChecked.GetValueOrDefault(true);

            if (TitlePanel != null) TitlePanel.Visibility = isCurrentlyExpanded ? Visibility.Visible : Visibility.Collapsed;

            if (SidebarNavigationPanel == null)
            {
                Debug.WriteLine("UpdateSidebarState: SidebarNavigationPanel é null.");
                return;
            }

            // COR PARA ÍCONES NÃO SELECIONADOS QUANDO A SIDEBAR ESTÁ RECOLHIDA
            SolidColorBrush foregroundParaIconeNaoSelecionadoRecolhido = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#3B7A57"));

            var radioButtonsAndTextNames = new List<Tuple<RadioButton, string>>
            {
                Tuple.Create(InitialPageRadioButton, "InitialPageButtonText"),
                Tuple.Create(SecondPageRadioButton, "SecondPageButtonText"),
                Tuple.Create(ThirdPageRadioButton, "ThirdPageButtonText"),
                Tuple.Create(FourthPageRadioButton, "FourthPageButtonText"),
                Tuple.Create(FifthPageRadioButton, "FifthPageButtonText"),
                Tuple.Create(SixthPageRadioButton, "SixthPageButtonText")
            };

            for (int i = 0; i < radioButtonsAndTextNames.Count; i++)
            {
                RadioButton rb = radioButtonsAndTextNames[i].Item1;
                string textBlockName = radioButtonsAndTextNames[i].Item2;

                if (rb != null)
                {
                    TextBlock textBlock = null;
                    if (rb.Content is Panel panelContent)
                    {
                        textBlock = panelContent.Children.OfType<TextBlock>().FirstOrDefault(tb => tb.Name == textBlockName);
                        if (textBlock == null)
                        {
                            textBlock = panelContent.Children.OfType<TextBlock>().LastOrDefault();
                        }
                    }

                    if (textBlock != null)
                    {
                        textBlock.Visibility = isCurrentlyExpanded ? Visibility.Visible : Visibility.Collapsed;
                    }

                    rb.HorizontalContentAlignment = isCurrentlyExpanded ? HorizontalAlignment.Left : HorizontalAlignment.Center;
                    rb.Padding = isCurrentlyExpanded ? new Thickness(15, 0, 10, 0) : new Thickness(0);

                    if (!isCurrentlyExpanded) // QUANDO A SIDEBAR ESTÁ RECOLHIDA
                    {
                        rb.Background = Brushes.Transparent;
                        if (rb.IsChecked == true)
                        {
                            rb.Foreground = Brushes.White;
                        }
                        else
                        {
                            rb.Foreground = foregroundParaIconeNaoSelecionadoRecolhido; // Ícone não selecionado fica VERDE
                        }
                    }
                    else // QUANDO A SIDEBAR ESTÁ EXPANDIDA
                    {
                        // Deixa o estilo XAML (<Style x:Key="SidebarRadioButtonStyle">) controlar
                        // O Foreground padrão no estilo XAML é #3B7A57 para não selecionado,
                        // e o Trigger para IsChecked=True define para White.
                        rb.ClearValue(RadioButton.ForegroundProperty);
                        rb.ClearValue(RadioButton.BackgroundProperty);
                    }
                }
            }

            if (SidebarColumn != null)
            {
                if (isCurrentlyExpanded)
                {
                    SidebarColumn.Width = new GridLength(0.1, GridUnitType.Star);
                    SidebarColumn.MaxWidth = 350;
                    if (ToggleSidebarButton != null) ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Right;
                }
                else
                {
                    SidebarColumn.Width = new GridLength(70);
                    SidebarColumn.MinWidth = 70;
                    if (ToggleSidebarButton != null) ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Center;
                }
            }
        }
    }
}