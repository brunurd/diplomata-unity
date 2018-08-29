using Diplomata.Editor;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class QuestListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    private DiplomataEditorData diplomataEditor;

    [MenuItem("Diplomata/Quests")]
    static public void Init()
    {
      DiplomataEditorData.Instantiate();
      QuestListMenu window = (QuestListMenu) GetWindow(typeof(QuestListMenu), false, "Quests");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 150, 110);
      window.Show();
    }

    public void OnEnable()
    {
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      // If empty show this message.
      if (diplomataEditor.quests.Length <= 0)
      {
        EditorGUILayout.HelpBox("No quests yet.", MessageType.Info);
      }

      // Quests loop to list.
      foreach (Quest quest in diplomataEditor.quests)
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
            diplomataEditor.quests = ArrayHelper.Remove(diplomataEditor.quests, quest);
            diplomataEditor.SaveQuests();
          }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndHorizontal();
      }

      // Add button.
      if (GUILayout.Button("Add Quest", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        var quest = new Quest();
        diplomataEditor.quests = ArrayHelper.Add(diplomataEditor.quests, quest);
        diplomataEditor.SaveQuests();
        QuestEditor.Open(quest);
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void OnDisable()
    {
      diplomataEditor.SaveQuests();
    }

    public void OnInspectorUpdate()
    {
      Repaint();
    }
  }
}
