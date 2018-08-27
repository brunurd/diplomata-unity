using UnityEditor;
using UnityEngine;

namespace Diplomata.Editor.Helpers
{
  /// <summary>
  /// The helper to improve the Unity GUI
  /// </summary>
  public class GUIHelper
  {
    public const byte MARGIN = 10;
    public const byte BUTTON_HEIGHT_SMALL = 20;
    public const byte BUTTON_HEIGHT = 30;
    public const byte BUTTON_HEIGHT_BIG = 40;
    public const int WINDOW_MIN_WIDTH = 400;

    public static bool focusOnStart = true;
    public static int strokeWidth = 1;

    public static Color BGColor = new Color(0.76078f, 0.76078f, 0.76078f);
    public static Color proBGColor = new Color(0.2196f, 0.2196f, 0.2196f);
    public static Color BGBoxColor = ColorHelper.ColorAdd(BGColor, 0.1647f);
    public static Color proBGBoxColor = ColorHelper.ColorAdd(proBGColor, 0.1647f);
    public static Color transparentColor = new Color(0, 0, 0, 0);
    public static Color grey = new Color(0.5f, 0.5f, 0.5f);
    public static Color freeTextColor = new Color(0.00392f, 0.00392f, 0.00392f);
    public static Color proTextColor = new Color(0.70588f, 0.70588f, 0.70588f);

    public static Texture2D transparentTexture;
    public static Texture2D softAlphaBlack;

    public static GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
    public static GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
    public static GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
    public static GUIStyle separatorStyle = new GUIStyle(GUI.skin.box);
    public static GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
    public static GUIStyle windowStyle = new GUIStyle();

    public static GUIContent textContent = new GUIContent("");
    public static RectOffset padding = new RectOffset(MARGIN, MARGIN, MARGIN, MARGIN);
    public static RectOffset textAreaPadding = new RectOffset(3, 3, 5, 5);
    public static RectOffset zeroPadding = new RectOffset(0, 0, 0, 0);
    private static Rect strokeRect = new Rect(0, 0, 0, 0);

    /// <summary>
    /// Handle the values on init a render loop
    /// </summary>
    public static void Init()
    {
      if (transparentTexture == null)
      {
        transparentTexture = UniformColorTexture(1, 1, transparentColor);
      }

      if (softAlphaBlack == null)
      {
        softAlphaBlack = UniformColorTexture(1, 1, new Color(0, 0, 0, 0.05f));
      }

      windowStyle.padding = padding;
      textAreaStyle.padding = textAreaPadding;

      boxStyle.padding = padding;
      boxStyle.richText = true;

      labelStyle.fontSize = 11;
      labelStyle.alignment = TextAnchor.MiddleLeft;
      labelStyle.richText = true;
      labelStyle.wordWrap = true;
      labelStyle.normal.textColor = Color.black;

      separatorStyle.normal.background = softAlphaBlack;

      textContent.text = "";
    }

    /// <summary>
    /// Focus a text field on the start of drawning a window
    /// </summary>
    /// <param name="name">The name registred on control name</param>
    public static void Focus(string name)
    {
      if (focusOnStart)
      {
        EditorGUI.FocusTextInControl(name);
        focusOnStart = false;
      }
    }

    public static Texture2D UniformColorTexture(int w, int h, Color color)
    {
      try
      {
        Texture2D tex = new Texture2D(w, h);

        for (int i = 0; i < tex.width; i++)
        {
          for (int j = 0; j < tex.height; j++)
          {
            tex.SetPixel(i, j, color);
          }
        }

        tex.Apply();
        return tex;
      }

      catch (System.Exception e)
      {
        Debug.LogWarning("Cannot create a texture in this context. " + e.Message);
        return Texture2D.whiteTexture;
      }
    }

    public static string Popup(string label, string choice, string[] array, params GUILayoutOption[] options)
    {
      var selected = 0;
      string str = choice;

      for (int i = 0; i < array.Length; i++)
      {
        if (str == array[i])
        {
          selected = i;
          break;
        }
      }

      GUILayout.BeginHorizontal();
      GUILayout.Label(label);
      selected = EditorGUILayout.Popup(selected, array, options);
      GUILayout.EndHorizontal();

      for (int i = 0; i < array.Length; i++)
      {
        if (selected == i)
        {
          str = array[i];
          break;
        }
      }

      return str;
    }

    public static void Separator()
    {
      EditorGUILayout.Separator();
      GUILayout.Box("", separatorStyle, GUILayout.ExpandWidth(true), GUILayout.Height(2));
      EditorGUILayout.Separator();
    }

    public static void DrawRectStroke(Rect rect, Color color)
    {
      strokeRect.xMin = rect.xMin - strokeWidth;
      strokeRect.xMax = rect.xMax + strokeWidth;
      strokeRect.yMin = rect.yMin - strokeWidth;
      strokeRect.yMax = rect.yMax + strokeWidth;
      EditorGUI.DrawRect(strokeRect, ColorHelper.ColorSub(color, 0.25f, -0.1f));
    }
  }
}
