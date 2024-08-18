using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
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

    public sealed class BindingResource
    {
        public Binding Binding { get; set; } = null;
    }
    [MarkupExtensionReturnType(typeof(object))]
    public sealed class BindingResourceExtension : MarkupExtension
    {
        public BindingResource Resource { get; set; } = null;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Resource.Binding.ProvideValue(serviceProvider);
        }
    }

    public class ContextMenuTemplateSelector : DataTemplateSelector
    {
        public DataTemplate IsSeparator { get; set; }
        public DataTemplate IsCommand { get; set; }
        public DataTemplate IsToDo { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null) return IsCommand;
            if ((item as MenuItem) != null) return IsCommand;
            if ((item as ToDo.Item) != null) return IsToDo;
            if ((item as Menu.Separator) != null) return IsSeparator;

            return IsCommand;
        }
    }

    public class ContextMenuElemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return Menu.ItemType.None;
            var menu = value as MenuItem;
            if (menu == null) return Menu.ItemType.None;

            var vm = menu.DataContext;
            if (vm == null) return Menu.ItemType.None;
            if ((vm as Menu.Separator) != null) return Menu.ItemType.Separator;
            if ((vm as ToDo.Item) != null) return Menu.ItemType.ToDo;
            if ((vm as Menu.RootMenuHeader) != null) return Menu.ItemType.RootMenuHeader;
            if ((vm as Menu.ToDoHeader) != null) return Menu.ItemType.ToDoHeader;
            if ((vm as Menu.SystemItem) != null) return Menu.ItemType.System;

            var cmd = vm as Menu.Command;
            if (cmd != null)
            {
                if (menu.Tag != null)
                {
                    return Menu.ItemType.None;
                }
                else
                {
                    menu.Tag = true;
                    return Menu.ItemType.Command;
                }

            }

            return Menu.ItemType.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class DebugConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            i++;

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
