#region Using Directives

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class StatusMessageToImagePathConverter : DependencyObject, IValueConverter
	{
		public string ItemType
		{
			get
			{
				return (string)GetValue(ItemTypeProperty);
			}
			set
			{
				SetValue(ItemTypeProperty, value);
			}
		}

		public static readonly DependencyProperty ItemTypeProperty =
			DependencyProperty.Register(
				"ItemType",
				typeof(string),
				typeof(StatusMessageToImagePathConverter),
				new PropertyMetadata(null));

		public object Convert(object statusMessage, Type targetType, object parameter, string language)
		{
			Status? status = SafeGetStatus(statusMessage);
			switch (status)
			{
				case Status.Failed:
					return string.Format("Assets/{0}Red.png", ItemType);
				case Status.Success:
					return string.Format("Assets/{0}Green.png", ItemType);
				case Status.Invalid:
					return string.Format("Assets/{0}Gray.png", ItemType);
				default:
					return string.Format("Assets/{0}White.png", ItemType);
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}

		private static Status? SafeGetStatus(object statusMessage)
		{
			if (ReferenceEquals(null, statusMessage))
			{
				return null;
			}

			return ((StatusMessage)statusMessage).Status;
		}
	}
}