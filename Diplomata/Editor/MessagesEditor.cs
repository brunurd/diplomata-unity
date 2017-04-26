using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesEditor {

        public static string[] messageList = new string[0];
        private const byte HEADER_HEIGHT = DGUI.BUTTON_HEIGHT_SMALL + (2 * DGUI.MARGIN);
        private const ushort SIDEBAR_WIDTH = 300;

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
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            
            GUILayout.BeginHorizontal(messagesWindowHeaderStyle, GUILayout.Height(HEADER_HEIGHT));

            if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
            
            GUILayout.EndHorizontal();
        }

        public static void Main() {
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
                column.emitter = DGUI.Popup("Emitter: ", column.emitter, Diplomata.preferences.characterList);
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
                                text += currentMessage.conditions[k].displayName;

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

                        var title = DictHandler.ContainsKey(currentMessage.title, Diplomata.preferences.currentLanguage);

                        if (title == null) {
                            currentMessage.title = ArrayHandler.Add(currentMessage.title, new DictLang(Diplomata.preferences.currentLanguage, currentMessage.columnId + " " + currentMessage.id));
                            title = DictHandler.ContainsKey(currentMessage.title, Diplomata.preferences.currentLanguage);
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

                        var content = DictHandler.ContainsKey(currentMessage.content, Diplomata.preferences.currentLanguage);

                        if (content == null) {
                            currentMessage.content = ArrayHandler.Add(currentMessage.content, new DictLang(Diplomata.preferences.currentLanguage, ""));
                            content = DictHandler.ContainsKey(currentMessage.content, Diplomata.preferences.currentLanguage);
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
                                text += currentMessage.effects[k].displayName;

                                if (k < currentMessage.effects.Length - 1) {
                                    text += "\n\n";
                                }
                            }

                            DGUI.textContent.text = text;
                            height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                            GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                        }
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

                    JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                }

                EditorGUILayout.Separator();
                GUILayout.EndVertical();

                GUILayout.Space(DGUI.MARGIN);
            }

            if (GUILayout.Button("Add Column", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(context.columnWidth))) {
                context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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

            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;

            DGUI.labelStyle.fontSize = 12;
            DGUI.labelStyle.alignment = TextAnchor.UpperCenter;
            DGUI.labelStyle.normal.textColor = Color.black;

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

                    var screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, Diplomata.preferences.currentLanguage);

                    if (screenplayNotes == null) {
                        message.screenplayNotes = ArrayHandler.Add(message.screenplayNotes, new DictLang(Diplomata.preferences.currentLanguage, ""));
                        screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, Diplomata.preferences.currentLanguage);
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
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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

                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
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

                        EditorGUI.BeginChangeCheck();

                        DGUI.labelStyle.fontSize = 11;
                        DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                        GUILayout.Label("<i>Condition " + j + "</i>\n", DGUI.labelStyle);
                        j++;

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Type: ");
                        condition.type = (Condition.Type)EditorGUILayout.EnumPopup(condition.type);
                        GUILayout.EndHorizontal();
                        
                        if (EditorGUI.EndChangeCheck()) {
                            switch (condition.type) {
                                case Condition.Type.None:
                                    condition.DisplayNone();
                                    break;

                                case Condition.Type.AfterOf:
                                    condition.DisplayAfterOf();
                                    break;

                                case Condition.Type.InfluenceEqualTo:
                                case Condition.Type.InfluenceGreaterThan:
                                case Condition.Type.InfluenceLessThan:
                                    condition.DisplayCompareInfluence();
                                    break;
                            }
                        }

                        switch (condition.type) {
                            case Condition.Type.AfterOf:
                                UpdateMessagesList(context);
                                condition.afterOfMessageName = DGUI.Popup("Message: ", condition.afterOfMessageName, messageList);
                                condition.DisplayAfterOf();
                                break;

                            case Condition.Type.InfluenceEqualTo:
                            case Condition.Type.InfluenceGreaterThan:
                            case Condition.Type.InfluenceLessThan:

                                GUILayout.BeginHorizontal();
                                EditorGUI.BeginChangeCheck();

                                if (condition.characterInfluencedName == string.Empty && CharacterMessagesManager.characterList.Length > 0) {
                                    condition.characterInfluencedName = CharacterMessagesManager.characterList[0];
                                }

                                condition.comparedInfluence = EditorGUILayout.IntField(condition.comparedInfluence);
                                condition.characterInfluencedName = DGUI.Popup(" in ", condition.characterInfluencedName, CharacterMessagesManager.characterList);

                                if (EditorGUI.EndChangeCheck()) {
                                    condition.DisplayCompareInfluence();
                                }
                                GUILayout.EndHorizontal();

                                break;
                        }

                        if (GUILayout.Button("Delete Condition", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                            message.conditions = ArrayHandler.Remove(message.conditions, condition);
                        }

                        DGUI.Separator();
                    }

                    if (GUILayout.Button("Add Condition", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        message.conditions = ArrayHandler.Add(message.conditions, new Condition());
                    }

                    break;

                case MessageEditorState.Effects:

                    GUILayout.Label("<b>Effects</b>", DGUI.labelStyle);
                    EditorGUILayout.Separator();

                    if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                        context.messageEditorState = MessageEditorState.Normal;
                    }

                    DGUI.Separator();

                    break;
            }

            if (context.messageEditorState != MessageEditorState.None) {
                DGUI.Separator();
            }

            if (GUILayout.Button("Remove Empty Columns", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                context.columns = Column.RemoveEmptyColumns(context.columns);
                context.messageEditorState = MessageEditorState.None;

                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }

            GUILayout.EndVertical();
        }

        public static void UpdateMessagesList(Context context) {
            messageList = new string[0];

            foreach (Column col in context.columns) {
                foreach (Message msg in col.messages) {
                    DictLang title = DictHandler.ContainsKey(msg.title, Diplomata.preferences.currentLanguage);
                    messageList = ArrayHandler.Add(messageList, title.value);
                }
            }
        }

    }

}