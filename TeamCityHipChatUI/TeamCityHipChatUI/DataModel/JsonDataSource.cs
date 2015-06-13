#region Using Directives

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.Storage;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	public sealed class JsonDataSource : IDataSource
	{
		public async Task LoadDataAsync()
		{
			jsonData = await GetData();
		}

		public async Task<IEnumerable<ConfigurationsGroup>> GetConfigurationGroupsAsync()
		{
			if (ReferenceEquals(null, jsonData))
			{
				await LoadDataAsync();
			}

			return GetGroupsWithItems(jsonData["Groups"]);
		}

		private IEnumerable<ConfigurationsGroup> GetGroupsWithItems(IJsonValue jsonGroups)
		{
			JsonArray jsonArray = jsonGroups.GetArray();
			foreach (JsonValue groupValue in jsonArray)
			{
				JsonObject jsonGroup = groupValue.GetObject();
				ConfigurationsGroup group = CreateConfigurationGroup(jsonGroup);

				AddJsonItemsToGroup(jsonGroup["Items"], group);

				yield return group;
			}
		}

		private void AddJsonItemsToGroup(IJsonValue jsonItems, ConfigurationsGroup group)
		{
			foreach (JsonValue itemValue in jsonItems.GetArray())
			{
				JsonObject itemObject = itemValue.GetObject();

				group.Items.Add(CreateConfigurationItem(itemObject));
			}
		}

		private ConfigurationsGroup CreateConfigurationGroup(JsonObject groupObject)
		{
			return new ConfigurationsGroup(
				groupObject["UniqueId"].GetString(),
				groupObject["Title"].GetString(),
				groupObject["Subtitle"].GetString(),
				groupObject["ImagePath"].GetString(),
				groupObject["Description"].GetString());
		}

		private ConfigurationItem CreateConfigurationItem(JsonObject itemObject)
		{
			return new ConfigurationItem(
				itemObject["UniqueId"].GetString(),
				itemObject["Title"].GetString(),
				itemObject["Subtitle"].GetString(),
				itemObject["ImagePath"].GetString(),
				itemObject["Description"].GetString(),
				itemObject["Content"].GetString(),
				itemObject["Configuration"].GetString());
		}

		private async Task<JsonObject> GetData()
		{
			var dataUri = new Uri("ms-appx:///DataModel/JsonData.json");

			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
			string jsonText = await FileIO.ReadTextAsync(file);

			return JsonObject.Parse(jsonText);
		}

		private static JsonObject jsonData;
	}
}