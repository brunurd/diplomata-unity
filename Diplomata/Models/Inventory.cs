using System;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata.Models
{
  /// <summary>
  /// The inventory class with all items and items categories.
  /// </summary>
  [Serializable]
  public class Inventory : Data
  {
    public Item[] items = new Item[0];
    [SerializeField] private string[] categories = new string[0];
    private int equipped = -1;

    /// <summary>
    /// All used categories in the items.
    /// </summary>
    /// <value>A array of categories.</value>
    public string[] Categories
    {
      get
      {
        return categories;
      }
    }

    /// <summary>
    /// Add a category to the categories array.
    /// </summary>
    /// <param name="category">The category to add.</param>
    public void AddCategory(string category)
    {
      if (category != "" && category != null)
      {
        if (categories == null)
        {
          categories = new string[0];
        }
        if (!ArrayHelper.Contains(categories, category))
          categories = ArrayHelper.Add(categories, category);
        RemoveNotUsedCategory();
      }
    }

    /// <summary>
    /// Clean the categories array removing unused categories.
    /// </summary>
    public void RemoveNotUsedCategory()
    {
      foreach (var category in categories)
      {
        var itemsWithCategory = Find.In(items).Where("category", category).Results;
        if (itemsWithCategory.Length == 0)
        {
          categories = ArrayHelper.Remove(categories, category);
        }
      }
    }

    /// <summary>
    /// Return if the player has a equipped item.
    /// </summary>
    /// <returns>True if the player is equipped with something.</returns>
    public bool IsEquipped()
    {
      if (equipped == -1)
      {
        return false;
      }

      else
      {
        return true;
      }
    }

    /// <summary>
    /// Return if the player has a specific equipped item.
    /// </summary>
    /// <param name="id">The item id.</param>
    /// <returns>True if the player is equipped with this id.</returns>
    public bool IsEquipped(int id)
    {
      if (id == equipped)
      {
        return true;
      }

      else
      {
        return false;
      }
    }

    /// <summary>
    /// Get the equipped id.
    /// </summary>
    /// <returns>The item id.</returns>
    public int GetEquipped()
    {
      return equipped;
    }

    /// <summary>
    /// Equip a specific item.
    /// </summary>
    /// <param name="id">The item id.</param>
    public void Equip(int id)
    {
      for (int i = 0; i < items.Length; i++)
      {
        if (items[i].id == id)
        {
          equipped = id;
          break;
        }

        else if (i == items.Length - 1)
        {
          equipped = -1;
        }
      }
    }

    /// <summary>
    /// Equip a item by the name in a specific language.
    /// </summary>
    /// <param name="name">The item name.</param>
    /// <param name="language">The item name language.</param>
    public void Equip(string name, string language = "English")
    {

      foreach (Item item in items)
      {
        LanguageDictionary itemName = DictionariesHelper.ContainsKey(item.name, language);

        if (itemName.value == name && itemName != null)
        {
          Equip(item.id);
          break;
        }
      }

      if (equipped == -1)
      {
        Debug.LogError("Cannot find the item \"" + name + "\" in " + language +
          " in the inventory.");
      }
    }

    /// <summary>
    /// Unequip the player.
    /// </summary>
    public void UnEquip()
    {
      equipped = -1;
    }

    /// <summary>
    /// Set the items images and sprites.
    /// </summary>
    public void SetImagesAndSprites()
    {
      foreach (Item item in items)
      {
        item.image = (Texture2D) Resources.Load(item.imagePath);
        item.highlightImage = (Texture2D) Resources.Load(item.highlightImagePath);

        if (item.image != null)
        {
          item.sprite = Sprite.Create(
            item.image,
            new Rect(0, 0, item.image.width, item.image.height),
            new Vector2(0.5f, 0.5f)
          );
        }

        if (item.highlightImage != null)
        {
          item.highlightSprite = Sprite.Create(
            item.highlightImage,
            new Rect(0, 0, item.highlightImage.width, item.highlightImage.height),
            new Vector2(0.5f, 0.5f)
          );
        }
      }
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var inventory = new InventoryPersistent();
      inventory.items = Data.GetArrayData<ItemPersistent>(items);
      return inventory;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var inventoryPersistent = (InventoryPersistent) persistentData;
      items = Data.SetArrayData<Item>(items, inventoryPersistent.items);
    }
  }
}
