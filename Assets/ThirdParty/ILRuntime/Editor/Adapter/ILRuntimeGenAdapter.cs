//-----------------------------------------------------------------------
// <filename>ILRuntimeGenAdapter</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #跨域继承,生成ILRuntime的适配器# </describe>
// <desc> 被CSHotfix标记的代码，属于弃用代码</desc>
// <desc> 此工具类即将被重写 </desc>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/6 星期四 21:46:41# </time>
//-----------------------------------------------------------------------
using CodeGenerationTools.Generator;
using GameFramework.Taurus;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
/// <summary>
/// 适配器生成类
/// </summary>
public class ILRuntimeGenAdapter
{
    #region 字段&属性
    private string _outputPath;

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

    /// <summary>
    /// 工具入口
    /// </summary>
    void ToolMain()
    {
        _outputPath = Application.dataPath + "/ThirdParty/ILRuntime/ILRuntime/Runtime/Adaptors/";
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);

        string out_path = EditorPrefs.GetString("out_path");
        string main_assembly_path = EditorPrefs.GetString("main_assembly_path");
        string il_assembly_path = EditorPrefs.GetString("il_assembly_path");

        _adaptorSingleInterfaceDic.Clear();
        _adaptorDic.Clear();
        _delegateCovDic.Clear();
        _delegateRegDic.Clear();

        LoadTemplates();
    }

    /// <summary>
    /// 加载所有模板
    /// </summary>
    private void LoadTemplates()
    {
        var tmpdPath = Application.dataPath + "/ThirdParty/ILRuntime/Adapters/Editor/Adapter/Template";
        _interfaceAdapterGenerator = new InterfaceAdapterGenerator();
        _interfaceAdapterGenerator.LoadTemplateFromFile(tmpdPath + "adaptor_single_interface.tmpd");

        _adGenerator = new AdaptorGenerator();
        _adGenerator.LoadTemplateFromFile(tmpdPath + "adaptor.tmpd");

        _helpGenerator = new HelperGenerator();
        _helpGenerator.LoadTemplateFromFile(tmpdPath + "helper.tmpd");
    }
}
#endif