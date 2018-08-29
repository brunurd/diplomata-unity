using Diplomata;
using Diplomata.Editor;
using Diplomata.Editor.Helpers;
using Diplomata.Editor.Windows;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Inspector
{
  /// <summary>
  /// The inspector class to DiplomataCharacter
  /// <seealso cref="DiplomataLib.DiplomataCharacter">
  /// </summary>
  [CustomEditor(typeof(DiplomataCharacter))]
  [CanEditMultipleObjects]
  public class DiplomataCharacterInspector : UnityEditor.Editor
  {
    public DiplomataCharacter diplomataCharacter;
    private static DiplomataEditorData diplomataEditor;

    /// <summary>
    /// Refresh right on enable on editor (on appears in the inspector)
    /// </summary>
    private void OnEnable()
    {
      Refresh();
    }

    /// <summary>
    /// The method to refresh the Diplomata Character on GUI
    /// </summary>
    private void Refresh()
    {
      DiplomataEditorData.Instantiate();
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataCharacter = target as DiplomataCharacter;
    }

    /// <summary>
    /// The inspector graphic loop
    /// </summary>
    public override void OnInspectorGUI()
    {
      GUIHelper.Init();
      serializedObject.Update();
      GUILayout.BeginVertical(GUIHelper.windowStyle);

      if (diplomataCharacter.talkable != null && diplomataEditor.characters.Count > 0)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character: ");

        if (!Application.isPlaying)
        {
          var selected = 0;

          for (var i = 0; i < diplomataEditor.characters.Count; i++)
          {
            if (diplomataEditor.characters[i].name == diplomataCharacter.talkable.name)
            {
              selected = i;
              break;
            }
          }

          var selectedBefore = selected;
          selected = EditorGUILayout.Popup(selected, diplomataEditor.options.characterList);

          for (var i = 0; i < diplomataEditor.characters.Count; i++)
          {
            if (selected == i)
            {
              diplomataCharacter.talkable = diplomataEditor.characters[i];
              diplomataEditor.characters[selectedBefore].onScene = false;
              diplomataCharacter.talkable.onScene = true;
              break;
            }
          }
        }

        else
        {
          GUILayout.Label(diplomataCharacter.talkable.name);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          Refresh();
        }

        GUIHelper.Separator();

        var showInfluence = true;

        if (diplomataCharacter.talkable.name == diplomataEditor.options.playerCharacterName)
        {
          EditorGUILayout.HelpBox("\nThis character is the player, he doesn't influence himself, use his messages only in the case he speaks with himself.\n", MessageType.Info);
          showInfluence = false;
        }

        if (GUILayout.Button("Edit Character", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          CharacterEditor.Edit((Character) diplomataCharacter.talkable);
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          TalkableMessagesManager.OpenContextMenu((Character) diplomataCharacter.talkable);
        }

        if (showInfluence)
        {
          GUIHelper.labelStyle.alignment = TextAnchor.UpperCenter;
          EditorGUILayout.Separator();

          if (EditorGUIUtility.isProSkin)
          {
            GUIHelper.labelStyle.normal.textColor = GUIHelper.proTextColor;
          }

          else
          {
            GUIHelper.labelStyle.normal.textColor = GUIHelper.freeTextColor;
          }

          if (diplomataCharacter.talkable.GetType() == typeof(Character))
          {
            Character character = (Character) diplomataCharacter.talkable;
            GUILayout.Label("Influence: <b>" + character.influence.ToString() + "</b>", GUIHelper.labelStyle);
          }
        }

        GUIHelper.Separator();
        GUILayout.BeginHorizontal();

        if (GUILayout.Button("Create Character", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_BIG)))
        {
          CharacterEditor.OpenCreate();
        }

        EditorGUILayout.HelpBox("Create does not interfe in this character.", MessageType.Info);
        GUILayout.EndHorizontal();
      }

      else
      {
        if (GUILayout.Button("Create Character", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          CharacterEditor.OpenCreate();
        }
      }

      GUILayout.EndVertical();
      serializedObject.ApplyModifiedProperties();
    }
  }
}
