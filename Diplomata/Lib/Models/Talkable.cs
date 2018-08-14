using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEngine;

namespace Diplomata.Models
{
  [Serializable]
  public class Talkable
  {
    public string name;
    public LanguageDictionary[] description;
    public Context[] contexts;

    [NonSerialized]
    public bool onScene;

    public Talkable() {}

    public Talkable(string name)
    {
      this.name = name;
      contexts = new Context[0];
      description = new LanguageDictionary[0];

      foreach (Language lang in DiplomataData.options.languages)
      {
        description = ArrayHelper.Add(description, new LanguageDictionary(lang.name, ""));
      }
    }
  }
}
