using System.Collections.Generic;
using UnityEngine;

namespace Diplomata {

    [ExecuteInEditMode]
    public class Manager : MonoBehaviour {
        public static Manager instance = null;
        public static Preferences preferences;
        public static GameProgress gameProgress;
        public static List<Character> characters;

        public void Awake() {
            if (instance == null) {
                instance = this;
                preferences = new Preferences();
                gameProgress = new GameProgress();
                characters = new List<Character>();

                if (Application.isPlaying) {
                    DontDestroyOnLoad(gameObject);
                }
            }

            else if (instance != this) {
                DestroyImmediate(gameObject);
            }
        }

        static public void Instantiate() {
            GameObject obj = new GameObject("Diplomata");
            obj.hideFlags = HideFlags.HideInHierarchy;
            obj.AddComponent<Manager>();
        }
    }

}