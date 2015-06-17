#region Using Directives

using System;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using TeamCityHipChatUI.Common;
using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI.Converters
{
	public class StatusMessageToImagePathConverter : IValueConverter
	{
		//public UserViewModel CurrentUser
		//{
		//	get
		//	{
		//		return (UserViewModel) GetValue(CurrentUserProperty);
		//	}
		//	set
		//	{
		//		SetValue(CurrentUserProperty, value);
		//	}
		//}

		//public static readonly DependencyProperty CurrentUserProperty =
		//DependencyProperty.Register("CurrentUser",
		//							typeof(UserViewModel),
		//							typeof(StatusMessageToImagePathConverter),
		//							new PropertyMetadata(null));

		public object Convert(object value, Type targetType, object parameter, string language)
		{
			StatusMessage statusMessage = value as StatusMessage;

			if (ReferenceEquals(null, statusMessage))
			{
				return string.Format("Assets/{0}White.png", parameter); 
			}

			switch (statusMessage.Status)
			{
				case Status.Failed:
					return string.Format("Assets/{0}Red.png", parameter);
				case Status.Success:
					return string.Format("Assets/{0}Green.png", parameter);
				case Status.Invalid:
					return string.Format("Assets/{0}Gray.png", parameter);
				default:
					// another option that was not taken into consideration
					return null;
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			throw new NotSupportedException();
		}
	}
}