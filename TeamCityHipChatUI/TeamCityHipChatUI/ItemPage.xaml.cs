﻿#region Using Directives

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using HipChat.Net;
using HipChat.Net.Http;
using HipChat.Net.Models.Response;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using TeamCityHipChatUI.Common;
using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI
{
	/// <summary>
	///     A page that displays details for a single item within a group.
	/// </summary>
	public sealed partial class ItemPage : Page
	{
		#region Type Initializer

		static ItemPage()
		{
			RoomName = AppConfig.Settings["HipChatRoomName"];
			Token = AppConfig.Settings["HipChatToken"];
			RunCommand = AppConfig.Settings["RunCommand"];
			StatusCommand = AppConfig.Settings["StatusCommand"];
			TeamCityUserName = AppConfig.Settings["TeamCityUser"];
			ClientsAppellative = AppConfig.Settings["ClientsAppellative"];
		}

		#endregion

		#region Constructors

		public ItemPage()
		{
			InitializeComponent();

			this.hipChat = new HipChatClient(new ApiConnection(new Credentials(Token)));

			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += NavigationHelper_LoadState;
			this.navigationHelper.SaveState += NavigationHelper_SaveState;
		}

		#endregion

		#region Public Properties

		/// <summary>
		///     Gets the <see cref="NavigationHelper" /> associated with this <see cref="Page" />.
		/// </summary>
		public NavigationHelper NavigationHelper
		{
			get
			{
				return this.navigationHelper;
			}
		}

		/// <summary>
		///     Gets the view model for this <see cref="Page" />.
		///     This can be changed to a strongly typed view model.
		/// </summary>
		public ObservableDictionary DefaultViewModel
		{
			get
			{
				return this.defaultViewModel;
			}
		}

		#endregion

		#region Events

		/// <summary>
		///     Populates the page with content passed during navigation. Any saved state is also
		///     provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		///     The source of the event; typically <see cref="NavigationHelper" />.
		/// </param>
		/// <param name="e">
		///     Event data that provides both the navigation parameter passed to
		///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
		///     a dictionary of state preserved by this page during an earlier
		///     session.  The state will be null the first time a page is visited.
		/// </param>
		private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			this.item = await HubDataSource.GetItemAsync(e.NavigationParameter as string);

			DefaultViewModel["Item"] = this.item;

			await SetStatus(this.item);
		}

		/// <summary>
		///     Preserves state associated with this page in case the application is suspended or the
		///     page is discarded from the navigation cache.  Values must conform to the serialization
		///     requirements of <see cref="SuspensionManager.SessionState" />.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper" />.</param>
		/// <param name="e">
		///     Event data that provides an empty dictionary to be populated with
		///     serializable state.
		/// </param>
		private async void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			// Save the unique state of the page here.

			await HubDataSource.SaveItemAsync(this.item);
		}
		
		/// <summary>
		/// Provides the functionality of starting the job on CI machine for the chosen configuration.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Data associated with this event.</param>
		private async void RunButton_OnClick(object sender, RoutedEventArgs e)
		{
			await ShowMessageDialogToContinue();
		}

		#region Message Dialog Methods

		private async Task ShowMessageDialogToContinue()
		{
			var messageDialog = new MessageDialog(GetConfirmationMessage(), "Continue");
			messageDialog.Commands.Add(new UICommand("Yes", YesCommandInvokedHandler));
			messageDialog.Commands.Add(new UICommand("Cancel", CancelCommandInvokedHandler));

			// Set the command that will be invoked by default
			messageDialog.DefaultCommandIndex = 0;

			// Set the command to be invoked when escape is pressed
			messageDialog.CancelCommandIndex = 1;

			await messageDialog.ShowAsync();
		}

		private async void YesCommandInvokedHandler(IUICommand command)
		{
			await SendRunCommand();

			this.RunButton.IsEnabled = false;
		}

		private async void CancelCommandInvokedHandler(IUICommand command)
		{
			// do nothing
		}

		private string GetConfirmationMessage()
		{
			return string.Format(
				"Are you sure you want to start the {0} on Team City machine?",
				this.item.Title.ToLower());
		}

		#endregion

		#endregion

		#region Private Methods

		private async Task SendRunCommand()
		{
			// get all rooms
			//IResponse<RoomItems<Entity>> rooms = await _hipChat.Rooms.GetAllAsync();

			// get room members
			//Task<IResponse<RoomItems<Mention>>> members = _hipChat.Rooms.GetMembersAsync("Aidaws");

			// send room notification
			await this.hipChat.Rooms.SendNotificationAsync(RoomName, GetRunNotification());
		}

		private async Task SetStatus(ConfigurationItem item)
		{
			// get recent room message history
			IResponse<RoomItems<Message>> jsonHistory = await this.hipChat.Rooms.GetHistoryAsync(RoomName);
			StatusMessage message = GetStatusMessage(jsonHistory);

			if (IsInvalidMessage(message))
			{
				DisableAllContent();
				return;
			}

			item.LastState = message.State;
			item.LastStatus = message.Status;
			item.LastRunDateTime = DateTime.Now;

			SetStateLabelText(message.State);
			SetStateLabelColor(message.Status);

			ToggleEnableDisableRunButton(message.State);
		}

		private StatusMessage GetStatusMessage(IResponse<RoomItems<Message>> jsonHistory)
		{
			Message[] messages =
				JsonConvert.DeserializeObject<RoomItems<Message>>(jsonHistory.Body.ToString()).Items;

			return
				messages.Where(x => IsTeamCityUser(x))
					.Reverse()
					.FirstOrDefault(x => x.MessageText.StartsWith(GetStatusNotification()))
					.ToStatusObject();
		}

		private void SetStateLabelText(State state)
		{
			this.StateValue.Text = state.ToString();
		}

		private void SetStateLabelColor(Status status)
		{
			if (IsFailed(status))
			{
				this.StateValue.Foreground = new SolidColorBrush(Colors.Red);
			}
			else
			{
				this.StateValue.Foreground = new SolidColorBrush(Colors.DarkSeaGreen);
			}
		}

		private void ToggleEnableDisableRunButton(State state)
		{
			if (IsRunning(state))
			{
				this.RunButton.IsEnabled = false;
			}
			else
			{
				this.RunButton.IsEnabled = true;
			}
		}

		private bool IsFailed(Status status)
		{
			return status == Status.Failed;
		}

		private bool IsRunning(State state)
		{
			return state == State.Running;
		}

		private bool IsInvalidMessage(StatusMessage message)
		{
			if (ReferenceEquals(null, message))
			{
				return true;
			}

			return message.Status == Status.Invalid || message.State == State.Invalid;
		}

		private void DisableAllContent()
		{
			this.RunButton.IsEnabled = false;
			this.DescriptionText.Foreground = new SolidColorBrush(Colors.DarkGray);
			this.StatusText.Foreground = new SolidColorBrush(Colors.DarkGray);
			this.StateValue.Text = "not available";
		}

		private static dynamic IsTeamCityUser(Message x)
		{
			// if TeamCity user was mentioned in HipChat by someone else
			if (x.From.GetType() == typeof(JObject))
			{
				return false;
			}

			return x.From == TeamCityUserName;
		}

		private string GetStatusNotification()
		{
			Guard.NotNullOrEmpty(() => ClientsAppellative, ClientsAppellative);
			Guard.NotNullOrEmpty(() => StatusCommand, StatusCommand);
			Guard.NotNull(() => this.item, this.item);

			return string.Format("@{0} {1} {2}", ClientsAppellative, StatusCommand, this.item.Configuration);
		}

		private string GetRunNotification()
		{
			Guard.NotNullOrEmpty(() => TeamCityUserName, TeamCityUserName);
			Guard.NotNullOrEmpty(() => RunCommand, RunCommand);
			Guard.NotNull(() => this.item, this.item);

			return string.Format("@{0} {1} {2}", TeamCityUserName, RunCommand, this.item.Configuration);
		}

		#endregion

		#region Constants and Fields

		private static readonly string RoomName;

		private static readonly string Token;

		private static readonly string RunCommand;

		private static readonly string StatusCommand;

		private static readonly string TeamCityUserName;

		private static readonly string ClientsAppellative;

		private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

		private readonly HipChatClient hipChat;

		private readonly NavigationHelper navigationHelper;

		private ConfigurationItem item;

		private DateTime lastTimeStatusChecked;

		#endregion

		#region NavigationHelper registration

		/// <summary>
		///     The methods provided in this section are simply used to allow
		///     NavigationHelper to respond to the page's navigation methods.
		///     <para>
		///         Page specific logic should be placed in event handlers for the
		///         <see cref="NavigationHelper.LoadState" />
		///         and <see cref="NavigationHelper.SaveState" />.
		///         The navigation parameter is available in the LoadState method
		///         in addition to page state preserved during an earlier session.
		///     </para>
		/// </summary>
		/// <param name="e">
		///     Provides data for navigation methods and event
		///     handlers that cannot cancel the navigation request.
		/// </param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedFrom(e);
		}

		#endregion
	}
}