using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Config
{
    [TypeConverter(typeof(Utility.EnumDisplayTypeConverter))]
    internal enum StartupPosition
    {
        [Display(Name = "指定なし")]
        None,
        [Display(Name = "右下")]
        BottomRight,
        [Display(Name = "左下")]
        BottomLeft,
        [Display(Name = "右上")]
        TopRight,
        [Display(Name = "左上")]
        TopLeft,
    }

    [TypeConverter(typeof(Utility.EnumDisplayTypeConverter))]
    internal enum Color
    {
        [Display(Name = "Ready状態文字色")]
        FontReady,
        FontDoing,
        FontDone,
        BackReady,
        BackDoing,
        BackDone,
    }
}
