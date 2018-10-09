using System;
using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Helpers;

namespace LavaLeak.Diplomata.Models.Submodels
{
  public enum VariableType
  {
    String = 0,
    Int = 1,
    Float = 2
  }

  [Serializable]
  public class LocalVariable
  {
    public string Name;
    public VariableType Type;
    public LanguageDictionary[] StringValue;
    public int IntValue;
    public float FloatValue;
    
    public LocalVariable(string name, VariableType type, object value)
    {
      Name = name;
      Type = type;
      Set(type, value, DiplomataManager.Data.options.currentLanguage);
    }

    public LocalVariable(string name, VariableType type, object value, string language)
    {
      Name = name;
      Type = type;
      Set(type, value, language);
    }

    public void Set(VariableType type, object value)
    {
      Set(type, value, DiplomataManager.Data.options.currentLanguage);
    }

    public void Set(VariableType type, object value, string language)
    {
      switch (type)
      {
        case VariableType.String:
          if (StringValue == null)
            StringValue = new LanguageDictionary[0];
          StringValue = ArrayHelper.Add(StringValue, new LanguageDictionary(language, (string) value));
          return;
        case VariableType.Int:
          IntValue = (int) value;
          return;
        case VariableType.Float:
          FloatValue = (float) value;
          return;
        default:
          throw new Exception("Undefined type.");
      }
    }
  }
}