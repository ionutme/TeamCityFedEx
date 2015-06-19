using System.ComponentModel;
using System.Runtime.CompilerServices;

using TeamCityHipChatUI.Common;

namespace TeamCityHipChatUI.DataModel
{
	public abstract class ConfigurationBase : INotifyPropertyChanged
	{
		protected ConfigurationBase(string uniqueId, string title, string subtitle, string description)
		{
			UniqueId = uniqueId;
			Title = title;
			Subtitle = subtitle;
			Description = description;
		}

		public string UniqueId { get; private set; }

		public string Title { get; private set; }

		public string Subtitle { get; private set; }

		public string Description { get; private set; }

		public override string ToString()
		{
			return Title;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}