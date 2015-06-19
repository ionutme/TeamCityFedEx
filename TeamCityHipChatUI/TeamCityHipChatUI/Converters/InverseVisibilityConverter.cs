#region Using Directives

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class InverseVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			switch ((Visibility)value)
			{
				case Visibility.Collapsed:
					return Visibility.Visible;
				case Visibility.Visible:
					return Visibility.Collapsed;
				default:
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}