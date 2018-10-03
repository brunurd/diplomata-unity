using System;
using LavaLeak.Diplomata.Dictionaries;

namespace LavaLeak.Diplomata.Models.Submodels
{
  [Serializable]
  public class AttachedContent
  {
    public LanguageDictionary[] content = new LanguageDictionary[0];
  }
}