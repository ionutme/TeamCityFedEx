#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Data.Json;
using Windows.Storage;

using Newtonsoft.Json;

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
				itemObject["FailedImagePath"].GetString(),
				itemObject["SuccessImagePath"].GetString(),
				itemObject["Description"].GetString(),
				itemObject["Content"].GetString(),
				itemObject["Configuration"].GetString(),
				LoadStatus(itemObject["LastStatus"]),
				LoadState(itemObject["LastState"]),
				LoadDateTime(itemObject["LastRunDateTime"]));
		}

		public async Task SaveDataAsync(ConfigurationItem configurationItem)
		{
			StorageFile file = await LoadStorageFile();

			IEnumerable<ConfigurationsGroup> existingData = await GetConfigurationGroupsAsync();
			
			List<ConfigurationItem> updatingData = existingData.Single().Items.ToList();
			int i = updatingData.FindIndex(x => x.UniqueId == configurationItem.UniqueId);
			updatingData[i] = configurationItem;

			FileIO.WriteTextAsync(file, await JsonConvert.SerializeObjectAsync(updatingData));
		}

		private async Task<JsonObject> GetData()
		{
			StorageFile file = await LoadStorageFile();

			string jsonText = await FileIO.ReadTextAsync(file);

			return JsonObject.Parse(jsonText);
		}

		private static async Task<StorageFile> LoadStorageFile()
		{
			if (!ReferenceEquals(null, StorageFile))
			{
				return StorageFile;
			}

			var dataUri = new Uri("ms-appx:///DataModel/JsonData.json");

			StorageFile = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

			return StorageFile;
		}

		public static StorageFile StorageFile { get; private set; }

		private DateTime LoadDateTime(IJsonValue jsonValue)
		{
			DateTime dateTime;
			if (DateTime.TryParse(jsonValue.GetString(), out dateTime))
			{
				return dateTime;
			}

			return DateTime.MinValue;
		}

		private static State LoadState(IJsonValue jsonValue)
		{
			State state;
			if (Enum.TryParse(jsonValue.GetString(), true, out state))
			{
				return state;
			}

			return State.Invalid;
		}

		private static Status LoadStatus(IJsonValue jsonValue)
		{
			Status status;
			if (Enum.TryParse(jsonValue.GetString(), true, out status))
			{
				return status;
			}

			return Status.Invalid;
		}

		private static JsonObject jsonData;
	}
}