using System;
using System.Collections.Generic;
using UnityEngine;


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

        [HideInInspector]
        public List<Character> characters = new List<Character>();

        [HideInInspector]
        public int currentCharacterIndex;

        public void Awake() {
            if (instance == null) {
                instance = this;

                if (Application.isPlaying) {
                    DontDestroyOnLoad(gameObject);
                }
            }

            else if (instance != this) {
                DestroyImmediate(gameObject);
            }

            UpdatePreferences();
            SetCharacters();
        }

        public void SetCharacters() {
            characters = new List<Character>();

            var charactersArray = (Character[])FindObjectsOfType(typeof(Character));

            foreach (Character character in charactersArray) {
                characters.Add(character);
            }
        }

        static public void UpdatePreferences() {
            TextAsset json = (TextAsset)Resources.Load("preferences");
            preferences = JsonUtility.FromJson<Preferences>(json.text);
            Options.language = preferences.subLanguages[0];
        }
    }
}