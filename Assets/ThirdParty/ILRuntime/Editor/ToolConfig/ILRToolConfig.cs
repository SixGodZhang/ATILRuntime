//-----------------------------------------------------------------------
// <filename>ILRToolConfig</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #ILRuntime 配置# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/19 星期三 20:23:50# </time>
//-----------------------------------------------------------------------

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

        private string _hotfixFolder;
        private string _templatePath;
        private string _bugfixPath;


        //line
        private string _line = new string('_', 150);//分割线
        private WhiteTreeView _whiteTreeView;
        [SerializeField] TreeViewState m_TreeViewState;
        #endregion

        #region 周期函数
        private void OnEnable()
        {
            _hotfixFolder = PathConfig.GetPath(ILPath.HotfixFolder);
            _templatePath = PathConfig.GetPath(ILPath.TemplatePath);
            _bugfixPath = PathConfig.GetPath(ILPath.HotBugfixNameSpace);
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
                    PathConfig.SetPath(ILPath.HotfixFolder, _hotfixFolder);
                }
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            _templatePath = EditorGUILayout.TextField("模板路径: ", _templatePath);
            if (GUILayout.Button("select", GUILayout.Width(100), GUILayout.Height(20)))
            {
                string tempPath = PathConfig.GetPath(ILPath.TemplatePath);//Application.dataPath + "/ThirdParty/ILRuntime/Editor/Adapter/Template";
                //tempPath = Direc
                _templatePath = EditorUtility.OpenFolderPanel("select Template Path", Application.dataPath, "");
                if (!string.IsNullOrWhiteSpace(_templatePath))
                {
                    _templatePath = _templatePath + "/";//folder
                    PathConfig.SetPath(ILPath.TemplatePath, _templatePath);
                }
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

        /// <summary>
        /// 清楚之前版本存储在EditorPref中的所有键值对
        /// </summary>
        //[MenuItem("ILRuntime/ClearILRConfig", false, 10)]
        //static void ClearILRConfig()
        //{
        //    EditorPrefs.DeleteAll();
        //    Debug.Log("!!! Clear ILRConfig Command end...");
        //}

        #endregion

        #region 内部类

        #endregion
    }


}
