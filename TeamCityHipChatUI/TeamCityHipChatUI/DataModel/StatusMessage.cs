#region Using Directives

using System;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	public class StatusMessage
	{
		public DateTime? CreationDate { get; private set; }

		public Status Status { get; set; }

		public State State { get; set; }

		public StatusMessage(Status status, State state)
		{
			Status = status;
			State = state;

			CreationDate = DateTime.Now;
		}
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