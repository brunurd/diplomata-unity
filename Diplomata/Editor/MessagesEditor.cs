using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class MessagesEditor {

        private const byte HEADER_HEIGHT = DGUI.BUTTON_HEIGHT_SMALL + (2 * DGUI.MARGIN);
        private const ushort SIDEBAR_WIDTH = 300;
        private const ushort COLUMN_WIDTH = 200;

        public static string[] languagesList;
        
        public static Color baseColor = new Color(0,0,0);
        private static Color headerBGColor;
        private static Color mainBGColor;
        private static Color sidebarBGColor;

        private static Rect headerRect;
        private static Rect mainRect;
        private static Rect sidebarRect;

        private static Vector2 scrollPos = new Vector2(0, 0);
        public static float mainMaxHeight = 0;
        private static Message message;
        
        public static void Draw() {
            DrawBG();

            SetLanguagesList();

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

            else {
                baseColor = new Color(0.8705f, 0.8705f, 0.8705f);
            }

            baseColor = DGUI.ColorMul(baseColor, DGUI.PlaymodeTint());

            headerBGColor = DGUI.ColorAdd(baseColor, 0.05f, 0.05f, 0.05f);
            mainBGColor = baseColor;
            sidebarBGColor = DGUI.ColorAdd(baseColor, -0.1f, -0.1f, -0.1f);

            headerRect = new Rect(0, 0, Screen.width, HEADER_HEIGHT);
            mainRect = new Rect(0, HEADER_HEIGHT, Screen.width - SIDEBAR_WIDTH, Screen.height - HEADER_HEIGHT - 22);
            sidebarRect = new Rect(Screen.width - SIDEBAR_WIDTH, HEADER_HEIGHT, SIDEBAR_WIDTH, Screen.height - HEADER_HEIGHT);

            EditorGUI.DrawRect(headerRect, headerBGColor);
            EditorGUI.DrawRect(mainRect, mainBGColor);
            EditorGUI.DrawRect(sidebarRect, sidebarBGColor);
        }

        public static void SetLanguagesList() {
            languagesList = new string[Diplomata.preferences.languages.Count];

            for (int i = 0; i < Diplomata.preferences.languages.Count; i++) {
                languagesList[i] = Diplomata.preferences.languages[i].name;
            }
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

                    var selected = 0;

                    for (var i = 0; i < Diplomata.preferences.languages.Count; i++) {
                        if (Diplomata.preferences.languages[i].name == context.currentLanguage) {
                            selected = i;
                            break;
                        }
                    }

                    GUILayout.Space(DGUI.MARGIN);
                    GUILayout.Label("Language: ");
                    selected = EditorGUILayout.Popup(selected, languagesList);

                    for (var i = 0; i < Diplomata.preferences.languages.Count; i++) {
                        if (selected == i) {
                            context.currentLanguage = Diplomata.preferences.languages[i].name;
                            break;
                        }
                    }

                    GUILayout.Space(DGUI.MARGIN);
                    GUILayout.Label("Filters: ");

                    context.conditionsFilter = GUILayout.Toggle(context.conditionsFilter, "Conditions");
                    GUILayout.Space(DGUI.MARGIN);
                    context.titleFilter = GUILayout.Toggle(context.titleFilter, "Title");
                    GUILayout.Space(DGUI.MARGIN);
                    context.contentFilter = GUILayout.Toggle(context.contentFilter, "Content");
                    GUILayout.Space(DGUI.MARGIN);
                    context.callbacksFilter = GUILayout.Toggle(context.callbacksFilter, "Callbacks");

                    GUILayout.Space(DGUI.MARGIN);
                    if (GUILayout.Button("Save", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                    }

                    GUILayout.Space(DGUI.MARGIN);
                    if (GUILayout.Button("Save as screenplay", GUILayout.Height(DGUI.BUTTON_HEIGHT_SMALL))) {
                        //
                    }

                    GUILayout.Space(DGUI.MARGIN);

                });

            }, GUILayout.Height(HEADER_HEIGHT));
        }

        public static void Main() {
            var character = CharacterMessagesManager.character;
            var context = CharacterMessagesManager.context;
            var buttonAutoMargin = 3;
            var buttonWidth = COLUMN_WIDTH - (2 * DGUI.MARGIN);

            Rect mainContentRect = new Rect(0, 0, (2 * DGUI.MARGIN) + ((context.columns.Length + 1) * (DGUI.MARGIN + COLUMN_WIDTH)), mainMaxHeight);

            float height = 0;

            if (DGUI.hasSlider) {
                buttonWidth -= 9;
            }

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

                            for (int j = 0; j < column.messages.Length; j++) {

                                Message currentMessage = column.messages[j];

                                foreach (Message msg in column.messages) {
                                    if (msg.id == j) {
                                        currentMessage = msg;
                                        break;
                                    }
                                }

                                var x = DGUI.MARGIN + (i * (COLUMN_WIDTH + DGUI.MARGIN));
                                var text = "";

                                if (context.conditionsFilter) {
                                    if (currentMessage.conditions.Length > 0) {
                                        text += "Conditions:\n";
                                    }

                                    foreach (Condition condition in currentMessage.conditions) {
                                        text += condition.displayName + "\n\n";
                                    }
                                }
                                
                                if (context.titleFilter && (!currentMessage.disposable || currentMessage.isAChoice)) {
                                    if (DictHandle.ContainsKey(currentMessage.title, context.currentLanguage) == null) {
                                        currentMessage.title = ArrayHandler.Add(currentMessage.title, new DictLang(context.currentLanguage, ""));
                                    }
                                    else {
                                        text += "Title:\n" + DictHandle.ContainsKey(currentMessage.title, context.currentLanguage).value + "\n\n";
                                    }
                                }

                                if (context.contentFilter) {
                                    if (DictHandle.ContainsKey(currentMessage.content, context.currentLanguage) == null) {
                                        currentMessage.content = ArrayHandler.Add(currentMessage.content, new DictLang(context.currentLanguage, ""));
                                    }
                                    else {
                                        text += "Content:\n" + DictHandle.ContainsKey(currentMessage.content, context.currentLanguage).value + "\n\n";
                                    }
                                }

                                if (context.callbacksFilter) {
                                    if (currentMessage.callbacks.Length > 0) {
                                        text += "Callbacks:\n";
                                    }

                                    foreach (Callback callback in currentMessage.callbacks) {
                                        text += callback.displayName + "\n\n";
                                    }
                                }

                                var color = currentMessage.color;

                                if (context.currentMessage.columnId != -1 && context.currentMessage.rowId != -1) {
                                    if (context.currentMessage.columnId == currentMessage.columnId && context.currentMessage.rowId == currentMessage.id) {
                                        color = DGUI.ColorAdd(currentMessage.color, -0.1f, -0.1f, -0.1f);
                                        message = currentMessage;
                                    }
                                }

                                var boxHeight = DGUI.Box(text, x, localHeight, COLUMN_WIDTH, color, TextAnchor.UpperLeft);

                                GUI.color = new Color(0, 0, 0, 0);

                                if (GUI.Button(new Rect(x, localHeight, COLUMN_WIDTH, boxHeight), "")) {
                                    context.messageEditorState = MessageEditorState.Normal;
                                    context.currentMessage.Set(currentMessage.columnId, currentMessage.id);
                                    message = currentMessage;
                                    EditorGUI.FocusTextInControl("");
                                }

                                GUI.color = DGUI.ResetColor();

                                GUILayout.Space(boxHeight);
                                localHeight += boxHeight;

                                GUILayout.Space(DGUI.MARGIN);
                                localHeight += DGUI.MARGIN;
                            }

                            GUILayout.Space(DGUI.MARGIN);

                            DGUI.Horizontal(() => {
                                GUILayout.Space(DGUI.MARGIN);
                                
                                GUI.color = DGUI.ColorMul(DGUI.ResetColor(), DGUI.PlaymodeTint());

                                if (GUILayout.Button("Add Message", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(buttonWidth))) {
                                    column.messages = ArrayHandler.Add(column.messages, new Message(column.messages.Length, column.emitter, column.id));
                                    JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                                }

                                GUILayout.Space(DGUI.MARGIN);
                            });

                            localHeight += DGUI.MARGIN + DGUI.BUTTON_HEIGHT + buttonAutoMargin;

                            if (localHeight > height) {
                                height = localHeight;
                            }

                        }, GUILayout.Width(COLUMN_WIDTH));

                        GUILayout.Space(DGUI.MARGIN - 4);

                    }

                    DGUI.Vertical(() => {

                        GUILayout.Space(-HEADER_HEIGHT);

                        if (GUILayout.Button("Add Column", GUILayout.Height(DGUI.BUTTON_HEIGHT), GUILayout.Width(buttonWidth))) {
                            context.columns = ArrayHandler.Add(context.columns, new Column(context.columns.Length));
                            JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                        }

                    }, GUILayout.Width(COLUMN_WIDTH));

                });

            }, scrollPos, mainRect, mainContentRect);
            
            mainMaxHeight = height;
        }

        public static void Sidebar() {
            var context = CharacterMessagesManager.context;

            DGUI.Horizontal(() => {
                GUILayout.Space(Screen.width - SIDEBAR_WIDTH + DGUI.MARGIN);

                DGUI.Vertical(() => {
                    GUILayout.Space(-mainMaxHeight + HEADER_HEIGHT + DGUI.MARGIN);

                    switch (context.messageEditorState) {

                        case MessageEditorState.Normal:

                            if (GUILayout.Button("Edit Conditions", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                context.messageEditorState = MessageEditorState.Conditions;
                            }

                            EditorGUILayout.Separator();

                            DGUI.Horizontal(() => {
                                message.disposable = GUILayout.Toggle(message.disposable, "Disposable");
                                message.isAChoice = GUILayout.Toggle(message.isAChoice, "Is a choice");
                            });
                        
                            EditorGUILayout.Separator();

                            if (!message.disposable || message.isAChoice) {

                                GUILayout.Label("Title:");

                                var title = DictHandle.ContainsKey(message.title, context.currentLanguage);
                                title.value = EditorGUILayout.TextField(title.value);

                                EditorGUILayout.Separator();

                            }

                            GUILayout.Label("Content:");

                            var content = DictHandle.ContainsKey(message.content, context.currentLanguage);
                            content.value = EditorGUILayout.TextField(content.value);

                            EditorGUILayout.Separator();

                            if (GUILayout.Button("Edit Callbacks", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                context.messageEditorState = MessageEditorState.Callbacks;
                            }

                            break;

                        case MessageEditorState.Conditions:

                            if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                context.messageEditorState = MessageEditorState.Normal;
                            }

                            break;

                        case MessageEditorState.Callbacks:

                            if (GUILayout.Button("< Back", GUILayout.Height(DGUI.BUTTON_HEIGHT))) {
                                context.messageEditorState = MessageEditorState.Normal;
                            }

                            break;
                    }
                    
                }, GUILayout.Width(SIDEBAR_WIDTH - (2 * DGUI.MARGIN)));

                GUILayout.Space(DGUI.MARGIN);
            });
        }

    }

}