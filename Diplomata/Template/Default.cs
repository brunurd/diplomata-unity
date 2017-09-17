using DiplomataLib;

public class Default : Template {
    
    public void Update() {
        if (talk.character != null) {
            TalkStart();
            ChoiceMenu();
            MessageBox();
            TalkEnd();
        }
    }

    public void TalkStart() {
        talk.onStart = () => {
            // Do something on starting talking
        };

        talk.Start();
    }

    public void ChoiceMenu() {
        choiceMenu.onStart = () => {
            talk.character.SwapStaticSprite();
        };

        choiceMenu.Start();

        choiceMenu.onUpdate = () => {
            // Do something on choice menu every frame
        };

        choiceMenu.Update();

        choiceMenu.onEnd = () => {
            // Do something after every choice
        };
    }

    public void MessageBox() {
        messageBox.onStart = () => {
            talk.character.SwapStaticSprite();
            talk.character.SetAnimatorAttributes();
            talk.character.PlayMessageAudioContent();
        };

        messageBox.Start();

        messageBox.onUpdate = () => {
            // Do something on message box every frame
        };

        messageBox.Update();

        messageBox.onEnd = () => {
            talk.character.StopMessageAudioContent();
        };
    }

    public void TalkEnd() {
        talk.onEnd = () => {
            talk.character.ResetAnimators();
            talk.character.StopMessageAudioContent();
        };
    }
}