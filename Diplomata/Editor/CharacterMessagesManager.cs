using DiplomataEditor.Core;
using DiplomataEditor.Editors;
using DiplomataEditor.Helpers;
using DiplomataEditor.ListMenu;
using DiplomataLib;
using UnityEditor;
using UnityEngine;

namespace DiplomataEditor
{
  public class CharacterMessagesManager : EditorWindow
  {
    private ushort iteractions = 0;
    public static Character character;
    public static Context context;
    public static Texture2D headerBG;
    public static Texture2D mainBG;
    public static Texture2D sidebarBG;
    public static Texture2D textAreaBGTextureNormal;
    public static Texture2D textAreaBGTextureFocused;
    public static Core.Diplomata diplomataEditor;

    public enum State
    {
      None,
      Context,
      Messages,
      Close
    }

    public static State state;

    public static void Init(State state = State.None)
    {
      CharacterMessagesManager window = (CharacterMessagesManager) GetWindow(typeof(CharacterMessagesManager), false, "Messages", true);
      window.minSize = new Vector2(1100, 300);

      CharacterMessagesManager.state = state;

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

    public void SetTextures()
    {
      var baseColor = ColorHelper.ResetColor();

      if (headerBG == null)
      {
        headerBG = GUIHelper.UniformColorTexture(1, 1, ColorHelper.ColorAdd(baseColor, 0.05f, 0.05f, 0.05f));
      }

      if (mainBG == null)
      {
        mainBG = GUIHelper.UniformColorTexture(1, 1, baseColor);
      }

      if (sidebarBG == null)
      {
        sidebarBG = GUIHelper.UniformColorTexture(1, 1, ColorHelper.ColorAdd(baseColor, -0.1f, -0.1f, -0.1f));
      }

      if (textAreaBGTextureNormal == null)
      {
        textAreaBGTextureNormal = GUIHelper.UniformColorTexture(1, 1, new Color(0.4f, 0.4f, 0.4f, 0.08f));
      }

      if (textAreaBGTextureFocused == null)
      {
        textAreaBGTextureFocused = GUIHelper.UniformColorTexture(1, 1, new Color(1, 1, 1, 1));
      }
    }

    public static void OpenContextMenu(Character currentCharacter)
    {
      character = currentCharacter;

      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingCharacter = currentCharacter.name;
      diplomataEditor.workingContextMessagesId = -1;
      Init(State.Context);
    }

    public static void OpenMessagesMenu(Character currentCharacter, Context currentContext)
    {
      character = currentCharacter;
      context = currentContext;

      diplomataEditor = (Core.Diplomata) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      diplomataEditor.workingCharacter = currentCharacter.name;
      diplomataEditor.workingContextMessagesId = currentContext.id;
      Init(State.Messages);
    }

    public static void Reset(string characterName)
    {
      if (character != null)
      {
        if (character.name == characterName)
        {
          diplomataEditor.workingCharacter = string.Empty;
          character = null;
          Init(State.Close);
        }
      }
    }

    public void OnGUI()
    {
      GUIHelper.Init();
      SetTextures();

      switch (state)
      {
        case State.None:
          if (diplomataEditor.workingCharacter != string.Empty)
          {
            character = Character.Find(diplomataEditor.characters, diplomataEditor.workingCharacter);

            if (diplomataEditor.workingContextMessagesId > -1)
            {
              context = Context.Find(character, diplomataEditor.workingContextMessagesId);
              MessagesEditor.Draw();
            }

            else
            {
              ContextListMenu.Draw();
            }
          }
          break;

        case State.Context:
          ContextListMenu.Draw();
          break;

        case State.Messages:
          MessagesEditor.Draw();
          break;
      }

      AutoSave();
    }

    private void AutoSave()
    {

      if (iteractions == 100 && character != null)
      {
        diplomataEditor.Save(character);
        iteractions = 0;
      }

      iteractions++;

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
