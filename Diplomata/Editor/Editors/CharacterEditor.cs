using DiplomataEditor.Core;
using DiplomataEditor.Helpers;
using DiplomataLib;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Editors
{
  public class CharacterEditor : EditorWindow
  {
    public static Character character;
    private string characterName = "";
    private Vector2 scrollPos = new Vector2(0, 0);
    private static Core.Diplomata diplomataEditor;

    public enum State
    {
      None,
      Create,
      Edit,
      Close
    }

    private static State state;

    public static void Init(State state = State.None)
    {
      CharacterEditor.state = state;
      GUIHelper.focusOnStart = true;

      CharacterEditor window = (CharacterEditor) GetWindow(typeof(CharacterEditor), false, "Character", true);

      if (state == State.Create)
      {
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 100);
      }

      else
      {
        window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH, 390);
      }

      if (state == State.Close)
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

    public static void OpenCreate()
    {
      character = null;

      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingCharacter = string.Empty;
      Init(State.Create);
    }

    public static void Edit(Character currentCharacter)
    {
      character = currentCharacter;

      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingCharacter = currentCharacter.name;
      Init(State.Edit);
    }

    public static void Reset(string characterName)
    {
      if (character != null)
      {
        if (character.name == characterName)
        {
          diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
          diplomataEditor.workingCharacter = string.Empty;
          character = null;
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
            DrawEditWindow();
          }
          else
          {
            DrawCreateWindow();
          }
          break;

        case State.Create:
          DrawCreateWindow();
          break;

        case State.Edit:
          DrawEditWindow();
          break;
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void DrawCreateWindow()
    {
      GUILayout.Label("Name: ");

      GUI.SetNextControlName("name");
      characterName = EditorGUILayout.TextField(characterName);

      GUIHelper.Focus("name");

      EditorGUILayout.Separator();

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Create();
      }

      if (GUILayout.Button("Cancel", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Close();
      }

      GUILayout.EndHorizontal();

      if (focusedWindow != null)
      {
        if (focusedWindow.ToString() == " (DiplomataEditor.CharacterEditor)")
        {
          if (Event.current.keyCode == KeyCode.Return)
          {
            Create();
          }
        }
      }
    }

    public void Create()
    {
      if (characterName != "")
      {
        diplomataEditor.AddCharacter(characterName);
      }

      else
      {
        Debug.LogError("Character name was empty.");
      }

      Close();
    }

    public void DrawEditWindow()
    {
      GUILayout.Label("Name: ");
      character.name = EditorGUILayout.TextField(character.name);

      GUIHelper.Separator();

      var description = DictHandler.ContainsKey(character.description, diplomataEditor.preferences.currentLanguage);

      if (description == null)
      {
        character.description = ArrayHandler.Add(character.description, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
        description = DictHandler.ContainsKey(character.description, diplomataEditor.preferences.currentLanguage);
      }

      GUIHelper.textContent.text = description.value;
      var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));

      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

      EditorGUILayout.Separator();

      EditorGUILayout.BeginHorizontal();

      var player = false;

      if (diplomataEditor.preferences.playerCharacterName == character.name)
      {
        player = true;
      }

      player = GUILayout.Toggle(player, " Is player");

      if (player)
      {
        diplomataEditor.preferences.playerCharacterName = character.name;
      }

      EditorGUILayout.EndHorizontal();

      if (character.name != diplomataEditor.preferences.playerCharacterName)
      {
        GUIHelper.Separator();

        GUILayout.Label("Character attributes (influenceable by): ");

        foreach (string attrName in diplomataEditor.preferences.attributes)
        {
          for (int i = 0; i < character.attributes.Length; i++)
          {
            if (character.attributes[i].key == attrName)
            {
              break;
            }
            else if (i == character.attributes.Length - 1)
            {
              character.attributes = ArrayHandler.Add(character.attributes, new DictAttr(attrName));
            }
          }
        }

        for (int i = 0; i < character.attributes.Length; i++)
        {
          character.attributes[i].value = (byte) EditorGUILayout.Slider(character.attributes[i].key, character.attributes[i].value, 0, 100);
        }

        GUIHelper.Separator();
      }

      else
      {
        EditorGUILayout.Separator();
      }

      GUILayout.BeginHorizontal();

      if (GUILayout.Button("Save", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Save();
        Close();
      }

      if (GUILayout.Button("Close", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        Save();
        Close();
      }

      GUILayout.EndHorizontal();
    }

    public void Save()
    {
      diplomataEditor.Save(character);
      diplomataEditor.SavePreferences();
    }

    public void OnDisable()
    {
      if (state == State.Edit && character != null)
      {
        Save();
      }
    }
  }
}
