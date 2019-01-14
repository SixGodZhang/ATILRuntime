//-----------------------------------------------------------------------
// <filename>LocateHotfixMethods</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #定位需要控制修复Bug的方法# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 20:52:38# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator;
using Editor_Mono.Cecil;
using ILCodeWeaving;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace GameFramework.Taurus
{
    /// <summary>
    /// bug委托类型信息
    /// </summary>
    public class DelegateInfo
    {
        /// <summary>
        /// 返回值类型
        /// </summary>
        public string ReturnTypeStr;
        /// <summary>
        /// 委托名称
        /// </summary>
        public string DelegateName;
        /// <summary>
        /// 参数名+参数类型混杂
        /// </summary>
        public string Parameters;
        /// <summary>
        /// 参数类型
        /// </summary>
        public string ParaetersType;
        /// <summary>
        /// 纯参数名
        /// </summary>
        public string PureParameters;
    }

    public class LocateHotfixMethods:Editor
    {

        /// <summary>
        /// 注入
        /// </summary>
        [MenuItem("ILRuntime/Inject",false,310)]
        public static void InjectAssembly()
        {
            string path = PathConfig.GetPath(ILPath.MainAssemblyPath);
            if (string.IsNullOrWhiteSpace(path))
            {
                UnityEngine.Debug.LogError("main_assembly_path == null");
                return;
            }

            //load dll
            ILCodeWeaver weaver = new ILCodeWeaver(path);

            //test
            // List<MethodDefinition> methodsList = weaver.GetAllInjectedMethods();
            List<Tuple<string, string, string, string, string>> tuples = weaver.GetInjectMessage();

            foreach (var tuple in tuples)
            {
                weaver.SetupHijackMethod(weaver.GetMethodDefinition(tuple.Item2, tuple.Item3))
                    .HijackMethod(tuple.Item1,
                    weaver.GetTypeDefinitionByName(tuple.Item4),
                    weaver.GetTypeDefinitionByName(tuple.Item5));
            }

            //inject code
            //weaver.SetupHijackMethod(weaver.GetMethodDefinition("GameFramework.Taurus.TestMainHotfix", "System.Void GameFramework.Taurus.TestMainHotfix::DoAction()"))
            //    .HijackMethod("ILRuntime.method_delegate0 ILRuntime.BugfixDelegateStatements::GameFramework_Taurus_TestMainHotfix_DoAction_IN__OUT_System_Void__Delegate",
            //    weaver.GetTypeDefinitionByName("ILRuntime.method_delegate0"),
            //    weaver.GetTypeDefinitionByName("ILRuntime.BugfixDelegateStatements"));

            //save code
            weaver.Reweave();

        }

        /// <summary>
        /// 检测主工程中的代码，并确定需要修复的方法
        /// </summary>
        [MenuItem("ILRuntime/Bugfix",false,300)]
        public static void GeneratorDelegates()
        {
            string mainPath = PathConfig.GetPath(ILPath.MainAssemblyPath);
            if (string.IsNullOrWhiteSpace(mainPath))
                UnityEngine.Debug.Log("main_assembly_path == null");

            List<ProtectMethodInfo> protectMethods = LoadProtectMethods(mainPath);

            var DelegateAndMethods = new Dictionary<DelegateInfo, List<ProtectMethodInfo>>();
            List<DelegateInfo> delegateInfos = GenerateDelegateTypes(protectMethods,ref DelegateAndMethods);

            //生成委托定义
            GeneratorDeledateDefines(delegateInfos);

            //生成委托声明
            GeneratorDeledateStatements(DelegateAndMethods);

            //生成委托注册和转换
            BugfixHelper helper = new BugfixHelper();
            helper.OnInit(delegateInfos);
            helper.GenerateCode();
        }

        /// <summary>
        /// 生成委托声明
        /// </summary>
        /// <param name="delegateInfos"></param>
        private static void GeneratorDeledateStatements(Dictionary<DelegateInfo, List<ProtectMethodInfo>> delegateAndMethods)
        {
            StringBuilder mis = new StringBuilder();
            foreach (var key in delegateAndMethods.Keys)
            {
                foreach (var mi in delegateAndMethods[key])
                {
                    mis.Append("public static " + key.DelegateName + " " + mi.MethodName + "__Delegate;\n\t\t\t");
                }
            }

            string out_path = PathConfig.GetPath(ILPath.AdapterOutputPath).Replace("Adapter", "Bugfix") + "/DelegateStatements.cs";
            if (File.Exists(out_path))
                File.Delete(out_path);

            //模板路径
            var tmpdPath = PathConfig.GetPath(ILPath.TemplatePath) + "bugfix_delegate_statements.tmpd"; //Application.dataPath + "/ThirdParty/ILRuntime/Editor/Adapter/Template/bugfix_delegate_statements.tmpd";
            BugfixFileManager manager = new BugfixFileManager(tmpdPath);
            manager.SetKeyValue("{$Namespace}", "ILRuntime");
            manager.SetKeyValue("{$DelegateStatements}", mis.ToString());
            //生成文件
            File.WriteAllText(out_path, manager.Generate());
        }

        /// <summary>
        /// 生成委托定义
        /// </summary>
        /// <param name="delegateInfos"></param>
        private static void GeneratorDeledateDefines(List<DelegateInfo> delegateInfos)
        {
            StringBuilder content = new StringBuilder();
            string returnStr = "";
            foreach (var @delegate in delegateInfos)
            {
                returnStr = @delegate.ReturnTypeStr == "System.Void" ? "void" : @delegate.ReturnTypeStr;

                content.Append("public delegate " + returnStr + " " + @delegate.DelegateName + "(" + @delegate.Parameters + ");\n\t\t\t");
            }

            string out_path = PathConfig.GetPath(ILPath.AdapterOutputPath).Replace("Adapter", "Bugfix") + "/DelegateDefines.cs";
            //模板路径
            var tmpdPath = PathConfig.GetPath(ILPath.TemplatePath) + "/bugfix_delegate_defines.tmpd";//Application.dataPath + "/ThirdParty/ILRuntime/Editor/Adapter/Template/bugfix_delegate_defines.tmpd";
            BugfixFileManager manager = new BugfixFileManager(tmpdPath);
            manager.SetKeyValue("{$Namespace}", "ILRuntime");
            manager.SetKeyValue("{$DelegateDefines}", content.ToString());
            //生成文件
            File.WriteAllText(out_path, manager.Generate());
        }

        /// <summary>
        /// 生成委托类型，同一方法签名的只生成一种类型
        /// </summary>
        /// <param name="protectMethods"></param>
        private static List<DelegateInfo> GenerateDelegateTypes(List<ProtectMethodInfo> protectMethods,ref Dictionary<DelegateInfo, List<ProtectMethodInfo>> DelegateAndMethods)
        {
            List<DelegateInfo> paramsSignatureList = new List<DelegateInfo>();
            List<Tuple<string, DelegateInfo>> DelegateIdentitis = new List<Tuple<string, DelegateInfo>>();
            int delegateIndex = 0;
            foreach (var mi in protectMethods)
            {
                DelegateInfo @delegate = new DelegateInfo();

                StringBuilder delegateParams = new StringBuilder();
                StringBuilder delegateParamsType = new StringBuilder();
                StringBuilder delegatePureParams = new StringBuilder();

                int index = 0;
                delegateParams.Append("System.Object" + " arg" + index + ",");
                delegateParamsType.Append("System.Object,");
                delegatePureParams.Append("arg" + index + ",");

                foreach (var param in mi.paramList)
                {
                    delegateParams.Append(GetParameterPrefix(param) + " " + param.ParamTypeStr + " arg" + (++index) + ",");
                    delegatePureParams.Append(GetParameterPrefix(param) + " arg" + index + ",");
                    delegateParamsType.Append(GetParameterPrefix(param) + " " + param.ParamTypeStr + ",");
                }

                //只创建唯一签名的方法委托
                string identidy = delegateParams.ToString() + "out:" + mi.ReturnTypeStr;
                bool isExsist = false;

                for (int i = 0; i < DelegateIdentitis.Count; i++)
                {
                    if (DelegateIdentitis[i].Item1 == identidy)
                    {
                        isExsist = true;
                        DelegateAndMethods[DelegateIdentitis[i].Item2].Add(mi);
                        break;
                    }
                }

                if (!isExsist)
                {
                    //委托名
                    @delegate.DelegateName = "method_delegate" + (delegateIndex++);

                    @delegate.PureParameters = delegatePureParams.ToString().Trim(' ').Trim(',');

                    //参数类型列表
                    @delegate.ParaetersType = delegateParamsType.ToString().Trim(' ').Trim(',');

                    //参数
                    @delegate.Parameters = delegateParams.ToString().Trim(' ').TrimEnd(',');

                    //返回值
                    @delegate.ReturnTypeStr = mi.ReturnTypeStr;

                    paramsSignatureList.Add(@delegate);

                    DelegateAndMethods.Add(@delegate, new List<ProtectMethodInfo>() { mi });

                    DelegateIdentitis.Add(new Tuple<string, DelegateInfo>(identidy, @delegate));
                }
            }

            //test
            foreach (var item in paramsSignatureList)
            {
                UnityEngine.Debug.Log("public " + item.ReturnTypeStr + " delegate " + item.DelegateName + "(" + item.Parameters + ")");
            }

            return paramsSignatureList;
        }

        /// <summary>
        /// 获取参数前缀
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string GetParameterPrefix(ParamData paramData)
        {
            string paramPrefix = string.Empty;
            switch (paramData.Prefix)
            {
                case ParamPrefix.None:
                    paramPrefix = "";
                    break;
                case ParamPrefix.Ref:
                    paramPrefix = "ref";
                    break;
                case ParamPrefix.Out:
                    paramPrefix = "out";
                    break;
            }

            return paramPrefix;
        }

        /// <summary>
        /// 获取参数类型的字符串,此处处理一些特殊的字符串
        /// </summary>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static void GetParameterTypeStr(ref ParamData paramData)
        {
            if (paramData.Prefix == ParamPrefix.Ref || paramData.Prefix == ParamPrefix.Out)
            {
                paramData.ParamTypeStr = paramData.ParamTypeStr.Replace("&", "");
            }
        }

        /// <summary>
        /// 加载所有需要保护的方法
        /// </summary>
        /// <param name="mainPath"></param>
        /// <returns></returns>
        private static List<ProtectMethodInfo> LoadProtectMethods(string mainPath)
        {
            List<ProtectMethodInfo> methodInfos = new List<ProtectMethodInfo>();
            Assembly assembly = Assembly.LoadFile(mainPath);
            var types = assembly.GetTypes();

            BindingFlags flags = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            List<Type> targetTypes = new List<Type>();
            foreach (var type in types)
            {
                var attribute = type.GetCustomAttribute(typeof(AllowHotfixAttribute)) as AllowHotfixAttribute;
                if (attribute != null)
                    targetTypes.Add(type);
            }

            foreach (var type in targetTypes)
            {
                var methods = type.GetMethods(flags);

                //==============================================================
                //遍历所有方法，提取所有可以进行热更修复的方法
                //注意:
                //1.params 修饰的参数 不可被修复 
                //
                //==============================================================
                foreach (var method in methods)
                {
                    ProtectMethodInfo methodInfo = new ProtectMethodInfo();
                    List<ParamData> paramDatas = new List<ParamData>();

                    //获取参数信息
                    bool isPassParameterTest = true;
                    StringBuilder parameter = new StringBuilder();
                    var parameters = method.GetParameters();
                    foreach (var param in parameters)
                    {
                        ParamData paramData = new ParamData();
                        if (param.IsDefined(typeof(System.ParamArrayAttribute)))
                        {//暂时不支持可选参数数组
                            UnityEngine.Debug.Log("not support this type parameter!");
                            isPassParameterTest = false;
                            break;
                        }

                        if (param.ParameterType.IsByRef && param.IsOut)
                        {//out 遇上述条件不是并列关系 out 修饰的参数既是ref 也是 out
                            paramData.Prefix = ParamPrefix.Out;
                            paramData.ParamTypeStr = param.ParameterType.ToString();
                            //===================暂时不支持ref参数,适配器原因======================
                            UnityEngine.Debug.Log("not support this type parameter!");
                            isPassParameterTest = false;
                            break;
                        }
                        else if (param.ParameterType.IsByRef && !param.IsOut)
                        {//ref 
                            paramData.Prefix = ParamPrefix.Ref;
                            paramData.ParamTypeStr = param.ParameterType.ToString();
                            //==================暂时不支持out参数,适配器原因=======================
                            UnityEngine.Debug.Log("not support this type parameter!");
                            isPassParameterTest = false;
                            break;
                        }
                        else if (!param.ParameterType.IsByRef && !param.IsOut)
                        {//normal
                            paramData.Prefix = ParamPrefix.None;
                            paramData.ParamTypeStr = param.ParameterType.ToString();
                        }
                        else
                        {
                            UnityEngine.Debug.Log("not support this type parameter!");
                            isPassParameterTest = false;
                            break;
                        }
                        string paramPrefix = GetParameterPrefix(paramData);
                        GetParameterTypeStr(ref paramData);

                        parameter.Append("0" + (string.IsNullOrWhiteSpace(paramPrefix) ? "" : paramPrefix + "_") + paramData.ParamTypeStr + "_");
                        paramDatas.Add(paramData);

                    }

                    if (!isPassParameterTest)
                    {
                        UnityEngine.Debug.LogError(method.Name + "未通过参数测试!");
                        continue;
                    }

                    //获取参数信息
                    methodInfo.paramList = paramDatas;

                    //获取返回值信息
                    methodInfo.ReturnTypeStr = method.ReturnType.ToString();

                    //方法名称
                    methodInfo.MethodName = (method.DeclaringType.ToString() + "_NM_" + method.Name + "_IN_" + parameter.ToString().TrimEnd('_') + "_OUT_" + methodInfo.ReturnTypeStr).Replace(".", "_").TrimEnd('_');

                    methodInfos.Add(methodInfo);
                }
            }

            //测试获取的方法
            //foreach (var mi in methodInfos)
            //{
            //    UnityEngine.Debug.Log("methodName: \n" + mi.MethodName);
            //}

            return methodInfos;
        }
    }
}
