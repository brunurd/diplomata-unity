using System.Collections.Generic;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class QuestListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private Options options;
    private Quest[] quests;

    [MenuItem("Diplomata/Quests", false, 0)]
    static public void Init()
    {
      QuestListMenu window = (QuestListMenu) GetWindow(typeof(QuestListMenu), false, "Quests");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 110);
      window.Show();
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      quests = QuestsController.GetQuests(options.jsonPrettyPrint);
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      // If empty show this message.
      if (quests.Length <= 0)
      {
        EditorGUILayout.HelpBox("No quests yet.", MessageType.Info);
      }

      // Quests loop to list.
      foreach (Quest quest in quests)
      {
        GUILayout.BeginHorizontal();

        // Set label properties.
        GUILayout.BeginHorizontal();
        if (EditorGUIUtility.isProSkin) GUIHelper.labelStyle.normal.textColor = Color.white;
        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(quest.Name, GUIHelper.labelStyle);
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
            quests = ArrayHelper.Remove(quests, quest);
            QuestsController.Save(quests, options.jsonPrettyPrint);
          }
        }
        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();
      }

      // Add button.
      if (GUILayout.Button("Add Quest", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        var quest = new Quest();
        quests = ArrayHelper.Add(quests, quest);
        QuestsController.Save(quests, options.jsonPrettyPrint);
        QuestEditor.Open(quest);
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void OnDisable()
    {
      QuestsController.Save(quests, options.jsonPrettyPrint);
    }

    public void OnInspectorUpdate()
    {
      Repaint();
    }
  }
}
