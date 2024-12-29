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
using System.Windows.Shapes;

namespace yedaisler.Config
{
    /// <summary>
    /// ColorPickerDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ColorPickerDialog : Window
    {
        public ColorPickerDialog()
        {
            InitializeComponent();

            this.DataContext = new ColorPickerDialogViewModel(this);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = this.ColorPicker.Width;
            this.Height = this.ColorPicker.Height;
        }
    }
}
