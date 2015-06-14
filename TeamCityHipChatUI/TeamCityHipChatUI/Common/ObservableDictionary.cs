#region Using Directives

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Windows.Foundation.Collections;

#endregion

namespace TeamCityHipChatUI.Common
{
	/// <summary>
	///     Implementation of IObservableMap that supports reentrancy for use as a default view
	///     model.
	/// </summary>
	public class ObservableDictionary : IObservableMap<string, object>
	{
		public event MapChangedEventHandler<string, object> MapChanged;

		private void InvokeMapChanged(CollectionChange change, string key)
		{
			MapChangedEventHandler<string, object> eventHandler = MapChanged;
			if (eventHandler != null)
			{
				eventHandler(this, new ObservableDictionaryChangedEventArgs(change, key));
			}
		}

		public void Add(string key, object value)
		{
			this.dictionary.Add(key, value);
			InvokeMapChanged(CollectionChange.ItemInserted, key);
		}

		public void Add(KeyValuePair<string, object> item)
		{
			Add(item.Key, item.Value);
		}

		public bool Remove(string key)
		{
			if (this.dictionary.Remove(key))
			{
				InvokeMapChanged(CollectionChange.ItemRemoved, key);
				return true;
			}
			return false;
		}

		public bool Remove(KeyValuePair<string, object> item)
		{
			object currentValue;
			if (this.dictionary.TryGetValue(item.Key, out currentValue) && Equals(item.Value, currentValue) &&
			    this.dictionary.Remove(item.Key))
			{
				InvokeMapChanged(CollectionChange.ItemRemoved, item.Key);
				return true;
			}
			return false;
		}

		public object this[string key]
		{
			get
			{
				return this.dictionary[key];
			}
			set
			{
				this.dictionary[key] = value;
				InvokeMapChanged(CollectionChange.ItemChanged, key);
			}
		}

		public void Clear()
		{
			string[] priorKeys = this.dictionary.Keys.ToArray();
			this.dictionary.Clear();
			foreach (string key in priorKeys)
			{
				InvokeMapChanged(CollectionChange.ItemRemoved, key);
			}
		}

		public ICollection<string> Keys
		{
			get
			{
				return this.dictionary.Keys;
			}
		}

		public bool ContainsKey(string key)
		{
			return this.dictionary.ContainsKey(key);
		}

		public bool TryGetValue(string key, out object value)
		{
			return this.dictionary.TryGetValue(key, out value);
		}

		public ICollection<object> Values
		{
			get
			{
				return this.dictionary.Values;
			}
		}

		public bool Contains(KeyValuePair<string, object> item)
		{
			return this.dictionary.Contains(item);
		}

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.dictionary.GetEnumerator();
		}

		public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			int arraySize = array.Length;
			foreach (KeyValuePair<string, object> pair in this.dictionary)
			{
				if (arrayIndex >= arraySize)
				{
					break;
				}
				array[arrayIndex++] = pair;
			}
		}

		private readonly Dictionary<string, object> dictionary = new Dictionary<string, object>();

		private class ObservableDictionaryChangedEventArgs : IMapChangedEventArgs<string>
		{
			public ObservableDictionaryChangedEventArgs(CollectionChange change, string key)
			{
				CollectionChange = change;
				Key = key;
			}

			public CollectionChange CollectionChange
			{
				get;
				private set;
			}

			public string Key
			{
				get;
				private set;
			}
		}
	}
}
