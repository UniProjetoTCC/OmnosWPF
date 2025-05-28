using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace OMNOS.Pages // Se você colocar em outra pasta, ajuste este namespace
{
    public class BooleanToUnderlineConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isMouseOver && isMouseOver)
            {
                return TextDecorations.Underline;
            }
            return null; // Sem sublinhado por padrão
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Não precisamos converter de volta para esta aplicação
            throw new NotImplementedException();
        }
    }
}