//-----------------------------------------------------------------------
// <filename>ITestable</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/17 星期一 14:18:35# </time>
//-----------------------------------------------------------------------

namespace GameFramework.Taurus.UnityEditor
{
    /// <summary>
    /// Test Interface
    /// </summary>
    interface ITestable
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool Init(string fileName);
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        bool Init(ILRuntime.Runtime.Enviorment.AppDomain app);
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="app"></param>
        /// <param name="type"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        bool Init(ILRuntime.Runtime.Enviorment.AppDomain app, string type, string method);
        /// <summary>
        /// 执行单元测试
        /// </summary>
        void Run();
        /// <summary>
        /// 检查单元测试执行结果
        /// </summary>
        /// <returns></returns>
        bool Check();
        /// <summary>
        /// 检测方法的的执行具体信息
        /// </summary>
        /// <returns></returns>
        TestResultInfo CheckResult();
    }
}
