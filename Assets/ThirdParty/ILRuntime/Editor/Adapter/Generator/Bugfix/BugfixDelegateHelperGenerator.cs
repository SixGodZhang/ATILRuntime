//-----------------------------------------------------------------------
// <filename>BugfixDelegateHelperGenerator</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #此类用来注册和转换 修复主工程的方法的 委托生成器# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/21 星期五 14:54:59# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using GameFramework.Taurus;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeGenerationTools.Generator
{
    public class BugfixDelegateHelperGenerator: GeneratorBase<List<DelegateInfo>>
    {
        private string _filePath;
        private BugfixDelegateConveterGenerator _dcg;
        private BugfixDelegateRegisterGenerator _drg;

        public override bool LoadData(List<DelegateInfo> data)
        {
            if (data == null)
                return false;

            //filter
            List<DelegateInfo> delegateList = FilterIllegalDelegate(data);

            //委托注册器
            var delegateRegStr = "";
            foreach (var val in delegateList)
            {
                delegateRegStr += CreateDelegateRegisterInit(val);
            }
            SetKeyValue("{$DelegateRegInit}", delegateRegStr);


            //委托转换器
            var delegateConvertStr = "";
            foreach (var delegateInfo in delegateList)
            {
                delegateConvertStr += CreateDelegateConvertorInit(delegateInfo);
            }
            SetKeyValue("{$DelegateInit}", delegateConvertStr);


            //其它
            SetKeyValue("{$Namespace}", "ILRuntime");

            return true;
        }



        /// <summary>
        /// 委托转换器
        /// </summary>
        /// <param name="delegateInfo"></param>
        /// <returns></returns>
        private string CreateDelegateConvertorInit(DelegateInfo delegateInfo)
        {
            if (delegateInfo == null)
                return null;

            var tmpd = delegateInfo.ReturnTypeStr == "System.Void" ? "delegate_void.tmpd" : "delegate_return.tmpd";
            _dcg.InitFromFile(_filePath + Path.AltDirectorySeparatorChar + tmpd, delegateInfo);
            return _dcg.Generate();
        }

        /// <summary>
        /// 过滤掉一些不符合要求的委托
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private List<DelegateInfo> FilterIllegalDelegate(List<DelegateInfo> data)
        {
            List<DelegateInfo> delegates = new List<DelegateInfo>();
            //过滤掉无参Action
            foreach (var item in data)
            {
                if (item.ReturnTypeStr.Equals("System.Void") && string.IsNullOrWhiteSpace(item.Parameters))
                    continue;
                delegates.Add(item);
            }

            return delegates;
        }

        public override bool LoadTemplateFromFile(string filePath)
        {
            _filePath = Path.GetDirectoryName(filePath);
            _dcg = new BugfixDelegateConveterGenerator();
            _drg = new BugfixDelegateRegisterGenerator();

            return base.LoadTemplateFromFile(filePath);
        }

        /// <summary>
        /// 委托注册器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string CreateDelegateRegisterInit(DelegateInfo @delegate)
        {
            string tmpd = null;
            tmpd = @delegate.ReturnTypeStr.Equals("System.Void")? "action_register.tmpd" : "function_register.tmpd";

            _drg.InitFromFile(_filePath + Path.AltDirectorySeparatorChar + tmpd, @delegate);

            return _drg.Generate();
        }
    }
}
