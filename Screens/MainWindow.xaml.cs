using System.Windows;
using System.Windows.Controls; 
using OMNOS.Pages; 
using System.Diagnostics; // Added for Debug.WriteLine

namespace OMNOS.Screens
{
    public partial class MainWindow : Window
    {
        public int LoadingAnimationDuration { get; set; } = 1500; 

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            UpdateSidebarState(); // Call after InitializeComponent to set initial state based on ToggleButton
            SimulateLoadingAndNavigate(new InitialPage());
        }

        private void ToggleSidebarButton_StateChanged(object sender, RoutedEventArgs e)
        {
            UpdateSidebarState();
        }

        private void InitialPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new InitialPage());
        }

        private void SecondPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPage(new SecondPage());
        }

        private async void NavigateToPage(Page page)
        {
            // Start loading visuals
            MainFrame.Visibility = Visibility.Collapsed; // Hide main content
            if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Visible;

            await Task.Delay(400); // Increased delay to see animation
            MainFrame.Navigate(page);

            // Stop loading visuals
            MainFrame.Visibility = Visibility.Visible; // Show main content
            if (PlaceholderBackgroundGrid != null) PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
        }

        private async void SimulateLoadingAndNavigate(Page page)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                PlaceholderBackgroundGrid.Visibility = Visibility.Visible;
                MainFrame.Visibility = Visibility.Collapsed;
            });

            await Task.Yield();

            await Task.Delay(LoadingAnimationDuration);

            await Dispatcher.InvokeAsync(() =>
            {
                MainFrame.NavigationService.Navigate(page);
                PlaceholderBackgroundGrid.Visibility = Visibility.Collapsed;
                MainFrame.Visibility = Visibility.Visible;
            });
        }

        private void UpdateSidebarState()
        {
            bool isCurrentlyExpanded = ToggleSidebarButton?.IsChecked.GetValueOrDefault(true) ?? true;

            // Find text elements within buttons. This assumes x:Name is set in XAML for these TextBlocks.
            TextBlock? initialPageText = InitialPageButton?.Template?.FindName("InitialPageButtonText", InitialPageButton) as TextBlock ?? InitialPageButton?.Content as TextBlock; // Fallback if text is direct content
            if (initialPageText == null && InitialPageButton?.Content is StackPanel spInitial) // If content is a StackPanel (like in a Viewbox)
            {
                foreach (var child in spInitial.Children)
                {
                    if (child is TextBlock tb && tb.Name == "InitialPageButtonText")
                    {
                        initialPageText = tb;
                        break;
                    }
                }
            }
            // A more robust way if Viewbox is used for button content:
            if (initialPageText == null && InitialPageButton?.Content is Viewbox vbInitial && vbInitial.Child is StackPanel spVbInitial)
            {
                 foreach (var child in spVbInitial.Children)
                {
                    if (child is TextBlock tb && tb.Name == "InitialPageButtonText")
                    {                        initialPageText = tb;                        break;                    }
                }
            }


            TextBlock? secondPageText = SecondPageButton?.Template?.FindName("SecondPageButtonText", SecondPageButton) as TextBlock ?? SecondPageButton?.Content as TextBlock;
            if (secondPageText == null && SecondPageButton?.Content is Viewbox vbSecond && vbSecond.Child is StackPanel spVbSecond)
            {
                 foreach (var child in spVbSecond.Children)
                {
                    if (child is TextBlock tb && tb.Name == "SecondPageButtonText")
                    {                        secondPageText = tb;                        break;                    }
                }
            }


            if (isCurrentlyExpanded) // Sidebar is expanded (X icon shown)
            {
                SidebarColumn.Width = new GridLength(0.15, GridUnitType.Star);
                SidebarColumn.MinWidth = 200;
                if (OmnosTitleText != null) OmnosTitleText.Visibility = Visibility.Visible; 
                
                if (initialPageText != null) initialPageText.Visibility = Visibility.Visible;
                if (secondPageText != null) secondPageText.Visibility = Visibility.Visible;

                if (InitialPageButton != null) InitialPageButton.Visibility = Visibility.Visible;
                if (SecondPageButton != null) SecondPageButton.Visibility = Visibility.Visible;

                if (ToggleSidebarButton != null) 
                {
                    Grid.SetColumn(ToggleSidebarButton, 1); // Original column
                    Grid.SetColumnSpan(ToggleSidebarButton, 1); // Reset span
                    ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Right; // Original alignment
                }
                if (InitialPageButton != null) InitialPageButton.HorizontalContentAlignment = HorizontalAlignment.Left;
                if (SecondPageButton != null) SecondPageButton.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
            else // Sidebar is collapsed (Hamburger icon shown)
            {
                SidebarColumn.Width = new GridLength(70);
                SidebarColumn.MinWidth = 70;
                if (OmnosTitleText != null) OmnosTitleText.Visibility = Visibility.Collapsed; 
                
                if (initialPageText != null) initialPageText.Visibility = Visibility.Collapsed;
                if (secondPageText != null) secondPageText.Visibility = Visibility.Collapsed;

                if (InitialPageButton != null) InitialPageButton.Visibility = Visibility.Visible; 
                if (SecondPageButton != null) SecondPageButton.Visibility = Visibility.Visible; 

                if (ToggleSidebarButton != null) 
                {
                    Grid.SetColumn(ToggleSidebarButton, 0);
                    Grid.SetColumnSpan(ToggleSidebarButton, 2);
                    ToggleSidebarButton.HorizontalAlignment = HorizontalAlignment.Center; 
                }
                if (InitialPageButton != null) InitialPageButton.HorizontalContentAlignment = HorizontalAlignment.Center;
                if (SecondPageButton != null) SecondPageButton.HorizontalContentAlignment = HorizontalAlignment.Center;
            }
        }
    }
}
