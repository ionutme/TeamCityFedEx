#region Using Directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using TeamCityHipChatUI.Common;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Creates a collection of groups and items with content read from a static json file.
	///     HubDataSource initializes with data read from a static json file included in the
	///     project.  This provides sample data at both design-time and run-time.
	/// </summary>
	public sealed class HubDataSource : IDataSource
	{
		public ObservableCollection<ConfigurationGroup> Groups
		{
			get
			{
				return this.groups;
			}
		}

		public static async Task<ObservableCollection<ConfigurationGroup>> GetGroupsAsync()
		{
			await DataSource.LoadDataAsync();

			return DataSource.Groups;
		}

		public static async Task<ConfigurationGroup> GetGroupAsync(string uniqueId)
		{
			Guard.NotNullOrEmpty(() => uniqueId, uniqueId);

			await DataSource.LoadDataAsync();

			// Simple linear search is acceptable for small data sets
			IEnumerable<ConfigurationGroup> matches =
				DataSource.Groups.Where(group => group.UniqueId.Equals(uniqueId));

			return matches.FirstOrDefault();
		}

		public static async Task<ConfigurationItem> GetItemAsync(string uniqueId)
		{
			Guard.NotNullOrEmpty(() => uniqueId, uniqueId);

			await DataSource.LoadDataAsync();

			// Simple linear search is acceptable for small data sets
			IEnumerable<ConfigurationItem> matches =
				DataSource.Groups.SelectMany(group => group.Items).Where(item => item.UniqueId.Equals(uniqueId));

			return matches.FirstOrDefault();
		}

		public async Task LoadDataAsync()
		{
			// if the groups are allready loaded, there is nothing more to do
			if (this.groups.Any())
			{
				return;
			}

			await this.jsonDataSource.LoadDataAsync();

			IEnumerable<ConfigurationGroup> localGroups =
				await this.jsonDataSource.GetConfigurationGroupsAsync();

			Groups.AddRange(localGroups.ToArray());
		}

		private static readonly HubDataSource DataSource = new HubDataSource();

		private readonly JsonDataSource jsonDataSource = new JsonDataSource();

		private readonly ObservableCollection<ConfigurationGroup> groups =
			new ObservableCollection<ConfigurationGroup>();
	}
}