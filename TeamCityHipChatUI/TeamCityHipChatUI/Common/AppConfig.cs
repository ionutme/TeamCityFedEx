#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Windows.Data.Xml.Dom;
using Windows.Storage;

#endregion

namespace TeamCityHipChatUI.Common
{
	public class AppConfig
	{
		public static Dictionary<string, string> Settings = new Dictionary<string, string>();
		
		public static async Task LoadSettings()
		{
			string config = await GetSettings();

			XmlElement[] configNodes = GetConfigNodes(config);
			foreach (XmlElement setting in configNodes)
			{
				string key = GetAttribute(setting, "key");
				if (!Settings.ContainsKey(key))
				{
					Settings.Add(key, GetAttribute(setting, "value"));
				}
			}
		}

		#region Private Methods

		private static XmlElement[] GetConfigNodes(string config)
		{
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(config);

			return xmlDocument.DocumentElement.ChildNodes.OfType<XmlElement>().ToArray();
		}

		private static string GetAttribute(IXmlNode settingsNode, string name)
		{
			Guard.NotNull(() => settingsNode, settingsNode);

			IXmlNode namedItem = settingsNode.Attributes.GetNamedItem(name);
			if (ReferenceEquals(null, namedItem))
			{
				throw new NullReferenceException(string.Format("The settings node cannot be loaded!"));
			}

			return namedItem.InnerText;
		}

		private static async Task<string> GetSettings()
		{
			var dataUri = new Uri("ms-appx:///appsettings.config");

			StorageFile file = await StorageFile.GetFileFromApplicationUriAsync(dataUri);

			return await FileIO.ReadTextAsync(file);
		}

		#endregion
	}
}