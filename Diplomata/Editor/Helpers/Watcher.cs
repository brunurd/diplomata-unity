using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Helpers
{
  [ExecuteInEditMode]
  public class Watcher : ScriptableObject
  {
    private List<bool> flags;
    private Action action;

    public void Set(Action action, params bool[] flags)
    {
      this.action = action;
      foreach (var flag in flags)
      {
        this.flags.Add(flag);
      }
    }

    private bool GetFlags()
    {
      foreach (var flag in flags)
      {
        if (!flag) return false;
      }
      return true;
    }

    private void Update()
    {
      if (GetFlags())
      {
        action();
      }
    }
  }
}
