using System.Collections.Generic;
using UnityEngine;

namespace DiplomataLib {
    
    [System.Serializable]
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    public class DiplomataCharacter : MonoBehaviour {
        
        public List<Message> choices = new List<Message>();
        public Message currentMessage;
        public Character character;
        public bool talking;
        public bool choiceMenu;

        private Dictionary<string, int> controlIndexes = new Dictionary<string, int>();
        private Context currentContext;
        private Column currentColumn;
        private int lastMessageId;
        private int lastColumnId;
        
        public void Start() {
            controlIndexes = new Dictionary<string, int>();
            controlIndexes.Add("context", 0);
            controlIndexes.Add("column", 0);
            controlIndexes.Add("message", 0);
            
            if (character != null && Application.isPlaying) {
                character = Character.Find(Diplomata.characters, character.name);
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

                if (!currentContext.happened) {

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
                            OnStartCallbacks();
                        }

                        else {
                            var msg = Message.Find(currentColumn.messages, controlIndexes["message"]);
                            var proceed = true;

                            if (msg != null) {

                                if (msg.alreadySpoked && msg.disposable) {
                                    controlIndexes["message"] += 1;
                                    Next(false);
                                }

                                else {
                                    if (msg.conditions.Length > 0) {

                                        foreach (Condition condition in msg.conditions) {

                                            switch (condition.type) {
                                                case Condition.Type.AfterOf:
                                                    if (condition.afterOf.messageId == lastMessageId && condition.afterOf.columnId == lastColumnId) {
                                                        condition.proceed = true;
                                                    }

                                                    else {
                                                        condition.proceed = false;
                                                    }

                                                    break;

                                                case Condition.Type.InfluenceEqualTo:
                                                    if (character.influence == condition.comparedInfluence) {
                                                        condition.proceed = true;
                                                    }

                                                    else {
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
                                                case Condition.Type.HasItem:
                                                    var item = Item.Find(Diplomata.inventory.items, condition.itemId);

                                                    if (item != null) {

                                                        if (item.have) {
                                                            condition.proceed = true;
                                                        }

                                                        else {
                                                            condition.proceed = false;
                                                        }

                                                    }

                                                    else {
                                                        Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                                                        condition.proceed = false;
                                                    }

                                                    break;
                                                case Condition.Type.ItemWasDiscarded:
                                                    var discardedItem = Item.Find(Diplomata.inventory.items, condition.itemId);

                                                    if (discardedItem != null) {

                                                        if (discardedItem.discarded) {
                                                            condition.proceed = true;
                                                        }

                                                        else {
                                                            condition.proceed = false;
                                                        }

                                                    }

                                                    else {
                                                        Debug.LogWarning("Cannot find the item with id " + condition.itemId + " to check.");
                                                        condition.proceed = false;
                                                    }

                                                    break;
                                                case Condition.Type.CustomFlagEqualTo:
                                                    var flag = Diplomata.customFlags.Find(condition.customFlag.name);

                                                    if (flag != null) {

                                                        if (flag.value == condition.customFlag.value) {
                                                            condition.proceed = true;
                                                        }

                                                        else {
                                                            condition.proceed = false;
                                                        }

                                                    }

                                                    else {
                                                        Debug.LogWarning("Cannot find the custom flag " + condition.customFlag.name);
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
                                            choiceMenu = true;
                                            choices.Add(msg);

                                            if (controlIndexes["message"] < lastMsg) {
                                                controlIndexes["message"] += 1;
                                                Next(false);
                                            }
                                        }

                                        else if (choices.Count == 0) {
                                            currentMessage = msg;
                                            OnStartCallbacks();
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
                    EndTalk();
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
                    var talkLog = TalkLog.Find(Diplomata.gameProgress.talkLog, character.name);

                    if (talkLog == null) {
                        Diplomata.gameProgress.talkLog = ArrayHandler.Add(Diplomata.gameProgress.talkLog, new TalkLog(character.name));
                        talkLog = TalkLog.Find(Diplomata.gameProgress.talkLog, character.name);
                    }

                    talkLog.messagesIds = ArrayHandler.Add(talkLog.messagesIds, (uint) currentMessage.id);

                    return DictHandler.ContainsKey(currentMessage.content, Diplomata.gameProgress.options.currentSubtitledLanguage).value;
                }

                else {
                    var errorText = "Current message to show is not setted.";
                    Debug.LogError(errorText);
                    talking = false;
                    return errorText;
                }
            }

            Debug.Log("Empty string returned.");
            return string.Empty;
        }

        public void PlayMessageAudioContent() {
            if (currentMessage != null) {
                var audioClipPath = DictHandler.ContainsKey(currentMessage.audioClipPath, Diplomata.gameProgress.options.currentDubbedLanguage);

                if (audioClipPath.value != string.Empty) {
                    if (currentMessage.audioClip == null) {
                        currentMessage.audioClip = (AudioClip) Resources.Load(audioClipPath.value);

                        if (currentMessage.audioClip == null) {
                            Debug.LogError("This audio clip doesn't exist or is not in the Resources folder.");
                        }
                    }

                    if (currentMessage.audioClip != null) {
                        var audioSource = GetComponent<AudioSource>();

                        if (audioSource != null) {
                            audioSource.PlayOneShot(currentMessage.audioClip, Diplomata.gameProgress.options.volumeScale);
                        }

                        else {
                            Debug.LogWarning("You have a audio clip in this message, but the game object don't have a AudioSource.");
                        }
                    }
                }
            }
        }

        public void StopMessageAudioContent() {
            if (currentMessage != null) {
                var audioClipPath = DictHandler.ContainsKey(currentMessage.audioClipPath, Diplomata.gameProgress.options.currentDubbedLanguage);

                if (audioClipPath.value != string.Empty) {

                    var audioSource = GetComponent<AudioSource>();

                    if (audioSource != null) {
                        audioSource.Stop();
                    }

                    else {
                        Debug.LogWarning("You have a audio clip in this message, but the game object don't have a AudioSource.");
                    }
                }
            }
        }

        public void SetAnimatorAttributes() {
            if (currentMessage != null) {
                if (currentMessage.animatorAttributesSetters.Length > 0) {
                    var animator = GetComponent<Animator>();

                    if (animator != null) {

                        foreach (AnimatorAttributeSetter animatorAttribute in currentMessage.animatorAttributesSetters) {
                            switch (animatorAttribute.type) {

                                case AnimatorControllerParameterType.Bool:
                                    animator.SetBool(animatorAttribute.name, animatorAttribute.setBool);
                                    break;

                                case AnimatorControllerParameterType.Float:
                                    animator.SetFloat(animatorAttribute.name, animatorAttribute.setFloat);
                                    break;

                                case AnimatorControllerParameterType.Int:
                                    animator.SetInteger(animatorAttribute.name, animatorAttribute.setInt);
                                    break;

                                case AnimatorControllerParameterType.Trigger:
                                    animator.SetTrigger(animatorAttribute.name);
                                    break;

                            }
                        }

                    }

                    else {
                        Debug.LogWarning("You have animation attributes in this message, but the game object don't have a Animator.");
                    }
                }
            }
        }

        public void ResetAnimator() {
            var animator = GetComponent<Animator>();

            if (animator != null) {
                animator.Rebind();
            }
                    
            else {
                Debug.LogWarning("This game object don't have a Animator.");
            }
        }

        public void SwapStaticSprite(Vector2 pivot) {
            if (currentMessage != null) {
                if (currentMessage.imagePath != string.Empty) {
                    var spriteRenderer = GetComponent<SpriteRenderer>();

                    if (spriteRenderer != null) {
                        spriteRenderer.sprite = currentMessage.GetSprite(pivot);
                    }

                    else {
                        Debug.LogWarning("You have a static image in this message, but the game object don't have a Sprite Renderer.");
                    }
                }
            }
        }

        public void SwapStaticSprite() {
            SwapStaticSprite(new Vector2(0.5f, 0.5f));
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
            talking = false;
        }

        public void NextMessage() {
            var hasFate = false;
            
            if (currentMessage != null) {

                controlIndexes["column"] = currentMessage.columnId;
                controlIndexes["message"] = currentMessage.id;
                
                lastColumnId = controlIndexes["column"];
                lastMessageId = controlIndexes["message"];

                foreach (Effect effect in currentMessage.effects) {

                    switch (effect.type) {
                        case Effect.Type.EndOfContext:
                            effect.endOfContext.GetContext(Diplomata.characters).happened = true;

                            if (currentContext.characterName == effect.endOfContext.characterName && currentContext.id == effect.endOfContext.contextId) {
                                currentContext.happened = true;
                            }

                            break;

                        case Effect.Type.GoTo:
                            controlIndexes["column"] = effect.goTo.columnId;
                            controlIndexes["message"] = effect.goTo.messageId;
                            hasFate = true;
                            break;

                        case Effect.Type.SetAnimatorAttribute:
                            var animator = GetComponent<Animator>();

                            if (animator != null) {
                                switch (effect.animatorAttributeSetter.type) {

                                    case AnimatorControllerParameterType.Bool:
                                        animator.SetBool(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setBool);
                                        break;

                                    case AnimatorControllerParameterType.Float:
                                        animator.SetFloat(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setFloat);
                                        break;

                                    case AnimatorControllerParameterType.Int:
                                        animator.SetInteger(effect.animatorAttributeSetter.name, effect.animatorAttributeSetter.setInt);
                                        break;

                                    case AnimatorControllerParameterType.Trigger:
                                        animator.SetTrigger(effect.animatorAttributeSetter.name);
                                        break;

                                }
                            }

                            else {
                                Debug.LogWarning("You have a animation attributes setter effect in this message, but the game object don't have a Animator.");
                            }

                            break;

                        case Effect.Type.GetItem:
                            var getItem = Item.Find(Diplomata.inventory.items, effect.itemId);

                            if (getItem != null) {
                                getItem.have = true;
                            }

                            else {
                                Debug.LogError("Cannot find the item with id " + effect.itemId + " to get.");
                            }

                            break;

                        case Effect.Type.DiscardItem:
                            var discardItem = Item.Find(Diplomata.inventory.items, effect.itemId);

                            if (discardItem != null) {
                                discardItem.discarded = true;
                            }

                            else {
                                Debug.LogError("Cannot find the item with id " + effect.itemId + " to discard.");
                            }

                            break;

                        case Effect.Type.SetCustomFlag:
                            var flag = Diplomata.customFlags.Find(effect.customFlag.name);

                            if (flag != null) {
                                flag.value = effect.customFlag.value;
                            }

                            else {
                                Debug.LogError("Cannot find the custom flag " + effect.customFlag.name);
                            }

                            break;
                    }

                    effect.onComplete.Invoke();
                }

                if (currentMessage.disposable) {
                    currentMessage.alreadySpoked = true;
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
                    choicesText.Add(DictHandler.ContainsKey(choice.title, Diplomata.gameProgress.options.currentSubtitledLanguage).value);
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

        public void OnStartCallbacks() {
            if (currentMessage != null) {
                foreach (Effect effect in currentMessage.effects) {
                    effect.onStart.Invoke();
                }
            }
        }

        public void ChooseMessage(string title) {
            if (currentColumn != null) {
                foreach (Message msg in choices) {
                    var localTitle = DictHandler.ContainsKey(msg.title, Diplomata.gameProgress.options.currentSubtitledLanguage).value;
                    
                    if (localTitle == title) {
                        currentMessage = msg;
                        OnStartCallbacks();
                        break;
                    }
                }

                if (currentMessage != null) {
                    choiceMenu = false;
                    choices = new List<Message>();
                    character.influence = SetInfluence();
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

        public byte SetInfluence() {
            if (currentMessage != null) {
                byte max = 0;
                List<byte> min = new List<byte>();

                foreach (DictAttr attrMsg in currentMessage.attributes) {
                    foreach (DictAttr attrChar in character.attributes) {
                        if (attrMsg.key == attrChar.key) {
                            if (attrMsg.value < attrChar.value) {
                                min.Add(attrMsg.value);
                                break;
                            }
                            else if (attrMsg.value >= attrChar.value) {
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
                return (byte)tempInfluence;
            }

            else {
                Debug.Log("Cannot set influence, no character attached or message selected.");
                return 50;
            }
        }

        public bool EmitterIsPlayer() {
            if (currentMessage != null) {
                if (currentMessage.emitter == Diplomata.preferences.playerCharacterName) {
                    return true;
                }
            }

            return false;
        }

        public string PlayerName() {
            return Diplomata.preferences.playerCharacterName;
        }

        public string Emitter() {
            if (currentColumn != null) {
                return currentColumn.emitter;
            }

            else if (currentMessage != null) {
                return currentMessage.emitter;
            }

            return null;
        }

        public Message FindMessage(int contextId, int columnId, int messageId) {
            return Message.Find(Column.Find(Context.Find(character, contextId), columnId).messages, messageId);
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