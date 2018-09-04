using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Helpers
{
  public static class UndoHelper
  {
    private static object obj;
    private static object cachedValue;

    public static void EventListener<T>(object obj, ref T value)
    {
      Event e = Event.current;

      if (e != null)
      {
        if (e.control)
        {
          Undo.ClearAll();
        }

        if (e.control && e.keyCode == KeyCode.Z && obj.Equals(UndoHelper.obj))
        {
          value = (T) cachedValue;
          GUI.SetNextControlName("noFocus");
          GUI.Label(new Rect(-100, -100, 1, 1), "");
          GUI.FocusControl("noFocus");
        }
      }
    }

    public static void AddObject(object obj, object value)
    {
      UndoHelper.obj = obj;
      cachedValue = value;
    }
  }
}
