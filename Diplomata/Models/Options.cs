using System;
using Diplomata.Helpers;
using Diplomata.Persistence;
using Diplomata.Persistence.Models;

namespace Diplomata.Models
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
    /// Return a array of persistent objects from a data object.
    /// </summary>
    /// <param name="array">The array of data objects.</param>
    /// <returns>A array of persistent objects.</returns>
    public override Persistent[] GetArrayData(Data[] array)
    {
      var optionsPersistent = new OptionsPersistent[0];
      foreach (var item in array)
      {
        optionsPersistent = ArrayHelper.Add(optionsPersistent, (OptionsPersistent) item.GetData());
      }
      return optionsPersistent;
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

    /// <summary>
    /// Set in a array of objects the data of a array of persistent data objects.
    /// </summary>
    /// <param name="data">A array of data objects.</param>
    /// <param name="persistentData">The array of persistent data objects.</param>
    public override void SetArrayData(ref Data[] data, Persistent[] persistentData)
    {
      foreach (var persistentObject in persistentData)
      {
        foreach (var dataObject in data)
        {
          dataObject.SetData(persistentObject);
        }
      }
    }
  }
}
