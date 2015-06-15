#region Using Directives

using System;

using Newtonsoft.Json;

#endregion

namespace TeamCityHipChatUI.DataModel
{
	/// <summary>
	///     Generic item data model.
	/// </summary>
	public class ConfigurationItem
	{
		public ConfigurationItem(
			string uniqueId,
			string title,
			string subtitle,
			string imagePath,
			string failedImagePath,
			string successImagePath,
			string description,
			string content,
			string configuration,
			Status lastStatus,
			State lastState,
			DateTime lastRunDateTime)
		{
			UniqueId = uniqueId;
			Title = title;
			Subtitle = subtitle;
			Description = description;
			ImagePath = imagePath;
			FailedImagePath = failedImagePath;
			SuccessImagePath = successImagePath;
			Content = content;
			Configuration = configuration;
			LastStatus = lastStatus;
			LastState = lastState;
			LastRunDateTime = lastRunDateTime;
		}

		[JsonProperty("UniqueId")]
		public string UniqueId { get; set; }

		[JsonProperty("Title")]
		public string Title { get; set; }

		[JsonProperty("Subtitle")]
		public string Subtitle { get; set; }

		[JsonProperty("Description")]
		public string Description { get; set; }

		[JsonProperty("ImagePath")]
		public string ImagePath { get; set; }

		[JsonProperty("FailedImagePath")]
		public string FailedImagePath { get; set; }

		[JsonProperty("SuccessImagePath")]
		public string SuccessImagePath { get; set; }

		[JsonProperty("Content")]
		public string Content { get; set; }

		[JsonProperty("Configuration")]
		public string Configuration { get; set; }

		[JsonProperty("LastStatus")]
		public Status LastStatus { get; set; }

		[JsonProperty("LastState")]
		public State LastState { get; set; }

		[JsonProperty("LastRunDateTime")]
		public DateTime LastRunDateTime { get; set; }

		public override string ToString()
		{
			return Title;
		}
	}
}