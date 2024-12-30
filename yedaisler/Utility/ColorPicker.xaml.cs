using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace yedaisler.Utility
{
    /// <summary>
    /// ColorPicker.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorPicker : UserControl
    {

        public ICommand OnOk
        {
            get { return (ICommand)GetValue(OnOkProperty); }
            set { SetValue(OnOkProperty, value); }
        }
        public static readonly DependencyProperty OnOkProperty =
            DependencyProperty.Register(nameof(OnOk), typeof(ICommand), typeof(ColorPicker));

        public ICommand OnCancel
        {
            get { return (ICommand)GetValue(OnCancelProperty); }
            set { SetValue(OnCancelProperty, value); }
        }
        public static readonly DependencyProperty OnCancelProperty =
            DependencyProperty.Register(nameof(OnCancel), typeof(ICommand), typeof(ColorPicker));


        public ColorPicker()
        {
            InitializeComponent();
        }
    }
}
