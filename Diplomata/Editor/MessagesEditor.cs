using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesEditor {
        private const byte HEADER_HEIGHT = DGUI.BUTTON_HEIGHT_SMALL + (2 * DGUI.MARGIN);
        private const byte LABEL_HEIGHT = HEADER_HEIGHT + 15;
        private const ushort SIDEBAR_WIDTH = 300;

        private static string[] messageList = new string[0];
        private static string[] characterList = new string[0];
        private static string[] contextList = new string[0];
        private static string[] itemList = new string[0];
        private static string[] customFlagsList = new string[0];
        private static string[] labelsList = new string[0];
        private static string[] booleanArray = new string[] { "True", "False" };
        public static GUIStyle messagesWindowHeaderStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle messagesWindowMainStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle messagesWindowSidebarStyle = new GUIStyle(DGUI.windowStyle);
        public static GUIStyle textAreaStyle = new GUIStyle(DGUI.textAreaStyle);
        public static GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
        public static GUIStyle fakeButtonStyle = new GUIStyle(GUI.skin.button);
        public static Color inactiveColor = new Color(0, 0, 0, 0.6f);

        private static Color proColor = new Color(0.2196f, 0.2196f, 0.2196f);
        private static Color defaultColor = new Color(0.9764f, 0.9764f, 0.9764f);
        private static Vector2 scrollPosMain = new Vector2(0, 0);
        private static Vector2 scrollPosSidebar = new Vector2(0, 0);
        private static Vector2 scrollPosLabelManager = new Vector2(0, 0);
        private static Message message;

        public static void Draw() {
            if (CharacterMessagesManager.context != null) {
                messagesWindowHeaderStyle.normal.background = CharacterMessagesManager.headerBG;
                messagesWindowMainStyle.normal.background = CharacterMessagesManager.mainBG;
                messagesWindowSidebarStyle.normal.background = CharacterMessagesManager.sidebarBG;

                messagesWindowMainStyle.alignment = TextAnchor.UpperLeft;
                messagesWindowSidebarStyle.alignment = TextAnchor.UpperLeft;

                Header();
                GUILayout.BeginHorizontal();
                Main();
                Sidebar();
                GUILayout.EndHorizontal();
                LabelManager();
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

            GUILayout.Label("Column Width: ");
            context.columnWidth = (ushort) EditorGUILayout.Slider(context.columnWidth, 116, 675);

            EditorGUILayout.Separator();

            GUILayout.Label("Font Size: ");
            context.fontSize = (ushort)EditorGUILayout.Slider(context.fontSize, 8, 36);

            DGUI.boxStyle.fontSize = context.fontSize;
            DGUI.labelStyle.fontSize = context.fontSize;
            textAreaStyle.fontSize = context.fontSize;

            EditorGUILayout.Separator();

            GUILayout.Label("Filters: ");
            context.idFilter = GUILayout.Toggle(context.idFilter, "Id ");
            context.conditionsFilter = GUILayout.Toggle(context.conditionsFilter, "Conditions ");
            context.contentFilter = GUILayout.Toggle(context.contentFilter, "Content ");
            context.effectsFilter = GUILayout.Toggle(context.effectsFilter, "Effects ");

            EditorGUILayout.Separator();

            if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                diplomataEditor.Save(character);
            }

            GUILayout.EndHorizontal();
        }

        public static void LabelManager() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            var height = ((180 + (context.labels.Length * 240)) >= Screen.width) ? LABEL_HEIGHT : HEADER_HEIGHT;

            scrollPosLabelManager = EditorGUILayout.BeginScrollView(scrollPosLabelManager,
                messagesWindowHeaderStyle, GUILayout.Width(Screen.width), GUILayout.Height(height));
            GUILayout.BeginHorizontal();

            GUILayout.Label("Labels: ", GUILayout.Width(60));

            for (int i = 0; i < context.labels.Length; i++) {
                var label = context.labels[i];
                label.name = EditorGUILayout.TextField(label.name, GUILayout.Width(100));
                label.color = EditorGUILayout.ColorField(label.color, GUILayout.Width(60));
                string show = (label.show) ? "hide" : "show";
                if (GUILayout.Button(show, GUILayout.Width(40), GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                    label.show = (label.show) ? false : true;
                }
                if (i > 0) {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        context.labels = ArrayHandler.Remove(context.labels, label);
                        foreach (Column col in context.columns) {
                            foreach (Message msg in col.messages) {
                                if (msg.labelId == label.id) {
                                    msg.labelId = context.labels[0].id;
                                }
                            }
                        }
                        diplomataEditor.Save(character);
                    }
                    GUILayout.Space(20);
                }
                else {
                    GUILayout.Space(40);
                }
            }

            if (GUILayout.Button("Add label", GUILayout.Width(100), GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                context.labels = ArrayHandler.Add(context.labels, new Label());
                context.labels[context.labels.Length - 1].name += " (" + (context.labels.Length - 1) + ")";
                diplomataEditor.Save(character);
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();
        }

        public static void Main() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            var width = Screen.width - SIDEBAR_WIDTH;

            ResetStyle();

            scrollPosMain = EditorGUILayout.BeginScrollView(scrollPosMain, messagesWindowMainStyle, GUILayout.Width(width));
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

                EditorGUI.BeginChangeCheck();
                column.emitter = DGUI.Popup("Emitter: ", column.emitter, diplomataEditor.preferences.characterList);
                if (EditorGUI.EndChangeCheck()) {
                    foreach (Message msg in column.messages) {
                        msg.emitter = column.emitter;
                    }
                }

                EditorGUILayout.Separator();

                for (int j = 0; j < column.messages.Length; j++) {

                    Message currentMessage = column.messages[j];

                    if (currentMessage.labelId == "") {
                        currentMessage.labelId = context.labels[0].id;
                    }

                    var label = Label.Find(context.labels, currentMessage.labelId);

                    foreach (Message msg in column.messages) {
                        if (msg.id == j) {
                            currentMessage = msg;
                            break;
                        }
                    }
                    
                    if (label.show) {
                        Rect boxRect = EditorGUILayout.BeginVertical(DGUI.boxStyle);

                        var color = (EditorGUIUtility.isProSkin) ? proColor : defaultColor;

                        DGUI.strokeWidth = 1;

                        if (context.currentMessage.columnId != -1 && context.currentMessage.rowId != -1) {
                            if (context.currentMessage.columnId == currentMessage.columnId && context.currentMessage.rowId == currentMessage.id) {
                                color = DGUI.ColorAdd(color, 0.1f);
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
                        proColor = new Color(0.2196f, 0.2196f, 0.2196f);
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
                                            if (condition.afterOf.GetMessage(context) != null) {
                                                text += condition.DisplayAfterOf(DictHandler.ContainsKey(condition.afterOf.GetMessage(context).content,
                                                    diplomataEditor.preferences.currentLanguage).value);
                                            }
                                            break;

                                        case Condition.Type.InfluenceEqualTo:
                                        case Condition.Type.InfluenceGreaterThan:
                                        case Condition.Type.InfluenceLessThan:
                                            text += condition.DisplayCompareInfluence();
                                            break;
                                        case Condition.Type.HasItem:
                                            var itemName = "";
                                            if (Item.Find(diplomataEditor.inventory.items, condition.itemId) != null) {
                                                itemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += condition.DisplayHasItem(itemName);
                                            break;
                                        case Condition.Type.DoesNotHaveTheItem:
                                            var itemNameDont = "";
                                            if (Item.Find(diplomataEditor.inventory.items, condition.itemId) != null) {
                                                itemNameDont = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += condition.DisplayDoesNotHaveItem(itemNameDont);
                                            break;
                                        case Condition.Type.ItemWasDiscarded:
                                            var itemNameDiscarded = "";
                                            if (Item.Find(diplomataEditor.inventory.items, condition.itemId) != null) {
                                                itemNameDiscarded = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += condition.DisplayItemWasDiscarded(itemNameDiscarded);
                                            break;
                                        case Condition.Type.ItemIsEquipped:
                                            var itemNameEquipped = "";
                                            if (Item.Find(diplomataEditor.inventory.items, condition.itemId) != null) {
                                                itemNameEquipped = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += condition.DisplayItemIsEquipped(itemNameEquipped);
                                            break;
                                        case Condition.Type.CustomFlagEqualTo:
                                            text += condition.DisplayCustomFlagEqualTo();
                                            break;
                                        case Condition.Type.InteractsWith:
                                            text += condition.DisplayInteractsWith(condition.interactWith);
                                            break;
                                    }

                                    if (k < currentMessage.conditions.Length - 1) {
                                        text += "\n\n";
                                    }
                                }

                                DGUI.labelStyle.normal.textColor = DGUI.ColorSub(DGUI.labelStyle.normal.textColor, 0, 0.4f);
                                DGUI.textContent.text = text;
                                height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                                GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                                DGUI.labelStyle.normal.textColor = DGUI.ColorAdd(DGUI.labelStyle.normal.textColor, 0, 0.4f);
                            }
                        }

                        EditorGUI.DrawRect(new Rect(boxRect.xMin, boxRect.yMin, boxRect.width, 5), label.color);
                        
                        if (context.contentFilter) {
                            DGUI.textContent.text = "<b><i>Content:</i></b>";
                            height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                            GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));

                            var content = DictHandler.ContainsKey(currentMessage.content, diplomataEditor.preferences.currentLanguage);

                            if (content == null) {
                                currentMessage.content = ArrayHandler.Add(currentMessage.content, new DictLang(diplomataEditor.preferences.currentLanguage, "[ Message content here ]"));
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
                                                text += effect.DisplayGoTo(DictHandler.ContainsKey(effect.goTo.GetMessage(context).content, diplomataEditor.preferences.currentLanguage).value);
                                            }
                                            break;
                                        case Effect.Type.SetAnimatorAttribute:
                                            text += effect.DisplaySetAnimatorAttribute();
                                            break;
                                        case Effect.Type.GetItem:
                                            var itemName = "";
                                            if (Item.Find(diplomataEditor.inventory.items, effect.itemId) != null) {
                                                itemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += effect.DisplayGetItem(itemName);
                                            break;
                                        case Effect.Type.DiscardItem:
                                            var discardItemName = "";
                                            if (Item.Find(diplomataEditor.inventory.items, effect.itemId) != null) {
                                                discardItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += effect.DisplayDiscardItem(discardItemName);
                                            break;
                                        case Effect.Type.EquipItem:
                                            var equipItemName = "";
                                            if (Item.Find(diplomataEditor.inventory.items, effect.itemId) != null) {
                                                equipItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name,
                                                    diplomataEditor.preferences.currentLanguage).value;
                                            }
                                            text += effect.DisplayEquipItem(equipItemName);
                                            break;
                                        case Effect.Type.SetCustomFlag:
                                            text += effect.DisplayCustomFlagEqualTo();
                                            break;
                                    }

                                    if (k < currentMessage.effects.Length - 1) {
                                        text += "\n\n";
                                    }
                                }

                                DGUI.labelStyle.normal.textColor = DGUI.ColorSub(DGUI.labelStyle.normal.textColor, 0, 0.4f);
                                DGUI.textContent.text = text;
                                height = DGUI.labelStyle.CalcHeight(DGUI.textContent, context.columnWidth);
                                GUILayout.Label(DGUI.textContent, DGUI.labelStyle, GUILayout.Width(context.columnWidth), GUILayout.Height(height));
                                DGUI.labelStyle.normal.textColor = DGUI.ColorAdd(DGUI.labelStyle.normal.textColor, 0, 0.4f);
                            }
                        }

                        if (currentMessage.disposable || currentMessage.isAChoice) {
                            DGUI.labelStyle.fontSize = context.fontSize;
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

                            DGUI.labelStyle.fontSize = context.fontSize;
                            DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                        }

                        if (GUI.Button(boxRect, "", buttonStyle)) {
                            SetMessage(currentMessage);
                            EditorGUI.FocusTextInControl("");
                        }

                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Separator();
                    }
                }

                if (GUILayout.Button("Add Message", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                    column.messages = ArrayHandler.Add(column.messages, new Message(column.messages.Length, column.emitter, column.id, context.labels[0].id));

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
            scrollPosSidebar = EditorGUILayout.BeginScrollView(scrollPosSidebar, messagesWindowSidebarStyle, GUILayout.Width(SIDEBAR_WIDTH), GUILayout.ExpandHeight(true));

            var diplomataEditor = CharacterMessagesManager.diplomataEditor;
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;

            if (EditorGUIUtility.isProSkin) {
                DGUI.labelStyle.normal.textColor = DGUI.proTextColor;
            }

            else {
                DGUI.labelStyle.normal.textColor = DGUI.freeTextColor;
            }

            DGUI.labelStyle.fontSize = 12;
            DGUI.labelStyle.alignment = TextAnchor.UpperCenter;

            if (message != null) {

                switch (context.messageEditorState) {

                    case MessageEditorState.Normal:

                        GUILayout.Label("<b>Properties</b>", DGUI.labelStyle);
                        EditorGUILayout.Separator();

                        var column = Column.Find(context, message.columnId);

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

                        DGUI.Separator();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Label: ");

                        var label = Label.Find(context.labels, message.labelId);
                        var labelIndex = 0;

                        UpdateLabelsList(context);

                        if (label != null) {
                            for (int labelI = 0; labelI < labelsList.Length; labelI++) {
                                if (label.name == labelsList[labelI]) {
                                    labelIndex = labelI;
                                    break;
                                }
                            }
                        }

                        EditorGUI.BeginChangeCheck();

                        labelIndex = EditorGUILayout.Popup(labelIndex, labelsList);

                        if (EditorGUI.EndChangeCheck()) {
                            message.labelId = context.labels[labelIndex].id;
                        }

                        GUILayout.EndHorizontal();
                        DGUI.Separator();

                        var screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, diplomataEditor.preferences.currentLanguage);

                        if (screenplayNotes == null) {
                            message.screenplayNotes = ArrayHandler.Add(message.screenplayNotes, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
                            screenplayNotes = DictHandler.ContainsKey(message.screenplayNotes, diplomataEditor.preferences.currentLanguage);
                        }

                        DGUI.labelStyle.alignment = TextAnchor.UpperLeft;
                        GUILayout.Label("Screenplay notes:\n<size=10>(Example: <i>whispering and gasping</i>)</size>", DGUI.labelStyle);
                        screenplayNotes.value = EditorGUILayout.TextField(screenplayNotes.value);

                        DGUI.Separator();

                        EditorGUILayout.Separator();

                        var audioClipPath = DictHandler.ContainsKey(message.audioClipPath, diplomataEditor.preferences.currentLanguage);

                        if (audioClipPath == null) {
                            message.audioClipPath = ArrayHandler.Add(message.audioClipPath, new DictLang(diplomataEditor.preferences.currentLanguage, string.Empty));
                            audioClipPath = DictHandler.ContainsKey(message.audioClipPath, diplomataEditor.preferences.currentLanguage);
                        }

                        message.audioClip = (AudioClip) Resources.Load(audioClipPath.value);

                        if (message.audioClip == null && audioClipPath.value != string.Empty) {
                            Debug.LogWarning("Cannot find the file \"" + audioClipPath.value + "\" in Resources folder.");
                        }

                        GUILayout.Label("Audio to play: ");
                        EditorGUI.BeginChangeCheck();

                        message.audioClip = (AudioClip) EditorGUILayout.ObjectField(message.audioClip, typeof(AudioClip), false);

                        if (EditorGUI.EndChangeCheck()) {
                            if (message.audioClip != null) {
                                var str = AssetDatabase.GetAssetPath(message.audioClip).Replace("Resources/", "¬");
                                var strings = str.Split('¬');
                                str = strings[1].Replace(".mp3","");
                                str = str.Replace(".aif", "");
                                str = str.Replace(".aiff", "");
                                str = str.Replace(".ogg", "");
                                str = str.Replace(".wav", "");
                                audioClipPath.value = str;
                            }

                            else {
                                audioClipPath.value = string.Empty;
                            }
                        }

                        EditorGUILayout.Separator();

                        message.image = (Texture2D)Resources.Load(message.imagePath);

                        if (message.image == null && message.imagePath != string.Empty) {
                            Debug.LogWarning("Cannot find the file \"" + message.imagePath + "\" in Resources folder.");
                        }

                        GUILayout.Label("Static image: ");
                        EditorGUI.BeginChangeCheck();

                        message.image = (Texture2D)EditorGUILayout.ObjectField(message.image, typeof(Texture2D), false);

                        if (EditorGUI.EndChangeCheck()) {
                            if (message.image != null) {
                                var str = AssetDatabase.GetAssetPath(message.image).Replace("Resources/", "¬");
                                var strings = str.Split('¬');
                                str = strings[1].Replace(".png", "");
                                str = str.Replace(".jpg", "");
                                str = str.Replace(".jpeg", "");
                                str = str.Replace(".psd", "");
                                str = str.Replace(".tga", "");
                                str = str.Replace(".tiff", "");
                                str = str.Replace(".gif", "");
                                str = str.Replace(".bmp", "");
                                message.imagePath = str;
                            }

                            else {
                                message.imagePath = string.Empty;
                            }
                        }

                        EditorGUILayout.Separator();

                        EditorGUILayout.HelpBox("\nMake sure to store this audio clip, texture and animator controllers in Resources folder.\n\n" +
                            "Use PlayMessageAudioContent() to play audio clip and StopMessageAudioContent() to stop.\n\n" +
                            "Use SwapStaticSprite(Vector pivot) to show static image.\n", MessageType.Info);

                        DGUI.Separator();

                        GUILayout.Label("Animator Attributes Setters");

                        foreach (AnimatorAttributeSetter animatorAttribute in message.animatorAttributesSetters) {

                            EditorGUILayout.Separator();

                            animatorAttribute.animator = (RuntimeAnimatorController)Resources.Load(animatorAttribute.animatorPath);

                            if (animatorAttribute.animator == null && animatorAttribute.animatorPath != string.Empty) {
                                Debug.LogWarning("Cannot find the file \"" + animatorAttribute.animatorPath + "\" in Resources folder.");
                            }

                            GUILayout.Label("Animator Controller: ");
                            EditorGUI.BeginChangeCheck();

                            animatorAttribute.animator = (RuntimeAnimatorController)EditorGUILayout.ObjectField(animatorAttribute.animator, typeof(RuntimeAnimatorController), false);

                            if (EditorGUI.EndChangeCheck()) {
                                if (animatorAttribute.animator != null) {
                                    var str = AssetDatabase.GetAssetPath(animatorAttribute.animator).Replace("Resources/", "¬");
                                    var strings = str.Split('¬');
                                    str = strings[1].Replace(".controller", "");
                                    animatorAttribute.animatorPath = str;
                                }

                                else {
                                    animatorAttribute.animatorPath = string.Empty;
                                }
                            }

                            GUILayout.BeginHorizontal();

                            GUILayout.Label("Type: ");
                            animatorAttribute.type = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup(animatorAttribute.type);

                            EditorGUI.BeginChangeCheck();

                            GUILayout.Label("Name: ");
                            animatorAttribute.name = EditorGUILayout.TextField(animatorAttribute.name);

                            if (EditorGUI.EndChangeCheck()) {
                                animatorAttribute.setTrigger = Animator.StringToHash(animatorAttribute.name);
                            }

                            GUILayout.EndHorizontal();

                            GUILayout.BeginHorizontal();

                            switch (animatorAttribute.type) {
                                case AnimatorControllerParameterType.Bool:
                                    string selected = animatorAttribute.setBool.ToString();

                                    EditorGUI.BeginChangeCheck();

                                    selected = DGUI.Popup("Set boolean to ", selected, booleanArray);

                                    if (EditorGUI.EndChangeCheck()) {

                                        if (selected == "True") {
                                            animatorAttribute.setBool = true;
                                        }

                                        else {
                                            animatorAttribute.setBool = false;
                                        }

                                    }

                                    break;

                                case AnimatorControllerParameterType.Float:
                                    GUILayout.Label("Set float to ");
                                    animatorAttribute.setFloat = EditorGUILayout.FloatField(animatorAttribute.setFloat);
                                    break;

                                case AnimatorControllerParameterType.Int:
                                    GUILayout.Label("Set integer to ");
                                    animatorAttribute.setInt = EditorGUILayout.IntField(animatorAttribute.setInt);
                                    break;

                                case AnimatorControllerParameterType.Trigger:
                                    GUILayout.Label("Pull the trigger of [ " + animatorAttribute.name + " ]");
                                    break;
                            }

                            GUILayout.EndHorizontal();
                            EditorGUILayout.Separator();

                            if (GUILayout.Button("Delete Animator Attribute Setter", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                                message.animatorAttributesSetters = ArrayHandler.Remove(message.animatorAttributesSetters, animatorAttribute);
                                diplomataEditor.Save(character);
                            }

                            DGUI.Separator();
                        }

                        if (GUILayout.Button("Add Animator Attribute Setter", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            message.animatorAttributesSetters = ArrayHandler.Add(message.animatorAttributesSetters, new AnimatorAttributeSetter());
                            diplomataEditor.Save(character);
                        }

                        DGUI.Separator();

                        GUILayout.Label("Edit: ");

                        GUILayout.BeginHorizontal();


                        if (GUILayout.Button("Conditions", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Conditions;
                        }

                        if (GUILayout.Button("Effects", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.messageEditorState = MessageEditorState.Effects;
                        }

                        GUILayout.EndHorizontal();

                        EditorGUILayout.Separator();

                        if (column.messages.Length > 1 || context.columns.Length > 1) {
                            DGUI.Separator();
                            GUILayout.Label("Move: ");

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

                                    column.messages = Message.ResetIDs(column.messages);
                                    leftCol.messages = Message.ResetIDs(leftCol.messages);

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

                                    column.messages = Message.ResetIDs(column.messages);
                                    rightCol.messages = Message.ResetIDs(rightCol.messages);

                                    message.emitter = rightCol.emitter;

                                    SetMessage(message);
                                    diplomataEditor.Save(character);
                                }
                            }

                            else {
                                GUILayout.Box("<color=" + color + ">Right</color>", fakeButtonStyle, GUILayout.Height(DGUI.BUTTON_HEIGHT));
                            }

                            GUILayout.EndHorizontal();
                        }

                        DGUI.Separator();
                        GUILayout.Label("Other options: ");

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("Duplicate", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {

                            column.messages = ArrayHandler.Add(column.messages, new Message(message, column.messages.Length));

                            SetMessage(null);

                            diplomataEditor.Save(character);
                        }

                        if (GUILayout.Button("Delete", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            if (EditorUtility.DisplayDialog("Are you sure?", "If you agree all this message data will be lost forever.", "Yes", "No")) {

                                column.messages = ArrayHandler.Remove(column.messages, message);

                                SetMessage(null);

                                column.messages = Message.ResetIDs(column.messages);

                                diplomataEditor.Save(character);
                            }
                        }

                        GUILayout.EndHorizontal();

                        DGUI.Separator();

                        GUILayout.Label("Columns: ");

                        GUILayout.BeginHorizontal();

                        if (GUILayout.Button("New column at left", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));

                            MoveColumnsToRight(context, column.id);

                            SetMessage(null);
                            diplomataEditor.Save(character);
                        }

                        if (GUILayout.Button("New column at right", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                            context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));

                            MoveColumnsToRight(context, column.id + 1);

                            SetMessage(null);
                            diplomataEditor.Save(character);
                        }

                        GUILayout.EndHorizontal();

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
                                        messageName = DictHandler.ContainsKey(condition.afterOf.GetMessage(context).content, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    messageName = DGUI.Popup("Message: ", messageName, messageList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Column col in context.columns) {
                                            foreach (Message msg in col.messages) {

                                                if (DictHandler.ContainsKey(msg.content, diplomataEditor.preferences.currentLanguage).value == messageName) {
                                                    condition.afterOf.uniqueId = msg.GetUniqueId();
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

                                case Condition.Type.HasItem:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var itemName = "";

                                    if (itemList.Length > 0) {
                                        itemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    itemName = DGUI.Popup("Has item ", itemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == itemName) {
                                                condition.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Condition.Type.DoesNotHaveTheItem:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var itemNameDont = "";

                                    if (itemList.Length > 0) {
                                        itemNameDont = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    itemNameDont = DGUI.Popup("Does not have the item ", itemNameDont, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == itemNameDont) {
                                                condition.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Condition.Type.ItemIsEquipped:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var equippedItemName = "";

                                    if (itemList.Length > 0) {
                                        equippedItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    equippedItemName = DGUI.Popup("Item is equipped ", equippedItemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == equippedItemName) {
                                                condition.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Condition.Type.ItemWasDiscarded:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var discardedItemName = "";

                                    if (itemList.Length > 0) {
                                        discardedItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, condition.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    discardedItemName = DGUI.Popup("Item was discarded ", discardedItemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == discardedItemName) {
                                                condition.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Condition.Type.CustomFlagEqualTo:
                                    UpdateCustomFlagsList();

                                    condition.customFlag.name = DGUI.Popup("Flag: ", condition.customFlag.name, customFlagsList);

                                    string selected = condition.customFlag.value.ToString();

                                    EditorGUI.BeginChangeCheck();

                                    selected = DGUI.Popup("is ", selected, booleanArray);

                                    if (EditorGUI.EndChangeCheck()) {

                                        if (selected == "True") {
                                            condition.customFlag.value = true;
                                        }

                                        else {
                                            condition.customFlag.value = false;
                                        }

                                    }

                                    break;

                                case Condition.Type.InteractsWith:
                                    GUILayout.BeginHorizontal();

                                    GUILayout.Label("Interacts with ");

                                    condition.interactWith = EditorGUILayout.TextField(condition.interactWith);

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
                                                    effect.endOfContext.Set(tempCharacter.name, ctx.id);
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    break;

                                case Effect.Type.GoTo:
                                    UpdateMessagesList(context);
                                    var messageContent = string.Empty;

                                    if (effect.goTo.GetMessage(context) != null) {
                                        messageContent = DictHandler.ContainsKey(effect.goTo.GetMessage(context).content, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    messageContent = DGUI.Popup("Message: ", messageContent, messageList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Column col in context.columns) {
                                            foreach (Message msg in col.messages) {

                                                if (DictHandler.ContainsKey(msg.content, diplomataEditor.preferences.currentLanguage).value == messageContent) {
                                                    effect.goTo.uniqueId = msg.GetUniqueId();
                                                    break;
                                                }

                                            }
                                        }
                                    }

                                    break;

                                case Effect.Type.SetAnimatorAttribute:

                                    effect.animatorAttributeSetter.animator = (RuntimeAnimatorController)Resources.Load(effect.animatorAttributeSetter.animatorPath);

                                    if (effect.animatorAttributeSetter.animator == null && effect.animatorAttributeSetter.animatorPath != string.Empty) {
                                        Debug.LogWarning("Cannot find the file \"" + effect.animatorAttributeSetter.animatorPath + "\" in Resources folder.");
                                    }

                                    GUILayout.Label("Animator Controller: ");
                                    EditorGUI.BeginChangeCheck();

                                    effect.animatorAttributeSetter.animator = (RuntimeAnimatorController)EditorGUILayout.ObjectField(effect.animatorAttributeSetter.animator,
                                        typeof(RuntimeAnimatorController), false);

                                    if (EditorGUI.EndChangeCheck()) {
                                        if (effect.animatorAttributeSetter.animator != null) {
                                            var str = AssetDatabase.GetAssetPath(effect.animatorAttributeSetter.animator).Replace("Resources/", "¬");
                                            var strings = str.Split('¬');
                                            str = strings[1].Replace(".controller", "");
                                            effect.animatorAttributeSetter.animatorPath = str;
                                        }

                                        else {
                                            effect.animatorAttributeSetter.animatorPath = string.Empty;
                                        }
                                    }

                                    GUILayout.BeginHorizontal();

                                    GUILayout.Label("Type: ");
                                    effect.animatorAttributeSetter.type = (AnimatorControllerParameterType)EditorGUILayout.EnumPopup(effect.animatorAttributeSetter.type);

                                    EditorGUI.BeginChangeCheck();

                                    GUILayout.Label("Name: ");
                                    effect.animatorAttributeSetter.name = EditorGUILayout.TextField(effect.animatorAttributeSetter.name);

                                    if (EditorGUI.EndChangeCheck()) {
                                        effect.animatorAttributeSetter.setTrigger = Animator.StringToHash(effect.animatorAttributeSetter.name);
                                    }

                                    GUILayout.EndHorizontal();

                                    GUILayout.BeginHorizontal();

                                    switch (effect.animatorAttributeSetter.type) {
                                        case AnimatorControllerParameterType.Bool:
                                            string selected = effect.animatorAttributeSetter.setBool.ToString();

                                            EditorGUI.BeginChangeCheck();

                                            selected = DGUI.Popup("Set boolean to ", selected, booleanArray);

                                            if (EditorGUI.EndChangeCheck()) {

                                                if (selected == "True") {
                                                    effect.animatorAttributeSetter.setBool = true;
                                                }

                                                else {
                                                    effect.animatorAttributeSetter.setBool = false;
                                                }

                                            }

                                            break;

                                        case AnimatorControllerParameterType.Float:
                                            GUILayout.Label("Set float to ");
                                            effect.animatorAttributeSetter.setFloat = EditorGUILayout.FloatField(effect.animatorAttributeSetter.setFloat);
                                            break;

                                        case AnimatorControllerParameterType.Int:
                                            GUILayout.Label("Set integer to ");
                                            effect.animatorAttributeSetter.setInt = EditorGUILayout.IntField(effect.animatorAttributeSetter.setInt);
                                            break;

                                        case AnimatorControllerParameterType.Trigger:
                                            GUILayout.Label("Pull the trigger of [ " + effect.animatorAttributeSetter.name + " ]");
                                            break;
                                    }

                                    GUILayout.EndHorizontal();

                                    break;

                                case Effect.Type.GetItem:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var itemName = "";

                                    if (itemList.Length > 0) {
                                        itemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    itemName = DGUI.Popup("Get item ", itemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == itemName) {
                                                effect.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Effect.Type.DiscardItem:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var discardItemName = "";

                                    if (itemList.Length > 0) {
                                        discardItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    discardItemName = DGUI.Popup("Discard item ", discardItemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == discardItemName) {
                                                effect.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Effect.Type.EquipItem:
                                    GUILayout.BeginHorizontal();
                                    UpdateItemList();

                                    var equipItemName = "";

                                    if (itemList.Length > 0) {
                                        equipItemName = DictHandler.ContainsKey(Item.Find(diplomataEditor.inventory.items, effect.itemId).name, diplomataEditor.preferences.currentLanguage).value;
                                    }

                                    EditorGUI.BeginChangeCheck();

                                    equipItemName = DGUI.Popup("Discard item ", equipItemName, itemList);

                                    if (EditorGUI.EndChangeCheck()) {
                                        foreach (Item item in diplomataEditor.inventory.items) {

                                            if (DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value == equipItemName) {
                                                effect.itemId = item.id;
                                                break;
                                            }

                                        }
                                    }

                                    GUILayout.EndHorizontal();
                                    break;

                                case Effect.Type.SetCustomFlag:
                                    UpdateCustomFlagsList();

                                    effect.customFlag.name = DGUI.Popup("Flag: ", effect.customFlag.name, customFlagsList);

                                    string effectSelected = effect.customFlag.value.ToString();

                                    EditorGUI.BeginChangeCheck();

                                    effectSelected = DGUI.Popup(" set to ", effectSelected, booleanArray);

                                    if (EditorGUI.EndChangeCheck()) {

                                        if (effectSelected == "True") {
                                            effect.customFlag.value = true;
                                        }

                                        else {
                                            effect.customFlag.value = false;
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
                            message.effects = ArrayHandler.Add(message.effects, new Effect(character.name));
                            diplomataEditor.Save(character);
                        }

                        break;
                }

            }

            GUILayout.Space(DGUI.MARGIN);

            if (GUILayout.Button("Remove Empty Columns", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                context.columns = Column.RemoveEmptyColumns(context.columns);
                context.messageEditorState = MessageEditorState.None;

                diplomataEditor.Save(character);
            }

            EditorGUILayout.EndScrollView();
        }

        public static void MoveColumnsToRight(Context context, int toIndex) {
            for (int i = context.columns.Length - 1; i >= toIndex; i--) {
                var col = Column.Find(context, i);
                var rightCol = Column.Find(context, i + 1);

                foreach (Message msg in col.messages) {
                    msg.columnId += 1;

                    msg.id = rightCol.messages.Length;

                    rightCol.messages = ArrayHandler.Add(rightCol.messages, msg);
                    col.messages = ArrayHandler.Remove(col.messages, msg);

                    col.messages = Message.ResetIDs(col.messages);
                    rightCol.messages = Message.ResetIDs(rightCol.messages);
                    
                    msg.emitter = rightCol.emitter;
                    rightCol.emitter = col.emitter;
                }
            }
        }

        public static void UpdateCharacterList() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            characterList = new string[0];

            foreach (string str in diplomataEditor.preferences.characterList) {
                if (str != diplomataEditor.preferences.playerCharacterName) {
                    characterList = ArrayHandler.Add(characterList, str);
                }
            }
        }

        public static void UpdateItemList() {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            itemList = new string[0];

            foreach (Item item in diplomataEditor.inventory.items) {
                itemList = ArrayHandler.Add(itemList, DictHandler.ContainsKey(item.name, diplomataEditor.preferences.currentLanguage).value);
            }
        }

        public static void UpdateMessagesList(Context context) {
            var diplomataEditor = CharacterMessagesManager.diplomataEditor;

            messageList = new string[0];

            foreach (Column col in context.columns) {
                foreach (Message msg in col.messages) {
                    DictLang content = DictHandler.ContainsKey(msg.content, diplomataEditor.preferences.currentLanguage);

                    if (content == null) {
                        msg.content = ArrayHandler.Add(msg.content, new DictLang(diplomataEditor.preferences.currentLanguage, ""));
                        content = DictHandler.ContainsKey(msg.content, diplomataEditor.preferences.currentLanguage);
                    }

                    messageList = ArrayHandler.Add(messageList, content.value);
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

        public static void UpdateCustomFlagsList() {
            customFlagsList = new string[0];

            foreach(Flag flag in CharacterMessagesManager.diplomataEditor.customFlags.flags) {
                customFlagsList = ArrayHandler.Add(customFlagsList, flag.name);
            }
        }

        public static void UpdateLabelsList(Context context) {
            labelsList = new string[0];

            foreach (Label label in context.labels) {
                labelsList = ArrayHandler.Add(labelsList, label.name);
            }
        }
    }

}