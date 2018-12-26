//-----------------------------------------------------------------------
// <filename>TestResultInfo</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/17 星期一 14:19:49# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace GameFramework.Taurus.UnityEditor
{
    /// <summary>
    /// 测试结果
    /// </summary>
    public class TestResultInfo:UnityEngine.Object
    {
        public TestResultInfo(string testName, bool result, string message)
        {
            TestName = testName;
            Result = result;
            Message = message;
        }

        public string TestName { get; private set; }

        public bool Result { get; private set; }

        public string Message { get; private set; }
    }
}
