//-----------------------------------------------------------------------
// <filename>GameFrameworkEditorTool</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #GameFramework 工具组# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/14 星期五 10:43:47# </time>
//-----------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
    public static class GFGUITool
    {

        public static GUIContent iconToolbarPlusMore = EditorGUIUtility.TrIconContent("Toolbar Plus More", "Choose to add to list");
        public static GUIContent iconToolbarMinus = EditorGUIUtility.TrIconContent("Toolbar Minus", "Remove selection from list");
        public static Rect footerRect = GUILayoutUtility.GetRect(500, 500, GUILayout.ExpandWidth(true));

        #region DrawPadding
        static public void DrawPadding()
        {
            if (!GFConfigEditor.minimalisticLook)
                GUILayout.Space(18f);
        }
        #endregion

        #region DrawProperty
        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>
        public static SerializedProperty DrawProperty(this SerializedObject serializedObject, string property, params GUILayoutOption[] options)
        {
            return DrawProperty(null, serializedObject, property, false, options);
        }

        /// <summary>
        /// Helper function that draws a serialized property.
        /// </summary>
        static public SerializedProperty DrawProperty(string label, SerializedObject serializedObject, string property, bool padding, params GUILayoutOption[] options)
        {
            SerializedProperty sp = serializedObject.FindProperty(property);

            if (sp != null)
            {
                if (GFConfigEditor.minimalisticLook) padding = false;

                if (padding) EditorGUILayout.BeginHorizontal();

                if (sp.isArray && sp.type != "string") DrawArray(serializedObject, property, label ?? property);
                else if (label != null) EditorGUILayout.PropertyField(sp, new GUIContent(label), options);
                else EditorGUILayout.PropertyField(sp, options);

                if (padding)
                {
                    DrawPadding();
                    EditorGUILayout.EndHorizontal();
                }
            }
            else Debug.LogWarning("Unable to find property " + property);
            return sp;
        }
        #endregion


        #region DrawArray
        /// <summary>
        /// Helper function that draws an array property.
        /// </summary>

        static public void DrawArray(this SerializedObject obj, string property, string title)
        {
            SerializedProperty sp = obj.FindProperty(property + ".Array.size");

            if (sp != null && GFGUITool.DrawHeader(title))
            {
                GFGUITool.BeginContents();
                int size = sp.intValue;
                int newSize = EditorGUILayout.IntField("Size", size);
                if (newSize != size) obj.FindProperty(property + ".Array.size").intValue = newSize;

                EditorGUI.indentLevel = 1;

                for (int i = 0; i < newSize; i++)
                {
                    SerializedProperty p = obj.FindProperty(string.Format("{0}.Array.data[{1}]", property, i));
                    if (p != null) EditorGUILayout.PropertyField(p);
                }
                EditorGUI.indentLevel = 0;
                GFGUITool.EndContents();
            }
        }
        #endregion

        #region DrawHeader
        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text) { return DrawHeader(text, text, false, GFConfigEditor.minimalisticLook); }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, string key) { return DrawHeader(text, key, false, GFConfigEditor.minimalisticLook); }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, bool detailed) { return DrawHeader(text, text, detailed, !detailed); }

        /// <summary>
        /// Draw a distinctly different looking header label
        /// </summary>

        static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
        {
            bool state = EditorPrefs.GetBool(key, true);

            if (!minimalistic) GUILayout.Space(3f);
            if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
            GUILayout.BeginHorizontal();
            GUI.changed = false;

            if (minimalistic)
            {
                if (state) text = "\u25BC" + (char)0x200a + text;
                else text = "\u25BA" + (char)0x200a + text;

                GUILayout.BeginHorizontal();
                GUI.contentColor = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.7f) : new Color(0f, 0f, 0f, 0.7f);
                if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
                GUI.contentColor = Color.white;
                GUILayout.EndHorizontal();
            }
            else
            {
                text = "<b><size=11>" + text + "</size></b>";
                if (state) text = "\u25BC " + text;
                else text = "\u25BA " + text;
                if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
            }

            if (GUI.changed) EditorPrefs.SetBool(key, state);

            if (!minimalistic) GUILayout.Space(2f);
            GUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
            if (!forceOn && !state) GUILayout.Space(3f);
            return state;
        }
        #endregion

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>

        static public void BeginContents() { BeginContents(GFConfigEditor.minimalisticLook); }

        static bool mEndHorizontal = false;

#if UNITY_4_7 || UNITY_5_5 || UNITY_5_6
	static public string textArea = "AS TextArea";
#else
        static public string textArea = "TextArea";
#endif

        /// <summary>
        /// Begin drawing the content area.
        /// </summary>

        static public void BeginContents(bool minimalistic)
        {
            if (!minimalistic)
            {
                mEndHorizontal = true;
                GUILayout.BeginHorizontal();
                EditorGUILayout.BeginHorizontal(textArea, GUILayout.MinHeight(10f));
            }
            else
            {
                mEndHorizontal = false;
                EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
                GUILayout.Space(10f);
            }
            GUILayout.BeginVertical();
            GUILayout.Space(2f);
        }

        /// <summary>
        /// End drawing the content area.
        /// </summary>

        static public void EndContents()
        {
            GUILayout.Space(3f);
            GUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();

            if (mEndHorizontal)
            {
                GUILayout.Space(3f);
                GUILayout.EndHorizontal();
            }

            GUILayout.Space(3f);
        }
    }
}
