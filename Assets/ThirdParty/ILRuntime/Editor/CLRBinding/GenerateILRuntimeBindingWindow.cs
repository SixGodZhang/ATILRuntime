//-----------------------------------------------------------------------
// <filename>GenerateILRuntimeBindingWindow</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/13 星期四 16:34:08# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
    public class GenerateILRuntimeBindingWindow: EditorWindow
    {
        #region 属性&字段
        private string _output_path = "";
        private string _hotfix_path = "";
        //line
        private string _line = new string('_', 150);//分割线

        private bool isClickAnalysis;

        private static GenerateILRuntimeBindingWindow _instance;

        public static GenerateILRuntimeBindingWindow Instance
        {
            get {
                if (_instance == null)
                    _instance = GetWindow<GenerateILRuntimeBindingWindow>();
                return _instance;
            }
        }

        private void OnEnable()
        {
            _hotfix_path = EditorPrefs.GetString("hotfix_path");
            _output_path = EditorPrefs.GetString("output_path");
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            _output_path = EditorGUILayout.TextField("输出目录: ", _output_path);
            if (GUILayout.Button("选择输出文件夹", GUILayout.Width(100), GUILayout.Height(20)))
            {
                _output_path = EditorUtility.OpenFolderPanel("选择文件夹", string.IsNullOrWhiteSpace(_output_path) ? Application.dataPath : _output_path, "");
                if (!string.IsNullOrWhiteSpace(_output_path))
                {
                    EditorPrefs.SetString("output_path", _output_path);
                }
                
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            _hotfix_path = EditorGUILayout.TextField("热更Assembly: ", _hotfix_path);

            if (GUILayout.Button("选择热更Assembly", GUILayout.Width(150), GUILayout.Height(20)))
            {
                _hotfix_path = EditorUtility.OpenFilePanelWithFilters("选择程序集", Application.dataPath + "/Game/Hotfix", new string[] { "BYTES", "bytes" });
                if(!string.IsNullOrWhiteSpace(_hotfix_path))
                    EditorPrefs.SetString("hotfix_path", _hotfix_path);
            }

            if (GUILayout.Button("Analysis", GUILayout.Width(100), GUILayout.Height(20)))
            {
                if (!isClickAnalysis)
                {
                    isClickAnalysis = true;
                    AnalysisBinding_Click();
                    isClickAnalysis = false;
                    AssetDatabase.Refresh();
                    EditorGUIUtility.ExitGUI();
                }
            }

            GUILayout.EndHorizontal();
            GUILayout.Label(_line);

            GUILayout.EndVertical();
        }
        #endregion

        #region 函数
        private void AnalysisBinding_Click()
        {
            Debug.Log(_output_path);
            ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
            using (FileStream fs = new FileStream(_hotfix_path, FileMode.Open, FileAccess.Read))
            {
                domain.LoadAssembly(fs);
            }
            //Crossbind Adapter is needed to generate the correct binding code
            ILRuntime.ILRuntimeHelper.Init(domain);

            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, _output_path);
        }
        #endregion

        #region 编辑器菜单
        [MenuItem("ILRuntime/Binding",false,110)]
        static void ShowGenerateILRuntimeAdapterWindow()
        {
            GenerateILRuntimeBindingWindow window = Instance;
            window.titleContent = new GUIContent("绑定自动分析工具");
            window.minSize = new Vector2(1000, 654);
            window.maxSize = new Vector2(1000, 654);
            window.Show();
        }
        #endregion
    }
}
