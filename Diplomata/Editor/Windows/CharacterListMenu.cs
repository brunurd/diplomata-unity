using LavaLeak.Diplomata.Editor.Controllers;
using LavaLeak.Diplomata.Editor.Helpers;
using LavaLeak.Diplomata.Helpers;
using LavaLeak.Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Windows
{
  public class CharacterListMenu : EditorWindow
  {
    public Vector2 scrollPos = new Vector2(0, 0);

    [MenuItem("Tools/Diplomata/Edit/Characters", false, 0)]
    static public void Init()
    {
      CharacterListMenu window = (CharacterListMenu) GetWindow(typeof(CharacterListMenu), false, "Character List");
      window.minSize = new Vector2(GUIHelper.WINDOW_MIN_WIDTH + 80, 300);
      window.Show();
    }

    public void OnGUI()
    {
      GUIHelper.Init();

      scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (Controller.Instance.Options.characterList.Length <= 0)
      {
        EditorGUILayout.HelpBox("No characters yet.", MessageType.Info);
      }

      for (int i = 0; i < Controller.Instance.Options.characterList.Length; i++)
      {
        var name = Controller.Instance.Options.characterList[i];
        var character = Character.Find(Controller.Instance.Characters, name);

        if (character.SetId())
        {
          CharactersController.Save(character, Controller.Instance.Options.jsonPrettyPrint);
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
        if (Controller.Instance.Options.playerCharacterName == name)
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

            if (name == Controller.Instance.Options.playerCharacterName)
              isPlayer = true;

            Controller.Instance.Characters.Remove(character);
            Controller.Instance.Options.characterList = ArrayHelper.Remove(Controller.Instance.Options.characterList, name);

            JSONHelper.Delete(name, "Diplomata/Characters/");

            if (isPlayer && Controller.Instance.Options.characterList.Length > 0)
            {
              Controller.Instance.Options.playerCharacterName = Controller.Instance.Options.characterList[0];
            }

            OptionsController.Save(Controller.Instance.Options, Controller.Instance.Options.jsonPrettyPrint);
            Controller.Instance.Characters = CharactersController.GetCharacters(Controller.Instance.Options);

            CharacterEditor.Reset(name);
            TalkableMessagesEditor.Reset(name);
            ContextEditor.Reset(name);
          }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndHorizontal();

        if (i < Controller.Instance.Options.characterList.Length - 1)
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
