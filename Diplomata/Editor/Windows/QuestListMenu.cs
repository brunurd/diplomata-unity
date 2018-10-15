using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using LavaLeak.Diplomata.Models.Submodels;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class QuestListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Tools/Diplomata/Edit/Quests", false, 0)]
    static public void Init()
    {
      QuestListMenu window = (QuestListMenu) GetWindow(typeof(QuestListMenu), false, "Quests");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 110);
      window.Show();
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      // If empty show this message.
      if (Controller.Instance.Quests.Length <= 0)
      {
        EditorGUILayout.HelpBox("No quests yet.", MessageType.Info);
      }

      // Quests loop to list.
      foreach (Quest quest in Controller.Instance.Quests)
      {
        GUILayout.BeginHorizontal();

        // Set label properties.
        GUILayout.BeginHorizontal();
        if (EditorGUIUtility.isProSkin) GUIHelper.labelStyle.normal.textColor = Color.white;
        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        var questName = DictionariesHelper.ContainsKey(quest.Name, Controller.Instance.Options.currentLanguage);
        if (questName != null)
          GUILayout.Label(questName.value, GUIHelper.labelStyle);
        GUILayout.EndHorizontal();

        GUILayout.Space(10.0f);

        // Setting buttons.
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));
        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          QuestEditor.Open(quest);
        }
        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            QuestEditor.Init(QuestEditor.State.Close);
            Controller.Instance.Quests = ArrayHelper.Remove(Controller.Instance.Quests, quest);
            QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
          }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
      }

      // Add button.
      if (GUILayout.Button("Add Quest", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        var quest = new Quest();
        Controller.Instance.Quests = ArrayHelper.Add(Controller.Instance.Quests, quest);
        quest.questStates = ArrayHelper.Add(quest.questStates, new QuestState());

        foreach (var language in Controller.Instance.Options.languagesList)
        {
          quest.questStates[0].ShortDescription =
            ArrayHelper.Add(quest.questStates[0].ShortDescription, new LanguageDictionary(language, "in progress."));
          quest.questStates[0].LongDescription =
            ArrayHelper.Add(quest.questStates[0].LongDescription, new LanguageDictionary(language, ""));
        }

        QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
        QuestEditor.Open(quest);
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void OnDisable()
    {
      QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
    }

    public void OnInspectorUpdate()
    {
      Repaint();
    }
  }
}
