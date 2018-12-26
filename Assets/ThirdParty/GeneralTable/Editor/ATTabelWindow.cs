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
    public class ATTabelWindow
    {
        public ATTabelView ATTableView;
        List<TestUnit> testList;
        [SerializeField] TreeViewState m_TreeViewState;
        [NonSerialized] bool m_Initialized;
        [SerializeField] MultiColumnHeaderState m_MultiColumnHeaderState;
        SearchField m_SearchField;

        /// <summary>
        /// 执行选中的单元测试
        /// </summary>
        private Action<List<TestUnit>> ExcuteUnitsTest;

        public ATTabelWindow(Action<List<TestUnit>> UnitCallBack)
        {
            testList = new List<TestUnit>();
            ExcuteUnitsTest = UnitCallBack;
        }

        public void OnGUI()
        {
            InitIfNeeded();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            OnDrawToolBar();
            OnDrawTreeView();
            OnDrawFunctionBar();
            OnDrawDebugLog();
            EditorGUILayout.EndVertical();
        }

        Rect TestButtonRect
        {
            get { return new Rect(30, 380, 100, 20); }
        }

        Rect LineRect
        {
            get { return new Rect(30, 390, Screen.width - 40, 20); }
        }

        Rect LogAreaRect
        {
            get { return new Rect(30, 430, Screen.width - 40, 200); }
        }

        Rect ShowLogAreaRect
        {
            get { return new Rect(0, 50, Screen.width - 60, LogAreaRect.height*10); }
        }


        private void OnDrawFunctionBar()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUI.Button(TestButtonRect,"单元测试"))
            {
                //TODO
                //执行单元测试
                ExcuteUnitsTest(ATTableView.GetSelectedUnits());
            }
            EditorGUILayout.EndHorizontal();
        }

        //line
        private string _line = new string('_', 150);//分割线
        Vector2 _scroll_debug_position = Vector2.zero;
        //Log      
        public List<Tuple<ToolLogType, string>> LogSet = new List<Tuple<ToolLogType, string>>();

        public void Log(ToolLogType type, string msg)
        {
            LogSet.Add(new Tuple<ToolLogType, string>(type, msg));
        }
        private Color _logArea = new Color(0.851f, 0.227f, 0.286f, 1.0f);
        private Color curColor = Color.white;
        private Color _normal = Color.white;
        private Color _info = Color.green;
        private Color _warning = Color.yellow;
        private Color _error = Color.red;
        private void OnDrawDebugLog()
        {
            GUI.color = Color.white;
            GUI.Label(LineRect,_line);
            GUI.color = _normal;

            //Debug视图 
            _scroll_debug_position = GUI.BeginScrollView(LogAreaRect,_scroll_debug_position, ShowLogAreaRect,false,true);

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

                ATTableView = new ATTabelView(m_TreeViewState, multiColumnHeader, testList);
                m_SearchField = new SearchField();
                m_SearchField.downOrUpArrowKeyPressed += ATTableView.SetFocusAndEnsureSelectedItem;
                m_Initialized = true;
            }
        }

        void OnDrawTreeView()
        {
            ATTableView.OnGUI(new Rect(30,65,Screen.width-80,300));
        }

        private void OnDrawToolBar()
        {
            ATTableView.searchString = m_SearchField.OnGUI( ATTableView.searchString);
        }
    }
}
