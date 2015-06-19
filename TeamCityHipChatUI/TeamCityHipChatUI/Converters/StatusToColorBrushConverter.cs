#region Using Directives

using System;

using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class StatusToColorBrushConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var status = (Status?)value;
			switch (status)
			{
				case Status.Failed:
					return new SolidColorBrush(Colors.Red);
				case Status.Success:
					return new SolidColorBrush(Colors.LimeGreen);
				default:
					return new SolidColorBrush(Colors.DarkGray);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return new NotSupportedException();
		}
	}
}