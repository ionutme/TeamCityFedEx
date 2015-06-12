using System;
using System.Linq;

using HipChat.Net.Models.Response;

using TeamCityHipChatUI.Data;

namespace TeamCityHipChatUI.Common
{
	public static class Extensions
	{
		 public static StatusMessage ToStatusObject(this Message message)
		 {
			 if (ReferenceEquals(null, message)) return null;

			 string[] m = message.MessageText.Split(' ').Last().Split(',');
			 
			 return new StatusMessage
			 {
				 Status = GetStatus(m.First()),
				 State = GetState(m.Last())
			 };
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