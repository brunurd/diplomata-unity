using System;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Inventory : Data
  {
    public Item[] items = new Item[0];
    private int equipped = -1;

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

    public int GetEquipped()
    {
      return equipped;
    }

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

    public void UnEquip()
    {
      equipped = -1;
    }

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
