//-----------------------------------------------------------------------
// <filename>SimpleTreeViewWindow</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/19 星期三 12:12:29# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameFramework.Taurus
{
    public class SimpleTreeViewWindow : EditorWindow
    {
        ATTabelView m_SimpleTreeView;
        List<TestUnit> testList = new List<TestUnit>();
        [SerializeField] TreeViewState m_TreeViewState;
        [NonSerialized] bool m_Initialized;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        SearchField m_SearchField;


        Rect multiColumnTreeViewRect
        {
            get { return new Rect(20, 30, position.width - 40, 400); }
        }

        Rect toolbarRect
        {
            get { return new Rect(20f, 10f, position.width - 40f, 20f); }
        }

        Rect UnitButtonRect
        {
            get { return new Rect(20, 435, 100, 20); }
        }

        Rect DebugBarLineRect
        {
            get { return new Rect(20, 460, position.width - 40, 20); }
        }

        Rect DebugBarViewRect
        {
            get { return new Rect(20, 480, position.width - 40, 200); }
        }

        Rect DebugViewRect
        {
            get { return new Rect(0, 0, DebugBarViewRect.width - 40, DebugBarViewRect.height * 2); }
        }

        private void OnEnable()
        {
            testList.Add(new TestUnit("name1", "success"));
            testList.Add(new TestUnit("qqeqame2", "success"));
            testList.Add(new TestUnit("name3", "success"));
            testList.Add(new TestUnit("qeqe", "false"));
            testList.Add(new TestUnit("name5", "success"));
            testList.Add(new TestUnit("eqwq", "success"));
            testList.Add(new TestUnit("name7", "success"));
            for (int i = 0; i < 100; i++)
            {
                testList.Add(new TestUnit("name1", "success"));
                LogSet.Add(new Tuple<ToolLogType, string>(ToolLogType.Info, "11"));
            }

        }

        private void OnGUI()
        {
            InitIfNeeded();

            EditorGUILayout.BeginVertical();
            OnDrawToolBar();
            OnDrawTreeView();
            OnDrawFunctionBar();
            OnDrawDebugLog();
            EditorGUILayout.EndVertical();
        }

        private void OnDrawFunctionBar()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUI.Button(UnitButtonRect, "单元测试"))
            {
                //TODO
                //执行单元测试
            }
            EditorGUILayout.EndHorizontal();
        }

        //line
        private string _line = new string('_', 150);//分割线
        Vector2 _scroll_debug_position = new Vector2();
        //Log      
        public List<Tuple<ToolLogType, string>> LogSet = new List<Tuple<ToolLogType, string>>();
        private Color _logArea = new Color(0.851f, 0.227f, 0.286f, 1.0f);
        private Color curColor = Color.white;
        private Color _normal = Color.white;
        private Color _info = Color.green;
        private Color _warning = Color.yellow;
        private Color _error = Color.red;
        private void OnDrawDebugLog()
        {
            GUI.color = Color.white;
            GUI.Label(DebugBarLineRect, _line);
            GUI.color = _normal;

            //Debug视图 
            _scroll_debug_position = GUI.BeginScrollView(DebugBarViewRect, _scroll_debug_position, DebugViewRect);

            foreach (var log in LogSet)
            {
                switch (log.Item1)
                {
                    case ToolLogType.Normal:
                        curColor = _normal;
                        break;
                    case ToolLogType.Info:
                        curColor = _info;
                        break;
                    case ToolLogType.Warning:
                        curColor = _warning;
                        break;
                    case ToolLogType.Error:
                        curColor = _error;
                        break;
                }
                GUI.color = curColor;
                GUILayout.Label(log.Item2);
                GUI.color = Color.white;
            }

            GUI.EndScrollView();
        }

        void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                // Check if it already exists (deserialized from window layout file or scriptable object)
                if (m_TreeViewState == null)
                    m_TreeViewState = new TreeViewState();

                bool firstInit = m_MultiColumnHeaderState == null;
                var headerState = ATTabelView.CreateDefaultMultiColumnHeaderState();
                if (MultiColumnHeaderState.CanOverwriteSerializedFields(m_MultiColumnHeaderState, headerState))
                    MultiColumnHeaderState.OverwriteSerializedFields(m_MultiColumnHeaderState, headerState);
                m_MultiColumnHeaderState = headerState;

                var multiColumnHeader = new MultiColumnHeader(headerState);
                if (firstInit)
                    multiColumnHeader.ResizeToFit();

                m_SimpleTreeView = new ATTabelView(m_TreeViewState, multiColumnHeader, testList);
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += m_SimpleTreeView.SetFocusAndEnsureSelectedItem;
                m_Initialized = true;
            }
        }

        void OnDrawTreeView()
        {
            m_SimpleTreeView.OnGUI(multiColumnTreeViewRect);
        }

        private void OnDrawToolBar()
        {
            m_SimpleTreeView.searchString = m_SearchField.OnGUI(toolbarRect, m_SimpleTreeView.searchString);
        }

        //[MenuItem("TreeViewTest/SimpleTreeViewWindow")]
        //public static void TreeViewTest()
        //{
        //    var window = GetWindow<SimpleTreeViewWindow>();
        //    window.titleContent = new GUIContent("ILRuntime测试工具");
        //    window.minSize = new Vector2(1000, 654);
        //    window.maxSize = new Vector2(1000, 654);
        //    window.Show();
        //}
    }
}
