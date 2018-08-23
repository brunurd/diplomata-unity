using System;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
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

      foreach (Language language in DiplomataData.options.languages)
      {
        name = ArrayHelper.Add(name, new LanguageDictionary(language.name, "[ Edit to change this name ]"));
        description = ArrayHelper.Add(description, new LanguageDictionary(language.name, ""));
      }
    }

    /// <summary>
    /// Find a item by id.
    /// </summary>
    /// <param name="array">A array of items.</param>
    /// <param name="itemId">The id of the item.</param>
    /// <returns>The item if found, or null.</returns>
    public static Item Find(Item[] array, int itemId)
    {
      return (Item) Helpers.Find.In(array).Where("id", itemId).Result;
    }

    /// <summary>
    /// Find a item by name in a specific language.
    /// </summary>
    /// <param name="items">A array of items.</param>
    /// <param name="name">The name of the item.</param>
    /// <param name="language">The specific language.</param>
    /// <returns>The item if found, or null.s</returns>
    public static Item Find(Item[] items, string name, string language = "English")
    {
      // TODO: use the Helpers.Find class here.
      foreach (Item item in items)
      {
        LanguageDictionary itemName = DictionariesHelper.ContainsKey(item.name, language);

        if (itemName.value == name && itemName != null)
        {
          return item;
        }
      }
      Debug.LogError("This item doesn't exist.");
      return null;
    }
  }
}
