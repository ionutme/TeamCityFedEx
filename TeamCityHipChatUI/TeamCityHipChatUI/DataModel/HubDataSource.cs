using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

// The data model defined by this file serves as a representative example of a strongly-typed
// model.  The property names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs. If using this model, you might improve app 
// responsiveness by initiating the data loading task in the code behind for App.xaml when the app 
// is first launched.

namespace TeamCityHipChatUI.Data
{
	/// <summary>
    /// Creates a collection of groups and items with content read from a static json file.
    /// 
    /// HubDataSource initializes with data read from a static json file included in the 
    /// project.  This provides sample data at both design-time and run-time.
    /// </summary>
    public sealed class HubDataSource
    {
        private static readonly HubDataSource DataSource = new HubDataSource();

        private readonly ObservableCollection<ConfigurationsGroup> _groups = new ObservableCollection<ConfigurationsGroup>();
        public ObservableCollection<ConfigurationsGroup> Groups
        {
            get { return this._groups; }
        }

        public static async Task<IEnumerable<ConfigurationsGroup>> GetGroupsAsync()
        {
            await DataSource.GetSampleDataAsync();

            return DataSource.Groups;
        }

        public static async Task<ConfigurationsGroup> GetGroupAsync(string uniqueId)
        {
            await DataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = DataSource.Groups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static async Task<ConfigurationItem> GetItemAsync(string uniqueId)
        {
            await DataSource.GetSampleDataAsync();
            // Simple linear search is acceptable for small data sets
            var matches = DataSource.Groups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        private async Task GetSampleDataAsync()
        {
            if (this._groups.Count != 0)
                return;

            Uri dataUri = new Uri("ms-appx:///DataModel/SampleData.json");

            StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);
            string jsonText = await FileIO.ReadTextAsync(file);
            JsonObject jsonObject = JsonObject.Parse(jsonText);
            JsonArray jsonArray = jsonObject["Groups"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject groupObject = groupValue.GetObject();
                ConfigurationsGroup group = new ConfigurationsGroup(groupObject["UniqueId"].GetString(),
                                                            groupObject["Title"].GetString(),
                                                            groupObject["Subtitle"].GetString(),
                                                            groupObject["ImagePath"].GetString(),
                                                            groupObject["Description"].GetString());

                foreach (JsonValue itemValue in groupObject["Items"].GetArray())
                {
                    JsonObject itemObject = itemValue.GetObject();
                    group.Items.Add(new ConfigurationItem(itemObject["UniqueId"].GetString(),
                                                       itemObject["Title"].GetString(),
                                                       itemObject["Subtitle"].GetString(),
                                                       itemObject["ImagePath"].GetString(),
                                                       itemObject["Description"].GetString(),
                                                       itemObject["Content"].GetString(),
													   itemObject["Configuration"].GetString()));
                }
                this.Groups.Add(group);
            }
        }
    }

	public class StatusMessage
	{
		public string Configuration { get; set; }

		public Status Status { get; set; }

		public State State { get; set; }
	}

	public enum Status
	{
		Success,

		Failed,

		Invalid
	}

	public enum State
	{
		Idle,

		Running,

		Invalid
	}
}