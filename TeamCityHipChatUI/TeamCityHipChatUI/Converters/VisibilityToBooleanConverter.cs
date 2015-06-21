#region Using Directives

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class VisibilityToBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			bool isInversed = !ReferenceEquals(null, parameter) && ((string)parameter) == "inverse";

			if ((Visibility)value == Visibility.Visible)
			{
				if (isInversed)
				{
					return false;
				}

				return true;
			}

			if (isInversed)
			{
				return true;
			}

			return false;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return new NotSupportedException();
		}
	}
}