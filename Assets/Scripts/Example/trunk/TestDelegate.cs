//-----------------------------------------------------------------------
// <filename>TestDelegate</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/12 星期三 11:57:01# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public delegate void TestCallBack(int id, string name);

    public class TestDelegate
    {
        public static TestCallBack callBack;
    }
}
