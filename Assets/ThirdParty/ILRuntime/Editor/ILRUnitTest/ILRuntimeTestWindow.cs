//-----------------------------------------------------------------------
// <filename>ILRuntimeTestWindow</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #ILRuntime 测试# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/17 星期一 12:59:38# </time>
//-----------------------------------------------------------------------
using ILRuntime;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace GameFramework.Taurus.UnityEditor
{
    public class ILRuntimeTestWindow:EditorWindow
    {
        #region 字段&属性
        private static ILRuntimeTestWindow _instance = null;
        public static ILRuntimeTestWindow Instance
        {

            get {
                if (_instance == null)
                    _instance = GetWindow<ILRuntimeTestWindow>();
                return _instance;
            }
        }

        /// <summary>
        /// IL域
        /// </summary>
        public static AppDomain App;

        private bool _isLoadAssembly;

        private string _hotfix_dll_path;
        private List<BaseTestUnit> _testUnitList = new List<BaseTestUnit>();//测试单元

        private Vector2 _scrollPositon;
        private ATTabelWindow _atTableWindow;
        
        #endregion

        #region 周期函数
        private void OnEnable()
        {
            _hotfix_dll_path = PathConfig.GetPath(ILPath.ILAssemblyPath);
            _atTableWindow = new ATTabelWindow(UnitTests);
            _isLoadAssembly = false;

            App = new AppDomain();
            if (App == null)
            {
                UnityEngine.Debug.LogError("appdomain == null in initialization...");
                return;
            }

            //开启Debug调试
            App.DebugService.StartDebugService(56000);
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            _hotfix_dll_path = EditorGUILayout.TextField("热更代码DLL路径: ", _hotfix_dll_path,GUILayout.Width(800));

            if (GUILayout.Button("select", GUILayout.Width(50), GUILayout.Height(20)))
            {
                _hotfix_dll_path = EditorUtility.OpenFilePanelWithFilters("Hotfix Assembly", PathConfig.GetPath(ILPath.HotfixFolder),
                    new string[] { "BYTES", "bytes" });
                if (!string.IsNullOrWhiteSpace(_hotfix_dll_path))
                {
                    PathConfig.SetPath(ILPath.ILAssemblyPath, _hotfix_dll_path);
                }
            }

            if (GUILayout.Button("load", GUILayout.Width(50), GUILayout.Height(20)))
            {
                if (!_isLoadAssembly)
                {
                    InitializeAppDomain();
                }

                LoadAllUnitTest();
                _atTableWindow.ATTableView.UpdateTableData(_testUnitList);
            }

            GUILayout.EndHorizontal();

            if (_atTableWindow != null)
            {
                _atTableWindow.OnGUI();
            }

            GUILayout.EndVertical();
        }


        /// <summary>
        /// load all unit test
        /// </summary>
        private void LoadAllUnitTest()
        {
            _testUnitList.Clear();

            var types = App.LoadedTypes.Values.ToList();

            foreach (var type in types)
            {
                //Debug.Log("type: " + type.FullName);
                var ilType = type as ILType;
                if (ilType == null)
                    continue;

                foreach (var methodInfo in ilType.GetMethods())
                {
                    string fullName = ilType.FullName;
                    //目前只支持无参数，无返回值测试
                    if (methodInfo.ParameterCount == 0 && methodInfo.IsStatic && ((ILRuntime.CLR.Method.ILMethod)methodInfo).Definition.IsPublic)
                    {
                        var testUnit = new StaticTestUnit();
                        testUnit.Init(App, fullName, methodInfo.Name);
                        _testUnitList.Add(testUnit);
                    }
                }
            }

        }
        #endregion

        #region  内部函数

        /// <summary>
        /// load hotfix code && initialize adapter and binding
        /// </summary>
        private void InitializeAppDomain()
        {
            if (string.IsNullOrWhiteSpace(_hotfix_dll_path))
            {
                UnityEngine.Debug.LogError("hotfix dll path == null");
                return;
            }

            try
            {
                using (FileStream fs = new FileStream(_hotfix_dll_path, FileMode.Open, FileAccess.Read))
                {
                    var path = Path.GetDirectoryName(_hotfix_dll_path);
                    var mdbPath = Path.Combine(path, "Hotfix.dll.mdb.bytes");
                    if (!File.Exists(mdbPath))
                    {
                        UnityEngine.Debug.LogError("hotfix.mdb.bytes 不存在!");
                    }

                    //mdb 调试有问题，故此处省去
                    App.LoadAssembly(fs, null, null);
                    _isLoadAssembly = true;

                    //委托
                    ILRuntime.ILRuntimeHelper.Init(App);
                    //绑定
                    ILRuntime.Runtime.Generated.CLRBindings.Initialize(App);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("read hot fix dll error! \n" + ex.Message + "\n source: " + ex.Source + "\n Trace: \n" + ex.StackTrace);
                return;
            }
        }

        public void UnitTests(List<TestUnit> units)
        {
            if (units == null)
                return;

            foreach (var item in units)
            {
                for (int i = 0; i < _testUnitList.Count; i++)
                {
                    if (item.TestName.ToLower() == _testUnitList[i].TestName.ToLower())
                    {
                        _testUnitList[i].Run();
                        //
                        var res = _testUnitList[i].CheckResult();
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("<=======================");
                        sb.Append("Test:");
                        sb.AppendLine(res.TestName);
                        sb.Append("TestResult:");
                        sb.AppendLine(res.Result.ToString());
                        sb.AppendLine("Log:");
                        sb.AppendLine(res.Message);
                        sb.AppendLine("=======================>");

                        UnityEngine.Debug.Log("Log:\n" + sb.ToString());
                        //
                        _atTableWindow.Log(res.Result?ToolLogType.Info:ToolLogType.Error, sb.ToString());
                        break;
                    }
                        
                }
            }

            _atTableWindow.ATTableView.UpdateTableData(_testUnitList);
        }

        #endregion

        #region 编辑器菜单
        [MenuItem("ILRuntime/ILRuntime Unit",false,200)]
        static void ShowILRuntimeUnitWindow()
        {
            ILRuntimeTestWindow window = Instance;
            window.titleContent = new GUIContent("热更代码单元测试");
            window.minSize = new Vector2(1000, 654);
            window.maxSize = new Vector2(1000, 654);
            window.Show();
        }
        #endregion

    }
}
