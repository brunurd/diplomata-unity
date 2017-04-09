using UnityEngine;

namespace Diplomata {

    public class GameProgress : MonoBehaviour {
        public static string currentSubtitledLanguage;

        public GameProgress() {
            currentSubtitledLanguage = Preferences.subLanguages[0];
        }
    }

}
