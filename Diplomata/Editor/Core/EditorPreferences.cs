using DiplomataEditor.Helpers;
using Diplomata.Preferences;
using Diplomata.Helpers;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Core
{
  public class EditorPreferences : EditorWindow
  {
    public static string[] attributesTemp = new string[0];
    public static Language[] languagesTemp = new Language[0];
    public static bool jsonPrettyPrintTemp = false;
    public static string currentLanguageTemp;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static DiplomataEditorManager diplomataEditor;

    [MenuItem("Diplomata/Preferences")]
    static public void Init()
    {
      DiplomataEditorManager.Instantiate();
      diplomataEditor = (DiplomataEditorManager) AssetHelper.Read("Diplomata.asset", "Diplomata/");

      attributesTemp = ArrayHelper.Copy(diplomataEditor.options.attributes);
      languagesTemp = ArrayHelper.Copy(diplomataEditor.options.languages);
      jsonPrettyPrintTemp = diplomataEditor.options.jsonPrettyPrint;
      currentLanguageTemp = string.Copy(diplomataEditor.options.currentLanguage);

      EditorPreferences window = (EditorPreferences) GetWindow(typeof(EditorPreferences), false, "Preferences");
      window.minSize = new Vector2(600, 325);
      window.Show();
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
      currentLanguageTemp = GUIHelper.Popup("Current Language", currentLanguageTemp, diplomataEditor.options.languagesList);
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
      GUILayout.BeginVertical(GUILayout.Width(Screen.width / 2));
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
        languagesTemp[i].subtitle = GUILayout.Toggle(languagesTemp[i].subtitle, "Sub");
        languagesTemp[i].dubbing = GUILayout.Toggle(languagesTemp[i].dubbing, "Dub");

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
      diplomataEditor.options.attributes = ArrayHelper.Copy(attributesTemp);
      diplomataEditor.options.languages = ArrayHelper.Copy(languagesTemp);
      diplomataEditor.options.jsonPrettyPrint = jsonPrettyPrintTemp;
      diplomataEditor.options.currentLanguage = string.Copy(currentLanguageTemp);
      diplomataEditor.options.SetLanguageList();

      diplomataEditor.SavePreferences();
      Close();
    }
  }
}
