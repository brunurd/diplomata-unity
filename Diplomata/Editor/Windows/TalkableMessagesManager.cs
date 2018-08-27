using Diplomata.Editor;
using Diplomata.Editor.Helpers;
using Diplomata.Helpers;
using Diplomata.Models;
using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Windows
{
  public class TalkableMessagesManager : UnityEditor.EditorWindow
  {
    private ushort iteractions = 0;
    public static Talkable talkable;
    public static Context context;
    public static Texture2D headerBG;
    public static Texture2D mainBG;
    public static Texture2D sidebarBG;
    public static Texture2D textAreaBGTextureNormal;
    public static Texture2D textAreaBGTextureFocused;
    public static DiplomataEditorData diplomataEditor;

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
      TalkableMessagesManager window = (TalkableMessagesManager) GetWindow(typeof(TalkableMessagesManager), false, "Messages", true);
      window.minSize = new Vector2(1100, 300);

      TalkableMessagesManager.state = state;

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
      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
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
        sidebarBG = GUIHelper.UniformColorTexture(1, 1, ColorHelper.ColorAdd(baseColor, -0.03333f));
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

    public static void OpenContextMenu(Talkable currentTalkable)
    {
      talkable = currentTalkable;

      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      Init(State.Context);
    }

    public static void OpenMessagesMenu(Talkable currentTalkable, Context currentContext)
    {
      talkable = currentTalkable;
      context = currentContext;

      diplomataEditor = (DiplomataEditorData) AssetHelper.Read("Diplomata.asset", "Diplomata/");
      Init(State.Messages);
    }

    public static void Reset(string talkableName)
    {
      if (talkable != null)
      {
        if (talkable.name == talkableName)
        {
          talkable = null;
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
          Init(State.Close);
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

      if (iteractions == 100 && talkable != null)
      {
        string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";
        diplomataEditor.Save(talkable, folderName);
        iteractions = 0;
      }

      iteractions++;

    }

    public void OnDisable()
    {
      if (talkable != null)
      {
        string folderName = (talkable.GetType() == typeof(Character)) ? "Characters" : "Interactables";
        diplomataEditor.Save(talkable, folderName);
      }
    }
  }
}
