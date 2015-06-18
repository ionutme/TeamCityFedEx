﻿#region Using Directives

using System.Linq;
using System.Threading.Tasks;

using HipChat.Net;
using HipChat.Net.Http;
using HipChat.Net.Models.Response;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI.Common
{
	public class ChatService
	{
		#region Type Initializer

		static ChatService()
		{
			Token = AppConfig.Settings["HipChatToken"];
			RoomName = AppConfig.Settings["HipChatRoomName"];
			RunCommand = AppConfig.Settings["RunCommand"];
			StatusCommand = AppConfig.Settings["StatusCommand"];
			TeamCityUserName = AppConfig.Settings["TeamCityUser"];
			ClientsAppellative = AppConfig.Settings["ClientsAppellative"];
		}

		#endregion

		#region Constructors

		public ChatService()
		{
			this.hipChatClient = new HipChatClient(CreateHipChatConnection());
		}

		#endregion

		#region Public Properties

		public async Task<StatusMessage> GetStatusMessageAsync(string configuration)
		{
			// get recent room message history
			IResponse<RoomItems<Message>> jsonHistory =
				await this.hipChatClient.Rooms.GetHistoryAsync(RoomName);

			return await GetStatusMessage(jsonHistory.Content(), configuration);
		}

		public async Task SendNotificationAsync(string configuration)
		{
			await this.hipChatClient.Rooms.SendNotificationAsync(RoomName, GetRunNotification(configuration));
		}

		#endregion

		#region Private Methods

		private static ApiConnection CreateHipChatConnection()
		{
			return new ApiConnection(new Credentials(Token));
		}

		private async Task<StatusMessage> GetStatusMessage(string jsonHistory, string configuration)
		{
			RoomItems<Message> roomItems =
				await
					Task.Factory.StartNew(() => JsonConvert.DeserializeObject<RoomItems<Message>>(jsonHistory));

			//roomItems.MaxResults = "10005";

			return
				roomItems.Items.Where(x => IsTeamCityUser(x))
					.Reverse()
					.FirstOrDefault(
						x =>
							x.MessageText.StartsWith(GetStatusNotification(configuration)) &&
							Extensions.GetMessageArguments(x)[1] != "none")
					.ToStatusObject();
		}

		private static dynamic IsTeamCityUser(Message x)
		{
			// if TeamCity user was mentioned in HipChat by someone else, it's not TeamCity => ignore
			if (x.From.GetType() == typeof(JObject))
			{
				return false;
			}

			return x.From == TeamCityUserName;
		}

		private string GetStatusNotification(string configuration)
		{
			Guard.NotNullOrEmpty(() => configuration, configuration);
			Guard.NotNullOrEmpty(() => ClientsAppellative, ClientsAppellative);
			Guard.NotNullOrEmpty(() => StatusCommand, StatusCommand);

			return string.Format("@{0} {1} {2}", ClientsAppellative, StatusCommand, configuration);
		}

		private string GetRunNotification(string configuration)
		{
			Guard.NotNullOrEmpty(() => configuration, configuration);
			Guard.NotNullOrEmpty(() => TeamCityUserName, TeamCityUserName);
			Guard.NotNullOrEmpty(() => RunCommand, RunCommand);

			return string.Format("@{0} {1} {2}", TeamCityUserName, RunCommand, configuration);
		}

		#endregion

		#region Constants and Fields

		private readonly HipChatClient hipChatClient;

		private static readonly string Token;

		private static readonly string RoomName;

		private static readonly string RunCommand;

		private static readonly string StatusCommand;

		private static readonly string TeamCityUserName;

		private static readonly string ClientsAppellative;

		#endregion
	}
}