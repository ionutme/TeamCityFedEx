#region Using Directives

using System;
using System.Collections.ObjectModel;

using Newtonsoft.Json;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Generic group data model.
	/// </summary>
	public class ConfigurationsGroup
	{
		public ConfigurationsGroup(
			String uniqueId,
			String title,
			String subtitle,
			String imagePath,
			String description)
		{
			UniqueId = uniqueId;
			Title = title;
			Subtitle = subtitle;
			Description = description;
			ImagePath = imagePath;
			Items = new ObservableCollection<ConfigurationItem>();
		}

		[JsonProperty("UniqueId")]
		public string UniqueId { get; set; }

		[JsonProperty("Title")]
		public string Title { get; set; }

		[JsonProperty("Subtitle")]
		public string Subtitle { get; set; }

		[JsonProperty("Description")]
		public string Description { get; set; }

		[JsonProperty("ImagePath")]
		public string ImagePath { get; set; }

		[JsonProperty("Items")]
		public ObservableCollection<ConfigurationItem> Items { get; set; }

		public override string ToString()
		{
			return Title;
		}
	}
}