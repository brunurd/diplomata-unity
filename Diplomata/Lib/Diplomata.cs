using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {

    [AddComponentMenu("")]
    [ExecuteInEditMode]
    public class Diplomata : MonoBehaviour {
        public static Diplomata instance = null;
        public static Preferences preferences;
        public static GameProgress gameProgress;
        public static List<Character> characters;

        public void Awake() {
            if (instance == null) {
                instance = this;

                if (Application.isPlaying) {
                    DontDestroyOnLoad(gameObject);
                }

                Restart();
            }

            else {
                DestroyImmediate(gameObject);
            }
        }

        public static void Restart() {
            preferences = new Preferences();
            preferences.Start();
            
            characters = new List<Character>();
            Character.UpdateList();
            
            gameProgress = new GameProgress();
            gameProgress.Start();
        }

        public static void Instantiate() {
            #if UNITY_EDITOR

            if (instance == null && FindObjectsOfType<Diplomata>().Length < 1) {
                GameObject obj = new GameObject("[ Diplomata ]");
                obj.AddComponent<Diplomata>();
            }

            Restart();
            
            #endif
        }

        private void CheckRepeated() {
            var repeated = FindObjectsOfType<Diplomata>();

            foreach (Diplomata item in repeated) {
                if (!item.Equals(instance)) {
                    DestroyImmediate(item.gameObject);
                }
            }
        }
    }

#if UNITY_EDITOR
    [UnityEditor.CustomEditor(typeof(Diplomata))]
    public class DiplomataWarning : UnityEditor.Editor {
        public override void OnInspectorGUI() {
            UnityEditor.EditorGUILayout.HelpBox("\nthis auto-generated file is a object to store all Diplomata data.\n\n" +
                "The object instantiate just one time in the game build in the first scene it's appear. (It's a Singleton)\n\n" +
                "The real data are stored in the resources folder, so don't worry if you need to delete this object during development.\n", UnityEditor.MessageType.Info);
        }
    }
#endif

}