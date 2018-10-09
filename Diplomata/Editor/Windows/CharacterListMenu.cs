using System;
using System.Collections.Generic;
using LavaLeak.Diplomata;
using LavaLeak.Diplomata.Editor;
using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class CharacterListMenu : UnityEditor.EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);
    public Options options;
    public List<Character> characters;

    [MenuItem("Diplomata/Characters", false, 0)]
    static public void Init()
    {
      CharacterListMenu window = (CharacterListMenu) GetWindow(typeof(CharacterListMenu), false, "Character List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnEnable()
    {
      options = OptionsController.GetOptions();
      characters = CharactersController.GetCharacters(options);
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (options.characterList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
      }

      for (int i = 0; i < options.characterList.Length; i++)
      {
        var name = options.characterList[i];
        var character = Character.Find(characters, name);

        if (character.SetId())
        {
          CharactersController.Save(character, options.jsonPrettyPrint);
        }

        GUILayout.BeginHorizontal();
        GUILayout.BeginHorizontal();

        if (EditorGUIUtility.isProSkin)
        {
          GUIHelper.labelStyle.normal.textColor = Color.white;
        }

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleLeft;
        GUILayout.Label(name, GUIHelper.labelStyle);

        GUIHelper.labelStyle.alignment = TextAnchor.MiddleRight;
        if (options.playerCharacterName == name)
        {
          GUILayout.Label("<b>[Player]</b>", GUIHelper.labelStyle);
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10.0f);

        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width / 2));

        if (GUILayout.Button("Edit", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          CharacterEditor.Edit(character);
          Close();
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          TalkableMessagesEditor.OpenContextMenu(character);
          Close();
        }

        if (GUILayout.Button("Delete", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          if (EditorUtility.DisplayDialog("Are you sure?", "Do you really want to delete?\nThis data will be lost forever.", "Yes", "No"))
          {
            var isPlayer = false;

            if (name == options.playerCharacterName)
            {
              isPlayer = true;
            }

            characters.Remove(character);
            options.characterList = ArrayHelper.Remove(options.characterList, name);

            JSONHelper.Delete(name, "Diplomata/Characters/");

            if (isPlayer && options.characterList.Length > 0)
            {
              options.playerCharacterName = options.characterList[0];
            }

            OptionsController.Save(options, options.jsonPrettyPrint);
            characters = CharactersController.GetCharacters(options);

            CharacterEditor.Reset(name);
            TalkableMessagesEditor.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < options.characterList.Length - 1)
        {
          GUIHelper.Separator();
        }
      }

      EditorGUILayout.Separator();

      if (GUILayout.Button("Create", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
      {
        CharacterEditor.OpenCreate();
        Close();
      }

      GUILayout.EndVertical();
      EditorGUILayout.EndScrollView();
    }

    public void OnInspectorUpdate()
    {
      Repaint();
    }
  }
}
