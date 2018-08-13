using DiplomataEditor.Core;
using DiplomataEditor.Helpers;
using DiplomataLib;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Editors
{
  public class ContextEditor : EditorWindow
  {
    public static DiplomataLib.Character character;
    public static Context context;
    private Vector2 scrollPos = new Vector2(0, 0);
    private static Core.Diplomata diplomataEditor;

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

      if (state == State.Close || character == null)
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
      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
    }

    public static void Edit(Character currentCharacter, Context currentContext)
    {
      character = currentCharacter;
      context = currentContext;

      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingContextEditId = context.id;
      Init(State.Edit);
    }

    public static void Reset(string characterName)
    {
      if (character != null)
      {
        if (character.name == characterName)
        {
          character = null;
          context = null;

          diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
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
            character = Character.Find(diplomataEditor.characters, diplomataEditor.workingCharacter);

            if (diplomataEditor.workingContextEditId > -1)
            {
              context = Context.Find(character, diplomataEditor.workingContextEditId);
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
      var name = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);
      var description = DictHandler.ContainsKey(context.description, diplomataEditor.preferences.currentLanguage);

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
      diplomataEditor.Save(character);
      Close();
    }

    public void OnDisable()
    {
      if (character != null)
      {
        diplomataEditor.Save(character);
      }
    }
  }

}
