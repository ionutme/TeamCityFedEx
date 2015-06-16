#region Using Directives

using System.ComponentModel;
using System.Runtime.CompilerServices;

using TeamCityHipChatUI.Annotations;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Item data model.
	/// </summary>
	public class ConfigurationItem : INotifyPropertyChanged
	{
		private string imagePath;

		public ConfigurationItem(
			string uniqueId,
			string title,
			string subtitle,
			string imagePath,
			string description,
			string content,
			string configuration)
		{
			UniqueId = uniqueId;
			Title = title;
			Subtitle = subtitle;
			Description = description;
			this.imagePath = imagePath;
			Content = content;
			Configuration = configuration;
		}

		public string UniqueId { get; private set; }

		public string Title { get; private set; }

		public string Subtitle { get; private set; }

		public string Description { get; private set; }

		public string Content { get; private set; }

		public string Configuration { get; private set; }

		public string ImagePath
		{
			get
			{
				return this.imagePath;
			}
			set
			{
				this.imagePath = value;
				OnPropertyChanged();
			}
		}

		public StatusMessage LastKnownState { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public override string ToString()
		{
			return Title;
		}

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