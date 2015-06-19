#region Using Directives

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Item data model.
	/// </summary>
	public class ConfigurationItem : ConfigurationBase
	{
		#region Constructor

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
		}

		#endregion

		#region Properties

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

		#endregion

		#region Private Fields

		private string imagePath;

		private StatusMessage lastKnownState;

		#endregion
	}
}