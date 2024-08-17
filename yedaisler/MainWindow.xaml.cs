using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yedaisler
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isLocationChanged = false;

        Config.Config config = new Config.Config();

        public MainWindow()
        {
            InitializeComponent();

            //var vm = new MainWindowViewModel(this);
            //this.DataContext = vm;

            //this.Width = 150;
            //this.Height = 100;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Init Config
            var config_vm = config.DataContext as Config.ConfigViewModel;
            await config_vm.InitAsync();

            //config.ShowDialog();
            var vm = this.DataContext as MainWindowViewModel;
            vm.Init(config);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //マウスボタン押下状態でなければ何もしない  
            if (e.ButtonState != MouseButtonState.Pressed) return;

            this.DragMove();
            //this.menu.IsOpen = false;
        }

        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!isLocationChanged)
            {
                this.menu.PlacementTarget = this;
                this.menu.Placement = PlacementMode.Top;
                this.menu.IsOpen = true;
            }
            isLocationChanged = false;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            isLocationChanged = true;
            this.menu.IsOpen = false;
        }
    }
}
