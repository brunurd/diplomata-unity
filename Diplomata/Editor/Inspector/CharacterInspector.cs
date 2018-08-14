using DiplomataEditor.Core;
using DiplomataEditor.Editors;
using DiplomataEditor.Helpers;
using Diplomata;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Inspector
{
  /// <summary>
  /// The inspector class to DiplomataCharacter
  /// <seealso cref="DiplomataLib.DiplomataCharacter">
  /// </summary>
  [CustomEditor(typeof(DiplomataCharacter))]
  [CanEditMultipleObjects]
  public class CharacterInspector : Editor
  {
    public DiplomataCharacter diplomataCharacter;
    private static DiplomataEditorManager diplomataEditor;

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
      DiplomataEditorManager.Instantiate();
      diplomataEditor = (DiplomataEditorManager) AssetHelper.Read("Diplomata.asset", "Diplomata/");
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

      if (diplomataCharacter.character != null && diplomataEditor.characters.Count > 0)
      {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Character: ");

        if (!Application.isPlaying)
        {
          var selected = 0;

          for (var i = 0; i < diplomataEditor.characters.Count; i++)
          {
            if (diplomataEditor.characters[i].name == diplomataCharacter.character.name)
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
              diplomataCharacter.character = diplomataEditor.characters[i];
              diplomataEditor.characters[selectedBefore].onScene = false;
              diplomataCharacter.character.onScene = true;
              break;
            }
          }
        }

        else
        {
          GUILayout.Label(diplomataCharacter.character.name);
        }

        GUILayout.EndHorizontal();

        if (GUILayout.Button("Refresh", GUILayout.Height(GUIHelper.BUTTON_HEIGHT_SMALL)))
        {
          Refresh();
        }

        GUIHelper.Separator();

        var showInfluence = true;

        if (diplomataCharacter.character.name == diplomataEditor.options.playerCharacterName)
        {
          EditorGUILayout.HelpBox("\nThis character is the player, he doesn't influence himself, use his messages only in the case he speaks with himself.\n", MessageType.Info);
          showInfluence = false;
        }

        if (GUILayout.Button("Edit Character", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          CharacterEditor.Edit(diplomataCharacter.character);
        }

        if (GUILayout.Button("Edit Messages", GUILayout.Height(GUIHelper.BUTTON_HEIGHT)))
        {
          CharacterMessagesManager.OpenContextMenu(diplomataCharacter.character);
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

          GUILayout.Label("Influence: <b>" + diplomataCharacter.character.influence.ToString() + "</b>", GUIHelper.labelStyle);
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
