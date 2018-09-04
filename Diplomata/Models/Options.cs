using System;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models.Submodels;
using LavaLeak.Diplomata.Persistence;
using LavaLeak.Diplomata.Persistence.Models;

namespace LavaLeak.Diplomata.Models
{
  [Serializable]
  public class Options : Data
  {
    public Language[] languages = new Language[] { new Language("English") };
    public string[] languagesList = new string[] { "English" };
    public string[] characterList = new string[0];
    public string[] interactableList = new string[0];
    public string[] attributes = new string[] { "fear", "politeness", "argumentation", "insistence", "charm", "confidence" };
    public string playerCharacterName;
    public bool jsonPrettyPrint;
    public string currentLanguage = "English";

    [NonSerialized]
    public float volumeScale = 1.0f;

    public void SetCurrentLanguage(string language)
    {
      currentLanguage = language;
      SetLanguageList();
    }

    public void SetLanguageList()
    {
      languagesList = new string[languages.Length];

      for (int i = 0; i < languages.Length; i++)
      {
        languagesList[i] = languages[i].name;
      }
    }

    /// <summary>
    /// Return the data of the object to save in a persistent object.
    /// </summary>
    /// <returns>A persistent object.</returns>
    public override Persistent GetData()
    {
      var options = new OptionsPersistent();
      options.currentLanguage = currentLanguage;
      options.volumeScale = volumeScale;
      return options;
    }

    /// <summary>
    /// Store in a object data from persistent object.
    /// </summary>
    /// <param name="persistentData">The persistent data object.</param>
    public override void SetData(Persistent persistentData)
    {
      var optionsPersistentData = (OptionsPersistent) persistentData;
      currentLanguage = optionsPersistentData.currentLanguage;
      volumeScale = optionsPersistentData.volumeScale;
    }
  }
}
