//-----------------------------------------------------------------------
// <filename>BugfixDelegateConveterGenerator</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/21 星期五 16:55:01# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using GameFramework.Taurus;

namespace CodeGenerationTools.Generator
{
    public class BugfixDelegateConveterGenerator : GeneratorBase<DelegateInfo>
    {
        public override bool LoadData(DelegateInfo data)
        {
            if (data == null)
                return false;

            SetKeyValue("{$DelegateName}", data.DelegateName);
            SetKeyValue("{$argsType}", data.ParaetersType);
            SetKeyValue("{$args}", data.PureParameters);
            if (data.ReturnTypeStr != "System.Void")
                SetKeyValue("{$returnType}", data.ReturnTypeStr);

            return true;
        }
    }
}
