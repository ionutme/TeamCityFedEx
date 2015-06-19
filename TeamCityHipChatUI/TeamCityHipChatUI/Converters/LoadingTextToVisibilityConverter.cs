using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace TeamCityHipChatUI.Converters
{
	public class LoadingTextToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool isLoading = SafeGetString(value).Contains("load");
			if(isLoading)
			{
				return Visibility.Visible;
			}

			return Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}

		private static string SafeGetString(object value)
		{
			var @string = (string) value;
			if (ReferenceEquals(null, @string))
			{
				return string.Empty;
			}

			return @string.ToLower();
		}
	}
}