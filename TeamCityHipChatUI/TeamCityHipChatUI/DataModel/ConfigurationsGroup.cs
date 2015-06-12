using System;
using System.Collections.ObjectModel;

namespace TeamCityHipChatUI.Data
{
	/// <summary>
	/// Generic group data model.
	/// </summary>
	public class ConfigurationsGroup
	{
		public ConfigurationsGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
		{
			this.UniqueId = uniqueId;
			this.Title = title;
			this.Subtitle = subtitle;
			this.Description = description;
			this.ImagePath = imagePath;
			this.Items = new ObservableCollection<ConfigurationItem>();
		}

		public string UniqueId { get; private set; }
		public string Title { get; private set; }
		public string Subtitle { get; private set; }
		public string Description { get; private set; }
		public string ImagePath { get; private set; }
		public ObservableCollection<ConfigurationItem> Items { get; private set; }

		public override string ToString()
		{
			return this.Title;
		}
	}
}