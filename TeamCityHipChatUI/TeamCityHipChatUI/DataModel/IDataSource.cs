using System.Threading.Tasks;

namespace TeamCityHipChatUI.DataModel
{
	public interface IDataSource
	{
		Task LoadDataAsync();
	}
}