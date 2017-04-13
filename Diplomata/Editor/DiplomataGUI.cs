using System;
using UnityEngine;
using UnityEditor;

namespace DiplomataEditor {

    public class DGUI {

        public const byte MARGIN = 10;

        public const byte BUTTON_HEIGHT_SMALL = 20;
        public const byte BUTTON_HEIGHT = 30;
        public const byte BUTTON_HEIGHT_BIG = 40;

        public const int WINDOW_MIN_WIDTH = 400;

        public static Rect noClipRect;

        public static void Vertical(Action callback) {
            GUILayout.BeginVertical();
            callback();
            GUILayout.EndVertical();
        }

        public static void Vertical(Action callback, int height) {
            GUILayout.BeginVertical(GUILayout.Height(height));
            callback();
            GUILayout.EndVertical();
        }

        public static void Vertical(Action callback, params GUILayoutOption[] options) {
            GUILayout.BeginVertical(options);
            callback();
            GUILayout.EndVertical();
        }

        public static void Horizontal(Action callback) {
            GUILayout.BeginHorizontal();
            callback();
            GUILayout.EndHorizontal();
        }

        public static void Horizontal(Action callback, int width) {
            GUILayout.BeginHorizontal(GUILayout.Width(width));
            callback();
            GUILayout.EndHorizontal();
        }

        public static void Horizontal(Action callback, params GUILayoutOption[] options) {
            GUILayout.BeginHorizontal(options);
            callback();
            GUILayout.EndHorizontal();
        }

        public static void WindowWrap(Action callback) {
            Horizontal(() => {
                GUILayout.Space(MARGIN);

                Vertical(() => {
                    GUILayout.Space(MARGIN);

                    callback();

                    GUILayout.Space(MARGIN);
                });

                GUILayout.Space(MARGIN);
            });
        }

        public static Vector2 ScrollWrap(Action callback, Vector2 scrollPosInput, Rect clipRect, Rect contentRect) {
            Vector2 scrollPosOutput = new Vector2(0, 0);
            scrollPosOutput = scrollPosInput;

            noClipRect = new Rect(0, 0, Screen.width, Screen.height - 22);

            scrollPosOutput = GUI.BeginScrollView(clipRect, scrollPosOutput, contentRect);

            Horizontal(() => {
                GUILayout.Space(MARGIN);

                Vertical(() => {
                    GUILayout.Space(MARGIN);

                    callback();

                    GUILayout.Space(MARGIN);
                });

                if (Screen.height - 22 < contentRect.height) {
                    GUILayout.Space(MARGIN);
                }

                GUILayout.Space(MARGIN);
            });

            GUI.EndScrollView();

            return scrollPosOutput;
        }

        public static Vector2 ScrollWindow(Action callback, Vector2 scrollPosInput, int contentHeight) {
            return ScrollWrap(callback, scrollPosInput, noClipRect, new Rect(0, 0, Screen.width - 15, contentHeight));
        }

        public static void Focus(Action callback, string name = "focus") {
            GUI.SetNextControlName(name);
            callback();
            EditorGUI.FocusTextInControl(name);
        }

        public static void Area(Action callback, int x, int y, int width, int height) {
            GUIStyle style = new GUIStyle();
            style.border = new RectOffset();

            GUILayout.BeginArea(new Rect(x, y, width, height), style);
            callback();
            GUILayout.EndArea();
        }
    }

}