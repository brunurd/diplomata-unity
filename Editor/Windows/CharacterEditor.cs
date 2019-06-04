using LavaLeak.Diplomata.Dictionaries;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class CharacterEditor : EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    private string characterName = "";
    public static Character character;

    public enum State
    {
      None = 0,
      Create = 1,
      Edit = 2,
      Close = 3
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

    public void OnDisable()
    {
      if (state == State.Edit && character != null)
      {
        Save();
      }
    }

    public static void OpenCreate()
    {
      character = null;
      Init(State.Create);
    }

    public static void Edit(Character currentCharacter)
    {
      character = currentCharacter;
      Init(State.Edit);
    }

    public static void Reset(string characterName)
    {
      if (character != null)
      {
        if (character.name == characterName)
        {
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
          Init(State.Close);
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
        if (focusedWindow.ToString() == " (Diplomata.Editor.Windows.CharacterEditor)")
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
        CharactersController.AddCharacter(characterName, Controller.Instance.Options, Controller.Instance.Characters);
      }
      else
      {
        Debug.LogError("Character name was empty.");
      }
      Close();
    }

    public void DrawEditWindow()
    {
      GUILayout.Label(string.Format("Name: {0}", character.name));

      GUIHelper.Separator();

      var description = DictionariesHelper.ContainsKey(character.description, Controller.Instance.Options.currentLanguage);

      if (description == null)
      {
        character.description = ArrayHelper.Add(character.description, new LanguageDictionary(Controller.Instance.Options.currentLanguage, ""));
        description = DictionariesHelper.ContainsKey(character.description, Controller.Instance.Options.currentLanguage);
      }

      GUIHelper.textContent.text = description.value;
      var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, position.width - (2 * GUIHelper.MARGIN));

      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

      EditorGUILayout.Separator();

      EditorGUILayout.BeginHorizontal();

      var player = false;

      if (Controller.Instance.Options.playerCharacterName == character.name)
      {
        player = true;
      }

      player = GUILayout.Toggle(player, " Is player");

      if (player)
      {
        Controller.Instance.Options.playerCharacterName = character.name;
      }

      EditorGUILayout.EndHorizontal();

      if (character.name != Controller.Instance.Options.playerCharacterName)
      {
        GUIHelper.Separator();

        GUILayout.Label("Character attributes (influenceable by): ");

        foreach (string attrName in Controller.Instance.Options.attributes)
        {
          if (character.attributes.Length == 0)
          {
            character.attributes = ArrayHelper.Add(character.attributes, new AttributeDictionary(attrName));
          }
          else
          {
            for (int i = 0; i < character.attributes.Length; i++)
            {
              if (character.attributes[i].key == attrName)
              {
                break;
              }
              else if (i == character.attributes.Length - 1)
              {
                character.attributes = ArrayHelper.Add(character.attributes, new AttributeDictionary(attrName));
              }
            }
          }
        }

        for (int i = 0; i < character.attributes.Length; i++)
        {
          if (ArrayHelper.Contains(Controller.Instance.Options.attributes, character.attributes[i].key))
          {
            character.attributes[i].value = (byte) EditorGUILayout.Slider(character.attributes[i].key, character.attributes[i].value, 0, 100);
          }
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
      CharactersController.Save(character, Controller.Instance.Options.jsonPrettyPrint);
      OptionsController.Save(Controller.Instance.Options, Controller.Instance.Options.jsonPrettyPrint);
    }
  }
}
