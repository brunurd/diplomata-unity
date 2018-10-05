using System;
using System.Collections.Generic;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models.Submodels;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;
using UnityEngine;

namespace LavaLeak.Diplomata.Models
{
  /// <summary>
  /// A base class to talkable objects.
  /// </summary>
  [Serializable]
  public class Talkable : Data
  {
    [SerializeField]
    private string uniqueId = Guid.NewGuid().ToString();

    public string name;
    public LanguageDictionary[] description;
    public Context[] contexts;

    [NonSerialized]
    public bool onScene;

    /// <summary>
    /// Basic constructor.
    /// </summary>
    /// <returns>The new Talkable.</returns>
    public Talkable() {}

    /// <summary>
    /// A talkable base constructor with name.
    /// </summary>
    /// <param name="name">The talkable name.</param>
    public Talkable(string name)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.name = name;
      contexts = new Context[0];
      description = new LanguageDictionary[0];

      foreach (Language lang in DiplomataManager.Data.options.languages)
      {
        description = ArrayHelper.Add(description, new LanguageDictionary(lang.name, ""));
      }
    }

    /// <summary>
    /// Set the uniqueId if it is empty or null.
    /// </summary>
    /// <returns>Return true if it change or false if don't.</returns>
    public bool SetId()
    {
      if (!string.IsNullOrEmpty(uniqueId)) return false;
      uniqueId = Guid.NewGuid().ToString();
      return true;
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      if (this.GetType() == typeof(Character))
      {
        var talkable = (Character) this;
        var talkablePersistent = new CharacterPersistent();
        talkablePersistent.id = uniqueId;
        talkablePersistent.influence = talkable.influence;
        talkablePersistent.contexts = Data.GetArrayData<ContextPersistent>(contexts);
        return talkablePersistent;
      }
      else if (this.GetType() == typeof(Interactable))
      {
        var talkable = (Interactable) this;
        var talkablePersistent = new InteractablePersistent();
        talkablePersistent.id = talkable.uniqueId;
        talkablePersistent.contexts = Data.GetArrayData<ContextPersistent>(talkable.contexts);
        return talkablePersistent;
      }
      else
      {
        var talkablePersistent = new TalkablePersistent();
        talkablePersistent.id = uniqueId;
        talkablePersistent.contexts = Data.GetArrayData<ContextPersistent>(contexts);
        return talkablePersistent;
      }
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      if (this.GetType() == typeof(Character))
      {
        var talkablePersistent = (CharacterPersistent) persistentData;
        uniqueId = talkablePersistent.id;
        ((Character) this).influence = talkablePersistent.influence;
        contexts = Data.SetArrayData<Context>(contexts, talkablePersistent.contexts);
      }
      else if (this.GetType() == typeof(Interactable))
      {
        var talkablePersistent = (InteractablePersistent) persistentData;
        uniqueId = talkablePersistent.id;
        contexts = Data.SetArrayData<Context>(contexts, talkablePersistent.contexts);
      }
      else
      {
        var talkablePersistent = (TalkablePersistent) persistentData;
        uniqueId = talkablePersistent.id;
        contexts = Data.SetArrayData<Context>(contexts, talkablePersistent.contexts);
      }
    }
  }
}
