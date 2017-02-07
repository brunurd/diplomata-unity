using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Diplomata {

    public class ExampleGUI : MonoBehaviour {
        
        public Character character;
        private List<string> choices = new List<string>();
        
        public void OnGUI() {
            
            // Talking routine
            if (character.talking) {

                // Character talk turn
                if (character.isTalking) {
                    GUILayout.Label( character.ShowMessageContent() );
                    
                    if (GUILayout.Button("Next")) {
                        character.NextMessage();
                    }
                }


                // Player talk turn
                if (character.waitingPlayer) {
                    choices = character.MessageChoices();

                    // Show choices buttons
                    foreach (string choice in choices) {
                        if (GUILayout.Button(choice)) {
                            character.ChooseMessage(choice);
                            GUILayout.Label( character.ShowMessageContent() );
                            character.NextMessage();

                            // Reset choices buttons
                            choices = new List<string>();
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

}