//-----------------------------------------------------------------------
// <filename>GenerateILRuntimeAdapterWindow</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #适配器生成窗口# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/13 星期四 12:23:25# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
    /// <summary>
    /// 工具的Log类型
    /// </summary>
    public enum ToolLogType
    {
        Normal,//正常,white
        Info,//重要,green
        Warning,//警告,yellow
        Error//错误,red
    }

    public class GenerateILRuntimeAdapterWindow: EditorWindow
    {
        #region 字段&属性
        public static GenerateILRuntimeAdapterWindow _instance = null;

        public static GenerateILRuntimeAdapterWindow Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = GetWindow<GenerateILRuntimeAdapterWindow>();
                }
                return _instance;
            }
        }

        private string _out_path = "";
        private string _main_assembly_path = "";
        private string _il_assembly_path = "";
        private Vector2 _scroll_position;

        //按钮状态
        private bool isClickLoadMainAssemblyBtn;
        private bool isClickLoadILAssemblyBtn;
        private bool isClickGenerateBtn;

        //line
        private string _line = new string('_', 150);//分割线

        //Log      
        public List<Tuple<ToolLogType, string>> LogSet = new List<Tuple<ToolLogType, string>>();
        private Color _logArea = new Color(0.851f, 0.227f, 0.286f, 1.0f);
        private Color curColor = Color.white;
        private Color _normal = Color.white;
        private Color _info = Color.green;
        private Color _warning = Color.yellow;
        private Color _error = Color.red;

        private readonly Dictionary<string, TypeDefinition> _adaptorSingleInterfaceDic = new Dictionary<string, TypeDefinition>();
        private readonly Dictionary<string, TypeDefinition> _adaptorDic = new Dictionary<string, TypeDefinition>();

        private readonly Dictionary<string, TypeDefinition> _delegateCovDic = new Dictionary<string, TypeDefinition>();
        private readonly Dictionary<string, TypeReference> _delegateRegDic = new Dictionary<string, TypeReference>();

        private AdaptorGenerator _adGenerator;
        private HelperGenerator _helpGenerator;
        private InterfaceAdapterGenerator _interfaceAdapterGenerator;

        private string _adaptorAttrName = "ILRuntime.Other.NeedAdaptorAttribute";
        private string _delegateAttrName = "ILRuntime.Other.DelegateExportAttribute";
        private string _singleInterfaceAttrName = "ILRuntime.Other.SingleInterfaceExportAttribute";
        #endregion

        #region 周期函数
        private void OnEnable()
        {
            _out_path = EditorPrefs.GetString("out_path");
            _main_assembly_path = EditorPrefs.GetString("main_assembly_path");
            _il_assembly_path = EditorPrefs.GetString("il_assembly_path");

            //LogSet.Clear();
            _adaptorSingleInterfaceDic.Clear();
            _adaptorDic.Clear();
            _delegateCovDic.Clear();
            _delegateRegDic.Clear();

            LoadTemplates();
        }

        private void OnGUI()
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            _out_path = EditorGUILayout.TextField("适配器输出目录: ", _out_path);

            if (GUILayout.Button("select", GUILayout.Width(50), GUILayout.Height(20)))
            {
                _out_path = EditorUtility.OpenFolderPanel("选择文件夹", Application.dataPath + "/ThirdParty/ILRuntime/ILRuntime/Runtime", "");
                EditorPrefs.SetString("out_path", _out_path);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            _main_assembly_path = EditorGUILayout.TextField("主工程Assembly", _main_assembly_path);
            if (GUILayout.Button("select",GUILayout.Width(50),GUILayout.Height(20)))
            {
                string path = Application.dataPath.Replace("Assets", "") + "Library\\ScriptAssemblies";
                _main_assembly_path = EditorUtility.OpenFilePanelWithFilters("选择主工程Assembly", path,
                    new string[] { "All files", "dll" });
                if (!string.IsNullOrWhiteSpace(_main_assembly_path))
                    EditorPrefs.SetString("main_assembly_path", _main_assembly_path);
            }

            if (GUILayout.Button("Load", GUILayout.Width(50), GUILayout.Height(20)))
            {
                //TODO
                //load main Assembly
                if (!isClickLoadMainAssemblyBtn)
                {
                    isClickLoadMainAssemblyBtn = true;
                    LoadMainProjectAssemblyClick();
                    isClickLoadMainAssemblyBtn = false;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            _il_assembly_path = EditorGUILayout.TextField("热更工程Assembly", _il_assembly_path);
            if (GUILayout.Button("select", GUILayout.Width(50), GUILayout.Height(20)))
            {
                _il_assembly_path = EditorUtility.OpenFilePanelWithFilters("热更工程Assembly", Application.dataPath + "/Game/Hotfix",
                    new string[] { "BYTES", "bytes" });

                if (!string.IsNullOrWhiteSpace(_il_assembly_path))
                    EditorPrefs.SetString("il_assembly_path", _il_assembly_path);
            }

            if (GUILayout.Button("Load", GUILayout.Width(50), GUILayout.Height(20)))
            {
                //TODO
                //load il Assembly
                if (!isClickLoadILAssemblyBtn)
                {
                    isClickLoadILAssemblyBtn = true;
                    LoadILScriptAssemblyClick();
                    isClickLoadILAssemblyBtn = false;
                }

            }

            GUILayout.EndHorizontal();
            GUILayout.Space(50);
            GUILayout.BeginHorizontal();
            GUILayout.Label(_out_path);
            if (GUILayout.Button("Generate", GUILayout.Width(80), GUILayout.Height(20)))
            {
                if (!isClickGenerateBtn)
                {
                    isClickGenerateBtn = true;
                    GenerateClick();
                    isClickGenerateBtn = false;
                    AssetDatabase.Refresh();
                    GUIUtility.ExitGUI();
                }
                
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear",GUILayout.Width(50),GUILayout.Height(20)))
            {
                LogSet.Clear();
            }
            GUILayout.EndHorizontal();

            GUILayout.Label(_line);
            //Debug视图 
            _scroll_position = GUILayout.BeginScrollView(_scroll_position);

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

            GUILayout.EndScrollView();

            GUILayout.Space(10);

            //if (isNeedRefresh)
            //{
            //    isNeedRefresh = false;
            //    GUILayout.BeginVertical();
            //}
            GUILayout.EndVertical();


        }
        #endregion

        #region 函数
        /// <summary>
        /// 加载所有模板
        /// </summary>
        private void LoadTemplates()
        {
            var tmpdPath = Application.dataPath + "/ThirdParty/ILRuntime/Editor/Adapter/Template/";
            _interfaceAdapterGenerator = new InterfaceAdapterGenerator();
            _interfaceAdapterGenerator.LoadTemplateFromFile(tmpdPath + "adaptor_single_interface.tmpd");

            _adGenerator = new AdaptorGenerator();
            _adGenerator.LoadTemplateFromFile(tmpdPath + "adaptor.tmpd");

            _helpGenerator = new HelperGenerator();
            _helpGenerator.LoadTemplateFromFile(tmpdPath + "helper.tmpd");
        }

        private void CreateILRuntimeHelper()
        {
            Print($"==================Begin create helper:=====================");

            _helpGenerator.LoadData(new Tuple<Dictionary<string, TypeDefinition>, Dictionary<string, TypeDefinition>, Dictionary<string, TypeReference>, Dictionary<string, TypeDefinition>>(_adaptorDic, _delegateCovDic, _delegateRegDic, _adaptorSingleInterfaceDic));
            var helperStr = _helpGenerator.Generate();

            using (var fs2 = File.Create(_out_path + "/helper.cs"))
            {
                var sw = new StreamWriter(fs2);
                sw.Write(helperStr);
                sw.Flush();
            }

            Print($"==============End create helper:===================");
        }

        private void CreateInterfaceAdapter(TypeDefinition type)
        {
            if (!type.IsInterface)
                return;

            Print($"================begin create interface adaptor:{type.Name}=======================");

            var adaptorName = type.Name + "Adaptor";


            using (var fs = File.Create(_out_path + "/" + adaptorName + ".cs"))
            {

                _interfaceAdapterGenerator.LoadData(type);
                var classbody = _interfaceAdapterGenerator.Generate();

                var sw = new StreamWriter(fs);
                sw.Write(classbody);
                sw.Flush();
            }

            Print($"================end create interface adaptor:{type.Name}=======================");
        }

        private void CreateAdaptor(TypeDefinition type)
        {
            if (type.IsInterface)
                return;


            Print($"================begin create adaptor:{type.Name}=======================");

            var adaptorName = type.Name + "Adaptor";

            using (var fs = File.Create(_out_path+"/" + adaptorName + ".cs"))
            {

                _adGenerator.LoadData(type);
                var classbody = _adGenerator.Generate();

                var sw = new StreamWriter(fs);
                sw.Write(classbody);
                sw.Flush();
            }

            Print($"================end create adaptor:{type.Name}=======================");

        }

        /// <summary>
        /// 生成
        /// </summary>
        private void GenerateClick()
        {
            if (_adaptorDic.Count <= 0 && _adaptorSingleInterfaceDic.Count <= 0 && _delegateCovDic.Count <= 0 && _delegateRegDic.Count <= 0)
            {
                Print("[Warnning] There is nothing to Generate",ToolLogType.Warning);
                return;
            }

            Print("===============================Clear Old Files================================");
            var files = Directory.GetFiles(_out_path);
            foreach (var file in files)
            {
                File.Delete(file);
            }

            Print("[=============================Generate Begin==============================]");

            foreach (var itemInterface in _adaptorSingleInterfaceDic.Values)
            {
                CreateInterfaceAdapter(itemInterface);
            }

            foreach (var type in _adaptorDic.Values)
            {
                CreateAdaptor(type);
            }

            CreateILRuntimeHelper();

            Print("[=============================Generate End=================================]");
        }

        private void LoadMainProjectAssemblyClick()
        {
            var targetPath = EditorPrefs.GetString("main_assembly_path");

            if (targetPath == "")
            {
                Print("_main_assembly_path == null!",ToolLogType.Error);
                return;
            }

            try
            {
                var module = ModuleDefinition.ReadModule(targetPath);
                if (module == null) return;

                _adaptorSingleInterfaceDic.Clear();
                _adaptorDic.Clear();
                _delegateCovDic.Clear();

                //TODO
                //系统相关的一些需要写适配器的接口
                List<TypeDefinition> types = WhiteTypeList.GetThirdPartyWhiteList();

                var system_white_reference_table = new List<TypeDefinition>(types);

                var typeList = module.GetTypes();//获取Module中的类型

                types.AddRange(typeList);

                foreach (var t in types)
                {
                    if (t.IsInterface)
                    {//如果是接口类型
                        //主工程中被标志的接口
                        foreach (var customAttribute in t.CustomAttributes)
                        {
                            if (customAttribute.AttributeType.FullName == _singleInterfaceAttrName)
                            {
                                Print("[Single Interface Export]" + t.FullName,ToolLogType.Info);
                                LoadInterfaceAdaptor(t);
                                break;
                            }
                        }

                        //白名单中的接口
                        if (system_white_reference_table.Contains(t))
                        {
                            Print("[System Interface Export]" + t.FullName,ToolLogType.Info);
                            LoadInterfaceAdaptor(t);
                            continue;
                        }
                    }
                    else
                    {
                        foreach (var customAttribute in t.CustomAttributes)
                        {
                            if (customAttribute.AttributeType.FullName == _adaptorAttrName)
                            {
                                Print("[Need Adaptor]" + t.FullName,ToolLogType.Info);
                                LoadAdaptor(t);
                                break;
                            }

                            if (customAttribute.AttributeType.FullName == _delegateAttrName)
                            {
                                //unity dll egg hurt name has '/'
                                var typeName = t.FullName.Replace("/", ".");
                                Print("[Delegate Export]" + typeName,ToolLogType.Info);
                                LoadDelegateConvertor(t);
                                break;
                            }
                        }

                        //白名单中的类型(不需要做)
                        if (system_white_reference_table.Contains(t))
                        {
                            Print("[System Type Export]" + t.FullName);
                            LoadAdaptor(t);
                            continue;
                        }
                    }
                }
                Print("----------------main scripts assembly loaded" , ToolLogType.Normal);
            }
            catch (Exception ex)
            {
                Print(ex.Message,ToolLogType.Error);
            }
        }

        /// <summary>
        /// 加载热更代码,此方法是用来进行委托注册的(在热更代码使用到的主工程中的委托实例将会被注册）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadILScriptAssemblyClick()
        {
            var mainProjPath = EditorPrefs.GetString("main_assembly_path");
            if (string.IsNullOrWhiteSpace(mainProjPath))
            {//如果未指定主工程路径
                if (EditorUtility.DisplayDialog("提示", "没有指定主工程路径,将只检测热更工程中的Action和Func并执行注册功能!", "ok", "cancal"))
                    return;
            }

            //
            var main_module = ModuleDefinition.ReadModule(mainProjPath);
            if (main_module == null)
            {
                Print("指定主工程文件有误,请重新指定!",ToolLogType.Error);
                return;
            }

            //获取主工程中的所有类型
            var alltypes = main_module.GetTypes();
            var delegatetypes = new List<TypeDefinition>();

            //遍历，将主工程中的所有类型都加入delegatetypes
            foreach (var item in alltypes)
            {
                if (item.BaseType != null && "System.MulticastDelegate".Equals(item.BaseType.FullName))
                {
                    delegatetypes.Add(item);
                }

            }

            var targetPath = EditorPrefs.GetString("il_assembly_path");

            if (targetPath == "")
            {
                Print("没有指定生产文件的存放目录!",ToolLogType.Error);
                return;
            }

            try
            {
                using (var fs = File.Open(targetPath, FileMode.Open))
                {
                    _delegateRegDic.Clear();
                    var module = ModuleDefinition.ReadModule(fs);
                    foreach (var typeDefinition in module.Types)
                    {
                        //如果是Debug类型的，需要手动注册并且转换
                        if (typeDefinition.Namespace == "Hotfix.Hotfix.Bugfix")
                            continue;

                        foreach (var methodDefinition in typeDefinition.Methods)
                        {
                            //方法主体包含指令，且指令不为null
                            if (methodDefinition?.Body?.Instructions == null)
                                continue;

                            foreach (var instruction in methodDefinition.Body.Instructions)
                            {
                                //前一条指令不为null
                                //前一条指令是将指向特定方法的非托管指针指向计算堆栈上面
                                //当前指令是创建一个新的类型
                                if (instruction.OpCode != OpCodes.Newobj || instruction.Previous == null ||
                                    instruction.Previous.OpCode != OpCodes.Ldftn) continue;

                                //当前操作数对象
                                var type = instruction.Operand as MethodReference;
                                //Print("begin: ");
                                var clrType = delegatetypes.Find((tt) =>
                                {
                                    //Print("==>"+name);
                                    //Print("===>"+type.DeclaringType.FullName);
                                    return type.DeclaringType.FullName.Equals(tt.FullName);
                                });

                                if (type == null ||
                                    (!type.DeclaringType.Name.Contains("Action") &&
                                     !type.DeclaringType.Name.Contains("Func") &&
                                        clrType == null)) continue;

                                //获取委托的全名
                                var typeName = type.DeclaringType.FullName;//.Replace("/", ".");
                                Print("[delegate register]" + typeName,ToolLogType.Info);

                                //加载委托注册器
                                LoadDelegateRegister(typeName, clrType == null ? type.DeclaringType : clrType);
                            }
                        }
                    }
                }

                Print("----------ilscript assembly loaded");
            }
            catch (Exception ex)
            {
                Print(ex.Message,ToolLogType.Error);
            }
        }

        private void LoadDelegateRegister(string key, TypeReference type)
        {
            if (!_delegateRegDic.ContainsKey(key))
                _delegateRegDic.Add(key, type);
            else
                _delegateRegDic[key] = type;
        }

        private void LoadDelegateConvertor(TypeDefinition type)
        {
            var key = type.FullName.Replace("/", ".");
            if (!_delegateCovDic.ContainsKey(key))
                _delegateCovDic.Add(key, type);
            else
                _delegateCovDic[type.FullName] = type;
        }

        /// <summary>
        /// 加载接口适配器(单独处理接口的)
        /// </summary>
        /// <param name="t"></param>
        private void LoadInterfaceAdaptor(TypeDefinition t)
        {
            if (!_adaptorSingleInterfaceDic.ContainsKey(t.FullName))
                _adaptorSingleInterfaceDic.Add(t.FullName, t);
            else
                _adaptorSingleInterfaceDic[t.FullName] = t;
        }

        private void LoadAdaptor(TypeDefinition type)
        {
            //var key = type.FullName.Replace("/", ".");
            if (!_adaptorDic.ContainsKey(type.FullName))
                _adaptorDic.Add(type.FullName, type);
            else
                _adaptorDic[type.FullName] = type;
        }

        /// <summary>
        /// 打印Log
        /// </summary>
        /// <param name="log"></param>
        public static void Print(string log, ToolLogType type= ToolLogType.Normal)
        {
            Instance.LogSet.Add(new Tuple<ToolLogType, string>(type, log));
        }
        #endregion

        #region 编辑器菜单
        [MenuItem("ILRuntime/Adapter",false,100)]
        static void ShowGenerateILRuntimeAdapterWindow()
        {
            GenerateILRuntimeAdapterWindow window = Instance;
            window.titleContent = new GUIContent("适配器自动生成工具");
            window.minSize = new Vector2(1000, 654);
            window.maxSize = new Vector2(1000, 654);
            window.Show();
        }
        #endregion
    }
}
