using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesEditor {

        private const byte HEADER_HEIGHT = DGUI.BUTTON_HEIGHT_SMALL + (2 * DGUI.MARGIN);
        private const ushort SIDEBAR_WIDTH = 300;
        private const byte EMITTER_FIELD_HEIGHT = 15;
        
        public static Color baseColor = new Color(0.8705f, 0.8705f, 0.8705f);
        private static Color headerBGColor;
        private static Color mainBGColor;
        private static Color sidebarBGColor;

        private static Rect headerRect = new Rect(0, 0, 0, HEADER_HEIGHT);
        private static Rect mainRect = new Rect(0, HEADER_HEIGHT, 0, 0);
        private static Rect sidebarRect = new Rect(0, HEADER_HEIGHT, SIDEBAR_WIDTH, 0);

        private static Rect mainContentRect = new Rect(0, 0, 0, 0);
        private static Vector2 scrollPos = new Vector2(0, 0);
        private static float mainMaxHeight = 0;
        private static Message message;

        private static Rect titleRect = new Rect(0, 0, 0, 0);
        private static Rect contentRect = new Rect(0, 0, 0, 0);
        private static Rect emitterFieldRect = new Rect(0, DGUI.MARGIN, 0, EMITTER_FIELD_HEIGHT);
        public static GUIStyle textAreaStyle = new GUIStyle(GUI.skin.textArea);
        public static GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        private static Texture2D textAreaBGTextureNormal = DGUI.UniformColorTexture(1, 1, new Color(0.5f, 0.5f, 0.5f, 0.06f));
        private static Texture2D textAreaBGTextureFocused = DGUI.UniformColorTexture(1, 1, new Color(1, 1, 1, 1));
        private static RectOffset textAreaPadding = new RectOffset(2, 2, 2, 2);

        public static void Draw() {
            DrawBG();

            if (CharacterMessagesManager.context != null) {
                Header();
                Main();
                Sidebar();
            }

            else {
                CharacterMessagesManager.Init();
            }
        }

        public static void DrawBG() {
            if (EditorGUIUtility.isProSkin) {
                baseColor = DGUI.proBGColor;
            }

            headerBGColor = DGUI.ColorAdd(baseColor, 0.05f, 0.05f, 0.05f);
            mainBGColor = baseColor;
            sidebarBGColor = DGUI.ColorAdd(baseColor, -0.1f, -0.1f, -0.1f);
            
            headerRect.width = Screen.width;

            mainRect.width = Screen.width - SIDEBAR_WIDTH;
            mainRect.height = Screen.height - HEADER_HEIGHT - 22;
            
            sidebarRect.x = Screen.width - SIDEBAR_WIDTH;
            sidebarRect.height = Screen.height - HEADER_HEIGHT;

            EditorGUI.DrawRect(headerRect, headerBGColor);
            EditorGUI.DrawRect(mainRect, mainBGColor);
            EditorGUI.DrawRect(sidebarRect, sidebarBGColor);
        }

        public static void Header() {
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
                
            DGUI.Vertical(() => {

                GUILayout.Space(DGUI.MARGIN);

                DGUI.Horizontal(() => {

                    GUILayout.Space(DGUI.MARGIN);

                    if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                        CharacterMessagesManager.OpenContextMenu(character);
                    }

                    GUILayout.Space(DGUI.MARGIN);
                    GUILayout.Label("Character: " + character.name);

                    GUILayout.Space(DGUI.MARGIN);
                    GUILayout.Label("Zoom: ");
                    context.columnWidth = (ushort) EditorGUILayout.Slider(context.columnWidth, 116, 675);
                    DGUI.boxStyle.fontSize = (context.columnWidth * 11) / 200;
                    textAreaStyle.fontSize = (context.columnWidth * 11) / 200;
                    DGUI.padding = (context.columnWidth * 10) / 200;
                    
                    GUILayout.Space(DGUI.MARGIN);
                    GUILayout.Label("Filters: ");
                    context.idFilter = GUILayout.Toggle(context.idFilter, "Id ");
                    context.conditionsFilter = GUILayout.Toggle(context.conditionsFilter, "Conditions ");
                    context.titleFilter = GUILayout.Toggle(context.titleFilter, "Title ");
                    context.contentFilter = GUILayout.Toggle(context.contentFilter, "Content ");
                    context.callbacksFilter = GUILayout.Toggle(context.callbacksFilter, "Callbacks ");

                    GUILayout.Space(DGUI.MARGIN);
                    if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                    }

                    GUILayout.Space(DGUI.MARGIN);

                });

            }, GUILayout.Height(HEADER_HEIGHT));
        }

        public static void Main() {
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            var buttonAutoMargin = 3;
            var buttonWidth = context.columnWidth - (2 * DGUI.MARGIN);
            
            mainContentRect.width = ((context.columns.Length + 1) * (DGUI.MARGIN + context.columnWidth)) - DGUI.MARGIN;
            mainContentRect.height = mainMaxHeight;

            float height = 0;

            if (DGUI.hasSlider) {
                buttonWidth -= 9;
            }

            ResetStyle();
            
            scrollPos = DGUI.ScrollWrap(() => {

                DGUI.Horizontal(() => {

                    for (int i = 0; i < context.columns.Length; i++) {
                        
                        Column column = context.columns[i];

                        foreach (Column col in context.columns) {
                            if (col.id == i) {
                                column = col;
                                break;
                            }
                        }

                        DGUI.Vertical(() => {

                            GUILayout.Space(-HEADER_HEIGHT - DGUI.MARGIN);

                            float localHeight = DGUI.MARGIN;
                            var selected = ArrayHandler.GetIndex(Diplomata.preferences.characterList, column.emitter);
                            
                            emitterFieldRect.x = DGUI.MARGIN + (i * (DGUI.MARGIN + context.columnWidth));
                            emitterFieldRect.width = (context.columnWidth - DGUI.MARGIN) / 2;

                            GUI.Label(emitterFieldRect, "Emitter: ");

                            emitterFieldRect.x += emitterFieldRect.width;

                            selected = EditorGUI.Popup(emitterFieldRect, selected, Diplomata.preferences.characterList);

                            if (selected != ArrayHandler.GetIndex(Diplomata.preferences.characterList, column.emitter)) {
                                column.emitter = Diplomata.preferences.characterList[selected];

                                foreach (Message msg in column.messages) {
                                    msg.emitter = column.emitter;
                                }
                            }

                            GUILayout.Space(EMITTER_FIELD_HEIGHT + DGUI.MARGIN);
                            localHeight += EMITTER_FIELD_HEIGHT + DGUI.MARGIN;

                            for (int j = 0; j < column.messages.Length; j++) {

                                Message currentMessage = column.messages[j];

                                foreach (Message msg in column.messages) {
                                    if (msg.id == j) {
                                        currentMessage = msg;
                                        break;
                                    }
                                }

                                var x = DGUI.MARGIN + (i * (context.columnWidth + DGUI.MARGIN));
                                var text = "";

                                if (context.idFilter) {
                                    text += "<i>[" + currentMessage.columnId + " " + currentMessage.id + "]</i>\n\n";
                                }

                                if (context.conditionsFilter) {
                                    if (currentMessage.conditions.Length > 0) {
                                        text += "<i>Conditions:</i>\n\n";
                                    }

                                    foreach (Condition condition in currentMessage.conditions) {
                                        text += condition.displayName + "\n\n";
                                    }
                                }

                                var difference = 0;
                                
                                if (context.titleFilter && (!currentMessage.disposable || currentMessage.isAChoice)) {
                                    if (DictHandler.ContainsKey(currentMessage.title, Diplomata.preferences.currentLanguage) == null) {
                                        currentMessage.title = ArrayHandler.Add(currentMessage.title, new DictLang(Diplomata.preferences.currentLanguage, currentMessage.columnId + " " + currentMessage.id));
                                    }

                                    else {
                                        text += "<i>Title:</i>\n\n";
                                        DGUI.textContent.text = text;

                                        titleRect.y = localHeight + textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth) + DGUI.padding - difference;

                                        var titleTemp = DictHandler.ContainsKey(currentMessage.title, Diplomata.preferences.currentLanguage);

                                        text += titleTemp.value + "\n\n";
                                        DGUI.textContent.text = titleTemp.value;

                                        titleRect.height = textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth) + difference;
                                    }
                                }


                                if (context.contentFilter) {
                                    if (DictHandler.ContainsKey(currentMessage.content, Diplomata.preferences.currentLanguage) == null) {
                                        currentMessage.content = ArrayHandler.Add(currentMessage.content, new DictLang(Diplomata.preferences.currentLanguage, ""));
                                    }

                                    else {
                                        text += "<i>Content:</i>\n\n";
                                        DGUI.textContent.text = text;

                                        contentRect.y = localHeight + textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth) + DGUI.padding - difference;

                                        var contentTemp = DictHandler.ContainsKey(currentMessage.content, Diplomata.preferences.currentLanguage);

                                        text += contentTemp.value + "\n\n";
                                        DGUI.textContent.text = contentTemp.value;

                                        contentRect.height = textAreaStyle.CalcHeight(DGUI.textContent, context.columnWidth) + difference;
                                    }
                                }

                                if (context.callbacksFilter) {
                                    if (currentMessage.effects.Length > 0) {
                                        text += "<i>Effects:</i>\n\n";
                                    }

                                    foreach (Effect callback in currentMessage.effects) {
                                        text += callback.displayName + "\n\n";
                                    }
                                }

                                var color = currentMessage.color;

                                DGUI.strokeWidth = 1;

                                if (context.currentMessage.columnId != -1 && context.currentMessage.rowId != -1) {
                                    if (context.currentMessage.columnId == currentMessage.columnId && context.currentMessage.rowId == currentMessage.id) {
                                        color = DGUI.ColorSub(currentMessage.color, 0.1f, 0.1f, 0.1f);
                                        message = currentMessage;
                                        DGUI.strokeWidth = 3;
                                    }
                                }

                                var boxHeight = DGUI.Box(text, x, localHeight, context.columnWidth, color, TextAnchor.UpperLeft);
                                
                                if (context.titleFilter && (!currentMessage.disposable || currentMessage.isAChoice)) {
                                    var title = DictHandler.ContainsKey(currentMessage.title, Diplomata.preferences.currentLanguage);
                                    titleRect.x = x + DGUI.padding;
                                    titleRect.width = context.columnWidth - (2 * DGUI.padding);

                                    GUI.SetNextControlName("title" + currentMessage.id);
                                    title.value = EditorGUI.TextArea(titleRect, title.value, textAreaStyle);
                                }

                                if (context.contentFilter) {
                                    var content = DictHandler.ContainsKey(currentMessage.content, Diplomata.preferences.currentLanguage);
                                    contentRect.x = x + DGUI.padding;
                                    contentRect.width = context.columnWidth - (2 * DGUI.padding);

                                    GUI.SetNextControlName("content" + currentMessage.id);
                                    content.value = EditorGUI.TextArea(contentRect, content.value, textAreaStyle);
                                }

                                if (GUI.GetNameOfFocusedControl() == "title" + currentMessage.id ||
                                GUI.GetNameOfFocusedControl() == "content" + currentMessage.id
                                ) {
                                    SetMessage(currentMessage);
                                }

                                if (GUI.Button(DGUI.boxFillRect, "", buttonStyle)) {
                                    SetMessage(currentMessage);
                                    EditorGUI.FocusTextInControl("");
                                }
                                
                                GUILayout.Space(boxHeight);
                                localHeight += boxHeight;

                                GUILayout.Space(DGUI.MARGIN);
                                localHeight += DGUI.MARGIN;
                            }

                            GUILayout.Space(DGUI.MARGIN);

                            DGUI.Horizontal(() => {
                                GUILayout.Space(DGUI.MARGIN);

                                if (GUILayout.Button("Add Message", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(buttonWidth))) {
                                    column.messages = ArrayHandler.Add(column.messages, new Message(column.messages.Length, column.emitter, column.id));

                                    SetMessage(null);
                                    
                                    JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                }

                                GUILayout.Space(DGUI.MARGIN);
                            });

                            localHeight += DGUI.MARGIN + DGUI.BUTTON_HEIGHT + buttonAutoMargin;

                            if (localHeight > height) {
                                height = localHeight;
                            }

                        }, GUILayout.Width(context.columnWidth));

                        GUILayout.Space(DGUI.MARGIN - 4);

                    }

                    DGUI.Vertical(() => {

                        GUILayout.Space(-HEADER_HEIGHT);

                        if (GUILayout.Button("Add Column", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(buttonWidth))) {
                            context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                        }

                    }, GUILayout.Width(context.columnWidth));

                });

            }, scrollPos, mainRect, mainContentRect);

            DGUI.boxStyle.fontSize = 11;
            textAreaStyle.fontSize = 11;
            DGUI.padding = 10;
            DGUI.strokeWidth = 1;

            if (GUI.Button(mainRect, "", buttonStyle)) {
                EditorGUI.FocusTextInControl("");
            }
            
            mainMaxHeight = height;
        }

        public static void Sidebar() {
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;

            DGUI.Horizontal(() => {
                GUILayout.Space(Screen.width - SIDEBAR_WIDTH + DGUI.MARGIN);

                DGUI.Vertical(() => {
                    GUILayout.Space(-mainMaxHeight + HEADER_HEIGHT + DGUI.MARGIN);
                    
                    DGUI.labelStyle.richText = true;
                    DGUI.labelStyle.alignment = TextAnchor.UpperCenter;
                    
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

                            DGUI.Horizontal(() => {
                                message.disposable = GUILayout.Toggle(message.disposable, "Disposable");
                                message.isAChoice = GUILayout.Toggle(message.isAChoice, "Is a choice");  
                            });

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

                            DGUI.textContent.text = screenplayNotes.value;
                            var height = DGUI.textAreaStyle.CalcHeight(DGUI.textContent, Screen.width - (2 * DGUI.MARGIN)) + 15;

                            GUILayout.Label("Screenplay notes: ");
                            screenplayNotes.value = EditorGUILayout.TextArea(screenplayNotes.value, DGUI.textAreaStyle, GUILayout.Height(height));

                            EditorGUILayout.Separator();

                            DGUI.Horizontal(() => {

                                if (GUILayout.Button("Edit Conditions", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                    context.messageEditorState = MessageEditorState.Conditions;
                                }

                                if (GUILayout.Button("Edit Effects", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                    context.messageEditorState = MessageEditorState.Effects;
                                }

                            });

                            EditorGUILayout.Separator();

                            if (column.messages.Length > 1 || context.columns.Length > 1) {
                                DGUI.Separator();
                                GUILayout.Label("Move: ");
                            }

                            DGUI.Horizontal(() => {
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

                                if (message.id > 0) {
                                    if (GUILayout.Button("Up", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                        Message.Find(column.messages, message.id - 1).id += 1;

                                        message.id -= 1;

                                        SetMessage(message);
                                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                    }
                                }

                                if (message.id < column.messages.Length - 1) {
                                    if (GUILayout.Button("Down", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                        Message.Find(column.messages, message.id + 1).id -= 1;

                                        message.id += 1;

                                        SetMessage(message);
                                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                    }
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

                            });

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

                            foreach (Condition condition in message.conditions) {

                                EditorGUI.BeginChangeCheck();

                                DGUI.Horizontal(() => {
                                    GUILayout.Label("Type: ");
                                    condition.type = (Condition.Type) EditorGUILayout.EnumPopup(condition.type);
                                });

                                EditorGUILayout.Separator();

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
                                        break;

                                    case Condition.Type.InfluenceEqualTo:
                                    case Condition.Type.InfluenceGreaterThan:
                                    case Condition.Type.InfluenceLessThan:

                                        DGUI.Horizontal(() => {
                                            EditorGUI.BeginChangeCheck();

                                            condition.comparedInfluence = EditorGUILayout.IntField(condition.comparedInfluence);
                                            condition.characterInfluencedName = DGUI.Popup(" in ", condition.characterInfluencedName, CharacterMessagesManager.characterList);
                                            
                                            if (EditorGUI.EndChangeCheck()) {
                                                condition.DisplayCompareInfluence();
                                            }
                                        });

                                        break;
                                }

                                EditorGUILayout.Separator();

                                if (GUILayout.Button("Delete Condition", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                    message.conditions = ArrayHandler.Remove(message.conditions,condition);
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

                    if (context.columns.Length == 0) {
                        GUILayout.Space(-HEADER_HEIGHT - DGUI.BUTTON_HEIGHT + DGUI.MARGIN);
                    }
                    
                    if (context.messageEditorState != MessageEditorState.None) {
                        DGUI.Separator();
                    }

                    if (GUILayout.Button("Remove Empty Columns", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        context.columns = Column.RemoveEmptyColumns(context.columns);
                        context.messageEditorState = MessageEditorState.None;

                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                    }

                }, GUILayout.Width(SIDEBAR_WIDTH - (2 * DGUI.MARGIN)));

                GUILayout.Space(DGUI.MARGIN);
            });
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
            textAreaStyle.normal.background = textAreaBGTextureNormal;
            textAreaStyle.normal.textColor = DGUI.transparentColor;
            textAreaStyle.focused.background = textAreaBGTextureFocused;
            textAreaStyle.padding = textAreaPadding;
            buttonStyle.normal.background = DGUI.transparentTexture;
            buttonStyle.active.background = DGUI.transparentTexture;
        }

    }

}