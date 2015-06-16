﻿#region Using Directives

using System;
using System.Collections.ObjectModel;
using System.Linq;

using Windows.ApplicationModel.Resources;
using Windows.Graphics.Display;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

using TeamCityHipChatUI.Common;
using TeamCityHipChatUI.DataModel;

#endregion

// The Hub Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace TeamCityHipChatUI
{
	/// <summary>
	///     A page that displays a grouped collection of items.
	/// </summary>
	public sealed partial class HubPage : Page
	{
		private readonly NavigationHelper navigationHelper;

		private readonly ObservableDictionary defaultViewModel = new ObservableDictionary();

		private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

		public HubPage()
		{
			InitializeComponent();

			// Hub is only supported in Portrait orientation
			DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

			NavigationCacheMode = NavigationCacheMode.Required;

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
			ObservableCollection<ConfigurationsGroup> dataGroups = await HubDataSource.GetGroupsAsync();

			LoadLastItemsState(dataGroups.Single().Items);

			DefaultViewModel["Groups"] = dataGroups;
		}

		private void LoadLastItemsState(ObservableCollection<ConfigurationItem> items)
		{
			foreach (ConfigurationItem item in items)
			{
				Status? status = ReferenceEquals(null, item.LastKnownState)
					? null as Status?
					: item.LastKnownState.Status;

				SetImagePath(item, status);
			}
		}

		private static void SetImagePath(ConfigurationItem item, Status? status)
		{
			switch (status)
			{
				case Status.Failed:
					item.ImagePath = string.Format("Assets/{0}Red.png", item.Title);
					break;
				case Status.Success:
					item.ImagePath = string.Format("Assets/{0}Green.png", item.Title);
					break;
				case Status.Invalid:
					item.ImagePath = string.Format("Assets/{0}Gray.png", item.Title);
					break;
				default:
					item.ImagePath = string.Format("Assets/{0}White.png", item.Title);
					break;
			}
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

		/// <summary>
		///     Shows the details of a clicked group in the <see cref="SectionPage" />.
		/// </summary>
		private void GroupSection_ItemClick(object sender, ItemClickEventArgs e)
		{
			string groupId = ((ConfigurationsGroup)e.ClickedItem).UniqueId;
			if (!Frame.Navigate(typeof(SectionPage), groupId))
			{
				throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
			}
		}

		/// <summary>
		///     Shows the details of an item clicked on in the <see cref="ItemPage" />
		/// </summary>
		private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
		{
			// Navigate to the appropriate destination page, configuring the new page
			// by passing required information as a navigation parameter
			string itemId = ((ConfigurationItem)e.ClickedItem).UniqueId;
			if (!Frame.Navigate(typeof(ItemPage), itemId))
			{
				throw new Exception(this.resourceLoader.GetString("NavigationFailedExceptionMessage"));
			}
		}

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
	}
}
