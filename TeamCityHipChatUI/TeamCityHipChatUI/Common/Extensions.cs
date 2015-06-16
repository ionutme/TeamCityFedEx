#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Linq;

using HipChat.Net.Http;
using HipChat.Net.Models.Response;

using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI.Common
{
	public static class Extensions
	{
		public static void AddRange<T>(this ObservableCollection<T> collection, T[] items)
		{
			foreach (T item in items)
			{
				collection.Add(item);
			}
		}

		public static string Content(this IResponse<RoomItems<Message>> roomItemsResponse)
		{
			return roomItemsResponse.Body.ToString();
		}

		public static StatusMessage ToStatusObject(this Message message)
		{
			if (ReferenceEquals(null, message))
			{
				return null;
			}

			// @clients status dev success,idle
			string[] m = message.MessageText.Split(' ')[3].Split(',');

			return new StatusMessage { Status = GetStatus(m[0]), State = GetState(m[1]) };
		}

		#region Private Methods

		private static Status GetStatus(string mStatus)
		{
			Status status;
			if (Enum.TryParse(mStatus, true, out status))
			{
				return status;
			}

			return Status.Invalid;
		}

		private static State GetState(string mState)
		{
			State state;
			if (Enum.TryParse(mState, true, out state))
			{
				return state;
			}

			return State.Invalid;
		}

		#endregion
	}
}