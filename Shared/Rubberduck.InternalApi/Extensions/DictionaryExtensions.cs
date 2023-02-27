﻿using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rubberduck.InternalApi.Extensions
{

    // See http://stackoverflow.com/questions/34362316/how-to-turn-icollectiont-into-ireadonlycollectiont
    public class ReadOnlyCollectionWrapper<T> : IReadOnlyCollection<T>
    {
        private ICollection<T> collection;
        public ReadOnlyCollectionWrapper(ICollection<T> collection)
        {
            this.collection = collection;
        }

        public int Count
        {
            get { return collection.Count; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return collection.GetEnumerator();
        }
    }

    public static class CollectionExtensions
    {
        public static IReadOnlyCollection<T> AsReadOnly<T>(this ICollection<T> collection)
        {
            return new ReadOnlyCollectionWrapper<T>(collection);
        }
    }

    public static class DictionaryExtensions
    {
        public static IDictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> source)
        {
            return new Dictionary<TKey, TValue>(source);
        }

        public static IEnumerable<TValue> AllValues<TKey, TValue>(
            this ConcurrentDictionary<TKey, ConcurrentBag<TValue>> source)
        {
            return source.SelectMany(item => item.Value).ToList();
        }

        public static IEnumerable<TValue> AllValues<TKey, TValue>(
            this IDictionary<TKey, IList<TValue>> source)
        {
            return source.SelectMany(item => item.Value).ToList();
        }

        public static IEnumerable<TValue> AllValues<TKey, TValue>(
            this IDictionary<TKey, List<TValue>> source)
        {
            return source.SelectMany(item => item.Value);
        }

        public static IEnumerable<TValue> AllValues<TKey1, TKey2, TValue>(
            this IDictionary<TKey1, IDictionary<TKey2, List<TValue>>> source)
        {
            return source.SelectMany(item => item.Value.AllValues());
        }

        public static ConcurrentDictionary<TKey, ConcurrentBag<TValue>> ToConcurrentDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> source)
        {
            return new ConcurrentDictionary<TKey, ConcurrentBag<TValue>>(source.Select(x => new KeyValuePair<TKey, ConcurrentBag<TValue>>(x.Key, new ConcurrentBag<TValue>(x))));
        }

        public static IDictionary<TKey, List<TValue>> ToDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> source)
        {
            return source.ToDictionary(group => group.Key, group => group.ToList());
        }

        public static IDictionary<TKey1, IDictionary<TKey2, List<TValue>>> ToDictionary<TKey1, TKey2, TValue>(this IEnumerable<IGrouping<TKey1, IGrouping<TKey2, TValue>>> source)
        {
            return source.ToDictionary(group => group.Key, group => group.ToDictionary());
        }

        public static IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> ToReadonlyDictionary<TKey, TValue>(this IEnumerable<IGrouping<TKey, TValue>> source)
        {
            return source.ToDictionary(group => group.Key, group => (IReadOnlyList<TValue>)group.ToList());
        }

        public static IReadOnlyDictionary<TKey, IReadOnlyList<TValue>> ToReadonlyDictionary<TKey, TValue>(this ConcurrentDictionary<TKey, ConcurrentBag<TValue>> source)
        {
            return source.ToDictionary(kvp => kvp.Key, kvp => (IReadOnlyList<TValue>)kvp.Value.ToList());
        }

        //See https://stackoverflow.com/a/3804852/5536802
        public static bool HasEqualContent<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IReadOnlyDictionary<TKey, TValue> otherDictionary)
        {
            return dictionary.Count == otherDictionary.Count && !dictionary.Except(otherDictionary).Any();
        }
    }
}
