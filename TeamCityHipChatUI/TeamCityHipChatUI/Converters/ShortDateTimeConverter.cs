using System;

using Windows.UI.Xaml.Data;

namespace TeamCityHipChatUI.Converters
{
	public class ShortDateTimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var dateTime = (DateTime)value;

			string dayOfWeek = string.Empty;
			if (dateTime.Day != DateTime.Now.Day)
			{
				dayOfWeek = ", " + dateTime.DayOfWeek;
			}

			return string.Concat(dateTime.ToString("HH:mm:ss tt"), dayOfWeek);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}