using DamnLibrary.Types;

namespace DamnLibrary.DamnLibrary.Utilities.Extensions;

public static class DictionaryExtensions
{
	/// <summary>
	/// Add all elements from other dictionary to this dictionary
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="other">Dictionary to add</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, Dictionary<TKey, TValue> other)
	{
		foreach (var otherPair in other)
		{
			dictionary.Add(otherPair.Key, otherPair.Value);
		}
	}

	/// <summary>
	/// Add all elements from array of Pair(Key, Value) to this dictionary
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="pairs">Array of pairs</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
		IEnumerable<Pair<TKey, TValue>> pairs)
	{
		foreach (var otherPair in pairs)
		{
			dictionary.Add(otherPair.First, otherPair.Second);
		}
	}

	/// <summary>
	/// Add all elements from array of KeyValuePair(Key, Value) to this dictionary
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="pairs">Array of pairs</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
		IEnumerable<KeyValuePair<TKey, TValue>> pairs)
	{
		foreach (var otherPair in pairs)
		{
			dictionary.Add(otherPair.Key, otherPair.Value);
		}
	}

	/// <summary>
	/// Add element to dictionary if it doesn't exist, otherwise change it
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="key">Key</param>
	/// <param name="value">Value</param>
	/// <typeparam name="TKey">Type of the key</typeparam>
	/// <typeparam name="TValue">Type of the value</typeparam>
	public static void AddOrChange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary.ContainsKey(key))
			dictionary[key] = value;
		else
			dictionary.Add(key, value);
	}

	/// <summary>
	/// Add element to dictionary if it doesn't exist
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="key">Key</param>
	/// <param name="value">Value</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	/// <returns></returns>
	public static bool AddIfNotExist<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary.ContainsKey(key))
			return false;

		dictionary.Add(key, value);
		return true;
	}

	/// <summary>
	/// Get value from dictionary if it exists, otherwise add it
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="key">Key</param>
	/// <param name="value">Value</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	/// <returns></returns>
	public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary.TryGetValue(key, out var result))
			return result;

		dictionary.Add(key, value);
		return value;
	}

	/// <summary>
	/// Get value from dictionary if it exists, otherwise add default value
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="key">Key</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	/// <returns></returns>
	public static TValue GetOrAddDefault<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
	{
		if (dictionary.TryGetValue(key, out var result))
			return result;

		dictionary.Add(key, default);
		return default;
	}

	/// <summary>
	/// Change value in dictionary if it exists
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="key">Key</param>
	/// <param name="value">Value</param>
	/// <typeparam name="TKey">Type of the keys</typeparam>
	/// <typeparam name="TValue">Type of the values</typeparam>
	/// <returns></returns>
	public static bool ChangeIfExist<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
	{
		if (dictionary.ContainsKey(key))
			return false;

		dictionary[key] = value;
		return true;
	}

	/// <summary>
	/// Cast dictionary to another values type with provided cast function
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="castFunction">Cast function</param>
	/// <typeparam name="TKey">Key type</typeparam>
	/// <typeparam name="TCurrentValue">Current value type</typeparam>
	/// <typeparam name="TNewValue">New value type</typeparam>
	/// <returns></returns>
	public static Dictionary<TKey, TNewValue> FuncCastValues<TKey, TCurrentValue, TNewValue>(
		this Dictionary<TKey, TCurrentValue> dictionary, Func<TCurrentValue, TNewValue> castFunction) =>
		dictionary.ToDictionary((pair) => pair.Key, (pair) => castFunction.Invoke(pair.Value));
	
	/// <summary>
	/// Cast dictionary to another values type with provided cast function
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="castFunction">Cast function</param>
	/// <typeparam name="TValue">Value type</typeparam>
	/// <typeparam name="TCurrentKey">Current key type</typeparam>
	/// <typeparam name="TNewKey">New key type</typeparam>
	/// <returns></returns>
	public static Dictionary<TNewKey, TValue> FuncCastKeys<TValue, TCurrentKey, TNewKey>(
		this Dictionary<TCurrentKey, TValue> dictionary, Func<TCurrentKey, TNewKey> castFunction) =>
		dictionary.ToDictionary((pair) => castFunction.Invoke(pair.Key), (pair) => pair.Value);

	/// <summary>
	/// Cast dictionary to another keys and values types with provided cast function
	/// </summary>
	/// <param name="dictionary">Dictionary</param>
	/// <param name="castFunction">Cast function</param>
	/// <typeparam name="TCurrentKey">Current keys type</typeparam>
	/// <typeparam name="TCurrentValue">Current values type</typeparam>
	/// <typeparam name="TNewKey">New keys type</typeparam>
	/// <typeparam name="TNewValue">New values type</typeparam>
	/// <returns></returns>
	public static Dictionary<TNewKey, TNewValue> FuncCast< TCurrentKey, TCurrentValue, TNewKey, TNewValue>(
		this Dictionary<TCurrentKey, TCurrentValue> dictionary,
		Func<KeyValuePair<TCurrentKey, TCurrentValue>, KeyValuePair<TNewKey, TNewValue>> castFunction) =>
		dictionary.Select(castFunction.Invoke).ToDictionary(newPair => newPair.Key, newPair => newPair.Value);
}