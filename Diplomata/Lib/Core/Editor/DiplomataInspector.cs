using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiplomataLib
{
  [UnityEditor.CustomEditor(typeof(Diplomata))]
  public class DiplomataInspector : Editor
  {
    public override void OnInspectorGUI()
    {
      UnityEditor.EditorGUILayout.HelpBox("\nthis auto-generated file is a object to store all Diplomata data.\n\n" +
        "The object instantiate just one time in the game build in the first scene it's appear. (It's a Singleton)\n\n" +
        "The real data are stored in the resources folder, so don't worry if you need to delete this object during development.\n", UnityEditor.MessageType.Info);
    }
  }
}