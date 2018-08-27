using System;
using Diplomata.Dictionaries;

namespace Diplomata.Helpers
{
  public class DictionariesHelper
  {
    public static AttributeDictionary ContainsKey(AttributeDictionary[] array, string key)
    {
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i].key == key)
        {
          return array[i];
        }
      }

      return null;
    }

    public static LanguageDictionary ContainsKey(LanguageDictionary[] array, string key)
    {
      for (int i = 0; i < array.Length; i++)
      {
        if (array[i].key == key)
        {
          return array[i];
        }
      }

      return null;
    }
  }
}
