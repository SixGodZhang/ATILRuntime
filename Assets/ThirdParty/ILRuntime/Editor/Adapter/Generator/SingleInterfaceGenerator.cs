//-----------------------------------------------------------------------
// <filename>SingleInterfaceGenerator</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/11 星期二 15:16:02# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using Mono.Cecil;
using System;

namespace CodeGenerationTools.Generator
{
    public class SingleInterfaceGenerator : GeneratorBase<MethodDefinition>
    {
        public override bool LoadData(MethodDefinition methodInfo)
        {
            if (methodInfo == null)
                return false;

            string argStr = "";
            string argNoTypeStr = "";

            SetKeyValue("{$MethodName}", methodInfo.Name);
            foreach (var pInfo in methodInfo.Parameters)//.GetParameters())
            {
                argStr += pInfo.ParameterType.Name + " " + pInfo.Name + ",";
                argNoTypeStr += pInfo.Name + ",";
            }
            argStr = argStr.Trim(',');
            argNoTypeStr = argNoTypeStr.Trim(',');
            SetKeyValue("{$args}", argStr);
            SetKeyValue("{$args_no_type}", argNoTypeStr);
            SetKeyValue("{$args_count}", methodInfo.Parameters.Count.ToString());

            SetKeyValue("{$comma}", argStr == "" ? "" : ",");

            if (methodInfo.ReturnType.FullName == "System.Void") return true;

            SetKeyValue("{$returnType}", methodInfo.ReturnType.Name);
            var returnStr = methodInfo.ReturnType.IsValueType ? "return 0;" : "return null;";
            SetKeyValue("{$returnDefault}", returnStr);

            return true;
        }
    }
}
