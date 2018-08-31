using System.Collections.Generic;
using Diplomata.Editor;
using Diplomata.Editor.Controllers;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class QuestEditor : UnityEditor.EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    public static Quest quest;
    private Quest[] quests;
    private Options options;

    public enum State
    {
      None,
      CreateEdit,
      Close
    }

    private static State state;

    public static void Init(State state = State.None)
    {
      QuestEditor.state = state;
      GUIHelper.focusOnStart = true;

      QuestEditor window = (QuestEditor) GetWindow(typeof(QuestEditor), false, "Quest", true);

      if (state == State.Close)
      {
        window.Close();
      }

      else
      {
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 160);
        window.Show();
      }
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      quests = QuestsController.GetQuests(options.jsonPrettyPrint);
    }

    public void OnDisable()
    {
      if (state == State.CreateEdit && quest != null)
      {
        Save();
      }
    }

    public static void Open(Quest currentQuest)
    {
      quest = currentQuest;
      Init(State.CreateEdit);
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      switch (state)
      {
        case State.None:
          Init(State.Close);
          break;

        case State.CreateEdit:
          // Set quest name.
          GUILayout.Label("Name: ");
          GUI.SetNextControlName("name");
          quest.Name = EditorGUILayout.TextField(quest.Name);
          GUIHelper.Focus("name");

          // Set label properties for quest states header.
          GUILayout.BeginHorizontal();
          if (EditorGUIUtility.isProSkin) GUIHelper.labelStyle.normal.textColor = Color.white;
          GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
          GUILayout.Label("Quest states:", GUIHelper.labelStyle);
          GUILayout.EndHorizontal();

          // Loop of the quest states.
          foreach (var questState in quest.GetQuestStates())
          {
            GUILayout.BeginHorizontal();
            questState.Name = EditorGUILayout.TextField(questState.Name);
            GUILayout.EndHorizontal();
          }

          // Buttons.
          GUILayout.BeginHorizontal();
          if (GUILayout.Button("Add quest state", GUILayout.Width(Screen.width / 2), GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
          {
            quest.AddState("New state.");
            Save();
          }
          GUILayout.EndHorizontal();
          EditorGUILayout.Separator();
          GUILayout.BeginHorizontal();
          if (GUILayout.Button("Save and close", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
          {
            Save();
            Close();
          }
          GUILayout.EndHorizontal();

          // Save and close on press Enter.
          if (focusedWindow != null)
          {
            if (focusedWindow.ToString() == "(Diplomata.Editor.Windows.QuestEditor)")
            {
              if (Event.current.keyCode == KeyCode.Return)
              {
                Save();
                Close();
              }
            }
          }
          break;
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void Save()
    {
      for (var i = 0; i < quests.Length; i++)
      {
        if (quests[i].GetId() == quest.GetId())
          quests[i] = quest;
      }
      QuestsController.Save(quests, options.jsonPrettyPrint);
    }
  }
}
