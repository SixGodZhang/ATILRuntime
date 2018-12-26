//-----------------------------------------------------------------------
// <filename>BugfixDelegateRegisterGenerator</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/21 星期五 16:54:13# </time>
//-----------------------------------------------------------------------

using CodeGenerationTools.Generator.Base;
using GameFramework.Taurus;
using System;
using System.Collections.Generic;

namespace CodeGenerationTools.Generator
{
    public class BugfixDelegateRegisterGenerator : GeneratorBase<DelegateInfo>
    {
        public override bool LoadData(DelegateInfo data)
        {
            if (data == null)
                return false;

            SetKeyValue("{$argsType}", data.ParaetersType);

            return true;
        }
    }
}
