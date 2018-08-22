using System.Collections.Generic;
using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Windows
{
  public class QuestEditor : EditorWindow
  {
    public static Quest quest;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static DiplomataEditorData diplomataEditor;

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
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public static void Open(Quest currentQuest)
    {
      quest = currentQuest;

      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingQuest = currentQuest.GetId();
      Init(State.CreateEdit);
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      // Set the last worked quest.
      if (state == State.None && diplomataEditor.workingQuest != string.Empty)
      {
        quest = Quest.Find(diplomataEditor.quests, diplomataEditor.workingQuest);
      }

      if (quest != null)
      {
        // TODO: Close window.
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
        foreach (QuestState state in quest.GetQuestStates())
        {
          GUILayout.BeginHorizontal();
          state.Name = EditorGUILayout.TextField(state.Name);
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
          if (focusedWindow.ToString() == "(DiplomataEditor.Windows.QuestEditor)")
          {
            if (Event.current.keyCode == KeyCode.Return)
            {
              Save();
              Close();
            }
          }
        }
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void Save()
    {
      diplomataEditor.SaveQuests();
    }

    public void OnDisable()
    {
      if (state == State.CreateEdit && quest != null)
      {
        Save();
      }
    }
  }
}
