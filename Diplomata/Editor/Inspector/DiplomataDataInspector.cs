using System.Collections.Generic;
using LavaLeak.Diplomata;
using UnityEditor;
using UnityEngine;

namespace LavaLeak.Diplomata.Editor.Inspector
{
  [CustomEditor(typeof(DiplomataData))]
  public class DiplomataManagerInspector : UnityEditor.Editor
  {
    public override void OnInspectorGUI()
    {
      EditorGUILayout.HelpBox("\nthis auto-generated file is a object to store all Diplomata data.\n\n" +
        "The object instantiate just one time in the game build in the first scene it's appear. (It's a Singleton)\n\n" +
        "The real data are stored in the resources folder, so don't worry if you need to delete this object during development.\n", MessageType.Info);
    }
  }
}
