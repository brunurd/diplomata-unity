using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
        public static Action onUpdate = delegate { };
        public static Action onEnd = delegate { };

        public static void Start(DiplomataCharacter character, GameObject box, Text emitter, Transform choiceList, GameObject choiceObject, bool selectFirst = true) {
            if (character.talking) {
                if (!box.activeSelf && character.choiceMenu) {
                    box.SetActive(true);
                    onStart();

                    emitter.text = character.PlayerName();

                    var choices = character.MessageChoices();

                    for (int i = 0; i < choices.Count; i++) {
                        var obj = Instantiate(choiceObject);
                        obj.SetActive(true);
                        obj.transform.SetParent(choiceList);
                        obj.transform.localScale = new Vector3(1, 1, 1);

                        var button = obj.GetComponent<Button>();
                        var text = obj.transform.GetChild(0).GetComponent<Text>();

                        text.text = choices[i];

                        button.onClick.AddListener(delegate {
                            End(character, box, choiceList, text.text);
                        });
                        
                        if (i == 0) {
                            if (selectFirst) {
                                EventSystem.current.SetSelectedGameObject(obj);
                            }
                        }
                    }
                }
            }
        }

        public static void Update(GameObject box) {
            if (box.activeSelf) {
                onUpdate();
            }
        }

        public static void End(DiplomataCharacter character, GameObject box, Transform choiceList, string choice) {
            character.ChooseMessage(choice);
            box.SetActive(false);
            onEnd();

            for (int i = 1; i < choiceList.childCount; i++) {
                Destroy(choiceList.GetChild(i).gameObject);
            }
        }
    }

    public class ShowMessage : ScriptableObject {

        public static Action onStart = delegate { };
        public static Action onUpdate = delegate { };
        public static Action onEnd = delegate { };

        public static int maxFrame = 1;
        public static string messageContent = "";

        private static bool letterByLetter;
        private static int currentFrame;
        private static int currentLenght;

        public static void Start(DiplomataCharacter character, GameObject box, GameObject playerEmitter, 
            GameObject emitter, Text content, Button button, Text buttonText, string nextText, string endText, 
            Image buttonImage, Sprite nextSprite, Sprite endSprite, bool letterByLetter = false) {

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

                    button.onClick.RemoveAllListeners();

                    if (character.IsLastMessage()) {
                        if (buttonText != null) {
                            buttonText.text = endText;
                        }

                        if (buttonImage != null && endSprite != null) {
                            buttonImage.sprite = endSprite;
                        }

                        button.onClick.AddListener(() => {
                            Talk.End(character, box);
                        });
                    }

                    else {
                        if (buttonText != null) {
                            buttonText.text = nextText;
                        }

                        if (buttonImage != null && nextSprite != null) {
                            buttonImage.sprite = nextSprite;
                        }

                        button.onClick.AddListener(() => {
                            End(character, box);
                        });
                    }

                    if (letterByLetter) {
                        ShowMessage.letterByLetter = true;
                        content.text = "";
                        button.gameObject.SetActive(false);
                    }

                    else {
                        content.text = character.ShowMessageContentSubtitle();
                    }
                }
            }
        }

        public static void Start(DiplomataCharacter character, GameObject box, GameObject playerEmitter,
            GameObject emitter, Text content, Button button, Text buttonText, string nextText, string endText, bool letterByLetter = false) {
            Start(character, box, playerEmitter, emitter, content, button, buttonText, nextText, endText, null, null, null, letterByLetter);
        }

        public static void Start(DiplomataCharacter character, GameObject box, GameObject playerEmitter,
            GameObject emitter, Text content, Button button, Image buttonImage, Sprite nextSprite, Sprite endSprite, bool letterByLetter = false) {
            Start(character, box, playerEmitter, emitter, content, button, null, "", "", buttonImage, nextSprite, endSprite, letterByLetter);
        }

        public static void Update(DiplomataCharacter character, GameObject box, Text content, Button button, bool letterByLetter = true) {
            if (box.activeSelf) {
                var fullContent = character.ShowMessageContentSubtitle();

                if (letterByLetter && ShowMessage.letterByLetter) {
                    if (currentFrame < maxFrame) {
                        currentFrame += 1;
                    }

                    else {
                        currentFrame = 0;

                        messageContent += fullContent[currentLenght];
                        currentLenght += 1;

                        content.text = messageContent;
                    }

                    if (currentLenght == fullContent.Length) {
                        currentFrame = 0;
                        currentLenght = 0;
                        messageContent = "";
                        ShowMessage.letterByLetter = false;
                        button.gameObject.SetActive(true);
                    }
                }

                onUpdate();
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
