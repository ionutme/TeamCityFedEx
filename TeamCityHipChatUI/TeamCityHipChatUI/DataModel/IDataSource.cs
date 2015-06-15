using System.Threading.Tasks;

namespace TeamCityHipChatUI.DataModel
{
	public interface IDataSource
	{
		Task LoadDataAsync();
		
		Task SaveDataAsync(ConfigurationItem item);
	}
}