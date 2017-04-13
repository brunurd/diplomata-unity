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
                    instance.gameObject.hideFlags = HideFlags.None;
                }
            }

            else {
                DestroyImmediate(gameObject);
            }

            Restart();
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
            if (instance == null && FindObjectsOfType<Diplomata>().Length < 1) {
                GameObject obj = new GameObject("[ Diplomata ]");
                obj.hideFlags = HideFlags.HideInHierarchy;
                obj.AddComponent<Diplomata>();
            }

            Restart();
        }

        private void CheckRepeated() {
            var repeated = FindObjectsOfType<Diplomata>();

            foreach (Diplomata item in repeated) {
                if (!item.Equals(instance)) {
                    DestroyImmediate(item.gameObject);
                }
            }
        }

        public static Character FindCharacter(string name) {
            foreach (Character character in characters) {
                if (character.name == name) {
                    return character;
                }
            }

            return null;
        }

        public static T[] ListToArray<T>(List<T> list) {
            T[] array = new T[list.Count];

            for (int i = 0; i < list.Count; i++) {
                array[i] = list[i];
            }

            return array;
        }

        public static T[] Add<T>(T[] array, T element) {
            var tempArray = new T[array.Length];

            for (var i = 0; i < tempArray.Length; i++) {
                tempArray[i] = array[i];
            }

            array = new T[tempArray.Length + 1];

            for (var i = 0; i < tempArray.Length; i++) {
                array[i] = tempArray[i];
            }

            array[array.Length - 1] = element;

            return array;
        }
    }

}