using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesEditor {
        
        private const byte HEADER_HEIGHT = DGUI.BUTTON_HEIGHT_SMALL + (2 * DGUI.MARGIN);
        private const ushort SIDEBAR_WIDTH = 300;

        private static string[] messageList = new string[0];
        private static string[] characterList = new string[0];
        private static string[] contextList = new string[0];
        public static GUIStyle messagesWindowHeaderStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle messagesWindowMainStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle messagesWindowSidebarStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle textAreaStyle = new GUIStyle(DGUI.textAreaStyle);
        public static GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        public static GUIStyle fakeButtonStyle = new GUIStyle(GUI.skin.button);

        private static Vector2 scrollPos = new Vector2(0, 0);
        private static Message message;
        
        public static void Draw() {
            if (CharacterMessagesManager.context != null) {
                messagesWindowHeaderStyle.normal.background = CharacterMessagesManager.headerBG;
                messagesWindowMainStyle.normal.background = CharacterMessagesManager.mainBG;
                messagesWindowSidebarStyle.normal.background = CharacterMessagesManager.sidebarBG;

                Header();

                GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
                Main();
                Sidebar();
                GUILayout.EndHorizontal();
            }

            else {
                CharacterMessagesManager.Init();
            }
        }

        public static void Header() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            
            GUILayout.BeginHorizontal(messagesWindowHeaderStyle, GUILayout.Height(HEADER_HEIGHT));

            if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                diplomataEditor.Save(character);
                CharacterMessagesManager.OpenContextMenu(character);
            }

            EditorGUILayout.Separator();

            GUILayout.Label("Character: " + character.name);

            EditorGUILayout.Separator();

            GUILayout.Label("Zoom: ");
            context.columnWidth = (ushort) EditorGUILayout.Slider(context.columnWidth, 116, 675);
            DGUI.boxStyle.fontSize = (context.columnWidth * 11) / 200;
            DGUI.labelStyle.fontSize = (context.columnWidth * 11) / 200;
            textAreaStyle.fontSize = (context.columnWidth * 11) / 200;
            
            EditorGUILayout.Separator();

            GUILayout.Label("Filters: ");
            context.idFilter = GUILayout.Toggle(context.idFilter, "Id ");
            context.conditionsFilter = GUILayout.Toggle(context.conditionsFilter, "Conditions ");
            context.titleFilter = GUILayout.Toggle(context.titleFilter, "Title ");
            context.contentFilter = GUILayout.Toggle(context.contentFilter, "Content ");
            context.effectsFilter = GUILayout.Toggle(context.effectsFilter, "Effects ");
            
            EditorGUILayout.Separator();

            if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                diplomataEditor.Save(character);
            }
            
            GUILayout.EndHorizontal();
        }

        public static void Main() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            var width = Screen.width - SIDEBAR_WIDTH;
            
            ResetStyle();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, messagesWindowMainStyle, GUILayout.Width(width));
            GUILayout.BeginHorizontal();
            
            for (int i = 0; i < context.columns.Length; i++) {

                Column column = context.columns[i];

                foreach (Column col in context.columns) {
                    if (col.id == i) {
                        column = col;
                        break;
                    }
                }

                GUILayout.BeginVertical(GUILayout.Width(context.columnWidth));

                GUILayout.Space(4);
                column.emitter = DGUI.Popup("Emitter: ", column.emitter, diplomataEditor.preferences.characterList);
                EditorGUILayout.Separator();
                
                for (int j = 0; j < column.messages.Length; j++) {

                    Message currentMessage = column.messages[j];

                    foreach (Message msg in column.messages) {
                        if (msg.id == j) {
                            currentMessage = msg;
                            break;
                        }
                    }

                    #region MESSAGE CARD
                    Rect boxRect = EditorGUILayout.BeginVertical(DGUI.boxStyle);

                    var color = currentMessage.color;
                    DGUI.strokeWidth = 1;

                    if (context.currentMessage.columnId != -1 && context.currentMessage.rowId != -1) {
                        if (context.currentMessage.columnId == currentMessage.columnId && context.currentMessage.rowId == currentMessage.id) {
                            color = DGUI.ColorAdd(currentMessage.color, 0.1f);
                            message = currentMessage;
                            DGUI.strokeWidth = 3;
                        }
                    }

                    DGUI.labelStyle.normal.textColor = Color.black;
                    textAreaStyle.normal.textColor = Color.black;

                    if (color.r * color.g * color.b < 0.07f) {
                        DGUI.labelStyle.normal.textColor = Color.white;
                        textAreaStyle.normal.textColor = Color.white;
                    }
                    
                    DGUI.DrawRectStroke(boxRect, color);
                    EditorGUI.DrawRect(boxRect, color);
                    
                    string text = string.Empty;
                    float height = 0;

                    DGUI.labelStyle.alignment = TextAnchor.UpperLeft;

                    if (context.idFilter) {
                        text += "<i>[" + currentMessage.columnId + " " + currentMessage.id + "]</i>";
                        DGUI.textContent.text = text;
                        height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                        GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                    }
                    
                    if (context.conditionsFilter) {
                        if (currentMessage.conditions.Length > 0) {
                            text = "<b><i>Conditions:</i></b>\n\n";
                            
                            for (int k = 0; k < currentMessage.conditions.Length; k++) {
                                var condition = currentMessage.conditions[k];

                                switch (condition.type) {
                                    case Condition.Type.None:
                                        text += condition.DisplayNone();
                                        break;
                                    case Condition.Type.AfterOf:
                                        text += condition.DisplayAfterOf(DictHandler.ContainsKey(condition.afterOf.GetMessage(context).title, diplomataEditor.preferences.currentLanguage).value);
                                        break;

                                    case Condition.Type.InfluenceEqualTo:
                                    case Condition.Type.InfluenceGreaterThan:
                                    case Condition.Type.InfluenceLessThan:
                                        text += condition.DisplayCompareInfluence();
                                        break;
                                }

                                if (k < currentMessage.conditions.Length - 1) {
                                    text += "\n\n";
                                }
                            }

                            DGUI.textContent.text = text;
                            height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                            GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                        }
                    }

                    if (context.titleFilter && (!currentMessage.disposable || currentMessage.isAChoice)) {
                        text = "<b><i>Title:</i></b>";
                        
                        DGUI.textContent.text = text;
                        height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                        GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));

                        var title = DictHandler.ContainsKey(currentMessage.title, diplomataEditor.preferences.currentLanguage);

                        if (title == null) {
                            currentMessage.title = ArrayHandler.Add(currentMessage.title, 
                                new DictLang(diplomataEditor.preferences.currentLanguage, currentMessage.columnId + " " + currentMessage.id));
                            title = DictHandler.ContainsKey(currentMessage.title, diplomataEditor.preferences.currentLanguage);
                        }

                        DGUI.textContent.text = title.value;
                        height = textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth);

                        GUI.SetNextControlName("title" + currentMessage.id);
                        title.value = EditorGUILayout.TextArea(title.value, textAreaStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                        EditorGUILayout.Separator();
                    }
                    
                    if (context.contentFilter) {
                        DGUI.textContent.text = "<b><i>Content:</i></b>";
                        height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                        GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));

                        var content = DictHandler.ContainsKey(currentMessage.content, diplomataEditor.preferences.currentLanguage);

                        if (content == null) {
                            currentMessage.content = ArrayHandler.Add(currentMessage.content, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
                            content = DictHandler.ContainsKey(currentMessage.content, diplomataEditor.preferences.currentLanguage);
                        }

                        DGUI.textContent.text = content.value;
                        height = textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth);

                        GUI.SetNextControlName("content" + currentMessage.id);
                        content.value = EditorGUILayout.TextArea(content.value, textAreaStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                        EditorGUILayout.Separator();
                    }

                    if (context.effectsFilter) {
                        if (currentMessage.effects.Length > 0) {
                            text = "<b><i>Effects:</i></b>\n\n";
                            
                            for (int k = 0; k < currentMessage.effects.Length; k++) {
                                var effect = currentMessage.effects[k];

                                switch (effect.type) {
                                    case Effect.Type.None:
                                        text += effect.DisplayNone();
                                        break;

                                    case Effect.Type.EndOfContext:
                                        if (effect.endOfContext.characterName != null) {
                                            text += effect.DisplayEndOfContext(DictHandler.ContainsKey(effect.endOfContext.GetContext(diplomataEditor.characters).name,
                                                diplomataEditor.preferences.currentLanguage).value);
                                        }
                                        break;

                                    case Effect.Type.GoTo:
                                        if (effect.goTo.GetMessage(context) != null) {
                                            text += effect.DisplayGoTo(DictHandler.ContainsKey(effect.goTo.GetMessage(context).title, diplomataEditor.preferences.currentLanguage).value);
                                        }
                                        break;
                                }

                                if (k < currentMessage.effects.Length - 1) {
                                    text += "\n\n";
                                }
                            }

                            DGUI.textContent.text = text;
                            height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                            GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                        }
                    }

                    if (currentMessage.disposable || currentMessage.isAChoice) {
                        DGUI.labelStyle.fontSize = (context.columnWidth * 9) / 200;
                        DGUI.labelStyle.alignment = TextAnchor.UpperRight;
                        DGUI.labelStyle.normal.textColor = DGUI.ColorSub(DGUI.labelStyle.normal.textColor, 0, 0.5f);

                        text = string.Empty;

                        if (currentMessage.disposable) {
                            text += "[ disposable ] ";
                        }

                        if (currentMessage.isAChoice) {
                            text += "[ is a choice ]";
                        }

                        DGUI.textContent.text = text;
                        height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                        GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));

                        DGUI.labelStyle.fontSize = (context.columnWidth * 11) / 200;
                        DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                    }

                    if (GUI.GetNameOfFocusedControl() == "title" + currentMessage.id || 
                        GUI.GetNameOfFocusedControl() == "content" + currentMessage.id) {
                        SetMessage(currentMessage);
                    }

                    if (GUI.Button(boxRect, "", buttonStyle)) {
                        SetMessage(currentMessage);
                        EditorGUI.FocusTextInControl("");
                    }

                    EditorGUILayout.EndVertical();
                    EditorGUILayout.Separator();
                    #endregion
                }

                if (GUILayout.Button("Add Message", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    column.messages = ArrayHandler.Add(column.messages, new Message(column.messages.Length, column.emitter, column.id));

                    SetMessage(null);

                    diplomataEditor.Save(character);
                }

                EditorGUILayout.Separator();
                GUILayout.EndVertical();

                GUILayout.Space(DGUI.MARGIN);
            }

            if (GUILayout.Button("Add Column", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(context.columnWidth))) {
                context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));
                diplomataEditor.Save(character);
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

            DGUI.labelStyle.padding = DGUI.zeroPadding;
        }

        public static void SetMessage(Message msg) {
            if (msg == null) {
                CharacterMessagesManager.context.messageEditorState = MessageEditorState.None;
                CharacterMessagesManager.context.currentMessage.Set(-1, -1);
            }

            else {
                CharacterMessagesManager.context.messageEditorState = MessageEditorState.Normal;
                CharacterMessagesManager.context.currentMessage.Set(msg.columnId, msg.id);
                message = msg;
            }
        }

        public static void ResetStyle() {
            textAreaStyle.normal.background = CharacterMessagesManager.textAreaBGTextureNormal;
            textAreaStyle.focused.background = CharacterMessagesManager.textAreaBGTextureFocused;
            buttonStyle.normal.background = DGUI.transparentTexture;
            buttonStyle.active.background = DGUI.transparentTexture;
            DGUI.labelStyle.padding = DGUI.padding;
        }

        public static void Sidebar() {
            GUILayout.BeginVertical(messagesWindowSidebarStyle, GUILayout.Width(SIDEBAR_WIDTH), GUILayout.ExpandHeight(true));

            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            
            DGUI.labelStyle.fontSize = 12;
            DGUI.labelStyle.alignment = TextAnchor.UpperCenter;
            DGUI.labelStyle.normal.textColor = Color.black;

            if (message != null) {

                switch (context.messageEditorState) {

                    case MessageEditorState.Normal:

                        GUILayout.Label("<b>Properties</b>", DGUI.labelStyle);
                        EditorGUILayout.Separator();

                        var column = Column.Find(context, message.columnId);

                        GUILayout.Label("Message Color:");
                        message.color = EditorGUILayout.ColorField(message.color);

                        EditorGUILayout.Separator();

                        var disposable = message.disposable;
                        var isAChoice = message.isAChoice;

                        GUILayout.BeginHorizontal();
                        message.disposable = GUILayout.Toggle(message.disposable, "Disposable");
                        message.isAChoice = GUILayout.Toggle(message.isAChoice, "Is a choice");
                        GUILayout.EndHorizontal();

                        if (message.disposable != disposable || message.isAChoice != isAChoice) {
                            EditorGUI.FocusTextInControl("");
                        }

                        if (message.isAChoice) {

                            EditorGUILayout.Separator();

                            GUILayout.Label("Message attributes (most influence in): ");

                            for (int i = 0; i < character.attributes.Length; i++) {
                                message.attributes[i].value = (byte)EditorGUILayout.Slider(message.attributes[i].key, message.attributes[i].value, 0, 100);
                            }

                        }

                        EditorGUILayout.Separator();

                        var screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, diplomataEditor.preferences.currentLanguage);

                        if (screenplayNotes == null) {
                            message.screenplayNotes = ArrayHandler.Add(message.screenplayNotes, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
                            screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, diplomataEditor.preferences.currentLanguage);
                        }

                        DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                        GUILayout.Label("Screenplay notes:\n<size=10>(Example: <i>whispering and gasping</i>)</size>", DGUI.labelStyle);
                        screenplayNotes.value = EditorGUILayout.TextField(screenplayNotes.value);

                        EditorGUILayout.Separator();

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("Edit Conditions", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Conditions;
                        }

                        if (GUILayout.Button("Edit Effects", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Effects;
                        }

                        GUILayout.EndHorizontal();

                        EditorGUILayout.Separator();

                        if (column.messages.Length > 1 || context.columns.Length > 1) {
                            DGUI.Separator();
                            GUILayout.Label("Move: ");
                        }

                        fakeButtonStyle.richText = true;
                        string color = "#989898";
                        GUILayout.BeginHorizontal();

                        if (column.id > 0) {
                            if (GUILayout.Button("Left", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                message.columnId -= 1;

                                var leftCol = Column.Find(context, message.columnId);

                                message.id = leftCol.messages.Length;

                                leftCol.messages = ArrayHandler.Add(leftCol.messages, message);
                                column.messages = ArrayHandler.Remove(column.messages, message);

                                Message.ResetIDs(column.messages);
                                Message.ResetIDs(leftCol.messages);

                                message.emitter = leftCol.emitter;

                                SetMessage(message);
                                diplomataEditor.Save(character);
                            }
                        }

                        else {
                            GUILayout.Box("<color=" + color + ">Left</color>", fakeButtonStyle, GUILayout.Height(DGUI.BUTTON_HEIGHT));
                        }

                        if (message.id > 0) {
                            if (GUILayout.Button("Up", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                Message.Find(column.messages, message.id - 1).id += 1;

                                message.id -= 1;

                                SetMessage(message);
                                diplomataEditor.Save(character);
                            }
                        }

                        else {
                            GUILayout.Box("<color=" + color + ">Up</color>", fakeButtonStyle, GUILayout.Height(DGUI.BUTTON_HEIGHT));
                        }

                        if (message.id < column.messages.Length - 1) {
                            if (GUILayout.Button("Down", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                Message.Find(column.messages, message.id + 1).id -= 1;

                                message.id += 1;

                                SetMessage(message);
                                diplomataEditor.Save(character);
                            }
                        }

                        else {
                            GUILayout.Box("<color=" + color + ">Down</color>", fakeButtonStyle, GUILayout.Height(DGUI.BUTTON_HEIGHT));
                        }

                        if (column.id < context.columns.Length - 1) {
                            if (GUILayout.Button("Right", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                message.columnId += 1;

                                var rightCol = Column.Find(context, message.columnId);

                                message.id = rightCol.messages.Length;

                                rightCol.messages = ArrayHandler.Add(rightCol.messages, message);
                                column.messages = ArrayHandler.Remove(column.messages, message);

                                Message.ResetIDs(column.messages);
                                Message.ResetIDs(rightCol.messages);

                                message.emitter = rightCol.emitter;

                                SetMessage(message);
                                diplomataEditor.Save(character);
                            }
                        }

                        else {
                            GUILayout.Box("<color=" + color + ">Right</color>", fakeButtonStyle, GUILayout.Height(DGUI.BUTTON_HEIGHT));
                        }

                        GUILayout.EndHorizontal();

                        if (column.messages.Length > 1 || context.columns.Length > 1) {
                            DGUI.Separator();
                        }

                        if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                            if (EditorUtility.DisplayDialog("Are you sure?", "If you agree all this message data will be lost forever.", "Yes", "No")) {

                                column.messages = ArrayHandler.Remove(column.messages, message);

                                SetMessage(null);

                                Message.ResetIDs(column.messages);

                                diplomataEditor.Save(character);
                            }
                        }

                        break;

                    case MessageEditorState.Conditions:

                        GUILayout.Label("<b>Conditions</b>", DGUI.labelStyle);
                        EditorGUILayout.Separator();

                        if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Normal;
                        }

                        DGUI.Separator();

                        var j = 0;

                        foreach (Condition condition in message.conditions) {
                            
                            DGUI.labelStyle.fontSize = 11;
                            DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                            GUILayout.Label("<i>Condition " + j + "</i>\n", DGUI.labelStyle);
                            j++;

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Type: ");
                            condition.type = (Condition.Type)EditorGUILayout.EnumPopup(condition.type);
                            GUILayout.EndHorizontal();
                            
                            switch (condition.type) {
                                case Condition.Type.AfterOf:
                                    UpdateMessagesList(context);

                                    var messageName = "";

                                    if (condition.afterOf.GetMessage(context) != null) {
                                        messageName = DictHandler.ContainsKey(condition.afterOf.GetMessage(context).title, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    messageName = DGUI.Popup("Message: ", messageName, messageList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Column col in context.columns) {
                                            foreach (Message msg in col.messages) {

                                                if (DictHandler.ContainsKey(msg.title, diplomataEditor.preferences.currentLanguage).value == messageName) {
                                                    condition.afterOf.Set(msg.columnId, msg.id);
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    break;

                                case Condition.Type.InfluenceEqualTo:
                                case Condition.Type.InfluenceGreaterThan:
                                case Condition.Type.InfluenceLessThan:
                                    UpdateCharacterList();

                                    GUILayout.BeginHorizontal();
                                    EditorGUI.BeginChangeCheck();

                                    if (condition.characterInfluencedName == string.Empty && characterList.Length > 0) {
                                        condition.characterInfluencedName = characterList[0];
                                    }

                                    condition.comparedInfluence = EditorGUILayout.IntField(condition.comparedInfluence);
                                    condition.characterInfluencedName = DGUI.Popup(" in ", condition.characterInfluencedName, characterList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        condition.DisplayCompareInfluence();
                                    }
                                    GUILayout.EndHorizontal();

                                    break;
                            }

                            if (GUILayout.Button("Delete Condition", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                message.conditions = ArrayHandler.Remove(message.conditions, condition);
                                diplomataEditor.Save(character);
                            }

                            DGUI.Separator();
                        }

                        if (GUILayout.Button("Add Condition", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            message.conditions = ArrayHandler.Add(message.conditions, new Condition());
                            diplomataEditor.Save(character);
                        }

                        break;

                    case MessageEditorState.Effects:

                        GUILayout.Label("<b>Effects</b>", DGUI.labelStyle);
                        EditorGUILayout.Separator();

                        if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Normal;
                        }

                        DGUI.Separator();

                        var k = 0;

                        foreach (Effect effect in message.effects) {

                            DGUI.labelStyle.fontSize = 11;
                            DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                            GUILayout.Label("<i>Effect " + k + "</i>\n", DGUI.labelStyle);
                            k++;

                            GUILayout.BeginHorizontal();
                            GUILayout.Label("Type: ");
                            effect.type = (Effect.Type)EditorGUILayout.EnumPopup(effect.type);
                            GUILayout.EndHorizontal();

                            switch (effect.type) {
                                case Effect.Type.EndOfContext:

                                    UpdateContextList();
                                    var contextName = string.Empty;

                                    if (effect.endOfContext.characterName != null) {
                                        if (effect.endOfContext.GetContext(diplomataEditor.characters) != null) {
                                            contextName = DictHandler.ContainsKey(effect.endOfContext.GetContext(diplomataEditor.characters).name,
                                                diplomataEditor.preferences.currentLanguage).value;
                                        }
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    contextName = DGUI.Popup("Context: ", contextName, contextList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Character tempCharacter in diplomataEditor.characters) {
                                            foreach (Context ctx in tempCharacter.contexts) {

                                                if (DictHandler.ContainsKey(ctx.name, diplomataEditor.preferences.currentLanguage).value == contextName) {
                                                    effect.endOfContext.Set(ctx.characterName, ctx.id);
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    break;

                                case Effect.Type.GoTo:
                                    UpdateMessagesList(context);
                                    var messageTitle = string.Empty;

                                    if (effect.goTo.GetMessage(context) != null) {
                                        messageTitle = DictHandler.ContainsKey(effect.goTo.GetMessage(context).title, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    messageTitle = DGUI.Popup("Message: ", messageTitle, messageList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Column col in context.columns) {
                                            foreach (Message msg in col.messages) {

                                                if (DictHandler.ContainsKey(msg.title, diplomataEditor.preferences.currentLanguage).value == messageTitle) {
                                                    effect.goTo.Set(msg.columnId, msg.id);
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    break;
                            }

                            if (GUILayout.Button("Delete Effect", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                message.effects = ArrayHandler.Remove(message.effects, effect);
                                diplomataEditor.Save(character);
                            }

                            DGUI.Separator();
                        }

                        if (GUILayout.Button("Add Effect", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            message.effects = ArrayHandler.Add(message.effects, new Effect());
                            diplomataEditor.Save(character);
                        }

                        break;
                }

            }

            if (context.messageEditorState != MessageEditorState.None) {
                DGUI.Separator();
            }

            if (GUILayout.Button("Remove Empty Columns", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                context.columns = Column.RemoveEmptyColumns(context.columns);
                context.messageEditorState = MessageEditorState.None;

                diplomataEditor.Save(character);
            }

            GUILayout.EndVertical();
        }

        public static void UpdateCharacterList() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            characterList = new string[diplomataEditor.preferences.characterList.Length - 1];

            foreach (string str in diplomataEditor.preferences.characterList) {
                if (str != diplomataEditor.preferences.playerCharacterName) {
                    characterList = ArrayHandler.Add(characterList, str);
                }
            }
        }

        public static void UpdateMessagesList(Context context) {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            messageList = new string[0];

            foreach (Column col in context.columns) {
                foreach (Message msg in col.messages) {
                    DictLang title = DictHandler.ContainsKey(msg.title, diplomataEditor.preferences.currentLanguage);

                    if (title == null) {
                        msg.title = ArrayHandler.Add(msg.title, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
                        title = DictHandler.ContainsKey(msg.title, diplomataEditor.preferences.currentLanguage);
                    }

                    messageList = ArrayHandler.Add(messageList, title.value);
                }
            }
        }

        public static void UpdateContextList() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            contextList = new string[0];

            foreach (Character character in diplomataEditor.characters) {
                foreach (Context context in character.contexts) {
                    DictLang contextName = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);

                    if (contextName == null) {
                        context.name = ArrayHandler.Add(context.name, new DictLang(diplomataEditor.preferences.currentLanguage, "Name [Change clicking on Edit]"));
                        contextName = DictHandler.ContainsKey(context.name, diplomataEditor.preferences.currentLanguage);
                    }

                    contextList = ArrayHandler.Add(contextList, contextName.value);
                }
            }
        }

    }

}