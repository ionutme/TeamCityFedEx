namespace TeamCityHipChatUI.Data
{
	/// <summary>
	/// Generic item data model.
	/// </summary>
	public class ConfigurationItem
	{
		public ConfigurationItem(string uniqueId, string title, string subtitle, string imagePath, string description, string content, string configuration)
		{
			UniqueId = uniqueId;
			Title = title;
			Subtitle = subtitle;
			Description = description;
			ImagePath = imagePath;
			Content = content;
			Configuration = configuration;
		}

		public string UniqueId { get; private set; }
		public string Title { get; private set; }
		public string Subtitle { get; private set; }
		public string Description { get; private set; }
		public string ImagePath { get; private set; }
		public string Content { get; private set; }
		public string Configuration { get; private set; }

		public override string ToString()
		{
			return this.Title;
		}
	}
}