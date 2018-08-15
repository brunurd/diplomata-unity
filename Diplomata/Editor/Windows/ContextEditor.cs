using Diplomata.Helpers;
using Diplomata.Models;
using DiplomataEditor;
using DiplomataEditor.Helpers;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Windows
{
  public class ContextEditor : EditorWindow
  {
    public static Talkable talkable;
    public static Context context;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static DiplomataEditorData diplomataEditor;

    public enum State
    {
      None,
      Edit,
      Close
    }

    private static State state;

    public static void Init(State state = State.None)
    {
      GUIHelper.focusOnStart = true;
      ContextEditor.state = state;

      ContextEditor window = (ContextEditor) GetWindow(typeof(ContextEditor), false, "Context Editor", true);
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 170);

      if (state == State.Close || talkable == null)
      {
        window.Close();
      }

      else
      {
        window.Show();
      }
    }

    public void OnEnable()
    {
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public static void Edit(Talkable currentTalkable, Context currentContext)
    {
      talkable = currentTalkable;
      context = currentContext;

      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingContextEditId = context.id;
      Init(State.Edit);
    }

    public static void Reset(string talkableName)
    {
      if (talkable != null)
      {
        if (talkable.name == talkableName)
        {
          talkable = null;
          context = null;

          diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
          diplomataEditor.workingContextEditId = -1;

          Init(State.Close);
        }
      }
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      switch (state)
      {
        case State.None:
          if (diplomataEditor.workingCharacter != string.Empty)
          {
            talkable = (Character) Character.Find(diplomataEditor.characters, diplomataEditor.workingCharacter);
            if (talkable == null) talkable = (Interactable) Interactable.Find(diplomataEditor.interactables, diplomataEditor.workingInteractable);

            if (diplomataEditor.workingContextEditId > -1)
            {
              context = Context.Find(talkable, diplomataEditor.workingContextEditId);
              DrawEditWindow();
            }
          }
          break;

        case State.Edit:
          DrawEditWindow();
          break;
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void DrawEditWindow()
    {
      var name = DictionariesHelper.ContainsKey(context.name, diplomataEditor.options.currentLanguage);
      var description = DictionariesHelper.ContainsKey(context.description, diplomataEditor.options.currentLanguage);

      if (name != null && description != null)
      {
        GUILayout.Label("Name: ");

        GUI.SetNextControlName("name");
        name.value = EditorGUILayout.TextField(name.value);

        GUIHelper.Focus("name");

        EditorGUILayout.Separator();

        GUIHelper.textContent.text = description.value;
        var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));

        GUILayout.Label("Description: ");
        description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

        EditorGUILayout.Separator();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Update", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          UpdateContext();
        }

        if (GUILayout.Button("Cancel", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          UpdateContext();
        }
        GUILayout.EndHorizontal();
      }
    }

    public void UpdateContext()
    {
      string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";
      diplomataEditor.Save(talkable, folderName);
      Close();
    }

    public void OnDisable()
    {
      if (talkable != null)
      {
        string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";
        diplomataEditor.Save(talkable, folderName);
      }
    }
  }

}
