using TeamCityHipChatUI.Common;
using TeamCityHipChatUI.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using HipChat.Net;
using HipChat.Net.Http;
using HipChat.Net.Models.Response;

using Newtonsoft.Json;

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkID=391641

namespace TeamCityHipChatUI
{
    /// <summary>
    /// A page that displays details for a single item within a group.
    /// </summary>
    public sealed partial class ItemPage : Page
    {
	    private string _configuration;
        private readonly NavigationHelper navigationHelper;
        private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

	    readonly HipChatClient _hipChat = new HipChatClient(new ApiConnection(new Credentials("UxgzTgsGSbHK0A0gbIdZ9l1AP7rIFZLG58WEzWzA")));

        public ItemPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        } 

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            
			var item = await HubDataSource.GetItemAsync((string)e.NavigationParameter);
            this.DefaultViewModel["Item"] = item;
	        _configuration = item.Configuration;

	        SetStatus(item);
        }

	    /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

	    private async void RunButton_OnClick(object sender, RoutedEventArgs e)
	    {
		    this.RunButton.IsEnabled = false;

		    SendRunCommand();
	    }

	    private async void SendRunCommand()
	    {
		    // get all rooms
		    //IResponse<RoomItems<Entity>> rooms = await _hipChat.Rooms.GetAllAsync();

		    // get room members
		    //Task<IResponse<RoomItems<Mention>>> members = _hipChat.Rooms.GetMembersAsync("Aidaws");

		    // send room notification
		    await _hipChat.Rooms.SendNotificationAsync("Aidaws", "@TeamCity run " + _configuration);
	    }

		private async void SetStatus(ConfigurationItem item)
		{
			// get recent room message history
		    IResponse<RoomItems<Message>> jsonHistory = await _hipChat.Rooms.GetHistoryAsync("Aidaws");
			StatusMessage message = GetStatusMessage(jsonHistory, _configuration);

			if (IsInvalidStatusMessage(message))
			{
				DisableAllContent();
				return;
			}

			StatusValue.Text = message.State.ToString();
			if (IsRunning(message.State))
			{
				this.RunButton.IsEnabled = false;
			}

			if (IsFailed(message.Status))
			{
				this.StatusValue.Foreground = new SolidColorBrush(Colors.Red);
			}
			else
			{
				this.StatusValue.Foreground = new SolidColorBrush(Colors.DarkSeaGreen);
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

	    private bool IsInvalidStatusMessage(StatusMessage message)
	    {
		    if (ReferenceEquals(null, message)) return true;

		    return message.Status == Status.Invalid || message.State == State.Invalid;
	    }

	    private void DisableAllContent()
	    {
		    this.RunButton.IsEnabled = false;
		    this.DescriptionText.Foreground = new SolidColorBrush(Colors.DarkGray);
		    this.StatusText.Foreground = new SolidColorBrush(Colors.DarkGray);
		    this.StatusValue.Text = "not available";
	    }

	    private StatusMessage GetStatusMessage(IResponse<RoomItems<Message>> jsonHistory, string config)
	    {
		    return
			    JsonConvert.DeserializeObject<RoomItems<Message>>(jsonHistory.Body.ToString())
				    .Items.Where(x => x.From == "TeamCity")
				    .Reverse()
					.FirstOrDefault(x => x.MessageText.StartsWith(string.Format("@Clients status {0}", config)))
				    .ToStatusObject();
	    }
    }
}
