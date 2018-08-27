using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using Diplomata.Persistence;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Talkable : Data
  {
    [SerializeField] private string uniqueId = Guid.NewGuid().ToString();
    public string name;
    public LanguageDictionary[] description;
    public Context[] contexts;

    [NonSerialized]
    public bool onScene;

    public Talkable() {}

    public Talkable(string name)
    {
      uniqueId = Guid.NewGuid().ToString();
      this.name = name;
      contexts = new Context[0];
      description = new LanguageDictionary[0];

      foreach (Language lang in DiplomataData.options.languages)
      {
        description = ArrayHelper.Add(description, new LanguageDictionary(lang.name, ""));
      }
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Return a array of persistent objects from a data object.
    /// </summary>
    /// <param name="array">The array of data objects.</param>
    /// <returns>A array of persistent objects.</returns>
    public override Persistent[] GetArrayData(Data[] array)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Set in a array of objects the data of a array of persistent data objects.
    /// </summary>
    /// <param name="data">A array of data objects.</param>
    /// <param name="persistentData">The array of persistent data objects.</param>
    public override void SetArrayData(ref Data[] data, Persistent[] persistentData)
    {
      throw new NotImplementedException();
    }
  }
}
