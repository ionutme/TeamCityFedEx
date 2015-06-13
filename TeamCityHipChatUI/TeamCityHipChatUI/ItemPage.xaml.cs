#region Using Directives

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI;
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
			ConfigurationItem item = await HubDataSource.GetItemAsync(e.NavigationParameter as string);
			DefaultViewModel["Item"] = item;

			// set DEV or RELEASE
			this.itemConfiguration = item.Configuration;

			await SetStatus(item);
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
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			// TODO: Save the unique state of the page here.
		}

		private async void RunButton_OnClick(object sender, RoutedEventArgs e)
		{
			this.RunButton.IsEnabled = false;

			await SendRunCommand();
		}

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
			Guard.NotNullOrEmpty(() => this.itemConfiguration, this.itemConfiguration);

			return string.Format("@{0} {1} {2}", ClientsAppellative, StatusCommand, this.itemConfiguration);
		}

		private string GetRunNotification()
		{
			Guard.NotNullOrEmpty(() => TeamCityUserName, TeamCityUserName);
			Guard.NotNullOrEmpty(() => RunCommand, RunCommand);
			Guard.NotNullOrEmpty(() => this.itemConfiguration, this.itemConfiguration);

			return string.Format("@{0} {1} {2}", TeamCityUserName, RunCommand, this.itemConfiguration);
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

		private string itemConfiguration;

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