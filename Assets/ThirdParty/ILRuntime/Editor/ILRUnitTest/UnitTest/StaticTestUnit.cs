//-----------------------------------------------------------------------
// <filename>StaticTestUnit</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/17 星期一 14:25:45# </time>
//-----------------------------------------------------------------------
namespace GameFramework.Taurus.UnityEditor
{
    /// <summary>
    /// 静态方法测试单元
    /// </summary>
    class StaticTestUnit : BaseTestUnit
    {
        public override void Run()
        {
            Invoke(TypeName, MethodName);
        }

        public override bool Check()
        {
            return Pass;
        }

        public override TestResultInfo CheckResult()
        {
            return new TestResultInfo(TypeName + "." + MethodName, Pass, Message.ToString());
        }
    }
}
