namespace Rubberduck.Parsing;

public static class DictionaryExtensions
{
    public static bool HasEqualContent<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, IReadOnlyDictionary<TKey, TValue> otherDictionary)
    {
        return dictionary.Count == otherDictionary.Count && !dictionary.Except(otherDictionary).Any();
    }
}