using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class QuestEditor : EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    public static Quest quest;

    public enum State
    {
      None = 0,
      CreateEdit = 1,
      Close = 2
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
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 240, 340);
        window.Show();
      }
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
          
          var questName = DictionariesHelper.ContainsKey(quest.Name, Controller.Instance.Options.currentLanguage);
          if (questName == null)
            quest.Name = ArrayHelper.Add(quest.Name, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));

          GUI.SetNextControlName("name");
          DictionariesHelper.ContainsKey(quest.Name, Controller.Instance.Options.currentLanguage).value =
            EditorGUILayout.TextField(DictionariesHelper.ContainsKey(quest.Name, Controller.Instance.Options.currentLanguage).value);
          GUIHelper.Focus("name");
          EditorGUILayout.Space();
          
          EditorGUILayout.SelectableLabel(string.Format("Quest Unique ID: {0}", quest.GetId()));

          // Set label properties for quest states header.
          GUILayout.BeginHorizontal();
          if (EditorGUIUtility.isProSkin) GUIHelper.labelStyle.normal.textColor = Color.white;
          GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
          GUILayout.Label("Quest states:", GUIHelper.labelStyle);
          GUILayout.EndHorizontal();
          GUIHelper.Separator();

          // Loop of the quest states.
          foreach (var questState in quest.questStates)
          {
            GUILayout.BeginHorizontal();
            var index = ArrayHelper.GetIndex(quest.questStates, questState);
            GUILayout.Label(string.Format("{0}.", (index + 1).ToString()), GUILayout.Width(25));

            // Descriptions.
            EditorGUILayout.BeginVertical();

            

            EditorGUILayout.LabelField("Short description:");
            
            var questStateShortDescription = DictionariesHelper.ContainsKey(questState.ShortDescription, Controller.Instance.Options.currentLanguage);
            if (questStateShortDescription == null)
              questState.ShortDescription = ArrayHelper.Add(questState.ShortDescription, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));
            
            DictionariesHelper.ContainsKey(questState.ShortDescription, Controller.Instance.Options.currentLanguage).value =
              EditorGUILayout.TextArea(DictionariesHelper
                .ContainsKey(questState.ShortDescription, Controller.Instance.Options.currentLanguage).value);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Long description:");
            
            var questStateLongDescription = DictionariesHelper.ContainsKey(questState.LongDescription, Controller.Instance.Options.currentLanguage);
            if (questStateLongDescription == null)
              questState.LongDescription = ArrayHelper.Add(questState.LongDescription, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));
            
            DictionariesHelper.ContainsKey(questState.LongDescription, Controller.Instance.Options.currentLanguage).value =
              EditorGUILayout.TextArea(DictionariesHelper
                .ContainsKey(questState.LongDescription, Controller.Instance.Options.currentLanguage).value);
            EditorGUILayout.Space();
            
            EditorGUILayout.SelectableLabel(string.Format("Quest State Unique ID: {0}", questState.GetId()));

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
            {
              if (index > 0)
              {
                quest.questStates = ArrayHelper.Swap(quest.questStates, index, index - 1);
                QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
              }
            }

            if (GUILayout.Button("Down", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
            {
              if (index < quest.questStates.Length - 1)
              {
                quest.questStates = ArrayHelper.Swap(quest.questStates, index, index + 1);
                QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
              }
            }

            if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
            {
              if (EditorUtility.DisplayDialog("Are you sure?",
                "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
              {
                quest.questStates = ArrayHelper.Remove(quest.questStates, questState);
                QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
              }
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            GUIHelper.Separator();
          }

          // Buttons.
          EditorGUILayout.Space();
          GUILayout.BeginHorizontal();
          if (GUILayout.Button("Add quest state", GUILayout.Width(Screen.width / 2),
            GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
          {
            quest.AddState("Short description.", "Long description.");
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
      for (var i = 0; i < Controller.Instance.Quests.Length; i++)
      {
        if (Controller.Instance.Quests[i].GetId() == quest.GetId())
          Controller.Instance.Quests[i] = quest;
      }

      QuestsController.Save(Controller.Instance.Quests, Controller.Instance.Options.jsonPrettyPrint);
    }
  }
}
