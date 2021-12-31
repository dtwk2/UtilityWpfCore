using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace UtilityWpf.Converter;

public class AbbreviationConverter : IValueConverter
{
    private const int AbbreviationMaxLength = 3;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var length = parameter is int param ? param : AbbreviationMaxLength;
        var arr = value.ToString().ToCharArray();
        if (arr.Length > length)
            return new string(arr.Take(3).ToArray()) + "...";
        return value.ToString();
    }

    public object ConvertBack(object value, Type targetTypes, object parameter, CultureInfo culture)
       => throw new NotImplementedException();

    public static AbbreviationConverter Instance { get; } = new AbbreviationConverter();
}