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

			return new StatusMessage(GetStatus(message), GetState(message));
		}

		public static string GetStatusAsString(IMessage message)
		{
			return GetMessageArguments(message)[0];
		}

		public static string GetStateAsString(IMessage message)
		{
			return GetMessageArguments(message)[1];
		}

		#region Private Methods

		private static Status GetStatus(IMessage message)
		{
			string value = GetStatusAsString(message);

			Status status;
			if (Enum.TryParse(value, true, out status))
			{
				return status;
			}

			return Status.Invalid;
		}

		private static State GetState(IMessage message)
		{
			string value = GetStateAsString(message);

			State state;
			if (Enum.TryParse(value, true, out state))
			{
				return state;
			}

			return State.Invalid;
		}

		/// <summary>
		/// For a message that looks like this:
		/// @clients status dev success,idle
		/// , the method will return a string array
		/// [success],[idle]
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		private static string[] GetMessageArguments(IMessage message)
		{
			return message.MessageText.Split(' ')[3].Split(',');
		}

		#endregion
	}
}