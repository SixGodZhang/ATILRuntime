//-----------------------------------------------------------------------
// <filename>InterfaceAdapterGenerator</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> #单独接口适配器生成器# </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/11 星期二 14:55:56# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CodeGenerationTools.Generator
{
    public class InterfaceAdapterGenerator : GeneratorBase<TypeDefinition>
    {
        private string _filePath = "";
        private SingleInterfaceGenerator _sig;

        public override bool LoadData(TypeDefinition type)
        {
            if (type == null)
                return false;
            if (!type.IsInterface)
                return false;

            var methodsbody = "";
            var methods = type.Methods;

            foreach (var methodInfo in methods.Where(methodInfo => methodInfo.DeclaringType.FullName != "System.Object"))
            {
                methodsbody += CreateInterfaceMethod(methodInfo);
            }

            SetKeyValue("{$Namespace}", type.Namespace);
            SetKeyValue("{$InterfaceName}", type.Name);
            SetKeyValue("{$MethodArea}", methodsbody);

            return true;
        }

        /// <summary>
        /// 创建接口方法
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        private string CreateInterfaceMethod(MethodDefinition methodInfo)
        {
            _sig.InitFromFile(
              _filePath + (methodInfo.ReturnType.FullName == "System.Void"
                ? "/method_interface_void.tmpd"
                : "/method_interface_return.tmpd"),
              methodInfo);
            return _sig.Generate();
        }

        public override bool LoadTemplateFromFile(string filePath)
        {
            _filePath = Path.GetDirectoryName(filePath);
            //TODO
            //具体代码生成器
            _sig = new SingleInterfaceGenerator();

            return base.LoadTemplateFromFile(filePath);
        }
    }
}
