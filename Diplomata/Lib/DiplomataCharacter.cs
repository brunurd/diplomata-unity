using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class DiplomataCharacter : MonoBehaviour {
        
        public Character character;
        public List<Message> choices = new List<Message>();
        public Message currentMessage;
        public bool talking;
        public bool choiceMenu;

        private int lastMessageId;
        private int lastColumnId;
        private Context currentContext;
        private Column currentColumn;
        private Dictionary<string, int> controlIndexes = new Dictionary<string, int>();
        
        public void Start() {
            if (Application.isPlaying) {
                if (character != null) {
                    if (character.startOnPlay) {
                        StartTalk();
                    }
                }

                else {
                    Debug.LogWarning("You don't attached a character in the Game Object!");
                }
            }
        }
        
        public void StartTalk() {
            if (character != null) {

                controlIndexes = new Dictionary<string, int>();
                controlIndexes.Add("context", 0);
                controlIndexes.Add("column", 0);
                controlIndexes.Add("message", 0);

                talking = true;
                currentContext = null;
                currentColumn = null;
                currentMessage = null;

                choices = new List<Message>();
                
                for (controlIndexes["context"] = 0; controlIndexes["context"] < character.contexts.Length; controlIndexes["context"]++) {
                    var context = Context.Find(character, controlIndexes["context"]);
                    var lastContext = character.contexts.Length - 1;

                    if (context != null) {
                        if (!context.happened) {
                            currentContext = context;
                            break;
                        }

                        else if (controlIndexes["context"] == lastContext) {
                            Debug.LogWarning("No more contexts avaliable in the character " + character.name + ".");
                            talking = false;
                            break;
                        }
                    }
                }

                if (currentContext == null) {
                    Debug.LogError("No context found in " + character.name + ".");
                    talking = false;
                }

                else {
                    Next(false);
                }
            }

            else {
                Debug.LogError("You don't attached a character in the Game Object!");
                talking = false;
            }
        }

        

        private void Next(bool hasFate) {
            if (character != null && currentContext != null) {

                currentColumn = Column.Find(currentContext, controlIndexes["column"]);

                if (currentColumn == null) {
                    for (controlIndexes["column"] = controlIndexes["column"]; controlIndexes["column"] < character.contexts.Length; controlIndexes["column"]++) {
                        currentColumn = Column.Find(currentContext, controlIndexes["column"]);

                        if (currentColumn != null) {
                            break;
                        }
                    }
                }

                if (currentColumn != null) {

                    if (hasFate) {
                        currentMessage = currentColumn.messages[controlIndexes["message"]];
                    }

                    else {
                        var msg = Message.Find(currentColumn.messages, controlIndexes["message"]);
                        var proceed = true;

                        if (msg != null) {
                            if (msg.conditions.Length > 0) {

                                foreach (Condition condition in msg.conditions) {

                                    switch (condition.type) {
                                        case Condition.Type.AfterOf:
                                            if (condition.afterOfMessageId != lastMessageId && condition.afterOfMessageColumnId != lastColumnId) {
                                                condition.proceed = false;
                                            }

                                            break;

                                        case Condition.Type.InfluenceEqualTo:
                                            if (character.influence != condition.comparedInfluence) {
                                                condition.proceed = false;
                                            }
                                            break;

                                        case Condition.Type.InfluenceGreaterThan:
                                            if (character.influence <= condition.comparedInfluence) {
                                                condition.proceed = false;
                                            }
                                            break;

                                        case Condition.Type.InfluenceLessThan:
                                            if (character.influence >= condition.comparedInfluence) {
                                                condition.proceed = false;
                                            }
                                            break;
                                    }

                                    if (!condition.custom.CheckAll()) {
                                        condition.proceed = false;
                                    }
                                }

                                proceed = Condition.CanProceed(msg.conditions);
                            }

                            var lastMsg = currentColumn.messages.Length - 1;

                            if (proceed) {
                                if (msg.isAChoice) {
                                    choices.Add(msg);

                                    if (controlIndexes["message"] < lastMsg) {
                                        controlIndexes["message"] += 1;
                                        Next(false);
                                    }
                                }

                                else if (choices.Count == 0) {
                                    currentMessage = msg;
                                }

                                else if (controlIndexes["message"] < lastMsg) {
                                    controlIndexes["message"] += 1;
                                    Next(false);
                                }

                                else {
                                    choiceMenu = true;
                                }
                            }

                            else if (controlIndexes["message"] == lastMsg) {

                                if (IsLastMessage()) {
                                    EndTalk();
                                }

                                else {
                                    controlIndexes["column"] += 1;
                                    controlIndexes["message"] = 0;
                                    Next(false);
                                }

                            }

                            else {
                                controlIndexes["message"] += 1;
                                Next(false);
                            }
                        }

                        else {
                            Debug.LogWarning("The column with id " + currentColumn.id + " of context " + currentContext.id + " of " + character.name + " is empty.");
                            talking = false;
                        }
                    }
                }

                else {
                    Debug.LogWarning("The current context don't have any column left.");
                    talking = false;
                }

            }

            else {
                Debug.LogError("Unable to found current context, this character don't have contexts or you need to use DiplomataCharacter.StartTalk() method to start the conversation.");
                talking = false;
            }
        }

        public string ShowMessageContentSubtitle() {

            if (talking) {
                if (currentMessage != null) {
                    string newContent = currentMessage.emitter + ":\n";
                    newContent += DictHandler.ContainsKey(currentMessage.content, Diplomata.gameProgress.currentSubtitledLanguage).value;

                    return newContent;
                }

                else {
                    var errorText = "There's no message to show.";
                    Debug.LogError(errorText);
                    talking = false;
                    return errorText;
                }
            }

            Debug.Log("Empty string returned.");
            return string.Empty;
        }

        public bool IsLastMessage() {
            if (controlIndexes["column"] < currentContext.columns.Length - 1) {
                return false;
            }

            else {
                return true;
            }
        }

        public void EndTalk() {
            Debug.Log("Dialogue with " + character.name + " ended.");
            talking = false;
        }

        public void NextMessage() {
            var hasFate = false;

            if (currentMessage != null) {
                lastColumnId = controlIndexes["column"];
                lastMessageId = controlIndexes["message"];

                foreach (Effect effect in currentMessage.effects) {

                    switch (effect.type) {
                        case Effect.Type.EndOfContext:
                            if (effect.endContextId > -1) {
                                foreach (Character characterTemp in Diplomata.characters) {
                                    var ctx = Context.Find(characterTemp, effect.endContextId);

                                    if (ctx != null) {
                                        ctx.happened = true;
                                    }
                                }
                            }

                            break;

                        case Effect.Type.GoTo:
                            if (effect.nextMessageColumnId > -1 && effect.nextMessageId > -1) {
                                controlIndexes["column"] = effect.nextMessageColumnId;
                                controlIndexes["message"] = effect.nextMessageId;
                            }
                            break;
                    }

                    effect.custom.Invoke();
                }
            }

            if (hasFate) {
                Next(true);
            }

            else if (IsLastMessage()) {
                EndTalk();
            }

            else {
                controlIndexes["column"] += 1;
                controlIndexes["message"] = 0;
                Next(false);
            }
        }
        
        public List<string> MessageChoices() {
            List<string> choicesText = new List<string>();

            if (choices.Count > 0) {                
                foreach (Message choice in choices) {
                    choicesText.Add(DictHandler.ContainsKey(choice.title, Diplomata.gameProgress.currentSubtitledLanguage).value);
                }
            }

            else {
                Debug.Log("There's no choice this time.");

                if (IsLastMessage()) {
                    EndTalk();
                }

                else {
                    controlIndexes["column"] += 1;
                    controlIndexes["message"] = 0;
                    Next(false);
                }
            }

            return choicesText;
        }

        public void ChooseMessage(string title) {
            if (currentColumn != null) {
                foreach (Message msg in currentColumn.messages) {
                    foreach (DictLang localTitle in msg.title) {
                        if (title == localTitle.value) {
                            currentMessage = msg;
                        }
                    }
                }

                if (currentMessage != null) {
                    lastColumnId = controlIndexes["column"];
                    lastMessageId = controlIndexes["message"];

                    choiceMenu = false;
                    choices = new List<Message>();
                    SetInfluence();

                    if (IsLastMessage()) {
                        EndTalk();
                    }

                    else {
                        controlIndexes["column"] += 1;
                        controlIndexes["message"] = 0;
                        Next(false);
                    }
                }

                else {
                    Debug.LogError("Unable to found the message with the title \"" + title + "\".");
                    talking = false;
                }
            }

            else {
                Debug.LogError("No column setted.");
                talking = false;
            }
        }

        public void SetInfluence() {
            if (character != null && currentMessage != null) {
                byte max = 0;
                List<byte> min = new List<byte>();

                foreach (DictAttr attrMsg in currentMessage.attributes) {
                    foreach (DictAttr attrChar in character.attributes) {
                        if (attrMsg.key == attrChar.key) {
                            if (attrMsg.value < attrChar.value) {
                                min.Add(attrMsg.value);
                                break;
                            }
                            if (attrMsg.value >= attrChar.value) {
                                min.Add(attrChar.value);
                                break;
                            }
                        }
                    }
                }

                foreach (byte val in min) {
                    if (val > max) {
                        max = val;
                    }
                }

                int tempInfluence = (max + character.influence) / 2;
                character.influence = (byte)tempInfluence;
            }

            else {
                Debug.Log("Cannot set influence, no character attached or message selected.");
            }
        }

        public Message FindMessage(string messageTitle, string language = "English") {
            if (character != null) {
                foreach (Context context in character.contexts) {
                    foreach (Column column in context.columns) {
                        for (int i = 0; i < column.messages.Length; i++) {
                            DictLang title = DictHandler.ContainsKey(Message.Find(column.messages, i).title, language);
                            if (title.value == messageTitle && title != null) {
                                return column.messages[i];
                            }
                        }
                    }
                }

                Debug.LogError("Cannot find the message \"" + messageTitle + "\" in " + language +
                    " in the character " + character.name + " messages. returned null.");
                return null;
            }

            else {
                Debug.LogError("This character doesn't exist. returned null.");
                return null;
            }
        }

        public Message FindMessage(string contextName, int columnId, int id, string language = "English") {
            if (character != null) {
                var context = Context.Find(character, contextName, language);

                foreach (Column col in context.columns) {
                    if (col.id == columnId) {
                        Message.Find(col.messages, id);
                    }
                }

                Debug.LogError("Cannot find the message with the id " + id + " in  the context " + contextName +
                    " in " + language + " in the character " + character.name + " messages. returned null.");
                return null;
            }

            else {
                Debug.LogError("This character doesn't exist. returned null.");
                return null;
            }
        }

        private void OnEnable() {
            if (character != null) {
                character.onScene = true;
            }
        }

        private void OnDisable() {
            if (character != null) {
                character.onScene = false;
            }
        }

        private void OnDestroy() {
            if (character != null) {
                character.onScene = false;
            }
        }
    }

}