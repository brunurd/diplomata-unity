using UnityEditor;
using UnityEngine;

namespace DiplomataEditor.Helpers
{
  public class ColorHelper
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="color"></param>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    /// <returns></returns>
    public static Color ColorAdd(Color color, float r, float g, float b, float a = 0)
    {
      Color newColor = new Color(0, 0, 0);
      newColor = color;
      newColor.r += r;
      newColor.g += g;
      newColor.b += b;
      newColor.a += a;
      return newColor;
    }

    public static Color ColorSub(Color color, float r, float g, float b, float a = 0)
    {
      Color newColor = color;
      newColor.r -= r;
      newColor.g -= g;
      newColor.b -= b;
      newColor.a -= a;
      return newColor;
    }

    public static Color ColorMul(Color color, float r, float g, float b, float a = 1)
    {
      Color newColor = color;
      newColor.r *= r;
      newColor.g *= g;
      newColor.b *= b;
      newColor.a *= a;
      return newColor;
    }

    public static Color ColorDiv(Color color, float r, float g, float b, float a = 1)
    {
      Color newColor = color;
      newColor.r /= r;
      newColor.g /= g;
      newColor.b /= b;
      newColor.a /= a;
      return newColor;
    }

    public static Color ColorAdd(Color colorA, Color colorB)
    {
      return ColorAdd(colorA, colorB.r, colorB.g, colorB.b, colorB.a);
    }

    public static Color ColorSub(Color colorA, Color colorB)
    {
      return ColorSub(colorA, colorB.r, colorB.g, colorB.b, colorB.a);
    }

    public static Color ColorMul(Color colorA, Color colorB)
    {
      return ColorMul(colorA, colorB.r, colorB.g, colorB.b, colorB.a);
    }

    public static Color ColorDiv(Color colorA, Color colorB)
    {
      return ColorDiv(colorA, colorB.r, colorB.g, colorB.b, colorB.a);
    }

    public static Color ColorAdd(Color color, float value, float alpha = 1.0f)
    {
      return ColorAdd(color, value, value, value, alpha);
    }

    public static Color ColorSub(Color color, float value, float alpha = 1.0f)
    {
      return ColorSub(color, value, value, value, alpha);
    }

    public static Color ColorMul(Color color, float value, float alpha = 1.0f)
    {
      return ColorMul(color, value, value, value, alpha);
    }

    public static Color ColorDiv(Color color, float value, float alpha = 1.0f)
    {
      return ColorDiv(color, value, value, value, alpha);
    }

    public static Color ResetColor()
    {
      if (EditorGUIUtility.isProSkin)
      {
        return GUIHelper.proBGColor;
      }

      else
      {
        return GUIHelper.BGColor;
      }
    }

    public static Color PlaymodeTint()
    {
      try
      {
        if (Application.isPlaying)
        {
          string[] playmodeTintArray = EditorPrefs.GetString("Playmode tint").Split(';');
          return new Color(float.Parse(playmodeTintArray[1]), float.Parse(playmodeTintArray[2]), float.Parse(playmodeTintArray[3]), float.Parse(playmodeTintArray[4]));
        }
      }

      catch
      {
        Debug.Log("Cannot get playmode tint.");
      }

      return Color.gray;
    }
  }
}
