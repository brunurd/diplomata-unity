using System;
using Diplomata.Preferences;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Item
  {
    public int id;
    public LanguageDictionary[] name;
    public LanguageDictionary[] description;
    public string imagePath = string.Empty;
    public string highlightImagePath = string.Empty;

    [NonSerialized]
    public Texture2D image;

    [NonSerialized]
    public Sprite sprite;

    [NonSerialized]
    public Texture2D highlightImage;

    [NonSerialized]
    public Sprite highlightSprite;

    [NonSerialized]
    public bool have;

    [NonSerialized]
    public bool discarded;

    public Item(int id)
    {
      this.id = id;

      foreach (Language language in DiplomataManager.options.languages)
      {
        name = ArrayHelper.Add(name, new LanguageDictionary(language.name, "[ Edit to change this name ]"));
        description = ArrayHelper.Add(description, new LanguageDictionary(language.name, ""));
      }
    }

    public static Item Find(Item[] items, int itemId)
    {

      foreach (Item item in items)
      {
        if (item.id == itemId)
        {
          return item;
        }
      }

      return null;
    }

    public static Item Find(Item[] items, string name, string language = "English")
    {

      foreach (Item item in items)
      {
        LanguageDictionary itemName = DictionariesHelper.ContainsKey(item.name, language);

        if (itemName.value == name && itemName != null)
        {
          return item;
        }
      }

      Debug.LogWarning("This item doesn't exist.");
      return null;
    }
  }
}
