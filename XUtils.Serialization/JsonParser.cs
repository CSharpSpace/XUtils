using System;
using System.Collections;
using System.Collections.Generic;
namespace XUtils.Serialization
{
	internal class JsonParser
	{
		internal static JsonObject ParseRawObject(IEnumerator enumerable)
		{
			JsonObject jsonObject = new JsonObject();
			while (enumerable.MoveNext())
			{
				object current = enumerable.Current;
				if (current is DictionaryEntry)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)current;
					string key = (string)dictionaryEntry.Key;
					if (dictionaryEntry.Value is ArrayList)
					{
						IEnumerator enumerator = (dictionaryEntry.Value as ArrayList).GetEnumerator();
						IList<JsonObject> value = JsonParser.ParseRawObjects(enumerator);
						jsonObject[key] = value;
					}
					else
					{
						if (dictionaryEntry.Value is IDictionary)
						{
							IEnumerator enumerator2 = (dictionaryEntry.Value as IDictionary).GetEnumerator();
							jsonObject[key] = JsonParser.ParseRawObject(enumerator2);
						}
						else
						{
							jsonObject[key] = dictionaryEntry.Value;
						}
					}
				}
			}
			return jsonObject;
		}
		internal static IList<JsonObject> ParseRawObjects(IEnumerator enumerable)
		{
			IList<JsonObject> list = new List<JsonObject>();
			while (enumerable.MoveNext())
			{
				JsonObject jsonObject = new JsonObject();
				object current = enumerable.Current;
				if (current is IDictionary)
				{
					IDictionary dictionary = current as IDictionary;
					foreach (string key in dictionary.Keys)
					{
						if (dictionary[key] is ArrayList)
						{
							IEnumerator enumerator2 = (dictionary[key] as ArrayList).GetEnumerator();
							IList<JsonObject> value = JsonParser.ParseRawObjects(enumerator2);
							jsonObject[key] = value;
						}
						else
						{
							if (dictionary[key] is IDictionary)
							{
								IEnumerator enumerator3 = (dictionary[key] as IDictionary).GetEnumerator();
								jsonObject[key] = JsonParser.ParseRawObject(enumerator3);
							}
							else
							{
								jsonObject[key] = dictionary[key];
							}
						}
					}
				}
				list.Add(jsonObject);
			}
			return list;
		}
	}
}
