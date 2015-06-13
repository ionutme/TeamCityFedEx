#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Linq;

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

		public static StatusMessage ToStatusObject(this Message message)
		{
			if (ReferenceEquals(null, message))
			{
				return null;
			}

			// @clients status dev success,idle
			string[] m = message.MessageText.Split(' ')[3].Split(',');

			return new StatusMessage { Status = GetStatus(m.First()), State = GetState(m.Last()) };
		}

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
	}
}