using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Diana.Core;

namespace DiplomataLib {

    public class Talk {

        public static bool canTalk;
        public static Action onStart = delegate { };
        public static Action onEnd = delegate { };

        public static void Start(DiplomataCharacter character) {
            if (character.talking) {
                canTalk = false;
                onStart();
            }
        }

        public static void End(DiplomataCharacter character, GameObject box) {
            onEnd();
            character.NextMessage();
            box.SetActive(false);
            canTalk = true;
        }

    }

    public class ChoiceMenu : ScriptableObject {

        public static Action onStart = delegate { };
        public static Action onEnd = delegate { };

        public static void Start(DiplomataCharacter character, GameObject box, Text emitter, Transform choiceList, GameObject choiceObject) {
            if (character.talking) {
                if (!box.activeSelf && character.choiceMenu) {
                    box.SetActive(true);
                    onStart();

                    emitter.text = character.PlayerName();

                    var choices = character.MessageChoices();

                    for (int i = 0; i < choices.Count; i++) {
                        var obj = GameObject.Instantiate(choiceObject);
                        obj.SetActive(true);
                        obj.transform.SetParent(choiceList);
                        obj.transform.localScale = new Vector3(1, 1, 1);

                        var button = obj.GetComponent<Button>();
                        var text = obj.transform.GetChild(0).GetComponent<Text>();

                        text.text = choices[i];

                        button.onClick.AddListener(delegate {
                            End(character, box, choiceList, text.text);
                        });

                        /*
                        if (i == 0) {
                            EventSystem.current.SetSelectedGameObject(obj);
                        }
                        */
                    }
                }
            }
        }

        public static void End(DiplomataCharacter character, GameObject box, Transform choiceList, string choice) {
            character.ChooseMessage(choice);
            box.SetActive(false);
            onEnd();

            for (int i = 1; i < choiceList.childCount; i++) {
                GameObject.Destroy(choiceList.GetChild(i).gameObject);
            }
        }
    }

    public class ShowMessage : ScriptableObject {

        public static Action onStart = delegate { };
        public static Action onEnd = delegate { };

        public static void Start(DiplomataCharacter character, GameObject box, GameObject playerEmitter,
            GameObject emitter, Text content, Button button, Text buttonText, string nextText, string endText) {
            if (character.talking) {
                if (!box.activeSelf && !character.choiceMenu) {
                    onStart();
                    box.SetActive(true);

                    if (character.EmitterIsPlayer()) {
                        playerEmitter.SetActive(true);
                        emitter.SetActive(false);
                        var text = playerEmitter.transform.GetChild(0).GetComponent<Text>();
                        text.text = character.Emitter();
                    }

                    else {
                        playerEmitter.SetActive(false);
                        emitter.SetActive(true);
                        var text = emitter.transform.GetChild(0).GetComponent<Text>();
                        text.text = character.Emitter();
                    }

                    content.text = character.ShowMessageContentSubtitle();

                    character.PlayMessageAudioContent();

                    button.onClick.RemoveAllListeners();

                    if (character.IsLastMessage()) {
                        buttonText.text = endText;

                        button.onClick.AddListener(() => {
                            Talk.End(character, box);
                        });
                    }

                    else {
                        buttonText.text = nextText;

                        button.onClick.AddListener(() => {
                            End(character, box);
                        });
                    }
                }
            }
        }

        public static void End(DiplomataCharacter character, GameObject box) {
            onEnd();
            character.NextMessage();

            if (!character.talking) {
                Talk.End(character, box);
            }

            box.SetActive(false);
        }

    }

}
