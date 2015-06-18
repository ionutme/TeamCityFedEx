#region Using Directives

using System;

using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using TeamCityHipChatUI.Common;
using TeamCityHipChatUI.DataModel;

#endregion

namespace TeamCityHipChatUI
{
	public sealed partial class SectionPage : Page
	{
		public SectionPage()
		{
			InitializeComponent();

			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += NavigationHelper_LoadState;
			this.navigationHelper.SaveState += NavigationHelper_SaveState;
		}

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

		/// <summary>
		///     Shows the details of an item clicked on in the <see cref="ItemPage" />
		/// </summary>
		/// <param name="sender">The GridView displaying the item clicked.</param>
		/// <param name="e">Event data that describes the item clicked.</param>
		private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			string itemId = ((ConfigurationItem)e.ClickedItem).UniqueId;
			if (!Frame.Navigate(typeof(ItemPage), itemId))
			{
				ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

				throw new Exception(resourceLoader.GetString("NavigationFailedExceptionMessage"));
			}
		}

		/// <summary>
		///     Populates the page with content passed during navigation.  Any saved state is also
		///     provided when recreating a page from a prior session.
		/// </summary>
		/// <param name="sender">
		///     The source of the event; typically <see cref="NavigationHelper" />
		/// </param>
		/// <param name="e">
		///     Event data that provides both the navigation parameter passed to
		///     <see cref="Frame.Navigate(Type, Object)" /> when this page was initially requested and
		///     a dictionary of state preserved by this page during an earlier
		///     session.  The state will be null the first time a page is visited.
		/// </param>
		private async void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
		{
			ConfigurationGroup group = await HubDataSource.GetGroupAsync((string) e.NavigationParameter);

			DefaultViewModel["Group"] = group;
		}

		/// <summary>
		///     Preserves state associated with this page in case the application is suspended or the
		///     page is discarded from the navigation cache.  Values must conform to the serialization
		///     requirements of <see cref="SuspensionManager.SessionState" />.
		/// </summary>
		/// <param name="sender">The source of the event; typically <see cref="NavigationHelper" /></param>
		/// <param name="e">
		///     Event data that provides an empty dictionary to be populated with
		///     serializable state.
		/// </param>
		private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
		{
			// TODO: Save the unique state of the page here.
		}

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
		/// <param name="e">Event data that describes how this page was reached.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			this.navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private readonly NavigationHelper navigationHelper;

		private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();
	}
}
