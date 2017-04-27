using UnityEngine;
using DiplomataLib;

public class ExampleGUI : MonoBehaviour {
    
    public DiplomataCharacter character;

    public void Start() {
        character = GetComponent<DiplomataCharacter>();
    }

    public void OnGUI() {
            
        // Talking loop
        if (character.talking) {
            
            // Choice menu
            if (character.choiceMenu) {
                var choices = character.MessageChoices();

                // Show choices buttons
                foreach (string choice in choices) {
                    if (GUILayout.Button(choice)) {
                        character.ChooseMessage(choice);
                    }
                }
            }

            // Simple message (not a choice)
            else {
                GUILayout.Label( character.ShowMessageContentSubtitle() );

                // Last message feedback
                if (character.IsLastMessage()) {
                    if (GUILayout.Button("Bye")) {
                        character.EndTalk();
                    }
                }

                // Next message input
                else {
                    if (GUILayout.Button("Next")) {
                        character.NextMessage();
                    }
                }

            }
        }

        else {
            // Behaviour to start a conversation:
            if (GUILayout.Button("Start conversation with " + name )) {
                character.StartTalk();
            }
        }

    }
}