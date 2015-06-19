#region Using Directives

using System;

using Windows.UI.Xaml.Data;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if ((bool)value)
			{
				return false;
			}

			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return new NotSupportedException();
		}
	}
}