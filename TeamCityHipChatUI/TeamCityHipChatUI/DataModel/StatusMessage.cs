#region Using Directives

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using TeamCityHipChatUI.Common;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	public class StatusMessage : INotifyPropertyChanged
	{
		#region Constructor

		public StatusMessage(Status status, State state)
		{
			this.status = status;
			this.state = state;

			this.creationDate = DateTime.Now;
		}

		#endregion

		#region Properties

		public DateTime? CreationDate
		{
			get
			{
				return this.creationDate;
			}
			private set
			{
				this.creationDate = value;
				OnPropertyChanged();
			}
		}

		public Status Status
		{
			get
			{
				return this.status;
			}
			private set
			{
				this.status = value;
				OnPropertyChanged();
			}
		}

		public State State
		{
			get
			{
				return this.state;
			}
			private set
			{
				this.state = value;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Events

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion

		#region Event Handlers

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if (handler != null)
			{
				handler(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#endregion

		#region Private Fields

		private Status status;

		private State state;

		private DateTime? creationDate;

		#endregion
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