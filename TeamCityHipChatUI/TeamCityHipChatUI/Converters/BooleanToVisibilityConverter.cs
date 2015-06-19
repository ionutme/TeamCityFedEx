#region Using Directives

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#endregion

namespace TeamCityHipChatUI.Converters
{
	internal class BooleanToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var boolValue = (bool)value;
			if (boolValue)
			{
				return Visibility.Visible;
			}

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if ((Visibility)value == Visibility.Visible)
			{
				return true;
			}

			return false;
		}
	}
}
