//-----------------------------------------------------------------------
// <filename>AllowHotfixAttribute</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 20:29:18# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    [AttributeUsage(AttributeTargets.Class,AllowMultiple =false,Inherited =false)]
    public class AllowHotfixAttribute:Attribute
    {

    }
}
