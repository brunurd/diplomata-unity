using UnityEngine;
using System;

namespace Diplomata {

    [Serializable]
    public class Preferences {
        public string[] attributes;
        public string[] subLanguages;
        public string[] dubLanguages;
    }

    [Serializable]
    public class Options {
        public static string language;
    }

    [Serializable]
    [ExecuteInEditMode]
    public class Manager : MonoBehaviour {
        public static Manager instance = null;
        public static Preferences preferences;
        public static Texture logo;
        [HideInInspector]
        public Options options;

        public void Awake() {
            if (instance == null) {
                instance = this;
            }

            else if (instance != this) {
                Destroy(gameObject);
            }

            UpdatePreferences();
        }

        static public void UpdatePreferences() {
            logo = (Texture)Resources.Load("DIPLOMATA-logo");
            TextAsset json = (TextAsset)Resources.Load("preferences");
            preferences = JsonUtility.FromJson<Preferences>(json.text);
            Options.language = preferences.subLanguages[0];
        }
    }
}