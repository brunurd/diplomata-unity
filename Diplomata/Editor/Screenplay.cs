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
            document.Init(21, 29.7f, RTFDocument.Orientation.Portrait, RTFDocument.Units.Centimeters);
            document.margin = new RTFDocument.Margin(2.54f, 2.54f, 2.54f, 2.54f);

            if (Environment.UserName != null) {
                document.author = Environment.UserName;
            }

            return document;
        }

        private static RTFDocument AddCharacter(RTFDocument document, Character character) {
            RTFStyle style = new RTFStyle(false, false, 12, "Courier", Color.black);
            RTFStyle styleAllcaps = new RTFStyle(false, false, false, false, true, false, 12, "Courier", Color.black, RTFStyle.Underline.None);

            RTFParagraph.Indent noIndent = new RTFParagraph.Indent(0, 0, 0);
            RTFParagraph.Indent messageContentIndent = new RTFParagraph.Indent(0, 2.54f, 0);

            var presentation = document.AppendParagraph(RTFParagraph.Alignment.Left, noIndent);
            presentation.AppendText(character.name, styleAllcaps);

            presentation.AppendText(", " + character.description, style);

            foreach (Context context in character.contexts) {

                var contextNamePar = document.AppendParagraph(RTFParagraph.Alignment.Left, noIndent);
                var name = DictHandler.ContainsKey(context.name, character.currentLanguage);
                contextNamePar.AppendText(name.value, styleAllcaps);

                var contextDescriptionPar = document.AppendParagraph(RTFParagraph.Alignment.Left, noIndent);
                var description = DictHandler.ContainsKey(context.description, character.currentLanguage);
                contextDescriptionPar.AppendText(description.value, style);

                foreach (Column column in context.columns) {
                    foreach (Message message in column.messages) {
                        var messagePar = document.AppendParagraph(RTFParagraph.Alignment.Left, messageContentIndent);
                        messagePar.AppendText("\t" + message.emitter + "\n", styleAllcaps);
                        var content = DictHandler.ContainsKey(message.content, character.currentLanguage);
                        messagePar.AppendText(content.value, style);
                    }
                }

            }

            document.AppendParagraph().AppendText("\n");

            return document;
        }
    }

}