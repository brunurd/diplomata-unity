using System;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models.Submodels;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// The item class.
  /// </summary>
  [Serializable]
  public class Item : Data
  {
    [SerializeField] private string uniqueId = Guid.NewGuid().ToString();
    public int id;
    public LanguageDictionary[] name;
    public LanguageDictionary[] description;
    public string imagePath = string.Empty;
    public string highlightImagePath = string.Empty;

    [SerializeField]
    private string category = string.Empty;

    [NonSerialized]
    public Texture2D image;
    private Sprite sprite;

    [NonSerialized]
    public Texture2D highlightImage;

    [NonSerialized]
    public Sprite highlightSprite;

    [NonSerialized]
    public bool have;

    [NonSerialized]
    public bool discarded;

    /// <summary>
    /// The item category.
    /// </summary>
    /// <value>The item category string.</value>
    public string Category
    {
      get
      {
        return category;
      }
      set
      {
        category = value.ToLower();
      }
    }

    /// <summary>
    /// Property to get sprite after it's setted.
    /// </summary>
    /// <value>Sprite from image.</value>
    public Sprite Sprite
    {
      get
      {
        SetImageAndSprite();
        return sprite;
      }
    }

    /// <summary>
    /// Get the item unique id.
    /// </summary>
    /// <returns>The unique id(a string guid).</returns>
    public string GetId()
    {
      return uniqueId;
    }

    /// <summary>
    /// Set image and sprite from the path.
    /// </summary>
    public void SetImageAndSprite()
    {
      if (image == null || sprite == null)
      {
        image = (Texture2D) Resources.Load(imagePath);
        highlightImage = (Texture2D) Resources.Load(highlightImagePath);

        if (image != null)
        {
          sprite = Sprite.Create(
            image,
            new Rect(0, 0, image.width, image.height),
            new Vector2(0.5f, 0.5f)
          );
        }

        if (highlightImage != null)
        {
          highlightSprite = Sprite.Create(
            highlightImage,
            new Rect(0, 0, highlightImage.width, highlightImage.height),
            new Vector2(0.5f, 0.5f)
          );
        }
      }
    }

    /// <summary>
    /// Instantiate a item with a id.
    /// </summary>
    /// <param name="id">The item id.</param>
    public Item(int id, Options options)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.id = id;

      foreach (Language language in options.languages)
      {
        name = ArrayHelper.Add(name, new LanguageDictionary(language.name, "[ Edit to change this name ]"));
        description = ArrayHelper.Add(description, new LanguageDictionary(language.name, ""));
      }
    }

    /// <summary>
    /// Set the uniqueId if it is empty or null.
    /// </summary>
    /// <returns>Return true if it change or false if don't.</returns>
    public bool SetId()
    {
      if (uniqueId == string.Empty || uniqueId == null)
      {
        uniqueId = Guid.NewGuid().ToString();
        return true;
      }
      return false;
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

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var item = new ItemPersistent();
      item.id = uniqueId;
      item.have = have;
      item.discarded = discarded;
      return item;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var itemPersistentData = (ItemPersistent) persistentData;
      uniqueId = itemPersistentData.id;
      have = itemPersistentData.have;
      discarded = itemPersistentData.discarded;
    }

    /// <summary>
    /// Return the name of the item.
    /// </summary>
    /// <param name="language">The language of the name.</param>
    /// <returns>The name of the item or empty.</returns>
    public string DisplayName(string language)
    {
      var nameResult = DictionariesHelper.ContainsKey(name, language);
      if (nameResult == null) return string.Empty;
      return nameResult.value;
    }

    /// <summary>
    /// Return the description of the item.
    /// </summary>
    /// <param name="language">The language of the name.</param>
    /// <returns>The description of the item or empty.</returns>
    public string DisplayDescription(string language)
    {
      var nameResult = DictionariesHelper.ContainsKey(description, language);
      if (nameResult == null) return string.Empty;
      return nameResult.value;
    }
  }
}
