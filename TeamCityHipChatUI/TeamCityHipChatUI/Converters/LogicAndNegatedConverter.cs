#region Using Directives

using System;
using System.Linq;

using Windows.UI.Xaml.Data;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class LogicAndNegatedConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			if (ReferenceEquals(null, value))
			{
				return null;
			}

			var boolValues = value as object[];
			if (ReferenceEquals(null, boolValues) || boolValues.Contains(null))
			{
				return false;
			}

			// if there is a progress bar showing ... progress, hide the RUN button
			if (boolValues.Contains(true))
			{
				return false;
			}

			return true;
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}
