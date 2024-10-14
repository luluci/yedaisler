﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace yedaisler.Utility
{
    internal class EnumDisplayTypeConverter : EnumConverter
    {
        public EnumDisplayTypeConverter(Type type) : base(type) { }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (!(value is null))
                {
                    var field = value.GetType().GetField(value.ToString());
                    if (field != null)
                    {
                        var attribute = field.GetCustomAttribute<DisplayAttribute>(false);
                        return attribute == null ? value.ToString() : attribute.GetName();
                    }
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
