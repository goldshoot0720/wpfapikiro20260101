using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace wpfkiro20260101
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            NavigateToHome(null, null); // 默認顯示首頁
        }

        private void NavigateToHome(object? sender, RoutedEventArgs? e)
        {
            MainFrame.Navigate(new HomePage());
        }

        private void NavigateToDashboard(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void NavigateToPhotos(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PhotosPage());
        }

        private void NavigateToVideos(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new VideosPage());
        }

        private void NavigateToSubscription(object sender, RoutedEventArgs e)
        {
            var subscriptionPage = new SubscriptionPage();
            MainFrame.Navigate(subscriptionPage);
        }

        private void NavigateToFood(object sender, RoutedEventArgs e)
        {
            var foodPage = new FoodPage();
            MainFrame.Navigate(foodPage);
        }

        private void NavigateToSettings(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }
    }
}