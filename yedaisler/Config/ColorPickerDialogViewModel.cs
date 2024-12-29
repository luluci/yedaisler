using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Config
{
    internal class ColorPickerDialogViewModel
    {
        Utility.ColorPickerViewModel ColorPicker { get; set; }

        public ColorPickerDialogViewModel(ColorPickerDialog window)
        {
            ColorPicker = window.ColorPicker.DataContext as Utility.ColorPickerViewModel;
        }
    }
}
