using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace UtilityWpf.Converter
{
    public class BooleanToColorConverter : BaseConverter<Color, bool>
    {
        public BooleanToColorConverter() : base(Colors.Black, Colors.Red)
        {
        }

        protected override bool Check(bool value) => value;

        protected override bool Convert(object value)
        {
            return System.Convert.ToBoolean(value);
        }

        public static BooleanToColorConverter Instance => new BooleanToColorConverter();
    }
}
