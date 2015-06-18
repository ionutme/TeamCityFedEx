#region Using Directives

using System;
using System.Collections.ObjectModel;

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
				return new StatusMessage(Status.Invalid, State.Invalid);
			}

			// @clients status dev success,idle
			string[] args = GetMessageArguments(message);

			return new StatusMessage(GetStatus(args[0]), GetState(args[1]));
		}

		/// <summary>
		/// For a message that looks like this:
		/// @clients status dev success,idle
		/// , the method will return a string array
		/// [success],[idle]
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string[] GetMessageArguments(Message message)
		{
			return message.MessageText.Split(' ')[3].Split(',');
		}

		#region Private Methods

		private static Status GetStatus(string value)
		{
			Status status;
			if (Enum.TryParse(value, true, out status))
			{
				return status;
			}

			return Status.Invalid;
		}

		private static State GetState(string value)
		{
			State state;
			if (Enum.TryParse(value, true, out state))
			{
				return state;
			}

			return State.Invalid;
		}

		#endregion
	}
}