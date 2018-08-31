using System;
using System.Collections.Generic;
using Diplomata.Dictionaries;
using Diplomata.Editor;
using Diplomata.Editor.Controllers;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class CharacterEditor : UnityEditor.EditorWindow
  {
    private Vector2 scrollPos = new Vector2(0, 0);
    private string characterName = "";
    public Options options;
    public List<Character> characters;
    public static Character character;

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
      options = OptionsController.GetOptions();
      characters = CharactersController.GetCharacters(options);
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
        CharactersController.AddCharacter(characterName, options, characters);
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

      var description = DictionariesHelper.ContainsKey(character.description, options.currentLanguage);

      if (description == null)
      {
        character.description = ArrayHelper.Add(character.description, new LanguageDictionary(options.currentLanguage, ""));
        description = DictionariesHelper.ContainsKey(character.description, options.currentLanguage);
      }

      GUIHelper.textContent.text = description.value;
      var height = GUIHelper.textAreaStyle.CalcHeight(GUIHelper.textContent, Screen.width - (2 * GUIHelper.MARGIN));

      GUILayout.Label("Description: ");
      description.value = EditorGUILayout.TextArea(description.value, GUIHelper.textAreaStyle, GUILayout.Height(height));

      EditorGUILayout.Separator();

      EditorGUILayout.BeginHorizontal();

      var player = false;

      if (options.playerCharacterName == character.name)
      {
        player = true;
      }

      player = GUILayout.Toggle(player, " Is player");

      if (player)
      {
        options.playerCharacterName = character.name;
      }

      EditorGUILayout.EndHorizontal();

      if (character.name != options.playerCharacterName)
      {
        GUIHelper.Separator();

        GUILayout.Label("Character attributes (influenceable by): ");

        foreach (string attrName in options.attributes)
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
          if (ArrayHelper.Contains(options.attributes, character.attributes[i].key))
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
      CharactersController.Save(character, options.jsonPrettyPrint);
      OptionsController.Save(options, options.jsonPrettyPrint);
    }
  }
}
