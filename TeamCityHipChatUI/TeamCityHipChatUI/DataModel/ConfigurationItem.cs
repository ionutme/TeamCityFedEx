#region Using Directives



#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Item data model.
	/// </summary>
	public class ConfigurationItem : ConfigurationBase
	{
		private string imagePath;

		private StatusMessage lastKnownState;

		public ConfigurationItem(
			string uniqueId,
			string title,
			string subtitle,
			string description,
			string content,
			string configuration)
			: base(uniqueId, title, subtitle, description)
		{
			Content = content;
			Configuration = configuration;

			// set a default value for the image path
			this.imagePath = string.Format("Assets/{0}White.png", Configuration);
		}

		public string Configuration { get; private set; }

		public string Content { get; private set; }

		public StatusMessage LastKnownState
		{
			get
			{
				return this.lastKnownState;
			}
			set
			{
				this.lastKnownState = value;
				OnPropertyChanged();
			}
		}

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
	}
}