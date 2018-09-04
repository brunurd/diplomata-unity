using System;

namespace LavaLeak.Diplomata.Dictionaries
{
  [Serializable]
  public class LanguageDictionary
  {
    public string key;
    public string value;

    public LanguageDictionary() {}
    public LanguageDictionary(string key, string value)
    {
      this.key = key;
      this.value = value;
    }
  }
}
