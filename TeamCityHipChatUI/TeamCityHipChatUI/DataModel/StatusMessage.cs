namespace TeamCityHipChatUI.DataModel
{
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