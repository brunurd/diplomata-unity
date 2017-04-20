using System;
using UnityEditor;
using RTFExporter;
using DiplomataLib;

namespace DiplomataEditor {

    public class Screenplay {

        public static void Save(Character character) {
            RTFDocument doc = CreateDocument();
            doc = AddCharacter(doc, character);

            RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay - " + character.name + ".rtf", doc);
            AssetDatabase.Refresh();
        }

        public static void SaveAll() {
            RTFDocument doc = CreateDocument();
            
            foreach (Character character in Diplomata.characters) {
                doc = AddCharacter(doc, character);
            }

            RTFParser.ToFile("Assets/" + PlayerSettings.productName + " Screenplay.rtf", doc);
            AssetDatabase.Refresh();
        }

        private static RTFDocument CreateDocument() {
            RTFDocument document = new RTFDocument();
            document.Init(21, 29.7f, Orientation.Portrait, Units.Centimeters);
            document.margin = new Margin(2.54f, 2.54f, 2.54f, 2.54f);

            if (Environment.UserName != null) {
                document.author = Environment.UserName;
            }

            return document;
        }

        private static RTFDocument AddCharacter(RTFDocument document, Character character) {
            RTFTextStyle style = new RTFTextStyle(false, false, 12, "Courier", Color.black);
            RTFTextStyle styleAllcaps = new RTFTextStyle(false, false, false, false, true, false, 12, "Courier", Color.black, Underline.None);

            RTFParagraphStyle noIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 0, 0), 0, 400);
            RTFParagraphStyle messageContentIndent = new RTFParagraphStyle(Alignment.Left, new Indent(0, 2.54f, 0), 0, 400);

            var presentation = document.AppendParagraph(noIndent);
            presentation.AppendText(character.name, styleAllcaps);
            
            var characterDescription = DictHandler.ContainsKey(character.description, Diplomata.preferences.currentLanguage);

            var text = ", " + characterDescription.value;

            if (text[text.Length - 1] != '.') {
                text += ".";
            }

            presentation.AppendText(text, style);
            
            foreach (Context context in character.contexts) {

                var contextPar = document.AppendParagraph(noIndent);

                var name = DictHandler.ContainsKey(context.name, Diplomata.preferences.currentLanguage);
                var contextDescription = DictHandler.ContainsKey(context.description, Diplomata.preferences.currentLanguage);

                contextPar.AppendText(name.value + "\n", styleAllcaps);
                contextPar.AppendText(contextDescription.value, style);

                foreach (Column column in context.columns) {
                    for (int i = 0; i < column.messages.Length; i++) {
                        var messagePar = document.AppendParagraph(messageContentIndent);
                        messagePar.AppendText("\t\t" + column.messages[i].emitter + "\n", styleAllcaps);

                        var screenplayNotes = DictHandler.ContainsKey(column.messages[i].screenplayNotes, Diplomata.preferences.currentLanguage);

                        if (screenplayNotes != null) {
                            if (screenplayNotes.value != "") {
                                messagePar.AppendText("\t(" + screenplayNotes.value + ")\n", style);
                            }
                        }

                        var content = DictHandler.ContainsKey(column.messages[i].content, Diplomata.preferences.currentLanguage);

                        if (column.messages.Length > 1) {
                            messagePar.AppendText("(" + i + "): " + content.value, style);
                        }

                        else {
                            messagePar.AppendText(content.value, style);
                        }
                    }
                }

            }

            return document;
        }
    }

}