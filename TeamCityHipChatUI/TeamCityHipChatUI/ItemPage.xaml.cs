#region Using Directives

using System;
using System.Threading.Tasks;

using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
		#region Constructors

		public ItemPage()
		{
			InitializeComponent();

			chatService = new ChatService();

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
			Guard.NotNull(() => this.item, this.item);

			DefaultViewModel["Item"] = this.item;

			await SetStatus();
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
			//TODO: Save the unique state of the page here
		}

		/// <summary>
		///     Provides the functionality of starting the job on CI machine for the chosen configuration.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Data associated with this event.</param>
		private async void RunButton_OnClick(object sender, RoutedEventArgs e)
		{
			await ShowMessageDialogToContinue();
		}

		/// <summary>
		/// Refresh the status of the current configuration event.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">Event arguments.</param>
		private async void SyncButton_OnClick(object sender, RoutedEventArgs e)
		{
			await SetStatus();
		}

		#region Message Dialog Methods

		private async Task ShowMessageDialogToContinue()
		{
			var messageDialog = new MessageDialog(GetConfirmationMessage(), "Continue");
			messageDialog.Commands.Add(new UICommand("Yes", YesCommandInvokedHandler));
			messageDialog.Commands.Add(new UICommand("Cancel"));

			// Set the command that will be invoked by default
			messageDialog.DefaultCommandIndex = 0;

			// Set the command to be invoked when escape is pressed
			messageDialog.CancelCommandIndex = 1;

			await messageDialog.ShowAsync();
		}

		private async void YesCommandInvokedHandler(IUICommand command)
		{
			this.RefreshProgressRing.IsActive = true;
			this.SyncButton.IsEnabled = false;

			await SendRunCommand();

			// start counting once every 9 seconds
			Countdown(
				9,
				TimeSpan.FromSeconds(1),
				counter => this.CounterLabel.Text = counter.ToString(),
				async () =>
				{
					await SetStatus();
					this.RefreshProgressRing.IsActive = false;
					this.SyncButton.IsEnabled = true;
				});
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
			await chatService.SendNotificationAsync(this.item.Configuration);
		}

		private async Task SetStatus()
		{
			StatusMessage message = await chatService.GetStatusMessageAsync(this.item.Configuration);

			UpdateItemLastKnownState(message);

			if (IsInvalidMessage(message))
			{
				DisableAllContent();
			}
			else
			{
				// TODO: enable all content
				SetStateLabelText(message.State);
			}
		}

		private void UpdateItemLastKnownState(StatusMessage message)
		{
			this.item.LastKnownState = message;
		}

		private bool IsInvalidMessage(StatusMessage message)
		{
			if (ReferenceEquals(null, message))
			{
				return true;
			}

			return message.Status == Status.Invalid || message.State == State.Invalid;
		}

		private void SetStateLabelText(State state)
		{
			this.StateValueLabel.Text = state.ToString();
		}

		private void DisableAllContent()
		{
			this.RunButton.IsEnabled = false;

			Brush disabledForegroundBrush = Resources["DisabledForeground"] as Brush;
			this.DescriptionLabel.Foreground = disabledForegroundBrush;
			this.StatusLabel.Foreground = disabledForegroundBrush;

			this.StateValueLabel.Text = "not available";
		}

		private void Countdown(int count, TimeSpan interval, Action<int> refreshAction, Action stopAction)
		{
			var timer = new DispatcherTimer { Interval = interval };
			timer.Tick += (sender, eventArgs) =>
			{
				if (count-- == 1)
				{
					timer.Stop();
					stopAction();
				}
				else
				{
					refreshAction(count);
				}
			};

			refreshAction(count);

			timer.Start();
		}

		#endregion

		#region Constants and Fields

		private static ChatService chatService;

		private readonly NavigationHelper navigationHelper;

		private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

		private ConfigurationItem item;

		#endregion

		#region NavigationHelper Registration

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