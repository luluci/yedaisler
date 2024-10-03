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
        bool isContextMenuOpen = false;

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
            vm.Init(this, config);

            //
            this.Topmost = true;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            // Window起動位置
            var rect = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
            var mat = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            // 右下
            this.Left = rect.Right / mat.M11 - this.Width;
            this.Top =rect.Bottom / mat.M22 - this.Height;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //マウスボタン押下状態でなければ何もしない  
            if (e.ButtonState != MouseButtonState.Pressed) return;

            this.DragMove();
            //this.menu.IsOpen = false;
        }

        int clickCount = 0;
        bool isDoubleClick = false;
        private async void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (clickCount == 0)
            {
                clickCount++;
                await Task.Delay(100);
            }
            else
            {
                isDoubleClick = true;
                return;
            }

            if (!isDoubleClick)
            {
                // single click event
                if (!isLocationChanged)
                {
                    if (!isContextMenuOpen)
                    {
                        this.menu.PlacementTarget = this;
                        this.menu.Placement = PlacementMode.Top;
                        this.menu.IsOpen = true;
                        //this.menu.StaysOpen = true;
                        isContextMenuOpen = true;
                    }
                    else
                    {
                        //this.menu.StaysOpen = false;
                        this.menu.IsOpen = false;
                        isContextMenuOpen = false;
                    }
                }
                isLocationChanged = false;
            }
            else
            {
                // double click event
                if (DataContext is MainWindowViewModel vm)
                {
                    vm.ExecAction();
                }
            }
            clickCount = 0;
            isDoubleClick = false;
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            isLocationChanged = true;
            //this.menu.StaysOpen = false;
            this.menu.IsOpen = false;
            isContextMenuOpen = false;
        }

        private void menu_Closed(object sender, RoutedEventArgs e)
        {
            isContextMenuOpen = false;
        }

        private void MenuItem_LostFocus(object sender, RoutedEventArgs e)
        {
            int i = 0;
            i++;
            var item = sender as MenuItem;
            //if (item != null)
            //{
            //    item.IsSubmenuOpen = false;
            //}
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

    public class ContextMenuItemElemConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Menu.RootMenuHeader)
            {
                return Menu.ItemType.RootMenuHeader;
            }
            if (value is Menu.SystemMenu)
            {
                return Menu.ItemType.SystemMenu;
            }
            if (value is Menu.SystemAppState)
            {
                return Menu.ItemType.SystemAppState;
            }
            if (value is Menu.SystemAppExit)
            {
                return Menu.ItemType.SystemAppExit;
            }

            if (value is ToDo.Item)
            {
                return Menu.ItemType.ToDo;
            }
            if (value is Menu.ToDoHeader)
            {
                return Menu.ItemType.ToDoHeader;
            }
            if (value is Menu.ToDoAction)
            {
                return Menu.ItemType.ToDoAction;
            }
            if (value is Menu.ToDoManualAction)
            {
                return Menu.ItemType.ToDoManualAction;
            }
            if (value is Menu.ToDoManualState)
            {
                return Menu.ItemType.ToDoManualState;
            }
            if (value is Menu.ToDoDispInBox)
            {
                return Menu.ItemType.ToDoDispInBox;
            }

            if (value is Menu.Separator)
            {
                return Menu.ItemType.Separator;
            }
            if (value is Menu.Command)
            {
                return Menu.ItemType.Command;
            }
            if (value is Menu.Label)
            {
                return Menu.ItemType.Label;
            }

            return Menu.ItemType.None;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ContextMenuStateBackColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var state = (ToDo.State)values[0];
                var vm = values[1] as MainWindowViewModel;

                switch (state)
                {
                    case ToDo.State.Ready:
                        return vm.BrushBackReady.Value;
                    case ToDo.State.Doing:
                        return vm.BrushBackDoing.Value;
                    case ToDo.State.Done:
                        return vm.BrushBackDone.Value;

                    case ToDo.State.None:
                    default:
                        return vm.BrushBackNone.Value;
                }
            }
            catch
            {
                var vm = values[1] as MainWindowViewModel;
                return vm.BrushBackNone.Value;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ContextMenuStateFontColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var state = (ToDo.State)values[0];
                var vm = values[1] as MainWindowViewModel;

                switch (state)
                {
                    case ToDo.State.Ready:
                        return vm.BrushFontReady.Value;
                    case ToDo.State.Doing:
                        return vm.BrushFontDoing.Value;
                    case ToDo.State.Done:
                        return vm.BrushFontDone.Value;

                    case ToDo.State.None:
                    default:
                        return vm.BrushFontNone.Value;
                }
            }
            catch
            {
                var vm = values[1] as MainWindowViewModel;
                return vm.BrushFontNone.Value;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ToDoActionDispConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var vm = values[1] as ToDo.Item;
            if (vm != null)
            {
                switch (vm.State.Value)
                {
                    case ToDo.State.Ready:
                        return vm.ToDoRef.Ready.Value.Name.Value;

                    case ToDo.State.Doing:
                        return vm.ToDoRef.Doing.Value.Name.Value;

                    case ToDo.State.Done:
                        return vm.ToDoRef.Done.Value.Name.Value;

                    default:
                        return "err: invalid state";
                }
            }
            else
            {
                return "err: invalid ViewModel";
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
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
    public class DebugMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            int i = 0;
            i++;

            return values[0];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
