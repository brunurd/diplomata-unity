using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models.Submodels;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class PreferencesEditor : EditorWindow
  {
    public static string[] attributesTemp = new string[0];
    public static Language[] languagesTemp = new Language[0];
    public static bool jsonPrettyPrintTemp = false;
    public static string currentLanguageTemp;
    private Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Tools/Diplomata/Preferences/Options", false, 1)]
    static public void Init()
    {
      var window = (PreferencesEditor) GetWindow(typeof(PreferencesEditor), false, "Preferences");
      window.minSize = new Vector2(600, 325);
      window.Show();
    }

    private void OnEnable()
    {
      attributesTemp = ArrayHelper.Copy(Controller.Instance.Options.attributes);
      languagesTemp = ArrayHelper.Copy(Controller.Instance.Options.languages);
      jsonPrettyPrintTemp = Controller.Instance.Options.jsonPrettyPrint;
      currentLanguageTemp = string.Copy(Controller.Instance.Options.currentLanguage);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      GUILayout.BeginHorizontal();
      DrawAttributes();
      DrawLanguages();
      GUILayout.EndHorizontal();

      GUIHelper.Separator();

      GUILayout.BeginHorizontal();
      jsonPrettyPrintTemp = GUILayout.Toggle(jsonPrettyPrintTemp, "JSON pretty print");
      currentLanguageTemp = GUIHelper.Popup("Current Language", currentLanguageTemp, Language.GetNames(languagesTemp));
      GUILayout.EndHorizontal();

      EditorGUILayout.Separator();
      EditorGUILayout.HelpBox("\nClose or enter in play mode will restore the data of this window.\n", MessageType.Info);

      EditorGUILayout.Separator();

      if (GUILayout.Button("Save Preferences", GUILayout.Height(30)))
      {
        Save();
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void DrawAttributes()
    {
      GUILayout.BeginVertical(GUILayout.Width(position.width / 2));
      GUILayout.Label("Attributes:");

      for (int i = 0; i < attributesTemp.Length; i++)
      {
        GUILayout.BeginHorizontal();
        attributesTemp[i] = EditorGUILayout.TextField(attributesTemp[i]);

        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
          attributesTemp = ArrayHelper.Remove(attributesTemp, attributesTemp[i]);
        }

        GUILayout.EndHorizontal();
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Add attribute"))
      {
        attributesTemp = ArrayHelper.Add(attributesTemp, "");
      }

      GUILayout.EndVertical();
    }

    public void DrawLanguages()
    {
      GUILayout.BeginVertical();

      GUILayout.Label("Languages:");

      for (int i = 0; i < languagesTemp.Length; i++)
      {
        GUILayout.BeginHorizontal();

        languagesTemp[i].name = EditorGUILayout.TextField(languagesTemp[i].name);

        // TODO: Implements dub and sub in the Options.
        // languagesTemp[i].subtitle = GUILayout.Toggle(languagesTemp[i].subtitle, "Sub");
        // languagesTemp[i].dubbing = GUILayout.Toggle(languagesTemp[i].dubbing, "Dub");

        if (GUILayout.Button("X", GUILayout.Width(20)))
        {
          languagesTemp = ArrayHelper.Remove(languagesTemp, languagesTemp[i]);
        }

        GUILayout.EndHorizontal();
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Add language"))
      {
        languagesTemp = ArrayHelper.Add(languagesTemp, new Language(""));
      }

      GUILayout.EndVertical();
    }

    public void Save()
    {
      Controller.Instance.Options.attributes = ArrayHelper.Copy(attributesTemp);
      Controller.Instance.Options.languages = ArrayHelper.Copy(languagesTemp);
      Controller.Instance.Options.jsonPrettyPrint = jsonPrettyPrintTemp;
      Controller.Instance.Options.currentLanguage = string.Copy(currentLanguageTemp);
      Controller.Instance.Options.SetLanguageList();
      CharactersController.UpdateList(Controller.Instance.Options);
      OptionsController.Save(Controller.Instance.Options, Controller.Instance.Options.jsonPrettyPrint);
      Close();
    }
  }
}
