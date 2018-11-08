using System;

namespace LavaLeak.Diplomata.Dictionaries
{
  [Serializable]
  public class AttributeDictionary
  {
    public string key;
    public byte value;

    public AttributeDictionary() {}
    public AttributeDictionary(string key)
    {
      this.key = key;
    }

    public override string ToString()
    {
      return value.ToString();
    }
  }
}
