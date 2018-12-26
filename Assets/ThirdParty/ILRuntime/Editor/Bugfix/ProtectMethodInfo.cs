//-----------------------------------------------------------------------
// <filename>ParamPrefix</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/20 星期四 21:07:10# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace GameFramework.Taurus
{
    public enum ParamPrefix
    {
        None,
        Ref,
        Out,
        Array,
        ParamArray
    }

    public class ParamData
    {
        /// <summary>
        /// 参数前缀
        /// </summary>
        public ParamPrefix Prefix = ParamPrefix.None;
        /// <summary>
        /// 参数类型
        /// </summary>
        public string ParamTypeStr;
    }

    public class ProtectMethodInfo
    {
        public string ReturnTypeStr;
        public List<ParamData> paramList;
        public string MethodName;
    }
}
