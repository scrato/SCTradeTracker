using SCTradeTracker.API;
using SCTradeTracker.ComputerVision;
using System.Drawing;
using System.Threading;
using System.Windows;

namespace SCTradeTracker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var scr = new Screenshot();
            Bitmap p = scr.ActiveWindow();
            var Client = new CVClient();
            await Client.ReadBitmapAsync(p, CancellationToken.None);
        }
    }
}
