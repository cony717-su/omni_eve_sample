using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shiftup.CommonLib
{
    static public class DictionaryExtension
    {
        static public void Merge<TKey, TContainer, TValue>(this IDictionary<TKey, TContainer> dict, IDictionary<TKey, TContainer> from)
            where TContainer : ICollection<TValue>, new()
        {
            foreach (var kvp in from)
            {
                if (dict.ContainsKey(kvp.Key) == false)
                    dict.Add(kvp.Key, new TContainer());

                foreach (var v in kvp.Value)
                    dict[kvp.Key].Add(v);
            }
        }

        static public void Merge<TKey, TValue>(this IDictionary<TKey, HashSet<TValue>> dict, IDictionary<TKey, HashSet<TValue>> from)
        {
            dict.Merge<TKey, HashSet<TValue>, TValue>(from);
        }
        static public void Merge<TKey, TValue>(this IDictionary<TKey, List<TValue>> dict, IDictionary<TKey, List<TValue>> from)
        {
            dict.Merge<TKey, List<TValue>, TValue>(from);
        }

    }
    static public class ReadOnlyDictionaryExtension
    {
        static public ReadOnlyDictionary<TKey, TValue> AsReadOnly<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return new ReadOnlyDictionary<TKey, TValue>(dict);
        }


        static public ReadOnlyDictionary<TKey, TBaseContainer>
            AsReadOnly<TKey, TBaseContainer, TContainer>(this IDictionary<TKey, TContainer> dict)
                where TContainer : TBaseContainer
                where TBaseContainer : IEnumerable
        {
            return new ReadOnlyDictionary<TKey, TBaseContainer>(
                dict.ToDictionary<KeyValuePair<TKey, TContainer>, TKey, TBaseContainer>(kvp => kvp.Key, kvp => kvp.Value));
        }
    }
    public class ReadOnlyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly IDictionary<TKey, TValue> _dictionary;

        public ReadOnlyDictionary()
        {
            _dictionary = new Dictionary<TKey, TValue>();
        }
        public ReadOnlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }

        #region IDictionary<TKey,TValue> Members

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            throw ReadOnlyException();
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dictionary.Keys; }
        }

        bool IDictionary<TKey, TValue>.Remove(TKey key)
        {
            throw ReadOnlyException();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _dictionary.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get { return _dictionary.Values; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _dictionary[key];
            }
        }

        TValue IDictionary<TKey, TValue>.this[TKey key]
        {
            get
            {
                return this[key];
            }
            set
            {
                throw ReadOnlyException();
            }
        }

        #endregion

        #region ICollection<KeyValuePair<TKey,TValue>> Members

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            throw ReadOnlyException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _dictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _dictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            throw ReadOnlyException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<TKey,TValue>> Members

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _dictionary.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private static Exception ReadOnlyException()
        {
            return new NotSupportedException("This dictionary is read-only");
        }
    }
}
