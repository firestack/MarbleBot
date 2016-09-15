using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace marbleBot.ViewModels
{
	public class DictList<K,V> : IDictionary<K, V>, IList<V>, ICollectionViewFactory
	{
		protected object dictLock = new object();
		protected Dictionary<K, int> dict = new Dictionary<K, int>();

		protected object itemsLock = new object();
		protected System.Collections.ObjectModel.ObservableCollection<V> items = new  System.Collections.ObjectModel.ObservableCollection<V>();

		public DictList()
		{
			System.Windows.Data.BindingOperations.EnableCollectionSynchronization(items, itemsLock);
			System.Windows.Data.BindingOperations.EnableCollectionSynchronization(dict, dictLock);
		}

		#region Dictionary Interface
		
		V IDictionary<K, V>.this[K key]
		{
			get
			{
				return items[dict[key]];
			}

			set
			{
				if (!dict.ContainsKey(key))
				{
					items.Add(value);
					dict[key] = items.Count - 1;
					return;
				}
				items[dict[key]] = value;
			}
		}

		int ICollection<KeyValuePair<K, V>>.Count
		{
			get
			{
				return dict.Count;
			}
		}

		bool ICollection<KeyValuePair<K, V>>.IsReadOnly
		{
			get
			{
				return false;
			}
		}

		ICollection<K> IDictionary<K, V>.Keys
		{
			get
			{
				return dict.Keys;
			}
		}

		ICollection<V> IDictionary<K, V>.Values
		{
			get
			{
				return items;
			}
		}

		void ICollection<KeyValuePair<K, V>>.Add(KeyValuePair<K, V> item)
		{
			if (!dict.ContainsKey(item.Key))
			{
				items.Add(item.Value);
				dict[item.Key] = items.Count - 1;
				return;
			}
			items[dict[item.Key]] = item.Value;
		}

		void IDictionary<K, V>.Add(K key, V value)
		{
			if (!dict.ContainsKey(key))
			{
				items.Add(value);
				dict[key] = items.Count - 1;
				return;
			}
			items[dict[key]] = value;
		}

		void ICollection<KeyValuePair<K, V>>.Clear()
		{
			dict.Clear();
		}

		bool ICollection<KeyValuePair<K, V>>.Contains(KeyValuePair<K, V> item)
		{
			return dict.ContainsKey(item.Key) && items.Contains(item.Value);
		}

		bool IDictionary<K, V>.ContainsKey(K key)
		{
			return dict.ContainsKey(key);
		}

		void ICollection<KeyValuePair<K, V>>.CopyTo(KeyValuePair<K, V>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return items.GetEnumerator();
		}

		IEnumerator<KeyValuePair<K, V>> IEnumerable<KeyValuePair<K, V>>.GetEnumerator()
		{
			return new DictListEnumerator<K, V>(this);
		}

		bool ICollection<KeyValuePair<K, V>>.Remove(KeyValuePair<K, V> item)
		{
			return ((IDictionary<K, V>)this).Remove(item.Key);
		}

		bool IDictionary<K, V>.Remove(K key)
		{
			if (!dict.ContainsKey(key))
			{
				return false;
			}
			items.RemoveAt(dict[key]);
			return dict.Remove(key);
		}

		bool IDictionary<K, V>.TryGetValue(K key, out V value)
		{
			if (dict.ContainsKey(key))
			{
				value = ((IDictionary<K, V>)this)[key];
				return true;
			}
			value = default(V);
			return false;
		}
		#endregion

		#region List Interface
		public V this[int index]
		{
			get
			{
				return ((IList<V>)items)[index];
			}

			set
			{
				((IList<V>)items)[index] = value;
			}
		}

		public int Count
		{
			get
			{
				return ((IList<V>)items).Count;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return ((IList<V>)items).IsReadOnly;
			}
		}

		public void Add(V item)
		{
			((IList<V>)items).Add(item);
		}

		public void Clear()
		{
			((IList<V>)items).Clear();
		}

		public bool Contains(V item)
		{
			return ((IList<V>)items).Contains(item);
		}

		public void CopyTo(V[] array, int arrayIndex)
		{
			((IList<V>)items).CopyTo(array, arrayIndex);
		}

		public IEnumerator<V> GetEnumerator()
		{
			return ((IList<V>)items).GetEnumerator();
		}

		public int IndexOf(V item)
		{
			return ((IList<V>)items).IndexOf(item);
		}

		public void Insert(int index, V item)
		{
			((IList<V>)items).Insert(index, item);
		}

		public bool Remove(V item)
		{
			return ((IList<V>)items).Remove(item);
		}

		public void RemoveAt(int index)
		{
			((IList<V>)items).RemoveAt(index);
		}

		#endregion

		#region ICollectionViewFactory
		ICollectionView ICollectionViewFactory.CreateView()
		{
			return new System.Windows.Data.ListCollectionView(items);
		}
		#endregion

		#region Enumerator
		public class DictListEnumerator<TKey, TValue> : IEnumerator<KeyValuePair<TKey, TValue>>
		{
			protected DictList<TKey, TValue> dl;
			IEnumerator<KeyValuePair<TKey, int>> dictEnumerator;

			public DictListEnumerator(DictList<TKey, TValue> dl)
			{
				this.dl = dl;
				dictEnumerator = (dl as IDictionary<TKey, int>).GetEnumerator();
			}

			protected KeyValuePair<TKey, TValue> currentItem;

			public KeyValuePair<TKey, TValue> Current
			{
				get
				{
					return currentItem;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return currentItem;
				}
			}

			public void Dispose()
			{ }

			public bool MoveNext()
			{
				if (!dictEnumerator.MoveNext())
				{
					return false;
				}
				currentItem = new KeyValuePair<TKey, TValue>(dictEnumerator.Current.Key, ((IList<TValue>)dl)[dictEnumerator.Current.Value]);
				return true;
			}

			public void Reset()
			{
				dictEnumerator.Reset();
				currentItem = new KeyValuePair<TKey, TValue>();
			}
		}
		#endregion
	}


}
