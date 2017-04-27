using UnityEngine;
using UnityEditor;
using DiplomataLib;

namespace DiplomataEditor {

    public class CharacterMessagesManager : EditorWindow {

        private ushort iteractions = 0;
        public static Character character;
        public static Context context;
        private Vector2 scrollPos = new Vector2(0, 0);
        public static Texture2D headerBG;
        public static Texture2D mainBG;
        public static Texture2D sidebarBG;
        public static Texture2D textAreaBGTextureNormal;
        public static Texture2D textAreaBGTextureFocused;
        public static Diplomata diplomataEditor;

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
            
            CharacterMessagesManager.state = state;
            
            if (state == State.Close) {
                window.Close();
            }

            else {
                window.Show();
            }
        }

        public void OnEnable() {
            diplomataEditor = (Diplomata) AssetHandler.Read("Diplomata.asset", "Diplomata/");
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

        public static void OpenContextMenu(Character currentCharacter) {
            character = currentCharacter;

            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
            diplomataEditor.SetWorkingCharacter(currentCharacter.name);
            diplomataEditor.SetWorkingContextMessagesId(-1);
            Init(State.Context);
        }

        public static void OpenMessagesMenu(Character currentCharacter, Context currentContext) {
            character = currentCharacter;
            context = currentContext;

            diplomataEditor = (Diplomata)AssetHandler.Read("Diplomata.asset", "Diplomata/");
            diplomataEditor.SetWorkingCharacter(currentCharacter.name);
            diplomataEditor.SetWorkingContextMessagesId(currentContext.id);
            Init(State.Messages);
        }

        public static void Reset(string characterName) {
            if (character != null) {
                if (character.name == characterName) {
                    diplomataEditor.SetWorkingCharacter(string.Empty);
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
                    if (diplomataEditor.workingCharacter != string.Empty) {
                        character = Character.Find(diplomataEditor.characters, diplomataEditor.workingCharacter);

                        if (diplomataEditor.workingContextMessagesId > -1) {
                            context = Context.Find(character, diplomataEditor.workingContextMessagesId);
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
            AutoSave();
        }

        private void AutoSave() {

            if (iteractions == 100 && character != null) {
                diplomataEditor.Save(character);
                iteractions = 0;
            }

            iteractions++;

        }

        public void OnDisable() {
            if (character != null) {
                diplomataEditor.Save(character);
            }
        }
    }

}
