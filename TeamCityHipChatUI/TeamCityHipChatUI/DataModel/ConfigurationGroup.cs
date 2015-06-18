#region Using Directives

using System;
using System.Collections.ObjectModel;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Generic group data model.
	/// </summary>
	public class ConfigurationGroup : ConfigurationBase
	{
		public ConfigurationGroup(String uniqueId, String title, String subtitle, String description)
			: base(uniqueId, title, subtitle, description)
		{
			Items = new ObservableCollection<ConfigurationItem>();
		}

		public ObservableCollection<ConfigurationItem> Items { get; private set; }
	}
}