using System;

namespace LavaLeak.Diplomata.Dictionaries
{
  [Serializable]
  public class LanguageDictionary
  {
    public string key = string.Empty;
    public string value = string.Empty;

    public LanguageDictionary() {}
    public LanguageDictionary(string key, string value)
    {
      this.key = key;
      this.value = value;
    }

    public override string ToString()
    {
      return value;
    }
  }
}
