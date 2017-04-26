using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterMessagesManager : EditorWindow {

        private ushort timer = 0;
        public static string[] characterList = new string[0];
        public static Character character;
        public static Context context;
        private Vector2 scrollPos = new Vector2(0, 0);
        public static Texture2D headerBG;
        public static Texture2D mainBG;
        public static Texture2D sidebarBG;
        public static Texture2D textAreaBGTextureNormal;
        public static Texture2D textAreaBGTextureFocused;
        private static EditorData editorData;

        public enum State {
            None,
            Context,
            Messages,
            Close
        }

        public static State state;
        
        public static void Init(State state = State.None) {
            CharacterMessagesManager window = (CharacterMessagesManager)GetWindow(typeof(CharacterMessagesManager), false, "Messages", true);
            window.minSize = new Vector2(960, 300);
            window.maximized = true;
            
            CharacterMessagesManager.state = state;
            
            UpdateCharacterList();

            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public void OnEnable() {
            editorData = (EditorData)AssetHandler.Read<EditorData>("editorData.asset", "Diplomata/");
        }

        public void SetTextures() {
            var baseColor = DGUI.ResetColor();

            if (headerBG == null) {
                headerBG = DGUI.UniformColorTexture(1, 1, DGUI.ColorAdd(baseColor, 0.05f, 0.05f, 0.05f));
            }

            if (mainBG == null) {
                mainBG = DGUI.UniformColorTexture(1, 1, baseColor);
            }

            if (sidebarBG == null) {
                sidebarBG = DGUI.UniformColorTexture(1, 1, DGUI.ColorAdd(baseColor, -0.1f, -0.1f, -0.1f));
            }

            if (textAreaBGTextureNormal == null) {
                textAreaBGTextureNormal = DGUI.UniformColorTexture(1, 1, new Color(0.4f, 0.4f, 0.4f, 0.08f));
            }

            if (textAreaBGTextureFocused == null) {
                textAreaBGTextureFocused = DGUI.UniformColorTexture(1, 1, new Color(1, 1, 1, 1));
            }
        }

        public static void UpdateCharacterList() {
            characterList = new string[Diplomata.preferences.characterList.Length - 1];

            foreach (string str in Diplomata.preferences.characterList) {
                if (str != Diplomata.preferences.playerCharacterName) {
                    characterList = ArrayHandler.Add(characterList, str);
                }
            }
        }

        public static void OpenContextMenu(Character currentCharacter) {
            character = currentCharacter;

            editorData = (EditorData)AssetHandler.Read<EditorData>("editorData.asset", "Diplomata/");
            editorData.SetWorkingCharacter(currentCharacter.name);
            editorData.SetWorkingContextMessagesId(-1);
            Init(State.Context);
        }

        public static void OpenMessagesMenu(Character currentCharacter, Context currentContext) {
            character = currentCharacter;
            context = currentContext;

            editorData = (EditorData)AssetHandler.Read<EditorData>("editorData.asset", "Diplomata/");
            editorData.SetWorkingCharacter(currentCharacter.name);
            editorData.SetWorkingContextMessagesId(currentContext.id);
            Init(State.Messages);
        }

        public static void Reset(string characterName) {
            if (character != null) {
                if (character.name == characterName) {
                    editorData.SetWorkingCharacter(string.Empty);
                    character = null;
                    Init(State.Close);
                }
            }
        }

        public void OnGUI() {
            DGUI.Init();
            SetTextures();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            switch (state) {
                case State.None:
                    if (editorData.workingCharacter != string.Empty) {
                        character = Character.Find(editorData.workingCharacter);

                        if (editorData.workingContextMessagesId > -1) {
                            context = Context.Find(character, editorData.workingContextMessagesId);
                            MessagesEditor.Draw();
                        }

                        else {
                            ContextListMenu.Draw();
                        }
                    }
                    break;

                case State.Context:
                    ContextListMenu.Draw();
                    break;

                case State.Messages:
                    MessagesEditor.Draw();
                    break;
            }

            EditorGUILayout.EndScrollView();
        }

        public void OnInspectorUpdate() {
            AutoSave();
            Repaint();
        }

        private void AutoSave() {

            if (timer == 120 && character != null) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
                timer = 0;
            }

            timer++;

        }

        public void OnDisable() {
            if (character != null) {
                JSONHandler.Update(character, character.name, "Diplomata/Characters/");
            }
        }
    }

}
