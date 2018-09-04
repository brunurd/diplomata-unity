using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class ContextEditor : UnityEditor.EditorWindow
  {
    public static Talkable talkable;
    public static Context context;
    private Vector2 scrollPos = new Vector2(0, 0);
    private Options options;

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
      options = OptionsController.GetOptions();
    }

    public static void Edit(Talkable currentTalkable, Context currentContext)
    {
      talkable = currentTalkable;
      context = currentContext;
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
          Init(State.Close);
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
      var name = DictionariesHelper.ContainsKey(context.name, options.currentLanguage);
      var description = DictionariesHelper.ContainsKey(context.description, options.currentLanguage);

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
      Save();
      Close();
    }

    public void OnDisable()
    {
      if (talkable != null)
      {
        Save();
      }
    }

    private void Save()
    {
      if (talkable.GetType() == typeof(Character))
      {
        CharactersController.Save((Character) talkable, options.jsonPrettyPrint);
      }
      else if (talkable.GetType() == typeof(Interactable))
      {
        InteractablesController.Save((Interactable) talkable, options.jsonPrettyPrint);
      }
    }
  }
}
