//-----------------------------------------------------------------------
// <filename>BugfixHelper</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/21 星期五 15:27:21# </time>
//-----------------------------------------------------------------------

using GameFramework.Taurus;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CodeGenerationTools.Generator
{
    public class BugfixHelper
    {
        private BugfixDelegateHelperGenerator _bugfixDelegateHelper;
        private string _out_path;

        private List<DelegateInfo> delegateInfoList = new List<DelegateInfo>();

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit(List<DelegateInfo> delegates)
        {
            _out_path = EditorPrefs.GetString("out_path");
            delegateInfoList.Clear();

            LoadTemplates();

            delegateInfoList = delegates;
        }

        /// <summary>
        /// 加载所有模板
        /// </summary>
        private void LoadTemplates()
        {
            var tmpdPath = Application.dataPath + "/ThirdParty/ILRuntime/Editor/Adapter/Template/";
            _bugfixDelegateHelper = new BugfixDelegateHelperGenerator();
            _bugfixDelegateHelper.LoadTemplateFromFile(tmpdPath + "bugfix_helper.tmpd");
        }

        /// <summary>
        /// 生成代码
        /// </summary>
        public void GenerateCode()
        {
            _bugfixDelegateHelper.LoadData(delegateInfoList);
            var helperStr = _bugfixDelegateHelper.Generate();
            UnityEngine.Debug.Log(helperStr);

            _out_path = _out_path.Replace("Adapter", "Bugfix");

            if (File.Exists(_out_path + "/bugfix_helper.cs"))
                File.Delete(_out_path + "/bugfix_helper.cs");

            using (var fs2 = File.Create(_out_path + "/bugfix_helper.cs"))
            {
                var sw = new StreamWriter(fs2);
                sw.Write(helperStr);
                sw.Flush();
            }
        }
    }
}
