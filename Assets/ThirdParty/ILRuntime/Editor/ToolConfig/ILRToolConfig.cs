//-----------------------------------------------------------------------
// <filename>ILRToolConfig</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #ILRuntime 配置# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/19 星期三 20:23:50# </time>
//-----------------------------------------------------------------------

using GameFramework.Taurus;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace GameFramework.Taurus
{
    public class ILRToolConfig:EditorWindow
    {
        #region 属性&字段
        private static ILRToolConfig _instance;

        public static ILRToolConfig Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GetWindow<ILRToolConfig>();
                return _instance;
            }
        }

        private string _injectAssembly;
        private string _hotfixFolder;
        //line
        private string _line = new string('_', 150);//分割线
        private WhiteTreeView _whiteTreeView;
        [SerializeField] TreeViewState m_TreeViewState;
        #endregion

        #region 周期函数
        private void OnEnable()
        {
            _hotfixFolder = EditorPrefs.GetString("hotfixFolder");
            _injectAssembly = EditorPrefs.GetString("InjectAssembly");
            if (m_TreeViewState == null)
                m_TreeViewState = new TreeViewState();

            _whiteTreeView = new WhiteTreeView(m_TreeViewState);
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _hotfixFolder = EditorGUILayout.TextField("热更DLL文件夹", _hotfixFolder);
            if (GUILayout.Button("选择输出文件夹", GUILayout.Width(100), GUILayout.Height(20)))
            {
                _hotfixFolder = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath, "");
                if (!string.IsNullOrWhiteSpace(_hotfixFolder))
                {
                    EditorPrefs.SetString("hotfixFolder", _hotfixFolder);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _injectAssembly = EditorGUILayout.TextField("Inject DLL Path: ", _injectAssembly);
            if (GUILayout.Button("select", GUILayout.Width(100), GUILayout.Height(20)))
            {
                string path = Application.dataPath.Replace("Assets", "") + "Library\\ScriptAssemblies";
                _injectAssembly = EditorUtility.OpenFilePanelWithFilters("select Inject Assembly", path,
                    new string[] { "All files", "dll" });
                if (!string.IsNullOrWhiteSpace(_injectAssembly))
                    EditorPrefs.SetString("InjectAssembly", _injectAssembly);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.LabelField("第三方DLL白名单 Type&Interface");
            EditorGUILayout.LabelField(_line);
            DrawWhiteGUI();
            EditorGUILayout.EndVertical();
        }


        void DrawWhiteGUI()
        {
            Rect rect = GUILayoutUtility.GetRect(0, 100000, 0, 100000);
            _whiteTreeView.OnGUI(rect);
        }
        #endregion

        #region 内部函数

        #endregion

        #region Unity窗口工具
        [MenuItem("ILRuntime/ILRConfig",false,10)]
        static void ILRConfig()
        {
            ILRToolConfig window = Instance;
            window.titleContent = new GUIContent("ILR 工具配置");
            window.minSize = new Vector2(1000, 654);
            window.maxSize = new Vector2(1000, 654);
            window.Show();
        }
        #endregion

        #region 内部类

        #endregion
    }


}
